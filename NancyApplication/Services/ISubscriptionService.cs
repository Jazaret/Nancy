using System.Collections.Generic;
using System.Net;

namespace NancyApplication {
    public interface ISubscriptionService
        {
            ActionResult<Subscription> CreateSubscription(string accountId, string topicId, string sessionToken);
            ActionResult<Subscription> ConfirmSubscription(string confirmationToken, string accountId, string sessionToken);
            HttpStatusCode DeleteSubscription(string subscriptionId, string accountId);
        }
}