using LagoVista.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.Core.PlatformSupport
{
    public interface ISerialPort : IDisposable
    {
        Task<bool> OpenAsync(SerialPortInfo serialPortInfo);
        bool IsConnected { get; }
        Task WriteAsync<T>(T contents);

        bool BytesAvailable { get; }

        Task<byte?> ReadByteAsync();
        Task<byte[]> ReadBufferAsync();
        Task<string> ReadStringAsync();
         
        Task CloseAsync();
    }
}
