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
                return Response.AsJson(topicService.GetAllTopics());
            });
            
            Get("Topics/Search", args => {
                List<Topic> result = topicService.SearchForNews(this.Request.Query["q"]);
                return Response.AsJson(result);
            });
        }
    }
}
