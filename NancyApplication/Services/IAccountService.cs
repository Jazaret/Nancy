using System.Collections.Generic;

namespace NancyApplication {
    public interface IAccountService
        {
            ActionResult<Account> AddAccount(string accountName, string accountPassword);
            ActionResult<Account> UpdateAccount(string accountId, string accountName, string accountPassword);
        }
}