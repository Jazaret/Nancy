using System;
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

        /// <summary>
        /// Adds an account to repository if valid
        /// </summary>
        public Account AddAccount(string accountName, string accountPassword)
        {
            var account = new Account(accountName,accountPassword);

            if (!account.IsValid()) {
                //return invalid response code 400
                throw new Exception("Invalid account data");
            }
            
            _accountRepo.AddAccount(account);

            return account;
        }

        /// <summary>
        /// Updates an account from the repository if valid.
        /// </summary>
        public Account UpdateAccount(string accountId, string accountName, string accountPassword)
        {
            var account = new Account(accountId,accountName,accountPassword);

            if (!account.IsValid()) {
                //return invalid response code 400
                throw new Exception("Invalid account data");
            }
            
            _accountRepo.UpdateAccount(account);
            
            return account;
        }
    }
}