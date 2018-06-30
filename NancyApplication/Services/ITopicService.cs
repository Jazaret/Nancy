using System.Collections.Generic;

namespace NancyApplication {
    public interface ITopicService
        {
            IEnumerable<Topic> GetAllTopics();
            IEnumerable<Topic> SearchForNews(string news);
        }
}