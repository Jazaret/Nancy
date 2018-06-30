namespace NancyApplication
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Net;
    using System.Collections.Generic;
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;
    using Newtonsoft.Json;

    /// <summary>
    /// Repository that handles the operations for the Subscription documents in DynamoDB
    /// </summary>
    public class SubscriptionRepostiory : BaseRepository, ISubscriptionRepository {
        protected const string SubscriptionCollection = "SubscriptionCollection";

        public SubscriptionRepostiory() : base() {
            Initialize().Wait();
        }
        
        /// <summary>
        /// Creates the collection if it does not exist. Note the Partition key is added. 
        /// </summary>
        private async Task Initialize()
        {
            DocumentCollection subscriptions = new DocumentCollection();
            subscriptions.Id = SubscriptionCollection;
            subscriptions.PartitionKey.Paths.Add("/AccountID");
            Collection = await this.Client.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri(TopicsDB), new DocumentCollection { Id = SubscriptionCollection });
        }

        /// <summary>
        /// Adds subscription document to the collection and sets the Subscription to unconfirmed until the ConfirmSubscription method is called.
        /// </summary>
        /// <returns>the confirmationToken the user must specify when confirming subscription</returns>
        public async Task<HttpStatusCode> AddSubscription(Subscription subscription)
        {
            return await CreateSubscriptionDocument(subscription);
        }

        /// <summary>
        /// Updates the subscription document. Uses ETag match for optimistic concurrency
        /// </summary>
        /// <param name="subscription"></param>
        /// <returns></returns>
        public async Task<HttpStatusCode> UpdateSubscription(Subscription subscription) {
            var ac = new AccessCondition {Condition = subscription.ETag, Type = AccessConditionType.IfMatch};
            var result = await this.Client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(TopicsDB, TopicsDB, subscription.Id), subscription,new RequestOptions {AccessCondition = ac});       
            return result.StatusCode;
        }

        /// <summary>
        /// Retrieves a subscription document using the confirmation token and the accountid
        /// </summary>
        public Subscription GetSubscription(string confirmationToken, string accountId) {
            return this.Client.CreateDocumentQuery<Subscription>(
                UriFactory.CreateDocumentCollectionUri(TopicsDB, SubscriptionCollection))
                .Where(c => c.ConfirmationToken == confirmationToken && c.AccountID == accountId).FirstOrDefault();     
        }

        /// <summary>
        /// Delete subscription document from collection. Requires partition key
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <returns></returns>
        public async Task<HttpStatusCode> DeleteSubscription(string subscriptionId, string accountId)
        {
            var result = await this.Client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(TopicsDB, SubscriptionCollection, subscriptionId), new RequestOptions {PartitionKey = new PartitionKey(accountId)});
            return result.StatusCode;
        }

        private async Task<HttpStatusCode> CreateSubscriptionDocument(Subscription subscription)
        {
            try
            {
                var readResult = await Client.ReadDocumentAsync(UriFactory.CreateDocumentUri(TopicsDB, SubscriptionCollection, subscription.Id),new RequestOptions { PartitionKey = new PartitionKey(subscription.AccountID) });
                return readResult.StatusCode;
            }
            catch (DocumentClientException de)
            {
                if (de.StatusCode == HttpStatusCode.NotFound)
                {
                    var result = await this.Client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(TopicsDB, SubscriptionCollection), subscription);
                    return result.StatusCode;
                }
                else
                {
                    throw;
                }
            }
        }     

    }
}