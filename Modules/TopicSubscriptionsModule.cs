namespace NancyApplication
{
    using Microsoft.Extensions.Caching.Distributed;
    using Nancy;
    using System;
    using System.Collections.Generic;

    public class TopicSubscriptionsModule : NancyModule
    {
        private ISubscriptionService _subscriptionService;

        public TopicSubscriptionsModule(ISubscriptionService subcriptionService)
        {
            _subscriptionService = subcriptionService;

            Post("Topics/{topicId}/Subscribe/{accountId}", args =>
            {
                string result = _subscriptionService.CreateSubscription(args.name, args.accountId);
                return Response.AsJson(new { ConfirmationToken = result });
            });

            Post("Topics/Subscribe/{confirmationToken}", args =>
            {
                _subscriptionService.ConfirmSubscription(args.confirmationToken);
                return HttpStatusCode.OK;
            });

            Delete("Topics/Subscription/{subscriptionId}", args => {
                _subscriptionService.DeleteSubscription(args.subscriptionId);
                return HttpStatusCode.OK;
            });
        }
    }
}
