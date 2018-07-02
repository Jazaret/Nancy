using System.Collections.Generic;
using System.Threading.Tasks;

namespace NancyApplication {
    public interface IAccountService
        {
            Task<ActionResult<Account>> AddAccount(string accountName, string accountPassword);
            Task<ActionResult<Account>> UpdateAccount(string accountId, string accountName, string accountPassword);
        }
}