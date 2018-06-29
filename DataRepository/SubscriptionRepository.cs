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

    public class SubscriptionRepostiory : BaseRepository {
        protected const string SubscriptionCollection = "SubscriptionCollection";
        public SubscriptionRepostiory() : base() {
            Initialize().Wait();
        }

        private async Task Initialize()
        {
            DocumentCollection myCollection = new DocumentCollection();
            myCollection.Id = SubscriptionCollection;
            myCollection.PartitionKey.Paths.Add("/accountId");
            await this.client.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri(TopicsDB), new DocumentCollection { Id = SubscriptionCollection });
        }

        public string AddSubscriptionRequest(string accountId, string topicId) {
            //create confirmation token and return
            throw new Exception("Not implemented yet");
        }

        public Subscription ConfirmSubscription(string confirmationToken) {
            throw new Exception("Not implemented yet");
        }

        private async Task ReplaceDocument(Subscription subscription)
        {

        }        
    }
}