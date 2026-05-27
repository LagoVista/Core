using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Interfaces
{
    public enum SignedServiceHttpTarget
    {
        PortalServer,
        ApiServer,
        RcgServer,
        McpServer,
        NotificationServer,
        ConfigurationServer
    }

    public interface IEnvironmentEndpoints
    {
        string PortalServer { get; }
        string ApiServer { get; }
        string RcgServer { get; }
        string McpServer { get; }
        string NotificationServer { get; }
        string ConfigurationServer { get; }

        string GetBaseUrl(SignedServiceHttpTarget target);
    }
}
