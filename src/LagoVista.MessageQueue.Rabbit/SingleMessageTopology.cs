using LagoVista.Core.MessageQueue;
using System;

namespace LagoVista.MessageQueue.Rabbit
{
    public class SingleMessageTopology<TMessage> : IMessageQueueTopology
    {
        private readonly MessageQueuePublishRoute _publishRoute;
        private readonly MessageQueueSubscriptionRoute _subscriptionRoute;

        public SingleMessageTopology(MessageQueuePublishRoute publishRoute)
        {
            _publishRoute = publishRoute ?? throw new ArgumentNullException(nameof(publishRoute));
        }

        public SingleMessageTopology(MessageQueuePublishRoute publishRoute, MessageQueueSubscriptionRoute subscriptionRoute)
        {
            _publishRoute = publishRoute ?? throw new ArgumentNullException(nameof(publishRoute));
            _subscriptionRoute = subscriptionRoute ?? throw new ArgumentNullException(nameof(subscriptionRoute));
        }

        public MessageQueuePublishRoute GetPublishRoute(Type messageType)
        {
            EnsureMessageType(messageType);
            return _publishRoute;
        }

        public MessageQueueSubscriptionRoute GetSubscriptionRoute(Type messageType)
        {
            EnsureMessageType(messageType);

            if (_subscriptionRoute == null)
                throw new InvalidOperationException($"No subscription route has been registered for message type '{typeof(TMessage).FullName}'.");

            return _subscriptionRoute;
        }

        private static void EnsureMessageType(Type messageType)
        {
            if (messageType == null) throw new ArgumentNullException(nameof(messageType));
            if (messageType != typeof(TMessage))
                throw new InvalidOperationException($"No route has been registered for message type '{messageType.FullName}'.");
        }
    }
}
