namespace NancyApplication
{
    using Microsoft.Extensions.Caching.Distributed;
    using Nancy;
    using System;
    using System.Collections.Generic;

    public class HomeModule : NancyModule
    {
        private ITopicService _topicService;
        private IDistributedCache _cache;

        public HomeModule(ITopicService topicService)
        {
            _topicService = topicService;
            _topicService.GetAllTopics();

            Get("/", args => { 
                return Response.AsRedirect("Topics/");
            });

            Get("Topics/", args => {
                return Response.AsJson(topicService.GetAllTopics());
            });
            
            Get("Topics/Search", args => {
                List<Topic> result = topicService.SearchForNews(this.Request.Query["q"]);
                return Response.AsJson(result);
            });

            //Get("/test/{name}", args => new Task() { Name = args.name });
        }
    }
}
