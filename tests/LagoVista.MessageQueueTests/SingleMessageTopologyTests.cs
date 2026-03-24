using LagoVista.Core.MessageQueue;
using LagoVista.MessageQueue.Rabbit;
using LagoVista.MessageQueue.RabbitMQ.IntegrationTests.TestSupport;
using NUnit.Framework;
using System;

namespace LagoVista.MessageQueue.RabbitMQ.IntegrationTests
{
    [TestFixture]
    public class SingleMessageTopologyTests
    {
        [Test]
        public void GetPublishRoute_When_Message_Type_Matches_Should_Return_Route()
        {
            var publishRoute = new MessageQueuePublishRoute
            {
                DestinationName = "integration.exchange",
                RouteKey = "integration.route"
            };

            var topology = new SingleMessageTopology<IntegrationMessage>(publishRoute);

            var result = topology.GetPublishRoute(typeof(IntegrationMessage));

            Assert.That(result, Is.SameAs(publishRoute));
        }

        [Test]
        public void GetSubscriptionRoute_When_Subscription_Route_Was_Not_Configured_Should_Throw()
        {
            var publishRoute = new MessageQueuePublishRoute
            {
                DestinationName = "integration.exchange",
                RouteKey = "integration.route"
            };

            var topology = new SingleMessageTopology<IntegrationMessage>(publishRoute);

            Assert.That(() => topology.GetSubscriptionRoute(typeof(IntegrationMessage)), Throws.InvalidOperationException);
        }

        [Test]
        public void GetPublishRoute_When_Message_Type_Does_Not_Match_Should_Throw()
        {
            var publishRoute = new MessageQueuePublishRoute
            {
                DestinationName = "integration.exchange",
                RouteKey = "integration.route"
            };

            var topology = new SingleMessageTopology<IntegrationMessage>(publishRoute);

            Assert.That(() => topology.GetPublishRoute(typeof(String)), Throws.InvalidOperationException);
        }
    }
}
