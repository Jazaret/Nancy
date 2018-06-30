using System.Collections.Generic;
using System.Net;

namespace NancyApplication {
    public interface ISubscriptionService
        {
            string CreateSubscription(string accountId, string topicId);
            HttpStatusCode ConfirmSubscription(string confirmationToken, string accountId);
            void DeleteSubscription(string subscriptionId, string accountId);
        }
}