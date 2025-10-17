// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 959b5b0ac9cc2fae843a185468c4f73f7f98e0b4a08efef5ac99aa5d252c07ab
// IndexVersion: 1
// --- END CODE INDEX META ---
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Core.PlatformSupport
{
    public interface IChannel
    {
        Task OpenAsync();
        bool IsConnected { get; }
        Task CloseAsync();

        Task WriteAsync(string msg);
        Task WriteAsync(byte[] buffer);
        Task<int> ReadAsync(byte[] bufffer, int start, int size, CancellationToken token = default(CancellationToken));

    }
}
