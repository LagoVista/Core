using LagoVista.Core.MessageQueue;
using System;

namespace LagoVista.MessageQueue.Rabbit
{
    public class RabbitMqServiceRegistration
    {
        public string ServiceName { get; set; }
        public RabbitMqConnectionSettings ConnectionSettings { get; set; }
        public IMessageQueueTopology Topology { get; set; }

        public void Validate()
        {
            if (String.IsNullOrWhiteSpace(ServiceName)) throw new InvalidOperationException("RabbitMQ service registration requires a ServiceName.");
            if (ConnectionSettings == null) throw new InvalidOperationException($"RabbitMQ service '{ServiceName}' requires ConnectionSettings.");
            if (Topology == null) throw new InvalidOperationException($"RabbitMQ service '{ServiceName}' requires a topology.");
            if (String.IsNullOrWhiteSpace(ConnectionSettings.HostName)) throw new InvalidOperationException($"RabbitMQ service '{ServiceName}' requires a host name.");
            if (String.IsNullOrWhiteSpace(ConnectionSettings.UserName)) throw new InvalidOperationException($"RabbitMQ service '{ServiceName}' requires a user name.");
            if (String.IsNullOrWhiteSpace(ConnectionSettings.Password)) throw new InvalidOperationException($"RabbitMQ service '{ServiceName}' requires a password.");
            if (String.IsNullOrWhiteSpace(ConnectionSettings.VirtualHost)) throw new InvalidOperationException($"RabbitMQ service '{ServiceName}' requires a virtual host.");
        }
    }
}