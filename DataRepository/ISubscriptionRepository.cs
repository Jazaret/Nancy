using System.Collections.Generic;

namespace NancyApplication {
    public interface ISubscriptionRepository
    {
        string AddSubscriptionRequest(string accountId, string topicId);
        void ConfirmSubscription(string confirmationToken);
    }
}