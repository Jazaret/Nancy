using System.Collections.Generic;

namespace NancyApplication {
    public interface IAccountRepository
    {
        Account AddAccount(string accountName, string displayName);
        Account UpdateAccount(string Id, string accountName, string displayName);
    }
}