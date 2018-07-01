using System;
using Xunit;
using Moq;
using NancyApplication;
using Newtonsoft.Json;
using System.Net;

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
        public void AssertAddSubscriptionCallsRepo()
        {
            //Given
            _mockTopicRepo.Setup(m => m.GetTopic(subTopicId)).Returns(new Topic());

            //When
            var result = _subscriptionService.CreateSubscription(subAccountId,subTopicId);
            
            //Then
            _mockTopicRepo.Verify(m => m.GetTopic(subTopicId));
            _mockSubRepo.Verify(m => m.GetSubscriptionByTopic(subTopicId,subAccountId));
            _mockSubRepo.Verify(m => m.AddSubscription(It.IsAny<Subscription>()));
            Assert.NotNull(result.Id);
            Assert.Equal(subTopicId,result.TopicID);
            Assert.Equal(subAccountId,result.AccountID);
        }

        [Fact]
        public void AssertAddSubscriptionWithExistingSubScriptioncDoesNotCallAddSubcription()
        {
            //Given
            _mockTopicRepo.Setup(m => m.GetTopic(subTopicId)).Returns(new Topic());
            _mockSubRepo.Setup(m => m.GetSubscriptionByTopic(subTopicId,subAccountId)).Returns(new Subscription());

            //When
            var result = _subscriptionService.CreateSubscription(subAccountId,subTopicId);
            
            //Then
            _mockTopicRepo.Verify(m => m.GetTopic(subTopicId));
            _mockSubRepo.Verify(m => m.GetSubscriptionByTopic(subTopicId,subAccountId));
            _mockSubRepo.Verify(m => m.AddSubscription(It.IsAny<Subscription>()),Times.Never());
            Assert.Null(result);
        }        

        /// <summary>
        /// If topic is null then Add subscription and get subscription should never be called
        /// </summary>
        [Fact]
        public void AssertAddSubscriptionWithNoTopicDoesNotCallAddSubcription()
        {
            //Given

            //When
            var result = _subscriptionService.CreateSubscription(subAccountId,subTopicId);
            
            //Then
            _mockTopicRepo.Verify(m => m.GetTopic(subTopicId));
            _mockSubRepo.Verify(m => m.GetSubscriptionByTopic(It.IsAny<string>(),It.IsAny<string>()),Times.Never());
            _mockSubRepo.Verify(m => m.AddSubscription(It.IsAny<Subscription>()),Times.Never());
            Assert.Null(result);
        }        

        [Fact]
        public void AssertDeleteSubscriptionCallsRepoToDelete()
        {
            //Given

            //When
            _subscriptionService.DeleteSubscription(subId,subAccountId);

            //Then
            _mockSubRepo.Verify(m => m.DeleteSubscription(subId,subAccountId));
        }    

        [Fact]
        public void AssertConfirmSubscriptionCallsRepo()
        {
            //Given
            _mockSubRepo.Setup(m => m.GetSubscriptionByConfirmation(subConfirmationId,subAccountId))
                .Returns(new Subscription{
                    Id = Guid.NewGuid().ToString(),
                    AccountID = subAccountId,
                    TopicID = subTopicId,
                    ConfirmationToken = subConfirmationId,
                    SubscriptionConfirmed = false
                });

            //When
            var result = _subscriptionService.ConfirmSubscription(subConfirmationId,subAccountId);

            //Then
            _mockSubRepo.Verify(m => m.GetSubscriptionByConfirmation(subConfirmationId,subAccountId));
            _mockSubRepo.Verify(m => m.UpdateSubscription(It.IsAny<Subscription>()));
        }     

        [Fact]
        public void AssertConfirmSubscriptionWithNoSubscriptionDoesNotCallUpdateSubscription()
        {
            //Given

            //When
            var result = _subscriptionService.ConfirmSubscription(subConfirmationId,subAccountId);

            //Then
            _mockSubRepo.Verify(m => m.GetSubscriptionByConfirmation(subConfirmationId,subAccountId));
            _mockSubRepo.Verify(m => m.UpdateSubscription(It.IsAny<Subscription>()),Times.Never);
            Assert.Equal(HttpStatusCode.NoContent,result);
        }                          

        [Fact]
        public void AssertConfirmSubscriptionAlreadyConfirmednDoesNotUpdateSubscription()
        {
            //Given
            _mockSubRepo.Setup(m => m.GetSubscriptionByConfirmation(subConfirmationId,subAccountId))
                .Returns(new Subscription{
                    Id = Guid.NewGuid().ToString(),
                    AccountID = subAccountId,
                    TopicID = subTopicId,
                    ConfirmationToken = subConfirmationId,
                    SubscriptionConfirmed = true
                });

            //When
            var result = _subscriptionService.ConfirmSubscription(subConfirmationId,subAccountId);

            //Then
            _mockSubRepo.Verify(m => m.GetSubscriptionByConfirmation(subConfirmationId,subAccountId));
            _mockSubRepo.Verify(m => m.UpdateSubscription(It.IsAny<Subscription>()),Times.Never);
            Assert.Equal(HttpStatusCode.NotModified,result);
        }     
    }
}