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
    using Microsoft.Extensions.Caching.Distributed;

    public class TopicRepository : ITopicRepository {
        protected const string TopicsDB = "TopicsDB";
        protected const string EndpointUrl = "https://topics.documents.azure.com:443/";
        protected const string PrimaryKey = "LmX3kOe6k6DADHOQbehETuHo09Evi3AVzEN2JhZL2Ax9XONxmmUgALHMOhfTbR2rPw5Xv6byrC5xsvAhRUSVXA==";

        protected DocumentClient client; 
        private IDistributedCache _cache;

        protected const string TopicsCollection = "TopicsCollection";
        
        public TopicRepository() {
            this.client = new DocumentClient(new Uri(EndpointUrl), PrimaryKey);
            this.client.CreateDatabaseIfNotExistsAsync(new Database { Id = TopicsDB });
            Initialize().Wait();
            //this._cache = cache;  
        }

        private async Task Initialize()
        {
            DocumentCollection myCollection = new DocumentCollection();
            myCollection.Id = TopicsCollection;
            //myCollection.PartitionKey.Paths.Add("/deviceId");
            await this.client.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri(TopicsDB), new DocumentCollection { Id = TopicsCollection });
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

        IEnumerable<Topic> ITopicRepository.GetTopics()
        {
            FeedOptions queryOptions = new FeedOptions { MaxItemCount = -1 };

            var result = this.client.CreateDocumentQuery<Topic>(
                    UriFactory.CreateDocumentCollectionUri(TopicsDB, TopicsCollection)).ToList();

            return result;
        }

        public IEnumerable<Topic> SearchForTopics(string news)
        {
            var cacheResult = _cache.GetString(news);
            
            if (cacheResult != null) { 
                return JsonConvert.DeserializeObject<List<Topic>>(cacheResult);; 
            }

            FeedOptions queryOptions = new FeedOptions { MaxItemCount = -1 };

            var result = this.client.CreateDocumentQuery<Topic>(
                    UriFactory.CreateDocumentCollectionUri(TopicsDB, TopicsCollection))
                    .Where(f => f.Name.Contains(news)).ToList();

            _cache.SetString(news,JsonConvert.SerializeObject(result));

            return result;
        }
    }
}