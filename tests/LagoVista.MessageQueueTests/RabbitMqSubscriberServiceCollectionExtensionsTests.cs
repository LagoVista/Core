using LagoVista.Core.MessageQueue;
using LagoVista.Core.PlatformSupport;
using LagoVista.MessageQueue.Rabbit;
using LagoVista.MessageQueue.RabbitMQ.IntegrationTests.TestSupport;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using System;
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

        [Test]
        public void AddRabbitMqSubscriber_With_Multiple_Handlers_Should_Register_Handlers_And_One_Hosted_Service()
        {
            var services = new ServiceCollection();
            services.AddSingleton<ILogger>(new TestAdminLogger());

            var configuration = BuildSubscriberConfiguration("BillingEvents");

            services.AddRabbitMqSubscriber(
                configuration,
                "BillingEvents",
                subscriber =>
                {
                    subscriber.AddHandler<TokenUsageRequest, TokenUsageRequestHandler>();
                    subscriber.AddHandler<BillingUsageRequest, BillingUsageRequestHandler>();
                });

            using var serviceProvider = services.BuildServiceProvider();

            var tokenHandler = serviceProvider.GetRequiredService<IMessageQueueHandler<TokenUsageRequest>>();
            var billingHandler = serviceProvider.GetRequiredService<IMessageQueueHandler<BillingUsageRequest>>();
            var hostedService = serviceProvider.GetServices<IHostedService>().Single();

            Assert.That(tokenHandler, Is.TypeOf<TokenUsageRequestHandler>());
            Assert.That(billingHandler, Is.TypeOf<BillingUsageRequestHandler>());
            Assert.That(hostedService, Is.TypeOf<RabbitMqSubscriberHostedService>());
            Assert.That(((RabbitMqSubscriberHostedService)hostedService).Name, Does.Contain("BillingEvents"));
        }

        [Test]
        public void AddRabbitMqSubscriber_With_Service_Name_Should_Use_Service_Name()
        {
            var services = new ServiceCollection();
            services.AddSingleton<ILogger>(new TestAdminLogger());

            var configuration = BuildSubscriberConfiguration("BillingEvents");

            services.AddRabbitMqSubscriber(
                configuration,
                "BillingEvents",
                "billing-events",
                subscriber => subscriber.AddHandler<TokenUsageRequest, TokenUsageRequestHandler>());

            using var serviceProvider = services.BuildServiceProvider();
            var hostedService = serviceProvider.GetServices<IHostedService>().Single();

            Assert.That(((RabbitMqSubscriberHostedService)hostedService).Name, Does.Contain("billing-events"));
        }

        [Test]
        public void AddRabbitMqSubscriber_With_Duplicate_Message_Class_Names_Should_Throw()
        {
            var services = new ServiceCollection();
            var configuration = BuildSubscriberConfiguration("BillingEvents");

            var exception = Assert.Throws<InvalidOperationException>(() =>
                services.AddRabbitMqSubscriber(
                    configuration,
                    "BillingEvents",
                    subscriber =>
                    {
                        subscriber.AddHandler<GenericRequest<TokenUsageRequest>, GenericTokenUsageRequestHandler>();
                        subscriber.AddHandler<GenericRequest<BillingUsageRequest>, GenericBillingUsageRequestHandler>();
                    }));

            Assert.That(exception.Message, Does.Contain("duplicate message class names"));
            Assert.That(exception.Message, Does.Contain("GenericRequest"));
        }

        [Test]
        public void AddRabbitMqSubscriber_With_No_Handlers_Should_Throw()
        {
            var services = new ServiceCollection();
            var configuration = BuildSubscriberConfiguration("BillingEvents");

            var exception = Assert.Throws<InvalidOperationException>(() =>
                services.AddRabbitMqSubscriber(configuration, "BillingEvents", subscriber => { }));

            Assert.That(exception.Message, Does.Contain("must register at least one message handler"));
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

        private sealed class TokenUsageRequest { }
        private sealed class BillingUsageRequest { }
        private sealed class GenericRequest<T> { }

        private sealed class TokenUsageRequestHandler : IMessageQueueHandler<TokenUsageRequest>
        {
            public System.Threading.Tasks.Task HandleAsync(MessageQueueContext<TokenUsageRequest> context, System.Threading.CancellationToken cancellationToken = default)
            {
                return System.Threading.Tasks.Task.CompletedTask;
            }
        }

        private sealed class BillingUsageRequestHandler : IMessageQueueHandler<BillingUsageRequest>
        {
            public System.Threading.Tasks.Task HandleAsync(MessageQueueContext<BillingUsageRequest> context, System.Threading.CancellationToken cancellationToken = default)
            {
                return System.Threading.Tasks.Task.CompletedTask;
            }
        }

        private sealed class GenericTokenUsageRequestHandler : IMessageQueueHandler<GenericRequest<TokenUsageRequest>>
        {
            public System.Threading.Tasks.Task HandleAsync(MessageQueueContext<GenericRequest<TokenUsageRequest>> context, System.Threading.CancellationToken cancellationToken = default)
            {
                return System.Threading.Tasks.Task.CompletedTask;
            }
        }

        private sealed class GenericBillingUsageRequestHandler : IMessageQueueHandler<GenericRequest<BillingUsageRequest>>
        {
            public System.Threading.Tasks.Task HandleAsync(MessageQueueContext<GenericRequest<BillingUsageRequest>> context, System.Threading.CancellationToken cancellationToken = default)
            {
                return System.Threading.Tasks.Task.CompletedTask;
            }
        }
    }
}
