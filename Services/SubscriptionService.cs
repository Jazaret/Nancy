using System.Collections.Generic;

namespace NancyApplication {
    /// <summary>
    /// Service that handles actions on Subscriptions
    /// </summary>
    public class SubscriptionService : ISubscriptionService
    {
        ISubscriptionRepository _subRepo;

        public SubscriptionService(ISubscriptionRepository subscriptionRepository) {
            _subRepo = subscriptionRepository;
        }
        
        public string CreateSubscription(string accountId, string topicId) {
            return _subRepo.AddSubscriptionRequest(accountId,topicId);
        }

        public void ConfirmSubscription(string confirmationToken, string accountId) {
            _subRepo.ConfirmSubscription(confirmationToken, accountId);
        }

        public void DeleteSubscription(string subscriptionId, string accountId)
        {
            _subRepo.DeleteSubscription(subscriptionId, accountId);
        }
    }
}