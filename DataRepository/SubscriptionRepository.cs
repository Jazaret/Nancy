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

    public class SubscriptionRepostiory : BaseRepository, ISubscriptionRepository {
        protected const string SubscriptionCollection = "SubscriptionCollection";
        public SubscriptionRepostiory() : base() {
            Initialize().Wait();
        }

        public string AddSubscriptionRequest(string accountId, string topicId)
        {
            throw new NotImplementedException();
        }

        public void ConfirmSubscription(string confirmationToken)
        {
            throw new NotImplementedException();
        }

        private async Task Initialize()
        {
            DocumentCollection myCollection = new DocumentCollection();
            myCollection.Id = SubscriptionCollection;
            myCollection.PartitionKey.Paths.Add("/accountId");
            await this.client.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri(TopicsDB), new DocumentCollection { Id = SubscriptionCollection });
        }
    }
}