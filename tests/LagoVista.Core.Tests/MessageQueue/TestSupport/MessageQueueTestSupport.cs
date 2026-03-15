using LagoVista.Core.Interfaces;
using LagoVista.Core.MessageQueue;
using System;
using System.Collections.Generic;

namespace LagoVista.Core.Tests.MessageQueue.TestSupport
{
    public class TestConnectionSettings : IConnectionSettings
    {
        public string Name { get; set; }
        public string Uri { get; set; }
        public string Baud { get; set; }
        public string IPAddressV4 { get; set; }
        public string IPAddressV6 { get; set; }
        public string AccessKey { get; set; }
        public string UserName { get; set; }
        public string AccountId { get; set; }
        public string DeviceId { get; set; }
        public string Password { get; set; }
        public string Port { get; set; }
        public string ResourceName { get; set; }
        public string ValidThrough { get; set; }
        public bool IsSSL { get; set; }
        public Func<bool> ValidationAction { get; set; }
        public Func<string> GetValidationErrors { get; set; }
        public Dictionary<string, string> Settings { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        public int TimeoutInSeconds { get; set; }
    }

    public class TestMessageA
    {
        public string Id { get; set; }
    }

    public class TestMessageB
    {
        public string Id { get; set; }
    }

    public class TestMessageQueueTopology : IMessageQueueTopology
    {
        public MessageQueuePublishRoute GetPublishRoute(Type messageType)
        {
            return new MessageQueuePublishRoute
            {
                DestinationName = "test.exchange",
                RouteKey = "test.route",
                ContentType = "application/json",
                Persistent = true
            };
        }

        public MessageQueueSubscriptionRoute GetSubscriptionRoute(Type messageType)
        {
            return new MessageQueueSubscriptionRoute
            {
                DestinationName = "test.exchange",
                QueueName = "test.queue",
                RouteKey = "test.route",
                Durable = true,
                Exclusive = false,
                AutoDelete = false,
                PrefetchCount = 1
            };
        }
    }
}
