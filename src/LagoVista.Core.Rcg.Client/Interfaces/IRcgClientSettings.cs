using LagoVista.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Rcg.Client.Interfaces
{
    public interface IRcgClientSettings
    {
        string GatewayBaseUrl { get; }
        string AppKey { get; set; }
        string Version { get; }
        int TimeoutSeconds { get; }
        RcgServerKeys ServerKeys { get; }

        string GetSigningKey();
    }
}
