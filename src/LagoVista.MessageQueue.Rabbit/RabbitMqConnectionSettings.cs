
using LagoVista.Core.Interfaces;
using System;
using System.Collections.Generic;

namespace LagoVista.MessageQueue.Rabbit
{
    public class RabbitMqConnectionSettings
    {
        public string Name { get; set; }
        public string HostName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string VirtualHost { get; set; } = "/";
        public int Port { get; set; } = 5672;
        public bool UseSsl { get; set; }
        public int TimeoutInSeconds { get; set; } = 30;
        public bool AutomaticRecoveryEnabled { get; set; } = true;
        public bool TopologyRecoveryEnabled { get; set; } = true;
        public Dictionary<string, string> Settings { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public static RabbitMqConnectionSettings From(IConnectionSettings settings)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));

            var rabbitSettings = new RabbitMqConnectionSettings
            {
                Name = settings.Name,
                HostName = settings.Uri,
                UserName = settings.UserName,
                Password = settings.Password,
                UseSsl = settings.IsSSL,
                TimeoutInSeconds = settings.TimeoutInSeconds <= 0 ? 30 : settings.TimeoutInSeconds,
                Settings = settings.Settings ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            };

            if (!String.IsNullOrWhiteSpace(settings.Port) && Int32.TryParse(settings.Port, out var port))
                rabbitSettings.Port = port;

            if (rabbitSettings.Settings.TryGetValue("VirtualHost", out var virtualHost) && !String.IsNullOrWhiteSpace(virtualHost))
                rabbitSettings.VirtualHost = virtualHost;

            if (rabbitSettings.Settings.TryGetValue("AutomaticRecoveryEnabled", out var automaticRecoveryEnabled) && Boolean.TryParse(automaticRecoveryEnabled, out var automaticRecovery))
                rabbitSettings.AutomaticRecoveryEnabled = automaticRecovery;

            if (rabbitSettings.Settings.TryGetValue("TopologyRecoveryEnabled", out var topologyRecoveryEnabled) && Boolean.TryParse(topologyRecoveryEnabled, out var topologyRecovery))
                rabbitSettings.TopologyRecoveryEnabled = topologyRecovery;

            return rabbitSettings;
        }
    }
}
