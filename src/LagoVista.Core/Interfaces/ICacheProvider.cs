// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: a72ecf549168a071d10e295d9c1c9703495a73dcb6f612fe16e5d94e74ed45a6
// IndexVersion: 2
// --- END CODE INDEX META ---
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LagoVista.Core.Interfaces
{
    public interface ICacheProvider
    {
        Task AddAsync<T>(string key, T value, TimeSpan? ttl = null);
        Task AddAsync(string key, string value, TimeSpan? ttl = null);
        Task AddToCollectionAsync(string collectionKey, string key, string value);
        Task RemoveAsync(string key);
        Task RemoveFromCollectionAsync(string collectionKey, string key);
        Task<string> GetAsync(string key);
        Task<T> GetAsync<T>(string key) where T : class;
        Task<T> GetAndDeleteAsync<T>(string key) where T : class;
        Task<IEnumerable<object>> GetCollection(string collectionKey);
        Task<string> GetFromCollection(string collectionKey, string key);
        Task<IDictionary<string, string>> GetManyAsync(IEnumerable<string> keys);
        Task<long> GetLongAsync(string key);
        Task<long> IncrementAsync(string key);
    }
}
