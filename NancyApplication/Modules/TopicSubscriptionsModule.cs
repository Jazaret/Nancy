namespace NancyApplication
{
    using Microsoft.Extensions.Caching.Distributed;
    using Nancy;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Nancy module that handles the Subscription endpoints
    /// </summary>
    public class TopicSubscriptionsModule : NancyModule
    {
        private ISubscriptionService _subscriptionService;

        public TopicSubscriptionsModule(ISubscriptionService subcriptionService)
        {
            _subscriptionService = subcriptionService;

            //Subscribe to topic
            Post("Subscriptions/{accountId}/Subscribe/{topicId}", args =>
            {
                var accountId = args.accountId;
                var topicId = args.topicId;
                string result = _subscriptionService.CreateSubscription(accountId, topicId);                
                return Response.AsJson(new { ConfirmationToken = result });
            });

            //Confirm topic subscription
            Put("Subscriptions/{accountId}/Confirm/{confirmationToken}", args =>
            {
                var confirmationToken = args.confirmationToken;
                var accountId = args.accountId;
                var resultStatusCode = _subscriptionService.ConfirmSubscription(confirmationToken, accountId);
                return resultStatusCode;        
            });

            //Delete topic
            Delete("Subscriptions/{accountId}/Subscription/{subscriptionId}", args => {
                var accountId = args.accountId;
                var subId = args.subscriptionId;
                _subscriptionService.DeleteSubscription(subId, accountId);
                return HttpStatusCode.OK;
            });
        }
    }
}
