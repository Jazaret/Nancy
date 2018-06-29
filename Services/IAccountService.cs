using System.Collections.Generic;

namespace NancyApplication {
    public interface IAccountService
        {
            Account AddAccount(string accountName, string accountPassword);
            Account UpdateAccount(string accountId, string accountName, string accountPassword);
        }
}