using LagoVista.Core.MessageQueue;
using LagoVista.Core.PlatformSupport;
using LagoVista.MessageQueue.Rabbit;
using LagoVista.MessageQueue.RabbitMQ.IntegrationTests.TestSupport;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace LagoVista.MessageQueue.RabbitMQ.IntegrationTests
{
    [TestFixture]
    public class RabbitMqSubscriberServiceCollectionExtensionsTests
    {
        [Test]
        public void AddRabbitMqSubscriber_When_Section_Is_Valid_Should_Register_Handler_And_Hosted_Service()
        {
            var services = new ServiceCollection();
            services.AddSingleton<ILogger>(new TestAdminLogger());

            var configuration = BuildSubscriberConfiguration("PlaidSyncSubscriber");

            services.AddRabbitMqSubscriber<IntegrationMessage, RegisteredIntegrationHandler>(configuration, "PlaidSyncSubscriber");

            using var serviceProvider = services.BuildServiceProvider();

            var handler = serviceProvider.GetRequiredService<IMessageQueueHandler<IntegrationMessage>>();
            var hostedService = serviceProvider.GetServices<IHostedService>().Single();

            Assert.That(handler, Is.Not.Null);
            Assert.That(hostedService, Is.Not.Null);
            Assert.That(hostedService, Is.TypeOf<RabbitMqSubscriberHostedService<IntegrationMessage>>());
        }

        private static IConfiguration BuildSubscriberConfiguration(string sectionName)
        {
            return new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    [$"{sectionName}:Name"] = sectionName,
                    [$"{sectionName}:HostName"] = "rabbit-host",
                    [$"{sectionName}:UserName"] = "subscriber-user",
                    [$"{sectionName}:Password"] = "subscriber-pass",
                    [$"{sectionName}:VirtualHost"] = "/plaid",
                    [$"{sectionName}:Port"] = "5672",
                    [$"{sectionName}:UseSsl"] = "false",
                    [$"{sectionName}:ExchangeName"] = "plaid.exchange",
                    [$"{sectionName}:QueueName"] = "plaid.queue",
                    [$"{sectionName}:RouteKey"] = "plaid.sync.requested"
                })
                .Build();
        }

        private sealed class RegisteredIntegrationHandler : IMessageQueueHandler<IntegrationMessage>
        {
            public System.Threading.Tasks.Task HandleAsync(MessageQueueContext<IntegrationMessage> context, System.Threading.CancellationToken cancellationToken = default)
            {
                return System.Threading.Tasks.Task.CompletedTask;
            }
        }
    }
}
