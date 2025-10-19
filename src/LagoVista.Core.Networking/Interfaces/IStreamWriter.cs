// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: c7ccb298c96d827bb1066b34c20ed0caf7fe3a4e683b6f99283e1cc40565ca48
// IndexVersion: 0
// --- END CODE INDEX META ---
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
