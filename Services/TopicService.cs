using System.Collections.Generic;

namespace NancyApplication {
    /// <summary>
    /// Service that handles actions on Topics
    /// </summary>
    public class TopicService : ITopicService
    {
        ITopicRepository _topicRepo;

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
        /// Get list of topics that contains the parameter
        /// </summary>
        /// <param name="news">paramter to search for</param>
        /// <returns>list of topics that contains parameter string</returns>
        public IEnumerable<Topic> SearchForNews(string news) {
            return _topicRepo.SearchForTopics(news);
        }
    }
}