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
            Post("Topics/{topicId}/Subscribe?u={accountId}", args =>
            {
                string result = _subscriptionService.CreateSubscription(args.accountId, args.topicId);
                return Response.AsJson(new { ConfirmationToken = result });
            });

            //Confirm topic subscription
            Post("Topics/Subscribe/{confirmationToken}?u={accountId}", args =>
            {
                _subscriptionService.ConfirmSubscription(args.confirmationToken, args.accountId);
                return HttpStatusCode.OK;
            });

            //Delete topic
            Delete("Topics/Subscription/{subscriptionId}?u={accountId}", args => {
                _subscriptionService.DeleteSubscription(args.subscriptionId, args.accountId);
                return HttpStatusCode.OK;
            });
        }
    }
}
