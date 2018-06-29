using System.Collections.Generic;

namespace NancyApplication {
    public interface IAccountRepository
    {
        Account AddAccount(string accountName, string password);
        Account UpdateAccount(string Id, string accountName, string password);
    }
}