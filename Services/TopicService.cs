using System.Collections.Generic;

namespace NancyApplication {
    public class TopicService : ITopicService
    {
        ITopicRepository _topicRepo;

        public TopicService(ITopicRepository topicRepository) {
            _topicRepo = topicRepository;
        }
        public IEnumerable<Topic> GetAllTopics()
        {
            return _topicRepo.GetTopics();
        }

        public IEnumerable<Topic> SearchForNews(string news) {
            return _topicRepo.SearchForTopics(news);
        }

        public string CreateSubscription(string accountId, string topicId) {
            var result = "";
            return result;
        }

        public void ConfirmSubscription(string confirmationToken) {
            return;
        }
    }
}