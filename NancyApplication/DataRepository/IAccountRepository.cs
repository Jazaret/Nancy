using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace NancyApplication {
    public interface IAccountRepository
    {
        Task<ActionResult<Account>> AddAccount(Account account);
        Task<ActionResult<Account>> UpdateAccount(Account account);
    }
}