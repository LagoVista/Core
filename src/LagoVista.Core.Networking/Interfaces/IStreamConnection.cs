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
