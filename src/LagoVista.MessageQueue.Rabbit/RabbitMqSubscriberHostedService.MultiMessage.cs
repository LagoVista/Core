using LagoVista.Core;
using LagoVista.Core.Interfaces;
using LagoVista.Core.MessageQueue;
using LagoVista.Core.Models.Diagnostics;
using LagoVista.Core.PlatformSupport;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.MessageQueue.Rabbit
{
    public class RabbitMqSubscriberHostedService : BackgroundService, IHostedServiceDiagnostics
    {
        private readonly string _serviceName;
        private readonly RabbitMqSubscriberSettings _settings;
        private readonly ILogger _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ConnectionFactory _connectionFactory;
        private readonly IReadOnlyDictionary<string, RabbitMqSubscriberHandlerRegistration> _registrations;
        private readonly IApplicationRuntimeState _runtimeState;
        private readonly HostedServiceDiagnosticSnapshot _snapShot = new HostedServiceDiagnosticSnapshot();

        private IConnection _connection;
        private RabbitMQ.Client.IChannel _channel;
        private string _consumerTag;
        private int _processedMessage;
        private int _activeMessageCount;

        public string Name => $"RabbitMqSubscriberHostedService - {_serviceName}";

        internal RabbitMqSubscriberHostedService(string serviceName, RabbitMqSubscriberSettings settings, IEnumerable<RabbitMqSubscriberHandlerRegistration> registrations, ILogger logger, IServiceScopeFactory scopeFactory, IApplicationRuntimeState runtimeState)
        {
            if (String.IsNullOrWhiteSpace(serviceName)) throw new ArgumentNullException(nameof(serviceName));
            _serviceName = serviceName;
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
            _runtimeState = runtimeState ?? throw new ArgumentNullException(nameof(runtimeState));
            if (registrations == null) throw new ArgumentNullException(nameof(registrations));

            _settings.Validate(serviceName);

            var registrationLookup = new Dictionary<string, RabbitMqSubscriberHandlerRegistration>(StringComparer.OrdinalIgnoreCase);
            foreach (var registration in registrations)
                registrationLookup.Add(registration.MessageTypeName, registration);

            _registrations = registrationLookup;

            _connectionFactory = new ConnectionFactory
            {
                ClientProvidedName = $"{nameof(RabbitMqSubscriberHostedService)}:{serviceName}",
                HostName = _settings.HostName,
                UserName = _settings.UserName,
                Password = _settings.Password,
                VirtualHost = _settings.VirtualHost,
                Port = _settings.Port,
                AutomaticRecoveryEnabled = _settings.AutomaticRecoveryEnabled,
                TopologyRecoveryEnabled = _settings.TopologyRecoveryEnabled,
                RequestedConnectionTimeout = TimeSpan.FromSeconds(_settings.TimeoutInSeconds <= 0 ? 30 : _settings.TimeoutInSeconds)
            };

            _connectionFactory.Ssl.Enabled = _settings.UseSsl;
        }

        public HostedServiceDiagnosticSnapshot GetSnapshot() => _snapShot;

        public async Task StartIt()
        {
            await ExecuteAsync(CancellationToken.None).ConfigureAwait(false);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                _logger.Trace($"{this.Tag()} starting '{_serviceName}'.", _serviceName.ToKVP("serviceName"), _settings.QueueName.ToKVP("queueName"), _settings.ExchangeName.ToKVP("destinationName"), _settings.RouteKey.ToKVP("routeKey"));
                await EnsureConnectedAsync(stoppingToken).ConfigureAwait(false);

                var consumer = new AsyncEventingBasicConsumer(_channel);
                consumer.ReceivedAsync += async (_, args) => await HandleMessageAsync(args).ConfigureAwait(false);

                _consumerTag = await _channel.BasicConsumeAsync(_settings.QueueName, false, consumer, stoppingToken).ConfigureAwait(false);
                _snapShot.Status = HostedServiceDiagnosticStatus.Running;
                _snapShot.StartedUtc = DateTime.UtcNow;
                _snapShot.LastActivity = "Started";
                _snapShot.LastActivityUtc = DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                _logger.AddException(this.Tag(), ex, _settings.UserName.ToKVP("userName"), _settings.VirtualHost.ToKVP("vhost"), _serviceName.ToKVP("serviceName"),
                    _settings.QueueName.ToKVP("queueName"), _settings.ExchangeName.ToKVP("destinationName"), _settings.RouteKey.ToKVP("routeKey"));
                _snapShot.Status = HostedServiceDiagnosticStatus.Error;
                _snapShot.LastError = ex.Message;
            }

            try
            {
                await Task.Delay(Timeout.Infinite, stoppingToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_channel != null && !_channel.IsClosed && !String.IsNullOrWhiteSpace(_consumerTag))
            {
                try
                {
                    await _channel.BasicCancelAsync(_consumerTag, cancellationToken: cancellationToken).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.AddException($"{nameof(RabbitMqSubscriberHostedService)}__StopAsync__BasicCancel", ex, _serviceName.ToKVP("serviceName"));
                }
            }

            while (Volatile.Read(ref _activeMessageCount) > 0)
                await Task.Delay(100, cancellationToken).ConfigureAwait(false);

            await base.StopAsync(cancellationToken).ConfigureAwait(false);

            SafeDispose(_channel);
            _channel = null;
            SafeDispose(_connection);
            _connection = null;
        }

        private async Task EnsureConnectedAsync(CancellationToken cancellationToken)
        {
            if (_connection != null && _connection.IsOpen && _channel != null && !_channel.IsClosed) return;

            _connection = await _connectionFactory.CreateConnectionAsync(cancellationToken).ConfigureAwait(false);
            _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

            await _channel.BasicQosAsync(0, _settings.PrefetchCount, false, cancellationToken).ConfigureAwait(false);
            await _channel.ExchangeDeclarePassiveAsync(_settings.ExchangeName).ConfigureAwait(false);
        }

        private async Task HandleMessageAsync(BasicDeliverEventArgs args)
        {
            Interlocked.Increment(ref _activeMessageCount);
            IDisposable workLease = null;
            var messageTypeName = args.BasicProperties?.Type;

            try
            {
                if (String.IsNullOrWhiteSpace(messageTypeName))
                    throw new InvalidOperationException($"RabbitMQ message received by '{_serviceName}' did not contain a message type.");

                if (!_registrations.TryGetValue(messageTypeName, out var registration))
                    throw new InvalidOperationException($"RabbitMQ subscriber '{_serviceName}' does not have a handler registered for message type '{messageTypeName}'.");

                var messageId = args.BasicProperties?.MessageId;
                _runtimeState.TryBeginWork(
                    "RabbitMQ",
                    messageTypeName,
                    messageId,
                    item => _logger.AddCustomEvent(
                        LogLevel.Error,
                        $"{nameof(RabbitMqSubscriberHostedService)}__LongRunningWork",
                        $"RabbitMQ handler '{item.Name}' on service '{_serviceName}' has been running for more than 5 minutes. MessageId={item.CorrelationId}, StartedUtc={item.StartedUtc:O}, WorkId={item.WorkId}."),
                    out workLease);

                var json = Encoding.UTF8.GetString(args.Body.ToArray());
                var payload = JsonConvert.DeserializeObject(json, registration.MessageType);
                if (payload == null)
                    throw new InvalidOperationException($"RabbitMQ message body for '{messageTypeName}' could not be deserialized.");

                using var scope = _scopeFactory.CreateScope();

                _snapShot.LastActivityUtc = DateTime.UtcNow;
                _snapShot.LastActivity = $"Process message {++_processedMessage} ({messageTypeName})";

                _logger.Trace($"{this.Tag()} handling message '{messageTypeName}' with {registration.HandlerType.Name}.");

                await registration.Dispatcher.DispatchAsync(payload, args, scope.ServiceProvider, CancellationToken.None).ConfigureAwait(false);
                await _channel.BasicAckAsync(args.DeliveryTag, false, CancellationToken.None).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _snapShot.LastError = ex.Message;
                _snapShot.LastErrorUtc = DateTime.UtcNow;

                _logger.AddException($"{this.Tag()}", ex, _serviceName.ToKVP("serviceName"), messageTypeName.ToKVP("messageType"));

                if (_channel != null && !_channel.IsClosed)
                    await _channel.BasicNackAsync(args.DeliveryTag, false, false, CancellationToken.None).ConfigureAwait(false);
            }
            finally
            {
                workLease?.Dispose();
                Interlocked.Decrement(ref _activeMessageCount);
            }
        }

        private static void SafeDispose(IDisposable disposable)
        {
            try { disposable?.Dispose(); } catch { }
        }
    }
}
