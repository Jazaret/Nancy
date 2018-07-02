using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

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
        public async Task<ActionResult<Account>> AddAccount(string accountName, string accountPassword)
        {
            var result = new ActionResult<Account>();
            var account = new Account(accountName,accountPassword);

            if (!account.IsValid()) {
                result.statusCode = HttpStatusCode.BadRequest;
                return result;
            }
            
            try {
                result = await _accountRepo.AddAccount(account);                    
            } catch (Exception ex) {
                Console.WriteLine(ex.InnerException.Message);
                //log - handle - consider retry
                result.statusCode = HttpStatusCode.InternalServerError;
                return result;
            }

            return result;
        }

        /// <summary>
        /// Updates an account from the repository if valid.
        /// </summary>
        public async Task<ActionResult<Account>> UpdateAccount(string accountId, string accountName, string accountPassword)
        {
            var result = new ActionResult<Account>();
            var account = new Account(accountId,accountName,accountPassword);

            if (!account.IsValid()) {
                result.statusCode = HttpStatusCode.BadRequest;
                return result;
            }
            
            try {
                result = await _accountRepo.UpdateAccount(account);
            } catch (Exception ex) {
                Console.WriteLine(ex.InnerException.Message);
                //log - handle - consider retry
                result.statusCode = HttpStatusCode.InternalServerError;
            }
            
            return result;
        }
    }
}