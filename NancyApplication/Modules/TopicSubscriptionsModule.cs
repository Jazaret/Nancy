namespace NancyApplication
{
    using Nancy;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Nancy module that handles the Subscription endpoints
    /// </summary>
    public class TopicSubscriptionsModule : NancyModule
    {       
        private const string _sessionTokenCookieName = "_sessionToken";
        private ISubscriptionService _subscriptionService;

        public TopicSubscriptionsModule(ISubscriptionService subcriptionService)
        {
            _subscriptionService = subcriptionService;

            //Subscribe to topic
            Post("Subscriptions/{accountId}/Subscribe/{topicId}", async args =>
            {
                var accountId = args.accountId;
                var topicId = args.topicId;                
                var sessionCookie = Request.Headers.Cookie.FirstOrDefault(c => c.Name == _sessionTokenCookieName);
                var sessionToken = sessionCookie == null ? null : sessionCookie.Value;
                ActionResult<Subscription> result = await _subscriptionService.CreateSubscription(accountId, topicId, sessionToken); 
                if (result.statusCode != (System.Net.HttpStatusCode)HttpStatusCode.Created){
                    return result.statusCode;
                }
                var subscription = result.resposeObject;
                var confirmationToken = subscription.ConfirmationToken;  
                var subId = subscription.Id;
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
                var response = Response.AsJson(new {ConfirmationToken = confirmationToken, links = links});
                if (!string.IsNullOrWhiteSpace(result.sessionToken)) {
                    await response.WithCookie(_sessionTokenCookieName,result.sessionToken);
                }
                return response;
            });

            //Confirm topic subscription
            Put("Subscriptions/{accountId}/Confirm/{confirmationToken}", async args =>
            {
                var confirmationToken = args.confirmationToken;
                var accountId = args.accountId;
                var sessionCookie = Request.Headers.Cookie.FirstOrDefault(c => c.Name == _sessionTokenCookieName);
                var sessionToken = sessionCookie == null ? null : sessionCookie.Value;
                ActionResult<Subscription> result = await _subscriptionService.ConfirmSubscription(confirmationToken, accountId, sessionToken);
                if (result.statusCode != (System.Net.HttpStatusCode)HttpStatusCode.OK) 
                {
                    //If status is Precondition failed then there is a concurrency violation.
                    if (result.statusCode == (System.Net.HttpStatusCode)HttpStatusCode.PreconditionFailed) {
                        return "There is an update conflit on this subscription. Please refresh the status of the subscription and try again";
                    }
                    return result.statusCode;
                }
                var subscription = result.resposeObject;
                var subId = subscription.Id;
                var links = new List<HyperMedia>{
                    new HyperMedia { 
                        Href = this.Request.Url, 
                        Rel = "self" 
                    },
                    new HyperMedia {
                        Href = $"{this.Request.Url.SiteBase}/Subscriptions/{accountId}/Subscription/{subId}", 
                        Rel = "delete"
                    }
                };             
                var response = Response.AsJson(links);
                if (!string.IsNullOrWhiteSpace(result.sessionToken)) {
                    await response.WithCookie(_sessionTokenCookieName,result.sessionToken);
                }                
                return Response.AsJson(links);        
            });

            //Delete topic
            Delete("Subscriptions/{accountId}/Subscription/{subscriptionId}", async args => {
                var accountId = args.accountId;
                var subId = args.subscriptionId;
                var result = await _subscriptionService.DeleteSubscription(subId, accountId);
                return result;
            });
        }
    }
}
