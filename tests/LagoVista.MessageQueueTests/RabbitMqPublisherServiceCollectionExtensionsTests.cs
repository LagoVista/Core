using LagoVista.Core.MessageQueue;
using LagoVista.MessageQueue.Rabbit;
using LagoVista.MessageQueue.RabbitMQ.IntegrationTests.TestSupport;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace LagoVista.MessageQueue.RabbitMQ.IntegrationTests
{
    [TestFixture]
    public class RabbitMqPublisherServiceCollectionExtensionsTests
    {
        [Test]
        public void AddRabbitMqPublisher_When_Section_Is_Valid_Should_Register_Publisher_Infrastructure()
        {
            var services = new ServiceCollection();
            services.AddSingleton<LagoVista.Core.PlatformSupport.ILogger>(new TestAdminLogger());

            var configuration = BuildPublisherConfiguration("PlaidSyncPublisher", "rabbit-host", "/plaid", "plaid.exchange", "plaid.sync.requested");

            services.AddRabbitMqPublisher<IntegrationMessage, IRegisteredIntegrationPublisher, RegisteredIntegrationPublisher>(configuration, "PlaidSyncPublisher");

            using var serviceProvider = services.BuildServiceProvider();

            var publisher = serviceProvider.GetRequiredService<IMessageQueuePublisher>();
            var registration = serviceProvider.GetRequiredService<IRegisteredIntegrationPublisher>();
            var registry = serviceProvider.GetRequiredService<IMessageQueueTypeRegistry>();

            Assert.That(publisher, Is.Not.Null);
            Assert.That(registration, Is.Not.Null);
            Assert.That(registry.GetServiceNameFor(typeof(IntegrationMessage)), Is.EqualTo("PlaidSyncPublisher"));
        }

        [Test]
        public void AddRabbitMqPublisher_When_Section_Is_Missing_Should_Throw()
        {
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>()).Build();

            Assert.That(() => services.AddRabbitMqPublisher<IntegrationMessage, IRegisteredIntegrationPublisher, RegisteredIntegrationPublisher>(configuration, "MissingPublisher"), Throws.Exception);
        }

        [Test]
        public void AddRabbitMqPublisher_When_MessageType_Is_Registered_Twice_Should_Throw_On_Resolution()
        {
            var services = new ServiceCollection();
            services.AddSingleton<LagoVista.Core.PlatformSupport.ILogger>(new TestAdminLogger());

            var configuration = BuildPublisherConfiguration("PublisherOne", "rabbit-host", "/plaid", "plaid.exchange", "plaid.sync.requested");

            services.AddRabbitMqPublisher<IntegrationMessage, IRegisteredIntegrationPublisher, RegisteredIntegrationPublisher>(configuration, "PublisherOne");
            services.AddRabbitMqPublisher<IntegrationMessage, IRegisteredIntegrationPublisher, RegisteredIntegrationPublisher>(configuration, "PublisherOne");

            using var serviceProvider = services.BuildServiceProvider();

            Assert.That(() => serviceProvider.GetRequiredService<IMessageQueueTypeRegistry>(), Throws.InvalidOperationException);
        }

        private static IConfiguration BuildPublisherConfiguration(string sectionName, string hostName, string virtualHost, string exchangeName, string routeKey)
        {
            return new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    [$"{sectionName}:Name"] = sectionName,
                    [$"{sectionName}:HostName"] = hostName,
                    [$"{sectionName}:UserName"] = "publisher-user",
                    [$"{sectionName}:Password"] = "publisher-pass",
                    [$"{sectionName}:VirtualHost"] = virtualHost,
                    [$"{sectionName}:Port"] = "5672",
                    [$"{sectionName}:UseSsl"] = "false",
                    [$"{sectionName}:ExchangeName"] = exchangeName,
                    [$"{sectionName}:RouteKey"] = routeKey
                })
                .Build();
        }

        private interface IRegisteredIntegrationPublisher
        {
        }

        private class RegisteredIntegrationPublisher : IRegisteredIntegrationPublisher
        {
        }
    }
}
