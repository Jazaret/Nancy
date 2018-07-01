using System.Threading.Tasks;
using StackExchange.Redis;

namespace NancyApplication {
    public interface ICacheService
    {
        Task<string> GetFromCache(string key);
        Task DeleteFromCache(string key);
        Task AddToCache(string key, string serializedData);
    }
}