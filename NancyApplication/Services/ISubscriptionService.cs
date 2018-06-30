using System.Collections.Generic;

namespace NancyApplication {
    public interface ISubscriptionService
        {
            string CreateSubscription(string accountId, string topicId);
            void ConfirmSubscription(string confirmationToken, string accountId);
            void DeleteSubscription(string subscriptionId, string accountId);
        }
}