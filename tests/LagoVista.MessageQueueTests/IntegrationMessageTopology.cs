using LagoVista.Core.MessageQueue;
using System;

namespace LagoVista.MessageQueue.RabbitMQ.IntegrationTests.TestSupport
{
    public class IntegrationMessageTopology : IMessageQueueTopology
    {
        public const string ExchangeName = "lv.mq.integration.exchange";
        public const string QueueName = "lv.mq.integration.queue";
        public const string RouteKey = "lv.mq.integration.test";

        public MessageQueuePublishRoute GetPublishRoute(Type messageType)
        {
            if (messageType == typeof(IntegrationMessage))
            {
                return new MessageQueuePublishRoute
                {
                    DestinationName = ExchangeName,
                    RouteKey = RouteKey,
                    ContentType = "application/json",
                    Persistent = false
                };
            }

            throw new InvalidOperationException($"No publish route is registered for '{messageType.FullName}'.");
        }

        public MessageQueueSubscriptionRoute GetSubscriptionRoute(Type messageType)
        {
            if (messageType == typeof(IntegrationMessage))
            {
                return new MessageQueueSubscriptionRoute
                {
                    DestinationName = ExchangeName,
                    QueueName = QueueName,
                    RouteKey = RouteKey,
                    Durable = false,
                    Exclusive = false,
                    AutoDelete = true,
                    PrefetchCount = 1
                };
            }

            throw new InvalidOperationException($"No subscription route is registered for '{messageType.FullName}'.");
        }
    }
}