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
        public string AddSubscriptionRequest(string accountId, string topicId)
        {
            var confirmationToken = Guid.NewGuid().ToString();
            var subscription = new Subscription {
                ID = Guid.NewGuid().ToString(),
                AccountID = accountId,
                TopicID = topicId,
                ConfirmationToken = confirmationToken,
                SubscriptionConfirmed = false
            };
            CreateSubscriptionDocument(subscription).Wait();
            return confirmationToken;
        }

        /// <summary>
        /// Updating subscription document to set the status as confirmed
        /// We can consider adding a dateTime UTC stamp if we want more information about when it was confirmed
        /// </summary>
        public bool ConfirmSubscription(string confirmationToken, string accountId)
        {
            var subscription = this.client.CreateDocumentQuery<Subscription>(
                    UriFactory.CreateDocumentCollectionUri(TopicsDB, SubscriptionCollection))
                    .Where(c => c.ConfirmationToken == confirmationToken && c.AccountID == accountId).FirstOrDefault();                                            

            if (subscription != null && !subscription.SubscriptionConfirmed) {
                subscription.SubscriptionConfirmed = true;
                UpdateSubscriptionDocument(subscription).Wait();
                return true;
            }

            return false;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="subscription"></param>
        /// <returns></returns>
        private async Task UpdateSubscriptionDocument(Subscription subscription)
        {
            await this.client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(TopicsDB, TopicsDB, subscription.ID), subscription);
        }        


        private async Task CreateSubscriptionDocument(Subscription subscription)
        {
            try
            {
                await this.client.ReadDocumentAsync(UriFactory.CreateDocumentUri(TopicsDB, SubscriptionCollection, subscription.ID));
                await client.ReadDocumentAsync(UriFactory.CreateDocumentUri(TopicsDB, SubscriptionCollection, subscription.ID),new RequestOptions { PartitionKey = new PartitionKey(subscription.AccountID) });
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