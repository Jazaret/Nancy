using System.Collections.Generic;
using System.Threading.Tasks;

namespace NancyApplication {
    public interface ITopicService
        {
            Task<ActionResult<IEnumerable<Topic>>> GetAllTopics();
            Task<ActionResult<IEnumerable<Topic>>> SearchForNews(string news);
        }
}