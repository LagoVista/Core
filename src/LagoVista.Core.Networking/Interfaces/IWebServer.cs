// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 8a13d8c92b6a240c504f842b5bbded25acdd38ffc0d43debe91a6e1c60bd4492
// IndexVersion: 1
// --- END CODE INDEX META ---
using LagoVista.Core.PlatformSupport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.Core.Networking.Interfaces
{
    public interface IWebServer
    {
        void StartServer(int port);
        int Port { get; }
        void RegisterAPIHandler(IApiHandler handler);

        bool ShowDiagnostics { get; set; }

        ILogger Logger { get; set; }

        string DefaultPageHtml { get; set; }
    }
}
