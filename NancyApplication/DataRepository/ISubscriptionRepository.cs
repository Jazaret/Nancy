using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace NancyApplication {
    public interface ISubscriptionRepository
    {
        Task<HttpStatusCode> AddSubscription(Subscription Subscription);
        Task<HttpStatusCode> DeleteSubscription(string subscriptionId, string accountId);
        Subscription GetSubscriptionByConfirmation(string confirmationToken, string accountId);
        Subscription GetSubscriptionByTopic(string topicId, string accountId);
        Task<HttpStatusCode> UpdateSubscription(Subscription subcription);
    }
}