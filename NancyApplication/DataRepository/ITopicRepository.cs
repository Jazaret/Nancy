using System.Collections.Generic;

namespace NancyApplication {
    public interface ITopicRepository
    {
        ActionResult<IEnumerable<Topic>> GetTopics();
        ActionResult<IEnumerable<Topic>> SearchForTopics(string news);
        ActionResult<Topic> GetTopic(string id);
    }
}