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
            DocumentCollection subscriptionsCollectionDefinition = new DocumentCollection();
            subscriptionsCollectionDefinition.Id = SubscriptionCollection;
            subscriptionsCollectionDefinition.PartitionKey.Paths.Add("/AccountID");
            this.Collection = await this.Client.CreateDocumentCollectionIfNotExistsAsync(
                UriFactory.CreateDatabaseUri(TopicsDB),
                subscriptionsCollectionDefinition,
                new RequestOptions { OfferThroughput = 10100 });
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
            return await ReplaceSubscriptionOptimistic(subscription);
        }

        /// <summary>
        /// Retrieves a subscription document using the confirmation token and the accountid
        /// </summary>
        public Subscription GetSubscriptionByConfirmation(string confirmationToken, string accountId) {
            return this.Client.CreateDocumentQuery<Subscription>(
                UriFactory.CreateDocumentCollectionUri(TopicsDB, SubscriptionCollection))
                .Where(c => c.ConfirmationToken == confirmationToken && c.AccountID == accountId).AsEnumerable().FirstOrDefault();                 
        }

        /// <summary>
        /// Retrieves a subscription document using the topic id and the accountid
        /// </summary>
        public Subscription GetSubscriptionByTopic(string topicId, string accountId) {
            return this.Client.CreateDocumentQuery<Subscription>(
                UriFactory.CreateDocumentCollectionUri(TopicsDB, SubscriptionCollection))
                .Where(c => c.TopicID == topicId && c.AccountID == accountId).AsEnumerable().FirstOrDefault();                 
        }        

        /// <summary>
        /// Delete subscription document from collection. Requires partition key
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <returns></returns>
        public async Task<HttpStatusCode> DeleteSubscription(string subscriptionId, string accountId)
        {
            try {
                var result = await this.Client.DeleteDocumentAsync(
                    UriFactory.CreateDocumentUri(TopicsDB, SubscriptionCollection, subscriptionId)
                    ,new RequestOptions { PartitionKey = new PartitionKey(accountId) });
                    return result.StatusCode;
            } catch (DocumentClientException ex) {
                Console.WriteLine(ex.Message);
                return ex.StatusCode.Value;
            } 
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
                return HttpStatusCode.InternalServerError;
            }
        }

        /// <summary>
        /// Create Subscription document in collection.
        /// </summary>
        private async Task<HttpStatusCode> CreateSubscriptionDocument(Subscription subscription)
        {
            try {
                var result = await this.Client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(TopicsDB, SubscriptionCollection), subscription);
                return result.StatusCode;
            } catch (DocumentClientException ex) {
                Console.WriteLine(ex.Message);
                return ex.StatusCode.Value;
            } 
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
                return HttpStatusCode.InternalServerError;
            }
        }     

        /// <summary>
        /// Replace account document in repository. Uses ETag match for optimistic concurrency
        /// </summary>
        public async Task<HttpStatusCode> ReplaceSubscriptionOptimistic(Subscription item) {
            try {            
                var document = (from f in this.Client.CreateDocumentQuery(
                                this.Collection.SelfLink, new FeedOptions{ PartitionKey = new PartitionKey(item.AccountID)})
                                where f.Id == item.Id
                                select f).AsEnumerable().FirstOrDefault();

                if (document == null) {return HttpStatusCode.NotFound;}

                var editSubscription = (Subscription)(dynamic) document;
                editSubscription.ReplaceWith(item);

                var ac = new AccessCondition {Condition = document.ETag, Type = AccessConditionType.IfMatch};
                
                var result = await this.Client.ReplaceDocumentAsync(document.SelfLink, editSubscription, new RequestOptions {AccessCondition = ac});
                return result.StatusCode;
            } catch (DocumentClientException ex) {
                Console.WriteLine(ex.Message + " " + ex.StatusCode);
                return ex.StatusCode.Value;
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
                return HttpStatusCode.InternalServerError;
            }
        }

    }
}