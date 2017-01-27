using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.Core.PlatformSupport
{
    public interface ISerialPort : IDisposable
    {
        Task<bool> OpenAsync(String portName, int baudRate);
        Task<Stream> GetStreamAsync();
        bool IsConnected { get; }
        Task CloseAsync();
    }
}
