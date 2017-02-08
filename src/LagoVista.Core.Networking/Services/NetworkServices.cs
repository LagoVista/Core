using LagoVista.Core.IOC;
using LagoVista.Core.Networking.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.Core.Networking.Services
{
    public class NetworkServices
    {
        public static ISSDPServer CreateSSDPServer()
        {
            ISSDPServer ssdpServer;
            if (SLWIOC.TryResolve<ISSDPServer>(out ssdpServer))
                return ssdpServer;

            throw new Exception("Nothing registered for type ISSDPServer.");
        }

        public static IWebServer CreateWebServer()
        {
            IWebServer webServer;
            if (SLWIOC.TryResolve(out webServer))
                return webServer;

            throw new Exception("Nothing registered for type IWebServer.");
        }

        public static ISSDPClient CreateSSDPClient()
        {
            ISSDPClient ssdpClient;
            if (SLWIOC.TryResolve(out ssdpClient))
                return ssdpClient;

            throw new Exception("Nothing registered for type ISSDPClient.");
        }
    }
}
