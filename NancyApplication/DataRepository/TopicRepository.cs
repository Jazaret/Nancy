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
            await this.Client.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri(TopicsDB), new DocumentCollection { Id = TopicsCollection });
        }

        /// <summary>
        /// Get list of all topics in document collection
        /// </summary>
        /// <returns>All topics</returns>
        IEnumerable<Topic> ITopicRepository.GetTopics()
        {            
            var result = this.Client.CreateDocumentQuery<Topic>(UriFactory.CreateDocumentCollectionUri(TopicsDB, TopicsCollection)).ToList();

            return result;
        }

        /// <summary>
        /// Search for specific topics based off of parameter
        /// </summary>
        /// <param name="news">news for which to search across all topics</param>
        /// <returns>topics that contains the parameter</returns>
        public IEnumerable<Topic> SearchForTopics(string news)
        {
            var result = this.Client.CreateDocumentQuery<Topic>(
                    UriFactory.CreateDocumentCollectionUri(TopicsDB, TopicsCollection))
                    .Where(f => f.Name.Contains(news)).ToList();
            return result;
        }    

        private async Task<HttpStatusCode> CreateDocument(Topic topic)
        {
            var result = await this.Client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(TopicsDB, TopicsCollection), topic);
            return result.StatusCode;
        }  
    }
}