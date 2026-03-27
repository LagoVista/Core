using LagoVista.Core.MessageQueue;
using LagoVista.Core.PlatformSupport;
using LagoVista.MessageQueue.Rabbit;
using LagoVista.MessageQueue.RabbitMQ.IntegrationTests.TestSupport;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using NUnit.Framework;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Testcontainers.RabbitMq;

namespace LagoVista.MessageQueue.RabbitMQ.IntegrationTests
{
    [TestFixture]
    public class RabbitMqSubscriberRegistrationIntegrationTests
    {
        private RabbitMqContainer _container;
        private TestAdminLogger _logger;

        [SetUp]
        public async Task SetUp()
        {
            _logger = new TestAdminLogger();

            _container = new RabbitMqBuilder("rabbitmq:4-management")
                .Build();

            await _container.StartAsync().ConfigureAwait(false);
        }

        [TearDown]
        public async Task TearDown()
        {
            if (_container != null)
                await _container.DisposeAsync().ConfigureAwait(false);
        }

        [Test]
        public async Task AddRabbitMqSubscriber_When_Section_Is_Valid_Should_Consume_Message_From_RabbitMq()
        {
            var sectionName = "IntegrationSubscriber";
            var settings = BuildSubscriberSettings(sectionName);
            var configuration = BuildConfiguration(sectionName, settings);
            var state = new IntegrationSubscriberState();

            var services = new ServiceCollection();
            services.AddSingleton<ILogger>(_logger);
            services.AddSingleton(state);
            services.AddRabbitMqSubscriber<IntegrationMessage, IntegrationMessageHandler>(configuration, sectionName);

            using var serviceProvider = services.BuildServiceProvider();
            var hostedService = serviceProvider.GetServices<IHostedService>().Single();

            await hostedService.StartAsync(CancellationToken.None).ConfigureAwait(false);

            try
            {
                var message = new IntegrationMessage
                {
                    Id = Guid.NewGuid().ToString("N"),
                    Name = "Consumed Through Subscriber Extension"
                };

                await PublishAsync(settings, message).ConfigureAwait(false);

                var received = await state.WaitForMessageAsync().ConfigureAwait(false);

                Assert.That(received, Is.Not.Null);
                Assert.That(received.Id, Is.EqualTo(message.Id));
                Assert.That(received.Name, Is.EqualTo(message.Name));
            }
            finally
            {
                await hostedService.StopAsync(CancellationToken.None).ConfigureAwait(false);
            }
        }

        private RabbitMqSubscriberSettings BuildSubscriberSettings(string sectionName)
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri(_container.GetConnectionString())
            };

            return new RabbitMqSubscriberSettings
            {
                Name = sectionName,
                HostName = factory.HostName,
                UserName = factory.UserName,
                Password = factory.Password,
                VirtualHost = factory.VirtualHost,
                Port = factory.Port,
                UseSsl = factory.Ssl.Enabled,
                ExchangeName = "integration.subscriber.exchange",
                QueueName = "integration.subscriber.queue",
                RouteKey = "integration.subscriber.route",
                Durable = true,
                Exclusive = false,
                AutoDelete = false,
                PrefetchCount = 1
            };
        }

        private static IConfiguration BuildConfiguration(string sectionName, RabbitMqSubscriberSettings settings)
        {
            return new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    [$"{sectionName}:Name"] = settings.Name,
                    [$"{sectionName}:HostName"] = settings.HostName,
                    [$"{sectionName}:UserName"] = settings.UserName,
                    [$"{sectionName}:Password"] = settings.Password,
                    [$"{sectionName}:VirtualHost"] = settings.VirtualHost,
                    [$"{sectionName}:Port"] = settings.Port.ToString(),
                    [$"{sectionName}:UseSsl"] = settings.UseSsl.ToString(),
                    [$"{sectionName}:ExchangeName"] = settings.ExchangeName,
                    [$"{sectionName}:QueueName"] = settings.QueueName,
                    [$"{sectionName}:RouteKey"] = settings.RouteKey,
                    [$"{sectionName}:Durable"] = settings.Durable.ToString(),
                    [$"{sectionName}:Exclusive"] = settings.Exclusive.ToString(),
                    [$"{sectionName}:AutoDelete"] = settings.AutoDelete.ToString(),
                    [$"{sectionName}:PrefetchCount"] = settings.PrefetchCount.ToString()
                })
                .Build();
        }

        private static async Task PublishAsync(RabbitMqSubscriberSettings settings, IntegrationMessage message)
        {
            var factory = new ConnectionFactory
            {
                HostName = settings.HostName,
                UserName = settings.UserName,
                Password = settings.Password,
                VirtualHost = settings.VirtualHost,
                Port = settings.Port
            };

            await using var connection = await factory.CreateConnectionAsync().ConfigureAwait(false);
            await using var channel = await connection.CreateChannelAsync().ConfigureAwait(false);

            await channel.ExchangeDeclareAsync(settings.ExchangeName, ExchangeType.Direct, settings.Durable, settings.AutoDelete).ConfigureAwait(false);
            await channel.QueueDeclareAsync(settings.QueueName, settings.Durable, settings.Exclusive, settings.AutoDelete).ConfigureAwait(false);
            await channel.QueueBindAsync(settings.QueueName, settings.ExchangeName, settings.RouteKey).ConfigureAwait(false);

            var properties = new BasicProperties
            {
                ContentType = "application/json",
                DeliveryMode = DeliveryModes.Persistent
            };

            var body = System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
            await channel.BasicPublishAsync(settings.ExchangeName, settings.RouteKey, false, properties, body, CancellationToken.None).ConfigureAwait(false);
        }

        private sealed class IntegrationSubscriberState
        {
            private readonly TaskCompletionSource<IntegrationMessage> _tcs = new TaskCompletionSource<IntegrationMessage>(TaskCreationOptions.RunContinuationsAsynchronously);

            public void SetMessage(IntegrationMessage message)
            {
                _tcs.TrySetResult(message);
            }

            public async Task<IntegrationMessage> WaitForMessageAsync()
            {
                var completed = await Task.WhenAny(_tcs.Task, Task.Delay(TimeSpan.FromSeconds(10))).ConfigureAwait(false);
                if (completed != _tcs.Task) Assert.Fail("Timed out waiting for subscriber to receive the message.");
                return await _tcs.Task.ConfigureAwait(false);
            }
        }

        private sealed class IntegrationMessageHandler : IMessageQueueHandler<IntegrationMessage>
        {
            private readonly IntegrationSubscriberState _state;

            public IntegrationMessageHandler(IntegrationSubscriberState state)
            {
                _state = state;
            }

            public Task HandleAsync(MessageQueueContext<IntegrationMessage> context, CancellationToken cancellationToken = default)
            {
                _state.SetMessage(context.Payload);
                return Task.CompletedTask;
            }
        }
    }
}
