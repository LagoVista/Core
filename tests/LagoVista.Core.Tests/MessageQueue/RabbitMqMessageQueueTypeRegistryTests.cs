using LagoVista.Core.Tests.MessageQueue.TestSupport;
using LagoVista.MessageQueue.Rabbit;
using NUnit.Framework;
using System;

namespace LagoVista.Core.Tests.MessageQueue
{
    [TestFixture]
    public class RabbitMqMessageQueueTypeRegistryTests
    {
        [Test]
        public void GetServiceNameFor_When_Type_Is_Registered_Should_Return_Service_Name()
        {
            var registry = new RabbitMqMessageQueueTypeRegistry(new[]
            {
                new RabbitMqTypeRegistration
                {
                    MessageType = typeof(TestMessageA),
                    ServiceName = "billing"
                }
            }, new[] { "billing" });

            var result = registry.GetServiceNameFor(typeof(TestMessageA));

            Assert.That(result, Is.EqualTo("billing"));
        }

        [Test]
        public void GetServiceNameFor_When_Type_Is_Not_Registered_Should_Throw()
        {
            var registry = new RabbitMqMessageQueueTypeRegistry(new[]
            {
                new RabbitMqTypeRegistration
                {
                    MessageType = typeof(TestMessageA),
                    ServiceName = "billing"
                }
            }, new[] { "billing" });

            var ex = Assert.Throws<InvalidOperationException>(() => registry.GetServiceNameFor(typeof(TestMessageB)));

            Assert.That(ex, Is.Not.Null);
            Assert.That(ex.Message, Does.Contain(typeof(TestMessageB).FullName));
        }

        [Test]
        public void Constructor_When_Type_Is_Registered_Twice_Should_Throw()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => new RabbitMqMessageQueueTypeRegistry(new[]
            {
                new RabbitMqTypeRegistration
                {
                    MessageType = typeof(TestMessageA),
                    ServiceName = "billing"
                },
                new RabbitMqTypeRegistration
                {
                    MessageType = typeof(TestMessageA),
                    ServiceName = "billing"
                }
            }, new[] { "billing" }));

            Assert.That(ex, Is.Not.Null);
            Assert.That(ex.Message, Does.Contain(typeof(TestMessageA).FullName));
        }

        [Test]
        public void Constructor_When_Type_Maps_To_Unknown_Service_Should_Throw()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => new RabbitMqMessageQueueTypeRegistry(new[]
            {
                new RabbitMqTypeRegistration
                {
                    MessageType = typeof(TestMessageA),
                    ServiceName = "billing"
                }
            }, new[] { "plaid" }));

            Assert.That(ex, Is.Not.Null);
            Assert.That(ex.Message, Does.Contain("billing"));
        }

        [Test]
        public void Constructor_When_Service_Name_Differs_Only_By_Case_Should_Treat_Service_As_Known()
        {
            var registry = new RabbitMqMessageQueueTypeRegistry(new[]
            {
                new RabbitMqTypeRegistration
                {
                    MessageType = typeof(TestMessageA),
                    ServiceName = "BILLING"
                }
            }, new[] { "billing" });

            var result = registry.GetServiceNameFor(typeof(TestMessageA));

            Assert.That(result, Is.EqualTo("BILLING"));
        }
    }
}
