using LagoVista.Core.Tests.MessageQueue.TestSupport;
using LagoVista.MessageQueue.Rabbit;
using NUnit.Framework;
using System;

namespace LagoVista.Core.Tests.MessageQueue
{
    [TestFixture]
    public class RabbitMqTypeRegistrationTests
    {
        [Test]
        public void Validate_When_MessageType_Is_Null_Should_Throw()
        {
            var registration = new RabbitMqTypeRegistration
            {
                MessageType = null,
                ServiceName = "billing"
            };

            var ex = Assert.Throws<InvalidOperationException>(() => registration.Validate());

            Assert.That(ex, Is.Not.Null);
            Assert.That(ex.Message, Does.Contain("MessageType"));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void Validate_When_ServiceName_Is_Missing_Should_Throw(string serviceName)
        {
            var registration = new RabbitMqTypeRegistration
            {
                MessageType = typeof(TestMessageA),
                ServiceName = serviceName
            };

            var ex = Assert.Throws<InvalidOperationException>(() => registration.Validate());

            Assert.That(ex, Is.Not.Null);
            Assert.That(ex.Message, Does.Contain("ServiceName"));
        }

        [Test]
        public void Validate_When_Registration_Is_Valid_Should_Not_Throw()
        {
            var registration = new RabbitMqTypeRegistration
            {
                MessageType = typeof(TestMessageA),
                ServiceName = "billing"
            };

            Assert.DoesNotThrow(() => registration.Validate());
        }
    }
}
