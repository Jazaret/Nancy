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

    public class TaskRepository : BaseRepository {

        public TaskRepository() : base() {
            Initialize().Wait();
        }

        private async Task Initialize()
        {
            DocumentCollection myCollection = new DocumentCollection();
            myCollection.Id = TopicsCollection;
            //myCollection.PartitionKey.Paths.Add("/deviceId");
            await this.client.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri(TopicsDB), new DocumentCollection { Id = TopicsCollection });
        }

        public List<Topic> GetTopics() {
            FeedOptions queryOptions = new FeedOptions { MaxItemCount = -1 };

            var result = this.client.CreateDocumentQuery<Topic>(
                    UriFactory.CreateDocumentCollectionUri(TopicsDB, TopicsCollection)).ToList();

            return result;
        }

        public List<Topic> SearchForNews(string news) {
            //TODO Add cache

            FeedOptions queryOptions = new FeedOptions { MaxItemCount = -1 };

            var result = this.client.CreateDocumentQuery<Topic>(
                    UriFactory.CreateDocumentCollectionUri(TopicsDB, TopicsCollection))
                    .Where(f => f.Name.Contains(news)).ToList();

            return result;
        }

        private async Task CreateDocumentIfNotExists(string databaseName, string collectionName, Topic topic)
        {
            try
            {
                await this.client.ReadDocumentAsync(UriFactory.CreateDocumentUri(databaseName, collectionName, topic.ID));
                Console.WriteLine($"Found {topic.ID}");
            }
            catch (DocumentClientException de)
            {
                if (de.StatusCode == HttpStatusCode.NotFound)
                {
                    await this.client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(databaseName, collectionName), topic);
                    Console.WriteLine($"Created Topic {topic.ID}");
                }
                else
                {
                    throw;
                }
            }
        }
    }
}