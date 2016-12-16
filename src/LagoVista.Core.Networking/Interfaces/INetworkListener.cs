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
