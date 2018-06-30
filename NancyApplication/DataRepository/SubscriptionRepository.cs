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
            await this.client.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri(TopicsDB), new DocumentCollection { Id = SubscriptionCollection });
        }

        /// <summary>
        /// Adds subscription document to the collection and sets the Subscription to unconfirmed until the ConfirmSubscription method is called.
        /// </summary>
        /// <returns>the confirmationToken the user must specify when confirming subscription</returns>
        public async Task AddSubscription(Subscription subscription)
        {
            await CreateSubscriptionDocument(subscription);
        }

        /// <summary>
        /// Updates the subscription document
        /// </summary>
        /// <param name="subscription"></param>
        /// <returns></returns>
        public async Task UpdateSubscription(Subscription subscription) {
            await this.client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(TopicsDB, TopicsDB, subscription.Id), subscription);
        }

        /// <summary>
        /// Retrieves a subscription document using the confirmation token and the accountid
        /// </summary>
        public Subscription GetSubscription(string confirmationToken, string accountId) {
            return this.client.CreateDocumentQuery<Subscription>(
                UriFactory.CreateDocumentCollectionUri(TopicsDB, SubscriptionCollection))
                .Where(c => c.ConfirmationToken == confirmationToken && c.AccountID == accountId).FirstOrDefault();             
        }

        /// <summary>
        /// Delete subscription document from collection. Requires partition key
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <returns></returns>
        public async Task DeleteSubscription(string subscriptionId, string accountId)
        {
            await this.client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(TopicsDB, SubscriptionCollection, subscriptionId), new RequestOptions {PartitionKey = new PartitionKey(accountId)});
        }

        private async Task CreateSubscriptionDocument(Subscription subscription)
        {
            try
            {
                await client.ReadDocumentAsync(UriFactory.CreateDocumentUri(TopicsDB, SubscriptionCollection, subscription.Id),new RequestOptions { PartitionKey = new PartitionKey(subscription.AccountID) });
            }
            catch (DocumentClientException de)
            {
                if (de.StatusCode == HttpStatusCode.NotFound)
                {
                    await this.client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(TopicsDB, SubscriptionCollection), subscription);
                }
                else
                {
                    throw;
                }
            }
        }     

    }
}