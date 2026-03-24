using LagoVista.MessageQueue.Rabbit;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace LagoVista.MessageQueue.RabbitMQ.IntegrationTests
{
    [TestFixture]
    public class RabbitMqSubscriberSettingsTests
    {
        [Test]
        public void Read_When_Section_Is_Valid_Should_Bind_Settings()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["PlaidSyncSubscriber:Name"] = "plaid-sync",
                    ["PlaidSyncSubscriber:HostName"] = "rabbit-host",
                    ["PlaidSyncSubscriber:UserName"] = "subscriber-user",
                    ["PlaidSyncSubscriber:Password"] = "subscriber-pass",
                    ["PlaidSyncSubscriber:VirtualHost"] = "/plaid",
                    ["PlaidSyncSubscriber:Port"] = "5678",
                    ["PlaidSyncSubscriber:UseSsl"] = "true",
                    ["PlaidSyncSubscriber:ExchangeName"] = "plaid.exchange",
                    ["PlaidSyncSubscriber:QueueName"] = "plaid.queue",
                    ["PlaidSyncSubscriber:RouteKey"] = "plaid.sync.requested",
                    ["PlaidSyncSubscriber:Durable"] = "true",
                    ["PlaidSyncSubscriber:Exclusive"] = "false",
                    ["PlaidSyncSubscriber:AutoDelete"] = "false",
                    ["PlaidSyncSubscriber:PrefetchCount"] = "5"
                })
                .Build();

            var settings = RabbitMqSubscriberSettings.Read(configuration, "PlaidSyncSubscriber");

            Assert.That(settings.Name, Is.EqualTo("plaid-sync"));
            Assert.That(settings.HostName, Is.EqualTo("rabbit-host"));
            Assert.That(settings.UserName, Is.EqualTo("subscriber-user"));
            Assert.That(settings.Password, Is.EqualTo("subscriber-pass"));
            Assert.That(settings.VirtualHost, Is.EqualTo("/plaid"));
            Assert.That(settings.Port, Is.EqualTo(5678));
            Assert.That(settings.UseSsl, Is.True);
            Assert.That(settings.ExchangeName, Is.EqualTo("plaid.exchange"));
            Assert.That(settings.QueueName, Is.EqualTo("plaid.queue"));
            Assert.That(settings.RouteKey, Is.EqualTo("plaid.sync.requested"));
            Assert.That(settings.Durable, Is.True);
            Assert.That(settings.Exclusive, Is.False);
            Assert.That(settings.AutoDelete, Is.False);
            Assert.That(settings.PrefetchCount, Is.EqualTo(5));
        }

        [Test]
        public void Read_When_QueueName_Is_Missing_Should_Throw()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["PlaidSyncSubscriber:HostName"] = "rabbit-host",
                    ["PlaidSyncSubscriber:UserName"] = "subscriber-user",
                    ["PlaidSyncSubscriber:Password"] = "subscriber-pass",
                    ["PlaidSyncSubscriber:VirtualHost"] = "/plaid",
                    ["PlaidSyncSubscriber:Port"] = "5672",
                    ["PlaidSyncSubscriber:ExchangeName"] = "plaid.exchange",
                    ["PlaidSyncSubscriber:RouteKey"] = "plaid.sync.requested"
                })
                .Build();

            var ex = Assert.Throws<InvalidOperationException>(() => RabbitMqSubscriberSettings.Read(configuration, "PlaidSyncSubscriber"));

            Assert.That(ex.Message, Does.Contain("QueueName"));
        }
    }
}
