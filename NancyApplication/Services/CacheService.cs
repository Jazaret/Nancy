using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace NancyApplication {
    public class CacheService : ICacheService
    {
        const string RedisConnectionString = "topics.redis.cache.windows.net:6380,password=mTDuw1qJqQSsm8rGNK4e7ko9csj7AJvPPpRSVCqO2CY=,ssl=True,abortConnect=False";
        private static Lazy<ConnectionMultiplexer> lazyConnection;        
        static object connectLock = new object();

        public CacheService()
        {
            if (lazyConnection == null)
            {
                lock (connectLock)
                {
                    if (lazyConnection == null)
                    {
                        lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
                        {
                            return ConnectionMultiplexer.Connect(RedisConnectionString);
                        });
                    }
                }
            }
        }
        private ConnectionMultiplexer Connection
        {
            get
            {
                return lazyConnection.Value;
            }
        }
        public async Task<string> GetFromCache(string key)
        {
            if (Connection.IsConnected)
            {
                var cache = Connection.GetDatabase();
                return await cache.StringGetAsync(key);
            }
            else
            {
                return null;
            }
        }

        public async Task DeleteFromCache(string key)
        {
            if (Connection.IsConnected)
            {
                var cache = Connection.GetDatabase();
                await cache.KeyDeleteAsync(key).ConfigureAwait(false);
            }
        }

        public async Task AddToCache(string key, string serializedData)
        {
            var GetMessagesCacheExpiryMinutes = 5;
            if (Connection.IsConnected)
            {
                var cache = Connection.GetDatabase();

                TimeSpan expiresIn;
                expiresIn = new TimeSpan(0, GetMessagesCacheExpiryMinutes, 0);
                await cache.StringSetAsync(key, serializedData, expiresIn).ConfigureAwait(false);
            }
        }
    }

}