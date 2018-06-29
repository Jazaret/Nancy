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

    /// <summary>
    /// Repository that handles the operations for the Topic documents in DynamoDB
    /// </summary>
    public class TopicRepository : BaseRepository, ITopicRepository {
        protected const string TopicsCollection = "TopicsCollection";
        private IDistributedCache _cache;
        
        public TopicRepository() : base() {
            Initialize().Wait();
        }

        /// <summary>
        /// Creates the Topics Collection if it does not exist.
        /// </summary>
        private async Task Initialize()
        {
            DocumentCollection topics = new DocumentCollection();
            topics.Id = TopicsCollection;
            await this.client.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri(TopicsDB), new DocumentCollection { Id = TopicsCollection });
        }

        /// <summary>
        /// Get list of all topics in document collection
        /// </summary>
        /// <returns>All topics</returns>
        IEnumerable<Topic> ITopicRepository.GetTopics()
        {
            var result = this.client.CreateDocumentQuery<Topic>(
                    UriFactory.CreateDocumentCollectionUri(TopicsDB, TopicsCollection)).ToList();

            return result;
        }

        /// <summary>
        /// Search for specific topics based off of parameter
        /// </summary>
        /// <param name="news">news for which to search across all topics</param>
        /// <returns>topics that contains the parameter</returns>
        public IEnumerable<Topic> SearchForTopics(string news)
        {
            string cacheResult = null;
            if (_cache != null) {
                cacheResult = _cache.GetString(news);
            }
            
            if (cacheResult != null) { 
                return JsonConvert.DeserializeObject<List<Topic>>(cacheResult);
            }

            var result = this.client.CreateDocumentQuery<Topic>(
                    UriFactory.CreateDocumentCollectionUri(TopicsDB, TopicsCollection))
                    .Where(f => f.Name.Contains(news)).ToList();

            if (_cache != null) {
                _cache.SetString(news,JsonConvert.SerializeObject(result));
            }

            return result;
        }    
    }
}