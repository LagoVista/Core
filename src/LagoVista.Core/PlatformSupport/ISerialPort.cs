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
        Task<Stream> OpenAsync(SerialPortInfo serialPortInfo);
        bool IsConnected { get; }
        Task CloseAsync();
    }

}
