using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace NancyApplication {
    public interface ISubscriptionRepository
    {
        Task<ActionResult<Subscription>> AddSubscription(Subscription Subscription);
        Task<HttpStatusCode> DeleteSubscription(string subscriptionId, string accountId);
        ActionResult<Subscription> GetSubscriptionByConfirmation(string confirmationToken, string accountId);
        ActionResult<Subscription> GetSubscriptionByTopic(string topicId, string accountId);
        Task<ActionResult<Subscription>> UpdateSubscription(Subscription subcription);
    }
}