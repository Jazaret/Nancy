using System.Collections.Generic;
using System.Net;

namespace NancyApplication {
    public interface ISubscriptionService
        {
            ActionResult<Subscription> CreateSubscription(string accountId, string topicId);
            ActionResult<Subscription> ConfirmSubscription(string confirmationToken, string accountId);
            HttpStatusCode DeleteSubscription(string subscriptionId, string accountId);
        }
}