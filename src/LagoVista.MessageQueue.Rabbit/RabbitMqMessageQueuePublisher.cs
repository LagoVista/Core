using LagoVista.Core.MessageQueue;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.MessageQueue.Rabbit
{
    public class RabbitMqMessageQueuePublisher : IMessageQueuePublisher, IDisposable
    {
        private readonly IMessageQueueTypeRegistry _typeRegistry;
        private readonly Dictionary<string, RabbitMqMessageQueueServiceClient> _serviceClients;
        private bool _disposed;

        public RabbitMqMessageQueuePublisher(IMessageQueueTypeRegistry typeRegistry, IEnumerable<RabbitMqMessageQueueServiceClient> serviceClients)
        {
            _typeRegistry = typeRegistry ?? throw new ArgumentNullException(nameof(typeRegistry));
            if (serviceClients == null) throw new ArgumentNullException(nameof(serviceClients));

            _serviceClients = new Dictionary<string, RabbitMqMessageQueueServiceClient>(StringComparer.OrdinalIgnoreCase);
            foreach (var serviceClient in serviceClients)
            {
                if (serviceClient == null) continue;

                if (_serviceClients.ContainsKey(serviceClient.ServiceName))
                    throw new InvalidOperationException($"RabbitMQ service '{serviceClient.ServiceName}' has already been registered.");

                _serviceClients.Add(serviceClient.ServiceName, serviceClient);
            }
        }

        public async Task PublishAsync<T>(T payload, CancellationToken cancellationToken = default)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(RabbitMqMessageQueuePublisher));
            if (payload == null) throw new ArgumentNullException(nameof(payload));

            var messageType = payload.GetType();
            var serviceName = _typeRegistry.GetServiceNameFor(messageType);

            if (!_serviceClients.TryGetValue(serviceName, out var serviceClient))
                throw new InvalidOperationException($"RabbitMQ service '{serviceName}' is not available for message type '{messageType.FullName}'.");

            await serviceClient.PublishAsync(payload, messageType, cancellationToken).ConfigureAwait(false);
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            foreach (var serviceClient in _serviceClients.Values)
                serviceClient.Dispose();

            _serviceClients.Clear();
        }
    }
}