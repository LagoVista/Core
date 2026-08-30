using System;

namespace LagoVista.Core.Configuration
{
    public class RemoteConfigurationSettings
    {
        public string ConfigurationServiceBaseUrl { get; set; }

        public string AuthorizationToken { get; set; }

        public int TimeoutMs { get; set; } = 60000;

        public void Validate()
        {
            if (String.IsNullOrWhiteSpace(ConfigurationServiceBaseUrl))
            {
                throw new InvalidOperationException("ConfigurationServiceBaseUrl is required.");
            }

            if (String.IsNullOrWhiteSpace(AuthorizationToken))
            {
                throw new InvalidOperationException("AuthorizationToken is required.");
            }

            if (TimeoutMs <= 0)
            {
                throw new InvalidOperationException("TimeoutMs must be greater than zero.");
            }
        }
    }
}
