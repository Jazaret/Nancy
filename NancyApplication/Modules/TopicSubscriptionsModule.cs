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
                var result = _subscriptionService.CreateSubscription(accountId, topicId); 
                if (result == null) { return HttpStatusCode.InternalServerError; }
                var confirmationToken = result.ConfirmationToken;  
                var subId = result.Id;
                var links = new List<HyperMedia>{
                    new HyperMedia { 
                        Href = this.Request.Url, 
                        Rel = "self" 
                    },
                    new HyperMedia {
                        Href = $"{this.Request.Url.SiteBase}/Subscriptions/{accountId}/Confirm/{confirmationToken}", 
                        Rel = "confirm"
                    },
                    new HyperMedia {
                        Href = $"{this.Request.Url.SiteBase}/Subscriptions/{accountId}/Subscription/{subId}", 
                        Rel = "delete"
                    }
                };             
                return Response.AsJson(new {ConfirmationToken = confirmationToken, links = links});
            });

            //Confirm topic subscription
            Put("Subscriptions/{accountId}/Confirm/{confirmationToken}", args =>
            {
                var confirmationToken = args.confirmationToken;
                var accountId = args.accountId;
                var resultStatusCode = _subscriptionService.ConfirmSubscription(confirmationToken, accountId);
                if ((System.Net.HttpStatusCode)resultStatusCode != (System.Net.HttpStatusCode)HttpStatusCode.OK) 
                {
                    //If status is Precondition failed then there is a concurrency violation.
                    if ((System.Net.HttpStatusCode)resultStatusCode == (System.Net.HttpStatusCode)HttpStatusCode.PreconditionFailed) {
                        return "There is an update conflit on this subscription. Please refresh the status of the subscription and try again";
                    }
                    return resultStatusCode;
                }
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
