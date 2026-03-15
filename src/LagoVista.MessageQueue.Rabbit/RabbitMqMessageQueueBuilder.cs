using LagoVista.Core.Interfaces;
using LagoVista.Core.MessageQueue;
using System;
using System.Collections.Generic;

namespace LagoVista.MessageQueue.Rabbit
{
    public class RabbitMqMessageQueueBuilder
    {
        private readonly List<RabbitMqServiceRegistration> _serviceRegistrations = new List<RabbitMqServiceRegistration>();
        private readonly List<RabbitMqTypeRegistration> _typeRegistrations = new List<RabbitMqTypeRegistration>();

        public IReadOnlyList<RabbitMqServiceRegistration> ServiceRegistrations => _serviceRegistrations;
        public IReadOnlyList<RabbitMqTypeRegistration> TypeRegistrations => _typeRegistrations;

        public RabbitMqMessageQueueBuilder AddService(string serviceName, RabbitMqConnectionSettings connectionSettings, IMessageQueueTopology topology)
        {
            _serviceRegistrations.Add(new RabbitMqServiceRegistration
            {
                ServiceName = serviceName,
                ConnectionSettings = connectionSettings,
                Topology = topology
            });

            return this;
        }

        public RabbitMqMessageQueueBuilder AddService(string serviceName, IConnectionSettings connectionSettings, IMessageQueueTopology topology)
        {
            if (connectionSettings == null) throw new ArgumentNullException(nameof(connectionSettings));
            return AddService(serviceName, RabbitMqConnectionSettings.From(connectionSettings), topology);
        }

        public RabbitMqMessageQueueBuilder AddMessageType<T>(string serviceName)
        {
            _typeRegistrations.Add(new RabbitMqTypeRegistration
            {
                MessageType = typeof(T),
                ServiceName = serviceName
            });

            return this;
        }
    }
}
