using LagoVista.MessageQueue.Rabbit;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace LagoVista.MessageQueue.RabbitMQ.IntegrationTests
{
    [TestFixture]
    public class RabbitMqPublisherSettingsTests
    {
        [Test]
        public void Read_When_Section_Is_Valid_Should_Bind_Settings()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["PlaidSyncPublisher:Name"] = "plaid-sync",
                    ["PlaidSyncPublisher:HostName"] = "rabbit-host",
                    ["PlaidSyncPublisher:UserName"] = "publisher-user",
                    ["PlaidSyncPublisher:Password"] = "publisher-pass",
                    ["PlaidSyncPublisher:VirtualHost"] = "/plaid",
                    ["PlaidSyncPublisher:Port"] = "5678",
                    ["PlaidSyncPublisher:UseSsl"] = "true",
                    ["PlaidSyncPublisher:ExchangeName"] = "plaid.exchange",
                    ["PlaidSyncPublisher:RouteKey"] = "plaid.sync.requested",
                    ["PlaidSyncPublisher:ContentType"] = "application/json",
                    ["PlaidSyncPublisher:Persistent"] = "true"
                })
                .Build();

            var settings = RabbitMqPublisherSettings.Read(configuration, "PlaidSyncPublisher");

            Assert.That(settings.Name, Is.EqualTo("plaid-sync"));
            Assert.That(settings.HostName, Is.EqualTo("rabbit-host"));
            Assert.That(settings.UserName, Is.EqualTo("publisher-user"));
            Assert.That(settings.Password, Is.EqualTo("publisher-pass"));
            Assert.That(settings.VirtualHost, Is.EqualTo("/plaid"));
            Assert.That(settings.Port, Is.EqualTo(5678));
            Assert.That(settings.UseSsl, Is.True);
            Assert.That(settings.ExchangeName, Is.EqualTo("plaid.exchange"));
            Assert.That(settings.RouteKey, Is.EqualTo("plaid.sync.requested"));
            Assert.That(settings.ContentType, Is.EqualTo("application/json"));
            Assert.That(settings.Persistent, Is.True);
        }

        [Test]
        public void Read_When_Section_Is_Missing_Should_Throw()
        {
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>()).Build();

            Assert.That(() => RabbitMqPublisherSettings.Read(configuration, "MissingPublisher"), Throws.Exception);
        }

        [Test]
        public void Read_When_Required_Field_Is_Missing_Should_Throw()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["PlaidSyncPublisher:HostName"] = "rabbit-host",
                    ["PlaidSyncPublisher:UserName"] = "publisher-user",
                    ["PlaidSyncPublisher:Password"] = "publisher-pass",
                    ["PlaidSyncPublisher:VirtualHost"] = "/plaid",
                    ["PlaidSyncPublisher:Port"] = "5672",
                    ["PlaidSyncPublisher:ExchangeName"] = "plaid.exchange"
                })
                .Build();

            var ex = Assert.Throws<InvalidOperationException>(() => RabbitMqPublisherSettings.Read(configuration, "PlaidSyncPublisher"));

            Assert.That(ex.Message, Does.Contain("RouteKey"));
        }
    }
}
