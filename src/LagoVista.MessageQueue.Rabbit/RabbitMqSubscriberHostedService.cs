using LagoVista.Core;
using LagoVista.Core.Interfaces;
using LagoVista.Core.MessageQueue;
using LagoVista.Core.Models.Dignostics;
using LagoVista.Core.PlatformSupport;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.MessageQueue.Rabbit
{
    public class RabbitMqSubscriberHostedService<TMessage> : BackgroundService, IHostedServiceDiagnostics
    {
        private readonly string _serviceName;
        private readonly RabbitMqSubscriberSettings _settings;
        private readonly IMessageQueueTopology _topology;
        private readonly ILogger _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ConnectionFactory _connectionFactory;

        HostedServiceDiagnosticSnapshot _snapShot = new HostedServiceDiagnosticSnapshot()
        {

        };

        private IConnection _connection;
        private RabbitMQ.Client.IChannel _channel;
        private string _consumerTag;

        public string Name => $"RabbitMqSubscriberHostedService - {_serviceName}";

        public HostedServiceDiagnosticSnapshot GetSnapshot()
        {
            return _snapShot;
        }

        public RabbitMqSubscriberHostedService(string serviceName, RabbitMqSubscriberSettings settings, IMessageQueueTopology topology, ILogger logger, IServiceScopeFactory scopeFactory)
        {
            if (String.IsNullOrWhiteSpace(serviceName)) throw new ArgumentNullException(nameof(serviceName));
            _serviceName = serviceName;
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _topology = topology ?? throw new ArgumentNullException(nameof(topology));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));

            _settings.Validate(serviceName);

            _connectionFactory = new ConnectionFactory
            {
                ClientProvidedName = $"{nameof(RabbitMqSubscriberHostedService<TMessage>)}:{serviceName}",
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

        public async Task StartIt()
        {
            await ExecuteAsync(CancellationToken.None).ConfigureAwait(false);   
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var route = _topology.GetSubscriptionRoute(typeof(TMessage));
         
            try
            {
                _logger.Trace($"{nameof(RabbitMqSubscriberHostedService<TMessage>)} starting '{_serviceName}'.", _serviceName.ToKVP("serviceName"), route.QueueName.ToKVP("queueName"), route.DestinationName.ToKVP("destinationName"), route.RouteKey.ToKVP("routeKey"));

                await EnsureConnectedAsync(route, stoppingToken).ConfigureAwait(false);

                var consumer = new AsyncEventingBasicConsumer(_channel);
                consumer.ReceivedAsync += async (_, args) => await HandleMessageAsync(args, stoppingToken).ConfigureAwait(false);

                _consumerTag = await _channel.BasicConsumeAsync(route.QueueName, false, consumer, stoppingToken).ConfigureAwait(false);
                _snapShot.Status = HostedServiceDiagnosticStatus.Running;
                _snapShot.StartedUtc = DateTime.UtcNow;
                _snapShot.LastActivity = "Started";
                _snapShot.LastActivityUtc = DateTime.UtcNow;  
            }
            catch (Exception ex)
            {
                _logger.AddException($"[RabbitMqSubscriberHostedService__ExecuteAsync__{nameof(RabbitMqSubscriberHostedService<TMessage>)}]", ex, _serviceName.ToKVP("serviceName"),
                    route.QueueName.ToKVP("queueName"), route.DestinationName.ToKVP("destinationName"), route.RouteKey.ToKVP("routeKey"));
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
                catch
                {
                }
            }

            await base.StopAsync(cancellationToken).ConfigureAwait(false);

            SafeDispose(_channel);
            _channel = null;

            SafeDispose(_connection);
            _connection = null;
        }

        private async Task EnsureConnectedAsync(MessageQueueSubscriptionRoute route, CancellationToken cancellationToken)
        {
            if (_connection != null && _connection.IsOpen && _channel != null && !_channel.IsClosed) return;

            _connection = await _connectionFactory.CreateConnectionAsync(cancellationToken).ConfigureAwait(false);
            _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

            await _channel.BasicQosAsync(0, route.PrefetchCount, false, cancellationToken).ConfigureAwait(false);
            await _channel.ExchangeDeclarePassiveAsync(_settings.ExchangeName);
        }

        int processedMessage;

        private async Task HandleMessageAsync(BasicDeliverEventArgs args, CancellationToken cancellationToken)
        {
            try
            {
                var json = Encoding.UTF8.GetString(args.Body.ToArray());
                var payload = JsonConvert.DeserializeObject<TMessage>(json);
                if (payload == null) throw new InvalidOperationException($"RabbitMQ message body for '{typeof(TMessage).FullName}' could not be deserialized.");

                using var scope = _scopeFactory.CreateScope();
                var handler = scope.ServiceProvider.GetRequiredService<IMessageQueueHandler<TMessage>>();

                var context = new MessageQueueContext<TMessage>
                {
                    Payload = payload,
                    MessageId = args.BasicProperties?.MessageId,
                    MessageType = typeof(TMessage).FullName,
                    ReceivedAtUtc = DateTime.UtcNow,
                    Headers = ConvertHeaders(args.BasicProperties?.Headers)
                };

                _snapShot.LastActivityUtc = DateTime.UtcNow;
                _snapShot.LastActivity = $"Process message {++processedMessage}";

                await handler.HandleAsync(context, cancellationToken).ConfigureAwait(false);
                await _channel.BasicAckAsync(args.DeliveryTag, false, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _snapShot.LastError = ex.Message;
                _snapShot.LastErrorUtc = DateTime.UtcNow;

                _logger.AddException($"{nameof(RabbitMqSubscriberHostedService<TMessage>)}__HandleMessageAsync", ex, _serviceName.ToKVP("serviceName"), typeof(TMessage).FullName.ToKVP("messageType"));

                if (_channel != null && !_channel.IsClosed)
                    await _channel.BasicNackAsync(args.DeliveryTag, false, false, cancellationToken).ConfigureAwait(false);
            }
        }

        private static IReadOnlyDictionary<string, string> ConvertHeaders(IDictionary<string, object> headers)
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (headers == null) return result;

            foreach (var header in headers)
            {
                if (header.Value is byte[] bytes)
                    result[header.Key] = Encoding.UTF8.GetString(bytes);
                else if (header.Value != null)
                    result[header.Key] = header.Value.ToString();
            }

            return result;
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
