using LagoVista.Core.MessageQueue;
using LagoVista.MessageQueue.Rabbit;
using LagoVista.MessageQueue.RabbitMQ.IntegrationTests.TestSupport;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using RabbitMQ.Client;
using System;
using System.Text;
using System.Threading.Tasks;
using Testcontainers.RabbitMq;

namespace LagoVista.MessageQueue.RabbitMQ.IntegrationTests
{
    [TestFixture]
    public class RabbitMqMessageQueuePublisherIntegrationTests
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
        public async Task PublishAsync_When_MessageTypeIsRegistered_Should_Publish_Message_To_RabbitMq()
        {
            var connectionSettings = BuildConnectionSettings();
            var topology = new IntegrationMessageTopology();
            var publisher = BuildPublisher(connectionSettings, topology);

            await EnsureQueueAndBindingAsync(connectionSettings, topology).ConfigureAwait(false);

            var message = new IntegrationMessage
            {
                Id = Guid.NewGuid().ToString("N"),
                Name = "Test Message"
            };

            await publisher.PublishAsync(message).ConfigureAwait(false);

            var body = await ReadSingleMessageAsync(connectionSettings, topology).ConfigureAwait(false);

            Assert.That(body, Does.Contain(message.Id));
            Assert.That(body, Does.Contain(message.Name));
        }

        [Test]
        public void AddRabbitMqMessageQueue_When_MessageTypeRegisteredTwice_Should_Throw()
        {
            var connectionSettings = BuildConnectionSettings();

            var ex = Assert.Throws<InvalidOperationException>(() =>
            {
                var services = new ServiceCollection();

                services.AddRabbitMqMessageQueue(builder =>
                {
                    builder
                        .AddService("billing", connectionSettings, new IntegrationMessageTopology())
                        .AddMessageType<IntegrationMessage>("billing")
                        .AddMessageType<IntegrationMessage>("billing");
                });
            });

            Assert.That(ex, Is.Not.Null);
        }

        [Test]
        public void PublishAsync_When_MessageTypeIsNotRegistered_Should_Throw()
        {
            var connectionSettings = BuildConnectionSettings();

            var registration = new RabbitMqServiceRegistration
            {
                ServiceName = "billing",
                ConnectionSettings = RabbitMqConnectionSettings.From(connectionSettings),
                Topology = new IntegrationMessageTopology()
            };

            var typeRegistry = new RabbitMqMessageQueueTypeRegistry(Array.Empty<RabbitMqTypeRegistration>(), new[] { "billing" });
            var publisher = new RabbitMqMessageQueuePublisher(typeRegistry, new[]
            {
                new RabbitMqMessageQueueServiceClient(registration, _logger)
            });

            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () => await publisher.PublishAsync(new IntegrationMessage()).ConfigureAwait(false));

            Assert.That(ex, Is.Not.Null);
            Assert.That(ex.Message, Does.Contain(typeof(IntegrationMessage).FullName));
        }

        private TestConnectionSettings BuildConnectionSettings()
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri(_container.GetConnectionString())
            };

            var settings = new TestConnectionSettings
            {
                Name = "integration-rabbit",
                Uri = factory.HostName,
                UserName = factory.UserName,
                Password = factory.Password,
                Port = factory.Port.ToString(),
                IsSSL = factory.Ssl.Enabled
            };

            settings.Settings["VirtualHost"] = factory.VirtualHost;

            return settings;
        }

        private RabbitMqMessageQueuePublisher BuildPublisher(TestConnectionSettings connectionSettings, IMessageQueueTopology topology)
        {
            var registration = new RabbitMqServiceRegistration
            {
                ServiceName = "billing",
                ConnectionSettings = RabbitMqConnectionSettings.From(connectionSettings),
                Topology = topology
            };

            var typeRegistry = new RabbitMqMessageQueueTypeRegistry(new[]
            {
                new RabbitMqTypeRegistration
                {
                    MessageType = typeof(IntegrationMessage),
                    ServiceName = "billing"
                }
            }, new[] { "billing" });

            return new RabbitMqMessageQueuePublisher(typeRegistry, new[]
            {
                new RabbitMqMessageQueueServiceClient(registration, _logger)
            });
        }

        private async Task EnsureQueueAndBindingAsync(TestConnectionSettings connectionSettings, IMessageQueueTopology topology)
        {
            var route = topology.GetSubscriptionRoute(typeof(IntegrationMessage));

            var factory = new ConnectionFactory
            {
                HostName = connectionSettings.Uri,
                UserName = connectionSettings.UserName,
                Password = connectionSettings.Password,
                VirtualHost = connectionSettings.Settings["VirtualHost"],
                Port = Int32.Parse(connectionSettings.Port)
            };

            await using var connection = await factory.CreateConnectionAsync().ConfigureAwait(false);
            await using var channel = await connection.CreateChannelAsync().ConfigureAwait(false);

            await channel.ExchangeDeclareAsync(route.DestinationName, ExchangeType.Direct, route.Durable, route.AutoDelete).ConfigureAwait(false);
            await channel.QueueDeclareAsync(route.QueueName, route.Durable, route.Exclusive, route.AutoDelete).ConfigureAwait(false);
            await channel.QueueBindAsync(route.QueueName, route.DestinationName, route.RouteKey).ConfigureAwait(false);
        }

        private async Task<string> ReadSingleMessageAsync(TestConnectionSettings connectionSettings, IMessageQueueTopology topology)
        {
            var route = topology.GetSubscriptionRoute(typeof(IntegrationMessage));

            var factory = new ConnectionFactory
            {
                HostName = connectionSettings.Uri,
                UserName = connectionSettings.UserName,
                Password = connectionSettings.Password,
                VirtualHost = connectionSettings.Settings["VirtualHost"],
                Port = Int32.Parse(connectionSettings.Port)
            };

            await using var connection = await factory.CreateConnectionAsync().ConfigureAwait(false);
            await using var channel = await connection.CreateChannelAsync().ConfigureAwait(false);

            var getResult = await channel.BasicGetAsync(route.QueueName, true).ConfigureAwait(false);
            Assert.That(getResult, Is.Not.Null, "Expected a message in the queue, but none was found.");

            return Encoding.UTF8.GetString(getResult.Body.ToArray());
        }
    }
}