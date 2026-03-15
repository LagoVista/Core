using System;

namespace LagoVista.MessageQueue.Rabbit
{
    public class RabbitMqTypeRegistration
    {
        public Type MessageType { get; set; }
        public string ServiceName { get; set; }

        public void Validate()
        {
            if (MessageType == null) throw new InvalidOperationException("RabbitMQ type registration requires a MessageType.");
            if (String.IsNullOrWhiteSpace(ServiceName)) throw new InvalidOperationException($"RabbitMQ type registration for '{MessageType}' requires a ServiceName.");
        }
    }
}