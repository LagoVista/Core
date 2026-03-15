using System;
using System.Collections.Generic;

namespace LagoVista.Core.MessageQueue
{
    public class MessageQueueContext<T>
    {
        public T Payload { get; set; }
        public string MessageId { get; set; }
        public string MessageType { get; set; }
        public DateTime ReceivedAtUtc { get; set; }
        public IReadOnlyDictionary<string, string> Headers { get; set; }
    }
}