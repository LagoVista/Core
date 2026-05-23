using LagoVista.Core.Models;
using LagoVista.Core.Rcg.Client.Interfaces;
using Microsoft.Extensions.Configuration;
using System;

namespace LagoVista.Core.Rcg.Client.Models
{
    public class RcgClientSettings : IRcgClientSettings
    {
        public string GatewayBaseUrl { get; set; }
        public string CallerId { get; set; }
        public string Version { get; set; }
        public int TimeoutSeconds { get; set; }
        public RcgServerKeys ServerKeys { get; set; }

        public RcgClientSettings(IConfiguration config)
        {
            var appKey = config["AppKey"];
            var configSection = config.GetSection("RcgClientSettings");

            GatewayBaseUrl = configSection.Require("GatewayBaseUrl");
            CallerId = appKey;
            Version = "1";
            TimeoutSeconds = Convert.ToInt32(configSection.Require("TimeoutSeconds") ?? "0"); 
            ServerKeys = config.Map<RcgServerKeys>();
        }

        public string GetSigningKey()
        {
            if (String.IsNullOrWhiteSpace(CallerId))
            {
                throw new InvalidOperationException("RCG caller id is required.");
            }

            if (ServerKeys == null)
            {
                throw new InvalidOperationException("RCG server keys are required.");
            }

            if (String.Equals(CallerId, "web", StringComparison.OrdinalIgnoreCase))
            {
                if (String.IsNullOrWhiteSpace(ServerKeys.PortalKey1))
                {
                    throw new InvalidOperationException("RCG portal signing key is required.");
                }

                return ServerKeys.PortalKey1;
            }

            if (String.Equals(CallerId, "api", StringComparison.OrdinalIgnoreCase))
            {
                if (String.IsNullOrWhiteSpace(ServerKeys.ApiKey1))
                {
                    throw new InvalidOperationException("RCG API signing key is required.");
                }

                return ServerKeys.ApiKey1;
            }

            throw new InvalidOperationException($"Unsupported RCG caller id '{CallerId}'.");
        }
    }
}
