using LagoVista.Core.Tests.MessageQueue.TestSupport;
using LagoVista.MessageQueue.Rabbit;
using NUnit.Framework;
using System;

namespace LagoVista.Core.Tests.MessageQueue
{
    [TestFixture]
    public class RabbitMqServiceRegistrationTests
    {
        [Test]
        public void Validate_When_Registration_Is_Valid_Should_Not_Throw()
        {
            var registration = CreateValidRegistration();

            Assert.DoesNotThrow(() => registration.Validate());
        }

        [Test]
        public void Validate_When_ServiceName_Is_Missing_Should_Throw()
        {
            var registration = CreateValidRegistration();
            registration.ServiceName = null;

            Assert.Throws<InvalidOperationException>(() => registration.Validate());
        }

        [Test]
        public void Validate_When_ConnectionSettings_Are_Missing_Should_Throw()
        {
            var registration = CreateValidRegistration();
            registration.ConnectionSettings = null;

            Assert.Throws<InvalidOperationException>(() => registration.Validate());
        }

        [Test]
        public void Validate_When_Topology_Is_Missing_Should_Throw()
        {
            var registration = CreateValidRegistration();
            registration.Topology = null;

            Assert.Throws<InvalidOperationException>(() => registration.Validate());
        }

        [Test]
        public void Validate_When_HostName_Is_Missing_Should_Throw()
        {
            var registration = CreateValidRegistration();
            registration.ConnectionSettings.HostName = null;

            Assert.Throws<InvalidOperationException>(() => registration.Validate());
        }

        [Test]
        public void Validate_When_UserName_Is_Missing_Should_Throw()
        {
            var registration = CreateValidRegistration();
            registration.ConnectionSettings.UserName = null;

            Assert.Throws<InvalidOperationException>(() => registration.Validate());
        }

        [Test]
        public void Validate_When_Password_Is_Missing_Should_Throw()
        {
            var registration = CreateValidRegistration();
            registration.ConnectionSettings.Password = null;

            Assert.Throws<InvalidOperationException>(() => registration.Validate());
        }

        [Test]
        public void Validate_When_VirtualHost_Is_Missing_Should_Throw()
        {
            var registration = CreateValidRegistration();
            registration.ConnectionSettings.VirtualHost = null;

            Assert.Throws<InvalidOperationException>(() => registration.Validate());
        }

        private static RabbitMqServiceRegistration CreateValidRegistration()
        {
            return new RabbitMqServiceRegistration
            {
                ServiceName = "billing",
                ConnectionSettings = new RabbitMqConnectionSettings
                {
                    HostName = "rabbit-host",
                    UserName = "rabbit-user",
                    Password = "rabbit-pass",
                    VirtualHost = "/billing"
                },
                Topology = new TestMessageQueueTopology()
            };
        }
    }
}
