using System.Collections.Generic;
using System.Threading.Tasks;

namespace NancyApplication {
    public interface ISubscriptionRepository
    {
        string AddSubscriptionRequest(string accountId, string topicId);
        bool ConfirmSubscription(string confirmationToken, string accountId);
        Task DeleteSubscription(string subscriptionId, string accountId);
    }
}