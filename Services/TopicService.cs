using System.Collections.Generic;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace NancyApplication {
    /// <summary>
    /// Service that handles actions on Topics
    /// </summary>
    public class TopicService : ITopicService
    {
        ITopicRepository _topicRepo;
        IDistributedCache _cache;

        public TopicService(ITopicRepository topicRepository) {
            _topicRepo = topicRepository;
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
            if (_cache != null) {
                cacheResult = _cache.GetString(news);
                if (cacheResult != null) { 
                    return JsonConvert.DeserializeObject<List<Topic>>(cacheResult);
                }
            }

            var result = _topicRepo.SearchForTopics(news);
            
            if (_cache != null) {
                _cache.SetString(news,JsonConvert.SerializeObject(result));
            }

            return result;
        }
    }
}