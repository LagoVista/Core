using LagoVista.Core.MessageQueue;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LagoVista.MessageQueue.Rabbit
{
    public class RabbitMqMessageQueueTypeRegistry : IMessageQueueTypeRegistry
    {
        private readonly Dictionary<Type, string> _serviceNamesByType;

        public RabbitMqMessageQueueTypeRegistry(IEnumerable<RabbitMqTypeRegistration> typeRegistrations, IEnumerable<string> knownServiceNames)
        {
            if (typeRegistrations == null) throw new ArgumentNullException(nameof(typeRegistrations));
            if (knownServiceNames == null) throw new ArgumentNullException(nameof(knownServiceNames));

            var serviceNameSet = new HashSet<string>(knownServiceNames, StringComparer.OrdinalIgnoreCase);
            _serviceNamesByType = new Dictionary<Type, string>();

            foreach (var registration in typeRegistrations)
            {
                registration.Validate();

                if (!serviceNameSet.Contains(registration.ServiceName))
                    throw new InvalidOperationException($"RabbitMQ message type '{registration.MessageType.FullName}' is mapped to unknown service '{registration.ServiceName}'.");

                if (_serviceNamesByType.ContainsKey(registration.MessageType))
                    throw new InvalidOperationException($"RabbitMQ message type '{registration.MessageType.FullName}' has already been registered.");

                _serviceNamesByType.Add(registration.MessageType, registration.ServiceName);
            }
        }

        public string GetServiceNameFor(Type messageType)
        {
            if (messageType == null) throw new ArgumentNullException(nameof(messageType));

            if (!_serviceNamesByType.TryGetValue(messageType, out var serviceName))
                throw new InvalidOperationException($"No RabbitMQ service has been registered for message type '{messageType.FullName}'.");

            return serviceName;
        }
    }
}
