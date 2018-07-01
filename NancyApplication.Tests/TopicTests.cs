using System;
using Xunit;
using Moq;
using NancyApplication;
using Newtonsoft.Json;

namespace NancyApplication.Tests
{
    public class TopicTests
    {
        private readonly TopicService _topicService;
        private readonly Mock<ITopicRepository> _mockRepo;
        private readonly Mock<ICacheService> _mockCache;

        public TopicTests() {
            _mockRepo = new Mock<ITopicRepository>(MockBehavior.Loose);
            _mockCache = new Mock<ICacheService>(MockBehavior.Loose);
            _topicService = new TopicService(_mockRepo.Object, _mockCache.Object);           
        }

        [Fact]
        public void AssertGetTopicsCallsGetTopicsRepo()
        {
            var result = _topicService.GetAllTopics();
            
            _mockRepo.Verify(m => m.GetTopics());
        }

        [Fact]
        public void AssertGetNewsCallsGetNewsRepo()
        {
            const string query = "any";
            var result = _topicService.SearchForNews(query);
            
            _mockRepo.Verify(m => m.SearchForTopics(query));
        }        
    }
}
