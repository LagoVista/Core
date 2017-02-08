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
    }
}
