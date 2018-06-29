using System.Collections.Generic;

namespace NancyApplication {
    /// <summary>
    /// Service that handles actions on Accounts
    /// </summary>
    public class AccountService : IAccountService
    {
        IAccountRepository _accountRepo;

        public AccountService(IAccountRepository accountRepository) {
            _accountRepo = accountRepository;
        }

        public Account AddAccount(string accountName, string accountPassword)
        {
            return _accountRepo.AddAccount(accountName, accountPassword);
        }

        public Account UpdateAccount(string accountId, string accountName, string accountPassword)
        {
            return _accountRepo.UpdateAccount(accountId, accountName, accountPassword);
        }
    }
}