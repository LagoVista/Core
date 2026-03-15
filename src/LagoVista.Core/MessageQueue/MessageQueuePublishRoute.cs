namespace LagoVista.Core.MessageQueue
{
    public class MessageQueuePublishRoute
    {
        public string DestinationName { get; set; }
        public string RouteKey { get; set; }
        public string ContentType { get; set; } = "application/json";
        public bool Persistent { get; set; } = true;
    }
}