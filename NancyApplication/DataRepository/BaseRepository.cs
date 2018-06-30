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
    /// Shared base object for all DynamoDB repos. TODO - Read endpoint and key from appsettings
    /// </summary>
    public class BaseRepository {
        protected const string TopicsDB = "TopicsDB";
        protected const string EndpointUrl = "https://topics.documents.azure.com:443/";
        protected const string PrimaryKey = "LmX3kOe6k6DADHOQbehETuHo09Evi3AVzEN2JhZL2Ax9XONxmmUgALHMOhfTbR2rPw5Xv6byrC5xsvAhRUSVXA==";

        protected DocumentClient Client;    
        protected DocumentCollection Collection;

        /// <summary>
        /// creates a Dyanmo document client and creates the db if it does not exist
        /// </summary>
        protected BaseRepository() {
            this.Client = new DocumentClient(new Uri(EndpointUrl), PrimaryKey);
            this.Client.CreateDatabaseIfNotExistsAsync(new Database { Id = TopicsDB });
        }          

        protected async Task<DocumentCollection> GetOrCreateCollectionAsync(string databaseId, string collectionId)
        {
            var databaseUri = UriFactory.CreateDatabaseUri(databaseId);

            var collection = Client.CreateDocumentCollectionQuery(databaseUri)
                                 .Where(c => c.Id == collectionId).AsEnumerable().FirstOrDefault() ??
                             await Client.CreateDocumentCollectionAsync(databaseUri, new DocumentCollection { Id = collectionId });

            return collection;
        }

    }    
}

