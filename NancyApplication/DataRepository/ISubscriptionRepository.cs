using System.Collections.Generic;
using System.Threading.Tasks;

namespace NancyApplication {
    public interface ISubscriptionRepository
    {
        Task AddSubscription(Subscription Subscription);
        Task DeleteSubscription(string subscriptionId, string accountId);
        Subscription GetSubscription(string confirmationToken, string accountId);
        Task UpdateSubscription(Subscription subcription);
    }
}