using System;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace NancyApplication {
    public class CacheService
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
        public ConnectionMultiplexer Connection
        {
            get
            {
                return lazyConnection.Value;
            }
        }

        
    }

}