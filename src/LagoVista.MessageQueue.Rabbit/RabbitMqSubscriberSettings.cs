using Microsoft.Extensions.Configuration;
using System;

namespace LagoVista.MessageQueue.Rabbit
{
    public class RabbitMqSubscriberSettings
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
        public string QueueName { get; set; }
        public string RouteKey { get; set; }
        public bool Durable { get; set; } = true;
        public bool Exclusive { get; set; }
        public bool AutoDelete { get; set; }
        public ushort PrefetchCount { get; set; } = 1;

        public static RabbitMqSubscriberSettings Read(IConfiguration configuration, string sectionName)
        {
            if(configuration == null) throw new ArgumentNullException(nameof(configuration));
            if(String.IsNullOrWhiteSpace(sectionName)) throw new ArgumentNullException(nameof(sectionName));

            var section = configuration.GetRequiredSection(sectionName);
            var settings = section.Get<RabbitMqSubscriberSettings>();
            if (settings == null)
                throw new InvalidOperationException($"Could not bind RabbitMQ subscriber settings from section '{sectionName}'.");

            settings.Validate(sectionName);
            return settings;
        }

        public RabbitMqConnectionSettings ToConnectionSettings()
        {
            Validate(Name ?? HostName ?? nameof(RabbitMqSubscriberSettings));

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
            if (String.IsNullOrWhiteSpace(HostName)) throw new InvalidOperationException($"RabbitMQ subscriber section '{sectionName}' requires HostName.");
            if (String.IsNullOrWhiteSpace(UserName)) throw new InvalidOperationException($"RabbitMQ subscriber section '{sectionName}' requires UserName.");
            if (String.IsNullOrWhiteSpace(Password)) throw new InvalidOperationException($"RabbitMQ subscriber section '{sectionName}' requires Password.");
            if (String.IsNullOrWhiteSpace(VirtualHost)) throw new InvalidOperationException($"RabbitMQ subscriber section '{sectionName}' requires VirtualHost.");
            if (Port <= 0) throw new InvalidOperationException($"RabbitMQ subscriber section '{sectionName}' requires a valid Port.");
            if (String.IsNullOrWhiteSpace(ExchangeName)) throw new InvalidOperationException($"RabbitMQ subscriber section '{sectionName}' requires ExchangeName.");
            if (String.IsNullOrWhiteSpace(QueueName)) throw new InvalidOperationException($"RabbitMQ subscriber section '{sectionName}' requires QueueName.");
            if (String.IsNullOrWhiteSpace(RouteKey)) throw new InvalidOperationException($"RabbitMQ subscriber section '{sectionName}' requires RouteKey.");
        }
    }
}
