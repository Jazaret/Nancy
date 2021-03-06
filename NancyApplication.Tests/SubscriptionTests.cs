using System;
using Xunit;
using Moq;
using NancyApplication;
using Newtonsoft.Json;
using System.Net;
using System.Threading.Tasks;

namespace NancyApplication.Tests
{
    public class SubscriptionTests
    {
        private readonly SubscriptionService _subscriptionService;
        private readonly Mock<ITopicRepository> _mockTopicRepo;
        private readonly Mock<ISubscriptionRepository> _mockSubRepo;
        private const string subId = "9761c728-6d36-4223-ad44-adb759c99b58";
        private const string subTopicId = "5054328b-f8ad-4aaf-8415-936ff1775eb1";
        private const string subAccountId = "48a84450-68e3-4019-9410-2cc82754ae78";
        private const string subConfirmationId = "24e4e4f8-f3b9-4165-8cf8-b12fadea6873";

        public SubscriptionTests() {
            _mockTopicRepo = new Mock<ITopicRepository>(MockBehavior.Loose);
            _mockSubRepo = new Mock<ISubscriptionRepository>(MockBehavior.Loose);
            _subscriptionService = new SubscriptionService(_mockSubRepo.Object,_mockTopicRepo.Object);
        }

        [Fact]
        public async Task AssertAddSubscriptionCallsRepoAsync()
        {
            var subscription = new Subscription{TopicID = subTopicId, AccountID=subAccountId};

            //Given
            var ar = new ActionResult<Topic>() {resposeObject= new Topic(), statusCode = HttpStatusCode.OK};
            _mockTopicRepo.Setup(m => m.GetTopic(subTopicId)).Returns(Task.FromResult(ar));
            
            var ar2 = new ActionResult<Subscription>() {resposeObject= subscription, statusCode = HttpStatusCode.Created};
            _mockSubRepo.Setup(m => m.AddSubscription(It.IsAny<Subscription>())).Returns(Task.FromResult(ar2));

            //When
            var result = await _subscriptionService.CreateSubscription(subAccountId,subTopicId, string.Empty);
            
            //Then
            _mockTopicRepo.Verify(m => m.GetTopic(subTopicId));
            _mockSubRepo.Verify(m => m.GetSubscriptionByTopic(subTopicId,subAccountId,string.Empty));
            _mockSubRepo.Verify(m => m.AddSubscription(It.IsAny<Subscription>()));
            Assert.Equal(subTopicId,result.resposeObject.TopicID);
            Assert.Equal(subAccountId,result.resposeObject.AccountID);
        }

        [Fact]
        public async Task AssertAddSubscriptionWithExistingSubScriptioncDoesNotCallAddSubcriptionAsync()
        {
            //Given
            var subscription = new Subscription{TopicID = subTopicId, AccountID=subAccountId};
            var ar = new ActionResult<Topic>() {resposeObject= new Topic(), statusCode = HttpStatusCode.OK};
            _mockTopicRepo.Setup(m => m.GetTopic(subTopicId)).Returns(Task.FromResult(ar));

            var ar2 = new ActionResult<Subscription>() {resposeObject= subscription, statusCode = HttpStatusCode.OK};
            _mockSubRepo.Setup(m => m.GetSubscriptionByTopic(subTopicId,subAccountId,string.Empty)).Returns(ar2);

            //When
            var result = await _subscriptionService.CreateSubscription(subAccountId,subTopicId,string.Empty);
            
            //Then
            _mockTopicRepo.Verify(m => m.GetTopic(subTopicId));
            _mockSubRepo.Verify(m => m.GetSubscriptionByTopic(subTopicId,subAccountId,string.Empty));
            _mockSubRepo.Verify(m => m.AddSubscription(It.IsAny<Subscription>()),Times.Never());
            Assert.Null(result.resposeObject);
        }        

