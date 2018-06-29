namespace NancyApplication
{
    using Nancy;
    using System;
    using System.Collections.Generic;

    public class HomeModule : NancyModule
    {
        public HomeModule()
        {
            Get("/", args => { 
                return Response.AsRedirect("Topics/");
            });

            Get("Topics/", args => {
                return Response.AsJson(new TaskRepository().GetTopics());
            });
            
            Get("Topics/Search", args => {
                List<Topic> topics = new TaskRepository().SearchForNews(this.Request.Query["q"]);
                return Response.AsJson(topics);
            });

            //Get("/test/{name}", args => new Task() { Name = args.name });
        }
    }
}
