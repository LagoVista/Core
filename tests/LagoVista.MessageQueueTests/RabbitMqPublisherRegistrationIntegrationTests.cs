using LagoVista.Core.MessageQueue;
using LagoVista.Core.PlatformSupport;
using LagoVista.MessageQueue.Rabbit;
using LagoVista.MessageQueue.RabbitMQ.IntegrationTests.TestSupport;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Testcontainers.RabbitMq;

namespace LagoVista.MessageQueue.RabbitMQ.IntegrationTests
{
    [TestFixture]
    public class RabbitMqPublisherRegistrationIntegrationTests
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
        public async Task AddRabbitMqPublisher_When_Section_Is_Valid_Should_Publish_Message_To_RabbitMq()
        {
            var sectionName = "IntegrationPublisher";
            var settings = BuildPublisherSettings(sectionName);
            var configuration = BuildConfiguration(sectionName, settings);
            var topology = BuildTopology(settings);

            await EnsureQueueAndBindingAsync(settings, topology).ConfigureAwait(false);

            var services = new ServiceCollection();
            services.AddSingleton<ILogger>(_logger);
            services.AddRabbitMqPublisher<IntegrationMessage, IIntegrationPublisher, IntegrationPublisher>(configuration, sectionName);

            using var serviceProvider = services.BuildServiceProvider();
            var publisher = serviceProvider.GetRequiredService<IMessageQueuePublisher>();

            var message = new IntegrationMessage
            {
                Id = Guid.NewGuid().ToString("N"),
                Name = "Published Through Registration Extension"
            };

            await publisher.PublishAsync(message).ConfigureAwait(false);

            var body = await ReadSingleMessageAsync(settings, topology).ConfigureAwait(false);

            Assert.That(body, Does.Contain(message.Id));
            Assert.That(body, Does.Contain(message.Name));
        }

        private RabbitMqPublisherSettings BuildPublisherSettings(string sectionName)
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri(_container.GetConnectionString())
            };

            return new RabbitMqPublisherSettings
            {
                Name = sectionName,
                HostName = factory.HostName,
                UserName = factory.UserName,
                Password = factory.Password,
                VirtualHost = factory.VirtualHost,
                Port = factory.Port,
                UseSsl = factory.Ssl.Enabled,
                ExchangeName = "integration.publisher.exchange",
                RouteKey = "integration.publisher.route"
            };
        }

        private static IConfiguration BuildConfiguration(string sectionName, RabbitMqPublisherSettings settings)
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
                    [$"{sectionName}:RouteKey"] = settings.RouteKey
                })
                .Build();
        }

        private static SingleMessageTopology<IntegrationMessage> BuildTopology(RabbitMqPublisherSettings settings)
        {
            var publishRoute = new MessageQueuePublishRoute
            {
                DestinationName = settings.ExchangeName,
                RouteKey = settings.RouteKey,
                ContentType = settings.ContentType,
                Persistent = settings.Persistent
            };

            var subscriptionRoute = new MessageQueueSubscriptionRoute
            {
                DestinationName = settings.ExchangeName,
                QueueName = "integration.publisher.queue",
                RouteKey = settings.RouteKey,
                Durable = true,
                Exclusive = false,
                AutoDelete = false,
                PrefetchCount = 1
            };

            return new SingleMessageTopology<IntegrationMessage>(publishRoute, subscriptionRoute);
        }

        private static async Task EnsureQueueAndBindingAsync(RabbitMqPublisherSettings settings, IMessageQueueTopology topology)
        {
            var route = topology.GetSubscriptionRoute(typeof(IntegrationMessage));

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

            await channel.ExchangeDeclareAsync(route.DestinationName, ExchangeType.Direct, route.Durable, route.AutoDelete).ConfigureAwait(false);
            await channel.QueueDeclareAsync(route.QueueName, route.Durable, route.Exclusive, route.AutoDelete).ConfigureAwait(false);
            await channel.QueueBindAsync(route.QueueName, route.DestinationName, route.RouteKey).ConfigureAwait(false);
        }

        private static async Task<string> ReadSingleMessageAsync(RabbitMqPublisherSettings settings, IMessageQueueTopology topology)
        {
            var route = topology.GetSubscriptionRoute(typeof(IntegrationMessage));

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

            var getResult = await channel.BasicGetAsync(route.QueueName, true).ConfigureAwait(false);
            Assert.That(getResult, Is.Not.Null, "Expected a message in the queue, but none was found.");

            return Encoding.UTF8.GetString(getResult.Body.ToArray());
        }

        private interface IIntegrationPublisher
        {
        }

        private sealed class IntegrationPublisher : IIntegrationPublisher
        {
        }
    }
}
