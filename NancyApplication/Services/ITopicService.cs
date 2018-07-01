using System.Collections.Generic;

namespace NancyApplication {
    public interface ITopicService
        {
            ActionResult<IEnumerable<Topic>> GetAllTopics();
            ActionResult<IEnumerable<Topic>> SearchForNews(string news);
        }
}