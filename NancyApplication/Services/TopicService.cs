using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace NancyApplication {
    /// <summary>
    /// Service that handles actions on Topics
    /// </summary>
    public class TopicService : ITopicService
    {
        ITopicRepository _topicRepo;

        private CacheService redisService;
        private ConnectionMultiplexer connectionMultiplexer;
        public TopicService(ITopicRepository topicRepository) {
            _topicRepo = topicRepository;
            redisService = new CacheService();
            connectionMultiplexer = redisService.Connection;
        }

        /// <summary>
        /// Get list of all topics
        /// </summary>
        /// <returns>list of all topics</returns>
        public IEnumerable<Topic> GetAllTopics()
        {
            return _topicRepo.GetTopics();
        }

        /// <summary>
        /// Get list of topics that contains the parameter. Uses caching for performance.
        /// </summary>
        /// <param name="news">paramter to search for</param>
        /// <returns>list of topics that contains parameter string</returns>
        public IEnumerable<Topic> SearchForNews(string news) {
            
            string cacheResult = null;
            
            cacheResult = GetFromCache(news).Result;
            if (cacheResult != null) { 
                return JsonConvert.DeserializeObject<List<Topic>>(cacheResult);
            }           

            var result = _topicRepo.SearchForTopics(news);
            
            var addCacheResult = AddToCache(news,JsonConvert.SerializeObject(result));

            return result;
        }

        private async Task<string> GetFromCache(string key)
        {
            if (connectionMultiplexer.IsConnected)
            {
                var cache = connectionMultiplexer.GetDatabase();
                return await cache.StringGetAsync(key);
            }
            else
            {
                return null;
            }
        }

        private async Task DeleteFromCache(string subdomain)
        {
            if (connectionMultiplexer.IsConnected)
            {
                var cache = connectionMultiplexer.GetDatabase();
                await cache.KeyDeleteAsync(subdomain).ConfigureAwait(false);
            }
        }

        private async Task AddToCache(string key, string serializedData)
        {
            var GetMessagesCacheExpiryMinutes = 5;
            if (connectionMultiplexer.IsConnected)
            {
                var cache = connectionMultiplexer.GetDatabase();

                TimeSpan expiresIn;
                expiresIn = new TimeSpan(0, GetMessagesCacheExpiryMinutes, 0);
                await cache.StringSetAsync(key, serializedData, expiresIn).ConfigureAwait(false);

            }
        }
    }
}