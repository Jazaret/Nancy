using System;
using Xunit;
using NancyApplication;

namespace NancyApplication.Tests
{
    public class AccountTests
    {        
        private readonly AccountService _accountService;

        public AccountTests() {
             _accountService = new AccountService(new MockAccountRepository());             
        }

        [Fact]
        public void AssertAccountValuesAreSet()
        {
            var id = Guid.NewGuid().ToString();
            var accountName = "test";
            var accountPassword = "test1";
            var result = _accountService.AddAccount(accountName,accountPassword);

            Assert.Equal(accountName,result.Name);
            Assert.Equal(accountPassword,result.Password);
        }
    }
}
