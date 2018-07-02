using System;
using Xunit;
using Moq;
using NancyApplication;
using Newtonsoft.Json;
using System.Net;
using System.Threading.Tasks;

namespace NancyApplication.Tests
{
    public class AccountTests
    {
        private readonly AccountService _accountService;
        private readonly Mock<IAccountRepository> _mockRepo;
        private const string accountName = "test";
        private const string accountPassword = "test1";

        public AccountTests() {
            _mockRepo = new Mock<IAccountRepository>(MockBehavior.Loose);
            _accountService = new AccountService(_mockRepo.Object);           
        }

        [Fact]
        public void AssertAddAccountValuesAreSet()
        {
            //Given            
            var id = Guid.NewGuid().ToString();
            var account = new Account(id,accountName,accountPassword);
            var ar = new ActionResult<Account>() {resposeObject= account, statusCode = HttpStatusCode.Created};
            _mockRepo.Setup(m => m.AddAccount(It.IsAny<Account>())).Returns(Task.FromResult(ar));

            //When
            var result = _accountService.AddAccount(accountName,accountPassword).Result;
            
            //Then
            _mockRepo.Verify(m => m.AddAccount(It.IsAny<Account>()));
            Assert.Equal(accountName,result.resposeObject.Name);
            Assert.Equal(accountPassword,result.resposeObject.Password);
            Assert.Equal(HttpStatusCode.Created,result.statusCode);
        }

        [Fact]
        public void AssertAddAccountInvalidAddAccountNeverCalled()
        {
            //Given

            //When
            var result = _accountService.AddAccount("","").Result;

            //Then
            _mockRepo.Verify(m => m.AddAccount(It.IsAny<Account>()),Times.Never());
            Assert.Equal(HttpStatusCode.BadRequest,result.statusCode);
        }

        [Fact]
        public void AssertUpdateAccount()
        {
            //Given
            var id = Guid.NewGuid().ToString();
            var account = new Account(id,accountName,accountPassword);
            var ar = new ActionResult<Account>() {resposeObject= account, statusCode = HttpStatusCode.OK};
            _mockRepo.Setup(m => m.UpdateAccount(It.IsAny<Account>())).Returns(Task.FromResult(ar));

            //When
            var result = _accountService.UpdateAccount(id,accountName,accountPassword).Result;

            //Then
            _mockRepo.Verify(m => m.UpdateAccount(It.IsAny<Account>()));
            var expectedStr = JsonConvert.SerializeObject(account);
            var actualStr = JsonConvert.SerializeObject(result.resposeObject);
            Assert.Equal(expectedStr, actualStr);
        }        
    }
}
