// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 3e765888d3e546f715f298df4a447c389dda9b2a9d0703b60ac40c3be3ff02a2
// IndexVersion: 1
// --- END CODE INDEX META ---
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.Core.Networking.Interfaces
{
    public enum ConnectionTypes
    {
        TCPIP,
        UDP,
        Serial,
    }

    public interface IStreamConnection : IDisposable
    {
        Task<IStreamWriter> GetStreamWriterAsync();
        Task<IStreamReader> GetStreamReaderAsync();
        Task<bool> ConnectAsync(ConnectionTypes connectionType);
        Task CloseAsync();
        bool IsConnected { get; }
    }
}
