using Microsoft.Extensions.Configuration;
using System;

namespace LagoVista.MessageQueue.Rabbit
{
    public class RabbitMqPublisherSettings
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
        public string ExchangeName { get; set; }
        public string RouteKey { get; set; }
        public string ContentType { get; set; } = "application/json";
        public bool Persistent { get; set; } = true;

        public static RabbitMqPublisherSettings Read(IConfiguration configuration, string sectionName)
        {

            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            if (String.IsNullOrEmpty(sectionName)) throw new ArgumentNullException(nameof(sectionName));

            var section = configuration.GetRequiredSection(sectionName);
            var settings = section.Get<RabbitMqPublisherSettings>();
            if (settings == null)
                throw new InvalidOperationException($"Could not bind RabbitMQ publisher settings from section '{sectionName}'.");

            settings.Validate(sectionName);
            return settings;
        }

        public RabbitMqConnectionSettings ToConnectionSettings()
        {
            Validate(Name ?? HostName ?? nameof(RabbitMqPublisherSettings));

            return new RabbitMqConnectionSettings
            {
                Name = Name,
                HostName = HostName,
                UserName = UserName,
                Password = Password,
                VirtualHost = VirtualHost,
                Port = Port <= 0 ? 5672 : Port,
                UseSsl = UseSsl,
                TimeoutInSeconds = TimeoutInSeconds <= 0 ? 30 : TimeoutInSeconds,
                AutomaticRecoveryEnabled = AutomaticRecoveryEnabled,
                TopologyRecoveryEnabled = TopologyRecoveryEnabled
            };
        }

        public void Validate(string sectionName)
        {
            if (String.IsNullOrWhiteSpace(HostName)) throw new InvalidOperationException($"RabbitMQ publisher section '{sectionName}' requires HostName.");
            if (String.IsNullOrWhiteSpace(UserName)) throw new InvalidOperationException($"RabbitMQ publisher section '{sectionName}' requires UserName.");
            if (String.IsNullOrWhiteSpace(Password)) throw new InvalidOperationException($"RabbitMQ publisher section '{sectionName}' requires Password.");
            if (String.IsNullOrWhiteSpace(VirtualHost)) throw new InvalidOperationException($"RabbitMQ publisher section '{sectionName}' requires VirtualHost.");
            if (Port <= 0) throw new InvalidOperationException($"RabbitMQ publisher section '{sectionName}' requires a valid Port.");
            if (String.IsNullOrWhiteSpace(ExchangeName)) throw new InvalidOperationException($"RabbitMQ publisher section '{sectionName}' requires ExchangeName.");
            if (String.IsNullOrWhiteSpace(RouteKey)) throw new InvalidOperationException($"RabbitMQ publisher section '{sectionName}' requires RouteKey.");
        }
    }
}
