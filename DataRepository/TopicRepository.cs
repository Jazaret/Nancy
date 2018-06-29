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

    public class TopicRepository : BaseRepository, ITopicRepository {
        protected const string TopicsCollection = "TopicsCollection";
        protected const string SubscriptionCollection = "SubscriptionCollection";
        protected const string AccountsCollection = "AccountsCollection";
        private IDistributedCache _cache;

        
        public TopicRepository() : base() {
            Initialize().Wait();
        }

        private async Task Initialize()
        {
            DocumentCollection myCollection = new DocumentCollection();
            myCollection.Id = TopicsCollection;
            await this.client.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri(TopicsDB), new DocumentCollection { Id = TopicsCollection });
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