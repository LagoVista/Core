using Castle.Core.Logging;
using LagoVista.Core.MessageQueue;
using LagoVista.Core.PlatformSupport;
using LagoVista.Core.Tests.MessageQueue.TestSupport;
using LagoVista.MessageQueue.Rabbit;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace LagoVista.Core.Tests.MessageQueue
{
    [TestFixture]
    public class RabbitMqServiceCollectionExtensionsTests
    {
        [Test]
        public void AddRabbitMqMessageQueue_When_No_Services_Are_Registered_Should_Throw()
        {
            var services = CreateServices();

            var ex = Assert.Throws<InvalidOperationException>(() => services.AddRabbitMqMessageQueue(builder =>
            {
                builder.AddMessageType<TestMessageA>("billing");
            }));

            Assert.That(ex, Is.Not.Null);
            Assert.That(ex.Message, Does.Contain("At least one RabbitMQ service"));
        }

        [Test]
        public void AddRabbitMqMessageQueue_When_No_Message_Types_Are_Registered_Should_Throw()
        {
            var services = CreateServices();

            var ex = Assert.Throws<InvalidOperationException>(() => services.AddRabbitMqMessageQueue(builder =>
            {
                builder.AddService("billing", CreateConnectionSettings(), new TestMessageQueueTopology());
            }));

            Assert.That(ex, Is.Not.Null);
            Assert.That(ex.Message, Does.Contain("At least one RabbitMQ message type"));
        }

        [Test]
        public void AddRabbitMqMessageQueue_When_Service_Name_Is_Duplicated_Should_Throw()
        {
            var services = CreateServices();

            var ex = Assert.Throws<InvalidOperationException>(() => services.AddRabbitMqMessageQueue(builder =>
            {
                builder
                    .AddService("billing", CreateConnectionSettings(), new TestMessageQueueTopology())
                    .AddService("billing", CreateConnectionSettings(), new TestMessageQueueTopology())
                    .AddMessageType<TestMessageA>("billing");
            }));

            Assert.That(ex, Is.Not.Null);
            Assert.That(ex.Message, Does.Contain("Duplicate RabbitMQ service registrations"));
        }

        [Test]
        public void AddRabbitMqMessageQueue_When_Message_Type_Is_Duplicated_Should_Throw()
        {
            var services = CreateServices();

            var ex = Assert.Throws<InvalidOperationException>(() => services.AddRabbitMqMessageQueue(builder =>
            {
                builder
                    .AddService("billing", CreateConnectionSettings(), new TestMessageQueueTopology())
                    .AddMessageType<TestMessageA>("billing")
                    .AddMessageType<TestMessageA>("billing");
            }));

            Assert.That(ex, Is.Not.Null);
            Assert.That(ex.Message, Does.Contain("Duplicate RabbitMQ message type registrations"));
        }

        [Test]
        public void AddRabbitMqMessageQueue_When_Message_Type_References_Unknown_Service_Should_Throw()
        {
            var services = CreateServices();

            var ex = Assert.Throws<InvalidOperationException>(() => services.AddRabbitMqMessageQueue(builder =>
            {
                builder
                    .AddService("billing", CreateConnectionSettings(), new TestMessageQueueTopology())
                    .AddMessageType<TestMessageA>("plaid");
            }));

            Assert.That(ex, Is.Not.Null);
            Assert.That(ex.Message, Does.Contain("unknown services"));
        }

        [Test]
        public void AddRabbitMqMessageQueue_When_Registration_Is_Valid_Should_Register_Publisher_And_Type_Registry()
        {
            var services = CreateServices();

            services.AddRabbitMqMessageQueue(builder =>
            {
                builder
                    .AddService("billing", CreateConnectionSettings(), new TestMessageQueueTopology())
                    .AddMessageType<TestMessageA>("billing");
            });

            using var serviceProvider = services.BuildServiceProvider();

            var typeRegistry = serviceProvider.GetService<IMessageQueueTypeRegistry>();
            var publisher = serviceProvider.GetService<IMessageQueuePublisher>();

            Assert.That(typeRegistry, Is.Not.Null);
            Assert.That(publisher, Is.Not.Null);
        }

        private static IServiceCollection CreateServices()
        {
            var services = new ServiceCollection();
            services.AddSingleton<PlatformSupport.ILogger, NoOpAdminLogger>();
            return services;
        }

        private static RabbitMqConnectionSettings CreateConnectionSettings()
        {
            return new RabbitMqConnectionSettings
            {
                Name = "billing",
                HostName = "rabbit-host",
                UserName = "rabbit-user",
                Password = "rabbit-pass",
                VirtualHost = "/billing",
                Settings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            };
        }

        private sealed class NoOpAdminLogger : PlatformSupport.ILogger
        {
            public bool DebugMode { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

            public void AddCustomEvent(object evt) { }

            public void AddCustomEvent(LogLevel level, string tag, string customEvent, params KeyValuePair<string, string>[] args)
            {
                throw new NotImplementedException();
            }

            public void AddException(string source, Exception ex, params KeyValuePair<string, string>[] details) { }
            public void AddInfo(string message, params KeyValuePair<string, string>[] details) { }

            public void AddKVPs(params KeyValuePair<string, string>[] args)
            {
                throw new NotImplementedException();
            }

            public void AddWarning(string message, params KeyValuePair<string, string>[] details) { }
            public void Debug(string message, params KeyValuePair<string, string>[] details) { }

            public void EndTimedEvent(TimedEvent evt)
            {
                throw new NotImplementedException();
            }

            public void Error(string message, params KeyValuePair<string, string>[] details) { }
            public void Exception(string message, Exception ex, params KeyValuePair<string, string>[] details) { }
            public void Info(string message, params KeyValuePair<string, string>[] details) { }
            public void LogTrace(string message) { }

            public TimedEvent StartTimedEvent(string area, string description)
            {
                throw new NotImplementedException();
            }

            public void Trace(string message, params KeyValuePair<string, string>[] details) { }

            public void TrackEvent(string message, Dictionary<string, string> parameters)
            {
                throw new NotImplementedException();
            }

            public void TrackMetric(string kind, string name, MetricType metricType, double count, params KeyValuePair<string, string>[] args)
            {
                throw new NotImplementedException();
            }

            public void TrackMetric(string kind, string name, MetricType metricType, int count, params KeyValuePair<string, string>[] args)
            {
                throw new NotImplementedException();
            }

            public void Warning(string message, params KeyValuePair<string, string>[] details) { }
        }
    }
}
