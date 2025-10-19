// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 2113a7b9d76d1cbf43b06bb159853c7d4b5c0e2352c16ca2e2976a4cbcc30b5d
// IndexVersion: 0
// --- END CODE INDEX META ---
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.Core.Networking.Interfaces
{
    public interface INetworkListener : IDisposable
    {
        event EventHandler<IStreamConnection> ConnectionReceived;

        void Start();
        Task CloseAsync();
    }
}
