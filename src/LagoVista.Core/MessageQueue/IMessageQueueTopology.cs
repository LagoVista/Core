using System;

namespace LagoVista.Core.MessageQueue
{
    public interface IMessageQueueTopology
    {
        MessageQueuePublishRoute GetPublishRoute(Type messageType);
        MessageQueueSubscriptionRoute GetSubscriptionRoute(Type messageType);
    }
}
