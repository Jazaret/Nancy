using System;
using Xunit;
using NancyApplication;
using Nancy;
using Nancy.Testing;
using Moq;

namespace NancyApplication.Tests
{
    public class TopicModuleTests
    {   
        private readonly Mock<ITopicRepository> _mockTopicRepo;
        private readonly Mock<ITopicService> _mockTopicService;
        public TopicModuleTests() {
            _mockTopicRepo = new Mock<ITopicRepository>(MockBehavior.Loose);
            _mockTopicService = new Mock<ITopicService>(MockBehavior.Loose);
        }
        [Fact]
        public void AssertRouteExists()
        {
            // Given
            var browser = new Browser((c) => c.Module<TopicsModule>()
                .Dependency<ITopicRepository>(_mockTopicRepo.Object)
                .Dependency<ITopicService>(_mockTopicService.Object)
            );
            
            // When
            var result = browser.Get("/", with => {
                with.HttpRequest();
            });
                
            // Then
            Assert.Equal(HttpStatusCode.OK, result.Result.StatusCode);
        }   
    } 
}