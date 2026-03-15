using LagoVista.Core.Tests.MessageQueue.TestSupport;
using LagoVista.MessageQueue.Rabbit;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LagoVista.Core.Tests.MessageQueue
{
    [TestFixture]
    public class RabbitMqMessageQueueBuilderTests
    {
        [Test]
        public void AddService_With_RabbitMqConnectionSettings_Should_Add_Service_Registration()
        {
            var builder = new RabbitMqMessageQueueBuilder();
            var connectionSettings = new RabbitMqConnectionSettings
            {
                Name = "billing",
                HostName = "rabbit-host",
                UserName = "rabbit-user",
                Password = "rabbit-pass",
                VirtualHost = "/billing"
            };
            var topology = new TestMessageQueueTopology();

            builder.AddService("billing", connectionSettings, topology);

            Assert.That(builder.ServiceRegistrations, Has.Count.EqualTo(1));
            Assert.That(builder.ServiceRegistrations[0].ServiceName, Is.EqualTo("billing"));
            Assert.That(builder.ServiceRegistrations[0].ConnectionSettings, Is.SameAs(connectionSettings));
            Assert.That(builder.ServiceRegistrations[0].Topology, Is.SameAs(topology));
        }

        [Test]
        public void AddService_With_IConnectionSettings_Should_Map_To_RabbitMqConnectionSettings()
        {
            var builder = new RabbitMqMessageQueueBuilder();
            var settings = new TestConnectionSettings
            {
                Name = "billing",
                Uri = "rabbit-host",
                UserName = "rabbit-user",
                Password = "rabbit-pass",
                Port = "5678",
                IsSSL = true,
                TimeoutInSeconds = 45,
                Settings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["VirtualHost"] = "/billing"
                }
            };

            builder.AddService("billing", settings, new TestMessageQueueTopology());

            var registration = builder.ServiceRegistrations.Single();
            Assert.That(registration.ServiceName, Is.EqualTo("billing"));
            Assert.That(registration.ConnectionSettings.HostName, Is.EqualTo("rabbit-host"));
            Assert.That(registration.ConnectionSettings.UserName, Is.EqualTo("rabbit-user"));
            Assert.That(registration.ConnectionSettings.Password, Is.EqualTo("rabbit-pass"));
            Assert.That(registration.ConnectionSettings.Port, Is.EqualTo(5678));
            Assert.That(registration.ConnectionSettings.UseSsl, Is.True);
            Assert.That(registration.ConnectionSettings.TimeoutInSeconds, Is.EqualTo(45));
            Assert.That(registration.ConnectionSettings.VirtualHost, Is.EqualTo("/billing"));
        }

        [Test]
        public void AddService_With_Null_IConnectionSettings_Should_Throw()
        {
            var builder = new RabbitMqMessageQueueBuilder();

            Assert.Throws<ArgumentNullException>(() => builder.AddService("billing", (TestConnectionSettings)null, new TestMessageQueueTopology()));
        }

        [Test]
        public void AddMessageType_Should_Add_Type_Registration()
        {
            var builder = new RabbitMqMessageQueueBuilder();

            builder.AddMessageType<TestMessageA>("billing");

            Assert.That(builder.TypeRegistrations, Has.Count.EqualTo(1));
            Assert.That(builder.TypeRegistrations[0].MessageType, Is.EqualTo(typeof(TestMessageA)));
            Assert.That(builder.TypeRegistrations[0].ServiceName, Is.EqualTo("billing"));
        }
    }
}
