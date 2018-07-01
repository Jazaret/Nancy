using System;
using Xunit;
using Moq;
using NancyApplication;
using Newtonsoft.Json;

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
            var result = _accountService.AddAccount(accountName,accountPassword);
            
            _mockRepo.Verify(m => m.AddAccount(It.IsAny<Account>()));
            Assert.Equal(accountName,result.Name);
            Assert.Equal(accountPassword,result.Password);
        }

        [Fact]
        public void AssertAddAccountInvalid()
        {
            var result = _accountService.AddAccount("","");

            _mockRepo.Verify(m => m.AddAccount(It.IsAny<Account>()),Times.Never());
            Assert.Null(result);
        }

        [Fact]
        public void AssertUpdateAccount()
        {
            var id = Guid.NewGuid().ToString();
            var account = new Account(id,accountName,accountPassword);

            var result = _accountService.UpdateAccount(id,accountName,accountPassword);

            _mockRepo.Verify(m => m.UpdateAccount(It.IsAny<Account>()));
            var expectedStr = JsonConvert.SerializeObject(account);
            var actualStr = JsonConvert.SerializeObject(result);
            Assert.Equal(expectedStr, actualStr);
        }        
    }
}
