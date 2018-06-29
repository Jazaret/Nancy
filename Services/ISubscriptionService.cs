using System.Collections.Generic;

namespace NancyApplication {
    public interface ISubscriptionService
        {
            string CreateSubscription(string accountId, string topicId);
            void ConfirmSubscription(string confirmationToken);
            void DeleteSubscription(string subscriptionId);
        }
}