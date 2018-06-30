using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace NancyApplication {
    public interface ISubscriptionRepository
    {
        Task<HttpStatusCode> AddSubscription(Subscription Subscription);
        Task<HttpStatusCode> DeleteSubscription(string subscriptionId, string accountId);
        Subscription GetSubscription(string confirmationToken, string accountId);
        Task<HttpStatusCode> UpdateSubscription(Subscription subcription);
    }
}