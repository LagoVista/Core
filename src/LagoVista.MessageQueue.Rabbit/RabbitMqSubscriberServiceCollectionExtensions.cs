using LagoVista.Core.MessageQueue;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System;

namespace LagoVista.MessageQueue.Rabbit
{
    public static class RabbitMqSubscriberServiceCollectionExtensions
    {
        public static IServiceCollection AddRabbitMqSubscriber<TMessage, THandler>(this IServiceCollection services, IConfiguration configuration, string sectionName)
            where THandler : class, IMessageQueueHandler<TMessage>
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            if (String.IsNullOrWhiteSpace(sectionName)) throw new ArgumentNullException(nameof(sectionName));

            var settings = RabbitMqSubscriberSettings.Read(configuration, sectionName);
            return services.AddRabbitMqSubscriber<TMessage, THandler>(settings, sectionName);
        }

        public static IServiceCollection AddRabbitMqSubscriber<TMessage, THandler>(this IServiceCollection services, RabbitMqSubscriberSettings settings, string serviceName)
    where THandler : class, IMessageQueueHandler<TMessage>
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (settings == null) throw new ArgumentNullException(nameof(settings));
            if (String.IsNullOrWhiteSpace(serviceName)) throw new ArgumentNullException(nameof(serviceName));

            settings.Validate(serviceName);

            var topology = new SingleMessageTopology<TMessage>(
                new MessageQueuePublishRoute
                {
                    DestinationName = settings.ExchangeName,
                    RouteKey = settings.RouteKey,
                    ContentType = "application/json",
                    Persistent = settings.Durable
                },
                new MessageQueueSubscriptionRoute
                {
                    DestinationName = settings.ExchangeName,
                    QueueName = settings.QueueName,
                    RouteKey = settings.RouteKey,
                    Durable = settings.Durable,
                    Exclusive = settings.Exclusive,
                    AutoDelete = settings.AutoDelete,
                    PrefetchCount = settings.PrefetchCount
                });

            services.AddTransient<IMessageQueueHandler<TMessage>, THandler>();

            services.AddSingleton<RabbitMqSubscriberHostedService<TMessage>>(serviceProvider =>
                new RabbitMqSubscriberHostedService<TMessage>(
                    serviceName,
                    settings,
                    topology,
                    serviceProvider.GetRequiredService<LagoVista.Core.PlatformSupport.ILogger>(),
                    serviceProvider.GetRequiredService<IServiceScopeFactory>()));

            services.AddSingleton<IHostedService>(serviceProvider =>
                serviceProvider.GetRequiredService<RabbitMqSubscriberHostedService<TMessage>>());

            return services;
        }
    }
}
