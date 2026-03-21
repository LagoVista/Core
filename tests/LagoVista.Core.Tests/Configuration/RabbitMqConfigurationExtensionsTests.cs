using LagoVista.Core.Models;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.Tests.Configuration
{
    [TestFixture]
    public class RabbitMqConfigurationExtensionsTests
    {
        [Test]
        public void GetRabbitMQSettings_Should_Throw_When_Configuration_Is_Null()
        {
            IConfiguration configuration = null;

            Assert.That(() => configuration.GetRabbitMQSettings("RabbitPlaidSub"),
                Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void GetRabbitMQSettings_Should_Throw_When_SectionKey_Is_Null()
        {
            var configuration = BuildConfiguration();

            Assert.That(() => configuration.GetRabbitMQSettings(null),
                Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void GetRabbitMQSettings_Should_Throw_When_Section_Is_Missing()
        {
            var configuration = BuildConfiguration();

            Assert.That(() => configuration.GetRabbitMQSettings("RabbitPlaidSub"),
                Throws.TypeOf<InvalidOperationException>()
                    .With.Message.EqualTo("Section 'RabbitPlaidSub' not found in configuration."));
        }

        [Test]
        public void GetRabbitMQSettings_Should_Throw_When_Uri_Is_Missing()
        {
            var configuration = BuildConfiguration(
                ("RabbitPlaidSub:UserId", "plaid-user"),
                ("RabbitPlaidSub:Password", "super-secret"),
                ("RabbitPlaidSub:Queue", "plaid-ingest"));

            Assert.That(() => configuration.GetRabbitMQSettings("RabbitPlaidSub"),
                Throws.TypeOf<InvalidOperationException>()
                    .With.Message.EqualTo("Missing required configuration value 'RabbitPlaidSub:Uri'."));
        }

        [Test]
        public void GetRabbitMQSettings_Should_Throw_When_UserId_Is_Missing()
        {
            var configuration = BuildConfiguration(
                ("RabbitPlaidSub:Uri", "amqp://rabbitmq"),
                ("RabbitPlaidSub:Password", "super-secret"),
                ("RabbitPlaidSub:Queue", "plaid-ingest"));

            Assert.That(() => configuration.GetRabbitMQSettings("RabbitPlaidSub"),
                Throws.TypeOf<InvalidOperationException>()
                    .With.Message.EqualTo("Missing required configuration value 'RabbitPlaidSub:UserId'."));
        }

        [Test]
        public void GetRabbitMQSettings_Should_Throw_When_Password_Is_Missing()
        {
            var configuration = BuildConfiguration(
                ("RabbitPlaidSub:Uri", "amqp://rabbitmq"),
                ("RabbitPlaidSub:UserId", "plaid-user"),
                ("RabbitPlaidSub:Queue", "plaid-ingest"));

            Assert.That(() => configuration.GetRabbitMQSettings("RabbitPlaidSub"),
                Throws.TypeOf<InvalidOperationException>()
                    .With.Message.EqualTo("Missing required configuration value 'RabbitPlaidSub:Password'."));
        }

        [Test]
        public void GetRabbitMQSettings_Should_Throw_When_Queue_Is_Missing()
        {
            var configuration = BuildConfiguration(
                ("RabbitPlaidSub:Uri", "amqp://rabbitmq"),
                ("RabbitPlaidSub:UserId", "plaid-user"),
                ("RabbitPlaidSub:Password", "super-secret"));

            Assert.That(() => configuration.GetRabbitMQSettings("RabbitPlaidSub"),
                Throws.TypeOf<InvalidOperationException>()
                    .With.Message.EqualTo("Missing required configuration value 'RabbitPlaidSub:Queue'."));
        }

        [Test]
        public void GetRabbitMQSettings_Should_Return_Settings_When_All_Values_Are_Present()
        {
            var configuration = BuildConfiguration(
                ("RabbitPlaidSub:Uri", "amqp://rabbitmq"),
                ("RabbitPlaidSub:UserId", "plaid-user"),
                ("RabbitPlaidSub:Password", "super-secret"),
                ("RabbitPlaidSub:Queue", "plaid-ingest"));

            var result = configuration.GetRabbitMQSettings("RabbitPlaidSub");

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.TypeOf<ConnectionSettings>());
            Assert.That(result.Uri, Is.EqualTo("amqp://rabbitmq"));
            Assert.That(result.UserName, Is.EqualTo("plaid-user"));
            Assert.That(result.Password, Is.EqualTo("super-secret"));
            Assert.That(result.ResourceName, Is.EqualTo("plaid-ingest"));
        }

        [Test]
        public void GetRabbitMQSettings_Should_Throw_When_Password_Is_Whitespace()
        {
            var configuration = BuildConfiguration(
                ("RabbitPlaidSub:Uri", "amqp://rabbitmq"),
                ("RabbitPlaidSub:UserId", "plaid-user"),
                ("RabbitPlaidSub:Password", "   "),
                ("RabbitPlaidSub:Queue", "plaid-ingest"));

            Assert.That(() => configuration.GetRabbitMQSettings("RabbitPlaidSub"),
                Throws.TypeOf<InvalidOperationException>()
                    .With.Message.EqualTo("Missing required configuration value 'RabbitPlaidSub:Password'."));
        }

        private static IConfigurationRoot BuildConfiguration(params (string Key, string Value)[] values)
        {
            var data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var item in values)
                data[item.Key] = item.Value;

            return new ConfigurationBuilder()
                .AddInMemoryCollection(data)
                .Build();
        }
    }
}