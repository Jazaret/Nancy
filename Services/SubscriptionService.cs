using System.Collections.Generic;

namespace NancyApplication {
    public class SubscriptionService : ISubscriptionService
    {
        ISubscriptionRepository _subRepo;

        public SubscriptionService(ISubscriptionRepository subscriptionRepository) {
            _subRepo = subscriptionRepository;
        }
        
        public string CreateSubscription(string accountId, string topicId) {
            var result = "";
            return result;
        }

        public void ConfirmSubscription(string confirmationToken) {
            return;
        }

        public void DeleteSubscription(string subscriptionId)
        {
            return;
        }
    }
}