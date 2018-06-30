using System.Threading.Tasks;
using NancyApplication;

namespace NancyApplication.Tests {
    public class MockAccountRepository : IAccountRepository
    {
        public Task AddAccount(Account account)
        {
            return null;
        }

        public Task UpdateAccount(Account account)
        {
            return null;
        }
    }
}