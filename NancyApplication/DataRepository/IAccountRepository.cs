using System.Collections.Generic;
using System.Threading.Tasks;

namespace NancyApplication {
    public interface IAccountRepository
    {
        Task AddAccount(Account account);
        Task UpdateAccount(Account account);
    }
}