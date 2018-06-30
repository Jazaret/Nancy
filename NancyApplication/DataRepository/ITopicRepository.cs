using System.Collections.Generic;

namespace NancyApplication {
    public interface ITopicRepository
    {
        IEnumerable<Topic> GetTopics();
        IEnumerable<Topic> SearchForTopics(string news);
    }
}