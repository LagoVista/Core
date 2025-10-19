// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: a72ecf549168a071d10e295d9c1c9703495a73dcb6f612fe16e5d94e74ed45a6
// IndexVersion: 0
// --- END CODE INDEX META ---
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
