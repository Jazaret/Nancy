using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace NancyApplication {
    public interface IAccountRepository
    {
        Task<HttpStatusCode> AddAccount(Account account);
        Task<HttpStatusCode> UpdateAccount(Account account);
    }
}