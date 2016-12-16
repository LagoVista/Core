using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.Core.Networking.Interfaces
{
    public interface IStreamWriter : IDisposable
    {
        bool AutoFlush { get; set; }

        Task WriteAsync(byte[] buffer);
        Task WriteAsync(String message);
        Task FlushAsync();

        
    }
}
