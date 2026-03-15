using LagoVista.Core.Tests.MessageQueue.TestSupport;
using LagoVista.MessageQueue.Rabbit;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace LagoVista.Core.Tests.MessageQueue
{
    [TestFixture]
    public class RabbitMqConnectionSettingsTests
    {
        [Test]
        public void From_When_Settings_Are_Valid_Should_Map_Values()
        {
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
                    ["VirtualHost"] = "/billing",
                    ["AutomaticRecoveryEnabled"] = "false",
                    ["TopologyRecoveryEnabled"] = "false"
                }
            };

            var result = RabbitMqConnectionSettings.From(settings);

            Assert.That(result.Name, Is.EqualTo("billing"));
            Assert.That(result.HostName, Is.EqualTo("rabbit-host"));
            Assert.That(result.UserName, Is.EqualTo("rabbit-user"));
            Assert.That(result.Password, Is.EqualTo("rabbit-pass"));
            Assert.That(result.Port, Is.EqualTo(5678));
            Assert.That(result.UseSsl, Is.True);
            Assert.That(result.TimeoutInSeconds, Is.EqualTo(45));
            Assert.That(result.VirtualHost, Is.EqualTo("/billing"));
            Assert.That(result.AutomaticRecoveryEnabled, Is.False);
            Assert.That(result.TopologyRecoveryEnabled, Is.False);
        }

        [Test]
        public void From_When_Port_Is_Missing_Should_Use_Default_Port()
        {
            var settings = new TestConnectionSettings
            {
                Uri = "rabbit-host",
                UserName = "rabbit-user",
                Password = "rabbit-pass"
            };

            var result = RabbitMqConnectionSettings.From(settings);

            Assert.That(result.Port, Is.EqualTo(5672));
        }

        [Test]
        public void From_When_Port_Is_Invalid_Should_Use_Default_Port()
        {
            var settings = new TestConnectionSettings
            {
                Uri = "rabbit-host",
                UserName = "rabbit-user",
                Password = "rabbit-pass",
                Port = "not-a-port"
            };

            var result = RabbitMqConnectionSettings.From(settings);

            Assert.That(result.Port, Is.EqualTo(5672));
        }

        [Test]
        public void From_When_Timeout_Is_Not_Positive_Should_Use_Default_Timeout()
        {
            var settings = new TestConnectionSettings
            {
                Uri = "rabbit-host",
                UserName = "rabbit-user",
                Password = "rabbit-pass",
                TimeoutInSeconds = 0
            };

            var result = RabbitMqConnectionSettings.From(settings);

            Assert.That(result.TimeoutInSeconds, Is.EqualTo(30));
        }

        [Test]
        public void From_When_Settings_Are_Null_Should_Throw()
        {
            Assert.Throws<ArgumentNullException>(() => RabbitMqConnectionSettings.From(null));
        }
    }
}
