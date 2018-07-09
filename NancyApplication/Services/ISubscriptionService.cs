using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace NancyApplication {
    public interface ISubscriptionService
        {
            Task<ActionResult<Subscription>> CreateSubscription(string accountId, string topicId, string sessionToken);
            Task<ActionResult<Subscription>> ConfirmSubscription(string confirmationToken, string accountId, string sessionToken);
            Task<HttpStatusCode> DeleteSubscription(string subscriptionId, string accountId);
        }
}