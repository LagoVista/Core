// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 8132913d63d63deac33b57a968431e6de0d81277326bb5c3fc32290257c334ea
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core.Networking.Models;
using LagoVista.Core.PlatformSupport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.Core.Networking.Interfaces
{
    public interface ISSDPServer
    {
        void MakeDiscoverable(int metaDataPort, UPNPConfiguration config);

        void RegisterAPIHandler(IApiHandler handler);

        bool ShowDiagnostics { get; set; }

        ILogger Logger { get; set; }
    }
}
