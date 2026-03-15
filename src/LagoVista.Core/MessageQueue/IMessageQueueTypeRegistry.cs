using System;

namespace LagoVista.Core.MessageQueue
{
    public interface IMessageQueueTypeRegistry
    {
        string GetServiceNameFor(Type messageType);
    }
}
