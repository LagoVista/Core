using LagoVista.Core.PlatformSupport;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LagoVista.Core;

namespace LagoVista.MessageQueue.Rabbit
{
    public class RabbitMqMessageQueueServiceClient : IDisposable
    {
        private readonly RabbitMqServiceRegistration _registration;
        private readonly ILogger _adminLogger;
        private readonly ConnectionFactory _connectionFactory;
        private readonly SemaphoreSlim _sync = new SemaphoreSlim(1, 1);

        private IConnection _connection;
        private RabbitMQ.Client.IChannel _channel;
        private bool _disposed;

        public string ServiceName => _registration.ServiceName;

        public RabbitMqMessageQueueServiceClient(RabbitMqServiceRegistration registration, ILogger adminLogger)
        {
            _registration = registration ?? throw new ArgumentNullException(nameof(registration));
            _registration.Validate();
            _adminLogger = adminLogger ?? throw new ArgumentNullException(nameof(adminLogger));

            _connectionFactory = new ConnectionFactory
            {
                ClientProvidedName = $"{nameof(RabbitMqMessageQueueServiceClient)}:{_registration.ServiceName}",
                HostName = _registration.ConnectionSettings.HostName,
                UserName = _registration.ConnectionSettings.UserName,
                Password = _registration.ConnectionSettings.Password,
                VirtualHost = _registration.ConnectionSettings.VirtualHost,
                Port = _registration.ConnectionSettings.Port,
                AutomaticRecoveryEnabled = _registration.ConnectionSettings.AutomaticRecoveryEnabled,
                TopologyRecoveryEnabled = _registration.ConnectionSettings.TopologyRecoveryEnabled,
                RequestedConnectionTimeout = TimeSpan.FromSeconds(_registration.ConnectionSettings.TimeoutInSeconds <= 0 ? 30 : _registration.ConnectionSettings.TimeoutInSeconds)
            };

            _connectionFactory.Ssl.Enabled = _registration.ConnectionSettings.UseSsl;
        }

        public async Task PublishAsync(object payload, Type messageType, CancellationToken cancellationToken = default)
        {
            if (payload == null) throw new ArgumentNullException(nameof(payload));
            if (messageType == null) throw new ArgumentNullException(nameof(messageType));

            await EnsureConnectedAsync(cancellationToken).ConfigureAwait(false);

            var route = _registration.Topology.GetPublishRoute(messageType);
            if (route == null) throw new InvalidOperationException($"No publish route found for message type '{messageType.FullName}' in service '{_registration.ServiceName}'.");
            if (String.IsNullOrWhiteSpace(route.DestinationName)) throw new InvalidOperationException($"Publish route for message type '{messageType.FullName}' in service '{_registration.ServiceName}' must provide a DestinationName.");
            if (String.IsNullOrWhiteSpace(route.RouteKey)) throw new InvalidOperationException($"Publish route for message type '{messageType.FullName}' in service '{_registration.ServiceName}' must provide a RouteKey.");

            var properties = new BasicProperties
            {
                ContentType = String.IsNullOrWhiteSpace(route.ContentType) ? "application/json" : route.ContentType,
                DeliveryMode = route.Persistent ? DeliveryModes.Persistent : DeliveryModes.Transient
            };

            var json = JsonConvert.SerializeObject(payload);
            var body = Encoding.UTF8.GetBytes(json);

            _adminLogger.Trace($"{nameof(RabbitMqMessageQueueServiceClient)} publishing '{messageType.FullName}' on service '{_registration.ServiceName}' to '{route.DestinationName}' with route key '{route.RouteKey}'.");

            await _channel.BasicPublishAsync(route.DestinationName, route.RouteKey, false, properties, body, cancellationToken).ConfigureAwait(false);

            _adminLogger.Trace($"{nameof(RabbitMqMessageQueueServiceClient)} published '{messageType.FullName}' on service '{_registration.ServiceName}'.");
        }

        private async Task EnsureConnectedAsync(CancellationToken cancellationToken)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(RabbitMqMessageQueueServiceClient));
            if (_channel != null && !_channel.IsClosed && _connection != null && _connection.IsOpen) return;

            await _sync.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                if (_channel != null && !_channel.IsClosed && _connection != null && _connection.IsOpen) return;

                SafeDispose(_channel);
                _channel = null;

                SafeDispose(_connection);
                _connection = null;

                var attempt = 0;
                while (true)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    try
                    {
                        _connection = await _connectionFactory.CreateConnectionAsync(cancellationToken).ConfigureAwait(false);
                        _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
                        return;
                    }
                    catch (Exception ex)
                    {
                        _adminLogger.AddException($"{nameof(RabbitMqMessageQueueServiceClient)}__EnsureConnectedAsync", ex, _registration.ServiceName.ToKVP("serviceName"), attempt.ToString().ToKVP("attempt"), _registration.ConnectionSettings.HostName.ToKVP("host"), _registration.ConnectionSettings.VirtualHost.ToKVP("virtualHost"));

                        attempt++;
                        if (attempt >= 5) throw;

                        await Task.Delay(TimeSpan.FromSeconds(1 + attempt), cancellationToken).ConfigureAwait(false);
                    }
                }
            }
            finally
            {
                _sync.Release();
            }
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            SafeDispose(_channel);
            _channel = null;

            SafeDispose(_connection);
            _connection = null;

            _sync.Dispose();
        }

        private static void SafeDispose(IDisposable disposable)
        {
            try
            {
                disposable?.Dispose();
            }
            catch
            {
            }
        }
    }
}