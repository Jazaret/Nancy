namespace NancyApplication
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.DependencyInjection;
    using System.Configuration;
    using Nancy.Owin;
    using StackExchange.Redis;

    public class Startup
    {
        public void Configure(IApplicationBuilder app)
        {
            app.UseOwin(x => x.UseNancy());
        }

        public void ConfigureServices(IServiceCollection services) {  
            // Add framework services.  
            services.RegisterServices();
        }  
    }

    public static class ServiceExtensions
    {
        public static IServiceCollection RegisterServices(
            this IServiceCollection services)
        {
            services.AddSingleton<ICacheService, CacheService>();
            
            services.AddTransient<ITopicService, TopicService>();
            services.AddTransient<ITopicRepository, TopicRepository>();
            services.AddTransient<IAccountService, AccountService>();
            services.AddTransient<IAccountRepository, AccountRepository>();
            services.AddTransient<ISubscriptionService, SubscriptionService>();
            services.AddTransient<ISubscriptionRepository, SubscriptionRepostiory>();

            return services;
        }
    }
}
