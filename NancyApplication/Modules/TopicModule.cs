namespace NancyApplication
{
    using Nancy;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Nancy module that handles Topic endpoints
    /// </summary>
    public class TopicsModule : NancyModule
    {
        private ITopicService _topicService;

        public TopicsModule(ITopicService topicService)
        {
            _topicService = topicService;

            Get("/", args => { 
                return "Welcome to the Nancy API for Topics!";
            });

            /// <summary>
            /// Get all topics in repository
            /// </summary>
            Get("Topics/", async (args, ct) => {
                var getResponse = await topicService.GetAllTopics();
                if (getResponse.statusCode != (System.Net.HttpStatusCode)HttpStatusCode.OK) {
                    return getResponse.statusCode;
                }
                var links = new List<HyperMedia>{
                    new HyperMedia { 
                        Href = this.Request.Url, 
                        Rel = "self" 
                    },
                    new HyperMedia {
                        Href = $"{this.Request.Url.SiteBase}/Topics/Search", 
                        Rel = "search" 
                    }
                };
                var topicList = new TopicList(getResponse.resposeObject,links);
                return Response.AsJson(topicList);
            });
            
            /// <summary>
            /// Search for topics in repository. 
            /// </summary>
            Get("Topics/Search", async (args, ct) => {
                var getResponse = await topicService.SearchForNews(this.Request.Query["q"]);
                if (getResponse.statusCode != (System.Net.HttpStatusCode)HttpStatusCode.OK) {
                    return getResponse.statusCode;
                }
                var links = new List<HyperMedia>{
                    new HyperMedia { 
                        Href = this.Request.Url, 
                        Rel = "self" 
                    },
                    new HyperMedia {
                        Href = $"{this.Request.Url.SiteBase}/Topics/", 
                        Rel = "index" 
                    }
                };
                var topicList = new TopicList(getResponse.resposeObject,links);
                return Response.AsJson(topicList);
            });
        }
    }
}
