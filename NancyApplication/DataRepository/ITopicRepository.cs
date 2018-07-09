using System.Collections.Generic;
using System.Threading.Tasks;

namespace NancyApplication {
    public interface ITopicRepository
    {
        Task<ActionResult<IEnumerable<Topic>>> GetTopics();
        Task<ActionResult<IEnumerable<Topic>>> SearchForTopics(string news);
        Task<ActionResult<Topic>> GetTopic(string id);
    }
}