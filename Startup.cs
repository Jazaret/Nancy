namespace NancyApplication
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.DependencyInjection;
    using System.Configuration;
    using Nancy.Owin;

    public class Startup
    {
        public void Configure(IApplicationBuilder app)
        {
            app.UseOwin(x => x.UseNancy());
        }

        public void ConfigureServices(IServiceCollection services) {  
            // Add framework services.  
            services.RegisterServices();
            services.AddDistributedRedisCache(options => {  
                options.Configuration = "topics.redis.cache.windows.net:6380,password=mTDuw1qJqQSsm8rGNK4e7ko9csj7AJvPPpRSVCqO2CY=,ssl=True,abortConnect=False"; //Configuration.GetConnectionString("RedisConnection");  
                options.InstanceName = "topics";
            });
        }  
    }

    public static class ServiceExtensions
    {
        public static IServiceCollection RegisterServices(
            this IServiceCollection services)
        {
            services.AddTransient<ITopicService, TopicService>();
            services.AddTransient<ITopicRepository, TopicRepository>();
            return services;
        }
    }
}
