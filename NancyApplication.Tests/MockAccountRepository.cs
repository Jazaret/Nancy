using System.Net;
using System.Threading.Tasks;
using NancyApplication;

namespace NancyApplication.Tests {
    public class MockAccountRepository : IAccountRepository
    {
        public Task<HttpStatusCode> AddAccount(Account account)
        {
            return null;
        }

        public Task<HttpStatusCode> UpdateAccount(Account account)
        {
            return null;
        }
    }
}