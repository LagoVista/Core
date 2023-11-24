using System.Collections.Generic;
using System.Threading.Tasks;

namespace LagoVista.Core.Interfaces
{
    public interface ICacheProvider
    {
        Task AddAsync(string key, string value);
        Task AddToCollectionAsync(string collectionKey, string key, string value);
        Task RemoveAsync(string key);
        Task RemoveFromCollectionAsync(string collectionKey, string key);
        Task<string> GetAsync(string key);
        Task<IEnumerable<object>> GetCollection(string collectionKey);
        Task<string> GetFromCollection(string collectionKey, string key);
    }
}
