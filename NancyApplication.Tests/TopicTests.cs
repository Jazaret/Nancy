using System;
using Xunit;
using Moq;
using NancyApplication;
using Newtonsoft.Json;
using System.Threading.Tasks;

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
        public async Task AssertGetTopicsCallsGetTopicsRepoAsync()
        {
            //Given

            //When
            var result = await _topicService.GetAllTopics();
            
            //Then
            _mockRepo.Verify(m => m.GetTopics());
        }

        [Fact]
        public async Task AssertGetNewsCallsGetNewsRepoAsync()
        {
            //Given
            const string query = "any";

            ///When
            var result = await _topicService.SearchForNews(query);
            
            //Then
            _mockRepo.Verify(m => m.SearchForTopics(query));
        }        
    }
}
