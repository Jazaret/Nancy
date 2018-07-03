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
        public ActionResult<Subscription> CreateSubscription(string accountId, string topicId, string sessionToken) {
            var result = new ActionResult<Subscription>();

            //Verify topic exists
            var getTopicResponse = _topicRepo.GetTopic(topicId);
            if (getTopicResponse ==  null || getTopicResponse.resposeObject == null) { 
                result.statusCode = HttpStatusCode.BadRequest;
                return result; 
            }
            //See if subscription already exists for this account/topic
            var existingSubResult = _subRepo.GetSubscriptionByTopic(topicId,accountId, sessionToken);
            if (existingSubResult?.resposeObject != null) { 
                result.statusCode = HttpStatusCode.BadRequest;
                return result; 
            }

            var subscription = new Subscription(topicId,accountId);
            result = _subRepo.AddSubscription(subscription).Result;
            return result;
        }

        /// <summary>
        /// Updating subscription to set the status as confirmed
        /// We can consider adding a dateTime UTC stamp if we want more information about when it was confirmed
        /// </summary>        
        public ActionResult<Subscription> ConfirmSubscription(string confirmationToken, string accountId, string sessionToken) {
            var result = new ActionResult<Subscription>();

            var getSubResult = _subRepo.GetSubscriptionByConfirmation(confirmationToken,accountId, sessionToken);
            if (getSubResult == null || getSubResult.resposeObject == null) { 
                result.statusCode = HttpStatusCode.NoContent;
                return result;
            }
            var subscription = getSubResult.resposeObject;
            if (subscription.SubscriptionConfirmed) { 
                result.statusCode = HttpStatusCode.NotModified;
                return result;
            }

            subscription.SubscriptionConfirmed = true;
            var updateTaskResult = _subRepo.UpdateSubscription(subscription,sessionToken).Result;
            return updateTaskResult;
        }

        /// <summary>
        /// Deletes the subscription from the repository
        /// </summary>
        public HttpStatusCode DeleteSubscription(string subscriptionId, string accountId)
        {
            return _subRepo.DeleteSubscription(subscriptionId, accountId).Result;
        }
    }
}