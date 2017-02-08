using LagoVista.Core.Networking.Models;
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
    }
}
