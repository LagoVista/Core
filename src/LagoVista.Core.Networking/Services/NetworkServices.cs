﻿using LagoVista.Core.IOC;
using LagoVista.Core.Networking.Interfaces;
using LagoVista.Core.PlatformSupport;
using System;

namespace LagoVista.Core.Networking.Services
{
    public class NetworkServices
    {
        public static ISSDPServer GetSSDPServer(ILogger logger = null)
        {
            ISSDPServer ssdpServer;
            if (SLWIOC.TryResolve<ISSDPServer>(out ssdpServer))
            {
                ssdpServer.Logger = logger == null ? SLWIOC.Get<ILogger>() : logger;
                return ssdpServer;
            }

            throw new Exception("Nothing registered for type ISSDPServer.");
        }

        public static IWebServer GetWebServer(ILogger logger = null)
        {
            IWebServer webServer;
            if (SLWIOC.TryResolve(out webServer))
            {
                webServer.Logger = logger == null ? SLWIOC.Get<ILogger>() : logger;
                return webServer;
            }

            throw new Exception("Nothing registered for type IWebServer.");
        }

        public static IWebServer CreateWebServer(ILogger logger = null)
        {
            var webServer = SLWIOC.Create<IWebServer>();
            webServer.Logger = logger == null ? SLWIOC.Get<ILogger>() : logger;
            return webServer;
        }

        public static ISSDPClient GetSSDPClient(ILogger logger = null)
        {
            ISSDPClient ssdpClient;
            if (SLWIOC.TryResolve(out ssdpClient))
            {
                ssdpClient.Logger = logger == null ? SLWIOC.Get<ILogger>() : logger;
                return ssdpClient;
            }

            throw new Exception("Nothing registered for type ISSDPClient.");
        }

        public static ISSDPClient CreateSSDPClient(ILogger logger = null)
        {
            var ssdpClient = SLWIOC.Create<ISSDPClient>();
            ssdpClient.Logger = logger == null ? SLWIOC.Get<ILogger>() : logger;
            return ssdpClient;
        }

    }
}
