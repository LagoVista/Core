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
        public string ExchangeName { get; set; }
        public string QueueName { get; set; }
        public string RouteKey { get; set; }

        public bool UseSsl { get; set; }
        public int TimeoutInSeconds { get; set; } = 30;
        public bool AutomaticRecoveryEnabled { get; set; } = true;
        public bool TopologyRecoveryEnabled { get; set; } = true;
        

        public bool Durable { get; set; } = true;
        public bool Exclusive { get; set; }
        public bool AutoDelete { get; set; }
        public ushort PrefetchCount { get; set; } = 1;

        public static RabbitMqSubscriberSettings Read(IConfiguration configuration, string sectionName)
        {
            if(configuration == null) throw new ArgumentNullException(nameof(configuration));
            if(String.IsNullOrWhiteSpace(sectionName)) throw new ArgumentException(nameof(sectionName));

            var section = configuration.GetRequiredSection(sectionName);

            //TODO There is probably another round of validating the settings for valid content for Rabbit but not today.
            return new RabbitMqSubscriberSettings()
            {
                Name = section.Require("Name"),
                HostName = section.Require("HostName"),
                UserName = section.Require("UserName"),
                Password = section.Require("Password"),
                RouteKey = section.Require("RouteKey"),
                VirtualHost = section.Optional("VirtualHost", "/"),
                Port = Convert.ToInt32(section.Optional("Port", "5672")),
                UseSsl = section.Optional("UseSsl", "false").ToLower() == "true",
                ExchangeName = section.Require("ExchangeName"),
                QueueName = section.Require("QueueName"),
                TimeoutInSeconds = Convert.ToInt32(section.Optional("TimeoutInSeconds", "30")),
                AutomaticRecoveryEnabled = section.Optional("AutomaticRecoveryEnabled", "true").ToLower() == "true",
                TopologyRecoveryEnabled = section.Optional("TopologyRecoveryEnabled", "true").ToLower() == "true",   
                Durable = section.Optional("Durable", "true").ToLower() == "true",
                Exclusive = section.Optional("Exclusive", "false").ToLower() == "true",
                AutoDelete = section.Optional("AutoDelete", "false").ToLower() == "true",
                PrefetchCount = Convert.ToUInt16(section.Optional("PrefetchCount", "1"))
            };    
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
