using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace NancyApplication {
    public interface ISubscriptionRepository
    {
        Task<ActionResult<Subscription>> AddSubscription(Subscription Subscription);
        Task<HttpStatusCode> DeleteSubscription(string subscriptionId, string accountId);
        ActionResult<Subscription> GetSubscriptionByConfirmation(string confirmationToken, string accountId, string sessionToken);
        ActionResult<Subscription> GetSubscriptionByTopic(string topicId, string accountId, string sessionToken);
        Task<ActionResult<Subscription>> UpdateSubscription(Subscription subcription, string sessionToken);
    }
}