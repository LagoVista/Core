using LagoVista.Core.MessageQueue;
using LagoVista.Core.PlatformSupport;
using LagoVista.MessageQueue.Rabbit;
using LagoVista.MessageQueue.RabbitMQ.IntegrationTests.TestSupport;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;

namespace LagoVista.MessageQueueTests
{
    public class LiveIntegrationTest
    {
        [Ignore("Simple integration test for live.")]
        [Test]
        public async Task ConnectAsync()
        {
            var subSerivce = new    RabbitMqSubscriberHostedService<IntegrationMessage>("TestSubscriber", new RabbitMqSubscriberSettings
            {
                HostName = "dev-queue.nuviot.com",
                UserName = "dev-tx-import-sub",
                Password = "************",
                VirtualHost = "dev",
                ExchangeName = "tx-import",
                Port = 5672,
                UseSsl = false,
                AutomaticRecoveryEnabled = true,
                TopologyRecoveryEnabled = true,
                TimeoutInSeconds = 30
            }, new SingleMessageTopology<IntegrationMessage>(new MessageQueuePublishRoute
            {
                DestinationName = "test-queue",
                RouteKey = "test-route",
             
            },
            new MessageQueueSubscriptionRoute
            {
                DestinationName = "dev.import.transactions",
                RouteKey = "test-route",

            }), Mock.Of<ILogger>(), Mock.Of<IServiceScopeFactory>());

            await subSerivce.StartIt();
        }

    }
}
