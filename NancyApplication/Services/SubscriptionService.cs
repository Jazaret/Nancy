using System;
using System.Collections.Generic;
using System.Net;

namespace NancyApplication {
    /// <summary>
    /// Service that handles actions on Subscriptions
    /// </summary>
    public class SubscriptionService : ISubscriptionService
    {
        ISubscriptionRepository _subRepo;
        ITopicRepository _topicRepo;

        public SubscriptionService(ISubscriptionRepository subscriptionRepository, ITopicRepository topicRepository) {
            _subRepo = subscriptionRepository;
            _topicRepo = topicRepository;
        }
        
        /// <summary>
        /// Creates a new subscription in the repository using the accountId and topicId.
        /// Verifies that topic exists and subscription does not
        /// </summary>
        /// <returns>new created subscription if successful</returns>
        public Subscription CreateSubscription(string accountId, string topicId) {
            //Verify topic exists
            var topic = _topicRepo.GetTopic(topicId);
            if (topic == null) { return null; }
            //See if subscription already exists for this account/topic
            var existingSub = _subRepo.GetSubscriptionByTopic(topicId,accountId);
            if (existingSub != null) { return null; }

            var subscription = new Subscription(topicId,accountId);
            var addResult = _subRepo.AddSubscription(subscription);
            return subscription;
        }

        /// <summary>
        /// Updating subscription to set the status as confirmed
        /// We can consider adding a dateTime UTC stamp if we want more information about when it was confirmed
        /// </summary>        
        public HttpStatusCode ConfirmSubscription(string confirmationToken, string accountId) {

            var subscription = _subRepo.GetSubscriptionByConfirmation(confirmationToken,accountId);
            if (subscription == null) { return HttpStatusCode.NoContent;}
            if (subscription.SubscriptionConfirmed) { return HttpStatusCode.NotModified;}

            subscription.SubscriptionConfirmed = true;
            var updateTaskResult = _subRepo.UpdateSubscription(subscription).Result;
            return updateTaskResult;
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