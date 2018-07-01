namespace NancyApplication
{
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
                var links = new List<HyperMedia>{
                    new HyperMedia { 
                        Href = this.Request.Url, 
                        Rel = "self" 
                    },
                    new HyperMedia {
                        Href = $"{this.Request.Url.SiteBase}/Subscriptions/{accountId}/Confirm/{result}", 
                        Rel = "confirm"
                    }
                };             
                return Response.AsJson(links);
            });

            //Confirm topic subscription
            Put("Subscriptions/{accountId}/Confirm/{confirmationToken}", args =>
            {
                var confirmationToken = args.confirmationToken;
                var accountId = args.accountId;
                var resultStatusCode = _subscriptionService.ConfirmSubscription(confirmationToken, accountId);
                if (resultStatusCode != HttpStatusCode.OK) { return resultStatusCode;}
                var links = new List<HyperMedia>{
                    new HyperMedia { 
                        Href = this.Request.Url, 
                        Rel = "self" 
                    }
                };             
                return Response.AsJson(links);        
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