        /// <summary>
        /// If topic is null then Add subscription and get subscription should never be called
        /// </summary>
        [Fact]
        public async Task AssertAddSubscriptionWithNoTopicDoesNotCallAddSubcriptionAsync()
        {
            //Given

            //When
            var result = await _subscriptionService.CreateSubscription(subAccountId,subTopicId,string.Empty);
            
            //Then
            _mockTopicRepo.Verify(m => m.GetTopic(subTopicId));
            _mockSubRepo.Verify(m => m.GetSubscriptionByTopic(It.IsAny<string>(),It.IsAny<string>(),It.IsAny<string>()),Times.Never());
            _mockSubRepo.Verify(m => m.AddSubscription(It.IsAny<Subscription>()),Times.Never());
            Assert.Null(result.resposeObject);
        }        

        [Fact]
        public async Task AssertDeleteSubscriptionCallsRepoToDeleteAsync()
        {
            //Given

            //When
            await _subscriptionService.DeleteSubscription(subId,subAccountId);

            //Then
            _mockSubRepo.Verify(m => m.DeleteSubscription(subId,subAccountId));
        }    

        [Fact]
        public async Task AssertConfirmSubscriptionCallsRepoAsync()
        {
            //Given
            var subscription = new Subscription{
                    Id = Guid.NewGuid().ToString(),
                    AccountID = subAccountId,
                    TopicID = subTopicId,
                    ConfirmationToken = subConfirmationId,
                    SubscriptionConfirmed = false
            };
            var ar = new ActionResult<Subscription>{resposeObject = subscription, statusCode = HttpStatusCode.OK};
            _mockSubRepo.Setup(m => m.GetSubscriptionByConfirmation(subConfirmationId,subAccountId,string.Empty))
                .Returns(ar);

            //When
            var result = await _subscriptionService.ConfirmSubscription(subConfirmationId,subAccountId, string.Empty);

            //Then
            _mockSubRepo.Verify(m => m.GetSubscriptionByConfirmation(subConfirmationId,subAccountId,string.Empty));
            _mockSubRepo.Verify(m => m.UpdateSubscription(It.IsAny<Subscription>(),string.Empty));
        }     

        [Fact]
        public async Task AssertConfirmSubscriptionWithNoSubscriptionDoesNotCallUpdateSubscriptionAsync()
        {
            //Given

            //When
            var result = await _subscriptionService.ConfirmSubscription(subConfirmationId,subAccountId, string.Empty);

            //Then
            _mockSubRepo.Verify(m => m.GetSubscriptionByConfirmation(subConfirmationId,subAccountId,string.Empty));
            _mockSubRepo.Verify(m => m.UpdateSubscription(It.IsAny<Subscription>(), It.IsAny<string>()),Times.Never);
            Assert.Equal(HttpStatusCode.NoContent,result.statusCode);
        }                          

        [Fact]
        public async Task AssertConfirmSubscriptionAlreadyConfirmednDoesNotUpdateSubscriptionAsync()
        {
            //Given
            var subscription = new Subscription{
                    Id = Guid.NewGuid().ToString(),
                    AccountID = subAccountId,
                    TopicID = subTopicId,
                    ConfirmationToken = subConfirmationId,
                    SubscriptionConfirmed = true
            };
            var ar = new ActionResult<Subscription>{resposeObject = subscription, statusCode = HttpStatusCode.OK};
            _mockSubRepo.Setup(m => m.GetSubscriptionByConfirmation(subConfirmationId,subAccountId,string.Empty))
                .Returns(ar);

            //When
            var result = await _subscriptionService.ConfirmSubscription(subConfirmationId,subAccountId, string.Empty);

            //Then
            _mockSubRepo.Verify(m => m.GetSubscriptionByConfirmation(subConfirmationId,subAccountId,string.Empty));
            _mockSubRepo.Verify(m => m.UpdateSubscription(It.IsAny<Subscription>(), It.IsAny<string>()),Times.Never);
            Assert.Equal(HttpStatusCode.NotModified,result.statusCode);
        }     
    }
}