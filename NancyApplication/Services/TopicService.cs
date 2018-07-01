using System;
using System.Collections.Generic;
using System.Net;
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

        private ICacheService _cacheService;
        public TopicService(ITopicRepository topicRepository, ICacheService cacheService) {
            _topicRepo = topicRepository;
            _cacheService = cacheService;
        }

        /// <summary>
        /// Get list of all topics
        /// </summary>
        /// <returns>list of all topics</returns>
        public ActionResult<IEnumerable<Topic>> GetAllTopics()
        {
            return  _topicRepo.GetTopics();
        }

        /// <summary>
        /// Get list of topics that contains the parameter. Uses caching for performance.
        /// </summary>
        /// <param name="news">paramter to search for</param>
        /// <returns>list of topics that contains parameter string</returns>
        public ActionResult<IEnumerable<Topic>> SearchForNews(string news) {

            string cacheResult = null;
            
            cacheResult = _cacheService.GetFromCache(news).Result;
            if (cacheResult != null) { 
                return JsonConvert.DeserializeObject<ActionResult<IEnumerable<Topic>>>(cacheResult);
            }

            var result = _topicRepo.SearchForTopics(news);

            if (result != null && result.statusCode == HttpStatusCode.OK) {
                var addCacheResult = _cacheService.AddToCache(news,JsonConvert.SerializeObject(result));
            }

            return result;
        }
    }
}