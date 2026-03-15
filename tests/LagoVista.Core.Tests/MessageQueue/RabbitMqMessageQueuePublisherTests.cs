using LagoVista.Core.MessageQueue;
using LagoVista.Core.Tests.MessageQueue.TestSupport;
using LagoVista.MessageQueue.Rabbit;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LagoVista.Core.Tests.MessageQueue
{
    [TestFixture]
    public class RabbitMqMessageQueuePublisherTests
    {
        [Test]
        public void PublishAsync_When_Payload_Is_Null_Should_Throw()
        {
            var publisher = CreatePublisherFor<PublisherDerivedMessage>("billing", Array.Empty<RabbitMqMessageQueueServiceClient>());

            var ex = Assert.ThrowsAsync<ArgumentNullException>(async () => await publisher.PublishAsync<PublisherDerivedMessage>(null).ConfigureAwait(false));

            Assert.That(ex, Is.Not.Null);
            Assert.That(ex.ParamName, Is.EqualTo("payload"));
        }

        [Test]
        public void PublishAsync_When_Publisher_Is_Disposed_Should_Throw()
        {
            var publisher = CreatePublisherFor<PublisherDerivedMessage>("billing", Array.Empty<RabbitMqMessageQueueServiceClient>());
            publisher.Dispose();

            var ex = Assert.ThrowsAsync<ObjectDisposedException>(async () => await publisher.PublishAsync(new PublisherDerivedMessage { Id = "1", Name = "test" }).ConfigureAwait(false));

            Assert.That(ex, Is.Not.Null);
            Assert.That(ex.ObjectName, Is.EqualTo(nameof(RabbitMqMessageQueuePublisher)));
        }

        [Test]
        public void PublishAsync_When_Resolved_Service_Client_Is_Missing_Should_Throw()
        {
            var publisher = CreatePublisherFor<PublisherDerivedMessage>("billing", Array.Empty<RabbitMqMessageQueueServiceClient>());

            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () => await publisher.PublishAsync(new PublisherDerivedMessage { Id = "1", Name = "test" }).ConfigureAwait(false));

            Assert.That(ex, Is.Not.Null);
            Assert.That(ex.Message, Does.Contain("billing"));
            Assert.That(ex.Message, Does.Contain(typeof(PublisherDerivedMessage).FullName));
        }

        [Test]
        public void PublishAsync_Should_Use_Runtime_Payload_Type_For_Registry_Lookup()
        {
            var publisher = CreatePublisher(new IMessageQueueTypeRegistry[]
            {
                new RuntimeTypeOnlyRegistry(typeof(PublisherDerivedMessage), "billing")
            }, Array.Empty<RabbitMqMessageQueueServiceClient>());

            PublisherBaseMessage payload = new PublisherDerivedMessage
            {
                Id = "1",
                Name = "derived"
            };

            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () => await publisher.PublishAsync(payload).ConfigureAwait(false));

            Assert.That(ex, Is.Not.Null);
            Assert.That(ex.Message, Does.Contain("billing"));
            Assert.That(ex.Message, Does.Contain(typeof(PublisherDerivedMessage).FullName));
        }

        private static RabbitMqMessageQueuePublisher CreatePublisherFor<TMessage>(string serviceName, IEnumerable<RabbitMqMessageQueueServiceClient> serviceClients)
        {
            var registry = new RabbitMqMessageQueueTypeRegistry(new[]
            {
                new RabbitMqTypeRegistration
                {
                    MessageType = typeof(TMessage),
                    ServiceName = serviceName
                }
            }, new[] { serviceName });

            return new RabbitMqMessageQueuePublisher(registry, serviceClients);
        }

        private static RabbitMqMessageQueuePublisher CreatePublisher(IEnumerable<IMessageQueueTypeRegistry> registries, IEnumerable<RabbitMqMessageQueueServiceClient> serviceClients)
        {
            IMessageQueueTypeRegistry registry = null;
            foreach (var item in registries)
                registry = item;

            return new RabbitMqMessageQueuePublisher(registry, serviceClients);
        }

        private sealed class RuntimeTypeOnlyRegistry : IMessageQueueTypeRegistry
        {
            private readonly Type _expectedRuntimeType;
            private readonly string _serviceName;

            public RuntimeTypeOnlyRegistry(Type expectedRuntimeType, string serviceName)
            {
                _expectedRuntimeType = expectedRuntimeType;
                _serviceName = serviceName;
            }

            public string GetServiceNameFor(Type messageType)
            {
                if (messageType != _expectedRuntimeType)
                    throw new InvalidOperationException($"Expected runtime type '{_expectedRuntimeType.FullName}' but got '{messageType?.FullName}'.");

                return _serviceName;
            }
        }
    }
}
