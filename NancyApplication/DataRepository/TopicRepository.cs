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
        ActionResult<IEnumerable<Topic>> ITopicRepository.GetTopics()
        {   
            var result = new ActionResult<IEnumerable<Topic>>();
            try {
                var queryResult = this.Client.CreateDocumentQuery<Topic>(UriFactory.CreateDocumentCollectionUri(TopicsDB, TopicsCollection)).ToList();
                result.resposeObject = queryResult;
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
        public ActionResult<IEnumerable<Topic>> SearchForTopics(string news)
        {
            var result = new ActionResult<IEnumerable<Topic>>();
            try {
                var queryResult = this.Client.CreateDocumentQuery<Topic>(
                        UriFactory.CreateDocumentCollectionUri(TopicsDB, TopicsCollection))
                        .Where(f => f.Name.Contains(news)).ToList();
                result.resposeObject = queryResult;
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

        private async Task<HttpStatusCode> CreateDocument(Topic topic)
        {
            var result = await this.Client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(TopicsDB, TopicsCollection), topic);
            return result.StatusCode;
        }

        public ActionResult<Topic> GetTopic(string id) {
            var result = new ActionResult<Topic>();
            try {
                var queryResult = this.Client.CreateDocumentQuery<Topic>(
                    UriFactory.CreateDocumentCollectionUri(TopicsDB, TopicsCollection))
                    .Where(t => t.ID == id).AsEnumerable().FirstOrDefault();
                result.statusCode = queryResult == null ? HttpStatusCode.NotFound : HttpStatusCode.OK;
                result.resposeObject = queryResult;
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