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
    public class BaseRepository {
        protected const string TopicsDB = "TopicsDB";
        protected const string TopicsCollection = "TopicsCollection";
        protected const string EndpointUrl = "https://topics.documents.azure.com:443/";
        protected const string PrimaryKey = "LmX3kOe6k6DADHOQbehETuHo09Evi3AVzEN2JhZL2Ax9XONxmmUgALHMOhfTbR2rPw5Xv6byrC5xsvAhRUSVXA==";

        protected DocumentClient client;    

        public BaseRepository() {
            this.client = new DocumentClient(new Uri(EndpointUrl), PrimaryKey);
            this.client.CreateDatabaseIfNotExistsAsync(new Database { Id = TopicsDB });
        }    

    }    
}

