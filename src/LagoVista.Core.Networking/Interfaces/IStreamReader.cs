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
