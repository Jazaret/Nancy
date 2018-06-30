using System;
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
            var subscription = new Subscription(topicId,accountId);
            _subRepo.AddSubscription(subscription);
            return subscription.ConfirmationToken;
        }

        /// <summary>
        /// Updating subscription to set the status as confirmed
        /// We can consider adding a dateTime UTC stamp if we want more information about when it was confirmed
        /// </summary>        
        public void ConfirmSubscription(string confirmationToken, string accountId) {

            var subscription = _subRepo.GetSubscription(confirmationToken,accountId);
            if (subscription != null && !subscription.SubscriptionConfirmed) {
                subscription.SubscriptionConfirmed = true;
                _subRepo.UpdateSubscription(subscription);
            }
        }

        /// <summary>
        /// Deletes the subscription from the repository
        /// </summary>
        public void DeleteSubscription(string subscriptionId, string accountId)
        {
            _subRepo.DeleteSubscription(subscriptionId, accountId);
        }
    }
}