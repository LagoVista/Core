// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 25a86db1830ee3514bb09d1a60d0ee1a787c5d276afd22036d750740f9ab428a
// IndexVersion: 1
// --- END CODE INDEX META ---
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.Core.Networking.Interfaces
{
    public interface IStreamReader : IDisposable
    {
        Task<long> ReadAsync(byte[] buffer);
    }
}
