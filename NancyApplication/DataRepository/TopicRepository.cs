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
    using Microsoft.Azure.Documents.Linq;

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
        public async Task<ActionResult<IEnumerable<Topic>>> GetTopics()
        {   
            var result = new ActionResult<IEnumerable<Topic>>();

            try {
                var topics = new List<Topic>();
                var queryResult = this.Client.CreateDocumentQuery<Topic>(UriFactory.CreateDocumentCollectionUri(TopicsDB, TopicsCollection)).AsDocumentQuery();
                while (queryResult.HasMoreResults) 
                {
                    var response = await queryResult.ExecuteNextAsync<Topic>();
	                topics.AddRange(response);
                }
                result.resposeObject = topics;
                result.statusCode = HttpStatusCode.OK;
                return result;
            } catch (DocumentClientException ex) {
                Console.WriteLine(ex.Message + " " + ex.StatusCode);
                result.statusCode = ex.StatusCode.Value;
                return result;
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
                result.statusCode = HttpStatusCode.InternalServerError;
                return result;
            }
        }

        /// <summary>
        /// Search for specific topics based off of parameter
        /// </summary>
        /// <param name="news">news for which to search across all topics</param>
        /// <returns>topics that contains the parameter</returns>
        public async Task<ActionResult<IEnumerable<Topic>>> SearchForTopics(string news)
        {
            var result = new ActionResult<IEnumerable<Topic>>();
            try {
                List<Topic> topics = new List<Topic>();
                var queryResult = this.Client.CreateDocumentQuery<Topic>(
                        UriFactory.CreateDocumentCollectionUri(TopicsDB, TopicsCollection))
                        .Where(f => f.Name.Contains(news)).AsDocumentQuery();
                while (queryResult.HasMoreResults) 
                {
                    var response = await queryResult.ExecuteNextAsync<Topic>();
	                topics.AddRange(response);
                }
                result.resposeObject = topics;
                result.statusCode = HttpStatusCode.OK;
                return result;
            } catch (DocumentClientException ex) {
                Console.WriteLine(ex.Message + " " + ex.StatusCode);
                result.statusCode = ex.StatusCode.Value;
                return result;
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
                result.statusCode = HttpStatusCode.InternalServerError;
                return result;
            }
        }    

        private async Task SeedTopics() {
            
            var topic1 = new Topic {
                Name = "Topic one name",
                ID = Guid.NewGuid().ToString()
            };
            var topic2 = new Topic {
                Name = "Topic two name",
                ID = Guid.NewGuid().ToString()
            };

            var topic3 = new Topic {
                Name = "Topic three name",
                ID = Guid.NewGuid().ToString()
            };

            await CreateDocument(topic1);
            await CreateDocument(topic2);
            await CreateDocument(topic3);
        }

        private async Task<HttpStatusCode> CreateDocument(Topic topic)
        {
            var result = await this.Client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(TopicsDB, TopicsCollection), topic);
            return result.StatusCode;
        }

        public async Task<ActionResult<Topic>> GetTopic(string id) {
            var result = new ActionResult<Topic>();
            try {
                var topic = new Topic();
                var queryResult = this.Client.CreateDocumentQuery<Topic>(
                    UriFactory.CreateDocumentCollectionUri(TopicsDB, TopicsCollection))
                    .Where(t => t.ID == id).AsDocumentQuery();
                while (queryResult.HasMoreResults) 
                {
                    var response = await queryResult.ExecuteNextAsync<Topic>();
                    topic = response.FirstOrDefault();
                }                    
                result.statusCode = topic == null ? HttpStatusCode.NotFound : HttpStatusCode.OK;
                result.resposeObject = topic;
                return result;
            } catch (DocumentClientException ex) {
                Console.WriteLine(ex.Message + " " + ex.StatusCode);
                result.statusCode = ex.StatusCode.Value;
                return result;
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
                result.statusCode = HttpStatusCode.InternalServerError;
                return result;
            }
        }
    }
}