namespace LagoVista.Core.MessageQueue
{
    public class MessageQueueSubscriptionRoute
    {
        public string DestinationName { get; set; }
        public string QueueName { get; set; }
        public string RouteKey { get; set; }
        public bool Durable { get; set; } = true;
        public bool Exclusive { get; set; }
        public bool AutoDelete { get; set; }
        public ushort PrefetchCount { get; set; } = 1;
    }
}