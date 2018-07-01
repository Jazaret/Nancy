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

            Get("Topics/", args => {
                var list = topicService.GetAllTopics();
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
                var topicList = new TopicList(list,links);
                return Response.AsJson(topicList);
            });
            
            Get("Topics/Search", args => {
                var list = topicService.SearchForNews(this.Request.Query["q"]);
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
                var topicList = new TopicList(list,links);
                return Response.AsJson(topicList);
            });
        }
    }
}
