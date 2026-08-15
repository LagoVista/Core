using LagoVista.Core.Interfaces;
using LagoVista.Core.MessageQueue;
using LagoVista.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LagoVista.MessageQueue.Rabbit
{
    public static class RabbitMqSubscriberServiceCollectionExtensions
    {
        public static IServiceCollection AddRabbitMqSubscriber<TMessage, THandler>(this IServiceCollection services, IConfiguration configuration, string sectionName, string serviceName)
            where THandler : class, IMessageQueueHandler<TMessage>
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            if (String.IsNullOrWhiteSpace(sectionName)) throw new ArgumentNullException(nameof(sectionName));

            var settings = RabbitMqSubscriberSettings.Read(configuration, sectionName);
            return services.AddRabbitMqSubscriber<TMessage, THandler>(settings, serviceName);
        }

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
            services.TryAddSingleton<IApplicationRuntimeState, ApplicationRuntimeStateService>();

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
                    serviceProvider.GetRequiredService<IServiceScopeFactory>(),
                    serviceProvider.GetRequiredService<IApplicationRuntimeState>()));

            services.AddSingleton<IHostedService>(serviceProvider =>
                serviceProvider.GetRequiredService<RabbitMqSubscriberHostedService<TMessage>>());

            services.AddSingleton<IHostedServiceDiagnostics>(serviceProvider =>
                serviceProvider.GetRequiredService<RabbitMqSubscriberHostedService<TMessage>>());

            return services;
        }

        public static IServiceCollection AddRabbitMqSubscriber(this IServiceCollection services, IConfiguration configuration, string sectionName, Action<RabbitMqSubscriberBuilder> configure)
        {
            return services.AddRabbitMqSubscriber(configuration, sectionName, sectionName, configure);
        }

        public static IServiceCollection AddRabbitMqSubscriber(this IServiceCollection services, IConfiguration configuration, string sectionName, string serviceName, Action<RabbitMqSubscriberBuilder> configure)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            if (String.IsNullOrWhiteSpace(sectionName)) throw new ArgumentNullException(nameof(sectionName));

            var settings = RabbitMqSubscriberSettings.Read(configuration, sectionName);
            return services.AddRabbitMqSubscriber(settings, serviceName, configure);
        }

        public static IServiceCollection AddRabbitMqSubscriber(this IServiceCollection services, RabbitMqSubscriberSettings settings, string serviceName, Action<RabbitMqSubscriberBuilder> configure)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (settings == null) throw new ArgumentNullException(nameof(settings));
            if (String.IsNullOrWhiteSpace(serviceName)) throw new ArgumentNullException(nameof(serviceName));
            if (configure == null) throw new ArgumentNullException(nameof(configure));

            settings.Validate(serviceName);
            services.TryAddSingleton<IApplicationRuntimeState, ApplicationRuntimeStateService>();

            var builder = new RabbitMqSubscriberBuilder();
            configure(builder);

            ValidateRegistrations(builder.Registrations, serviceName);

            foreach (var registration in builder.Registrations)
            {
                services.AddTransient(
                    typeof(IMessageQueueHandler<>).MakeGenericType(registration.MessageType),
                    registration.HandlerType);
            }

            var registrations = builder.Registrations.ToList();

            services.AddSingleton<RabbitMqSubscriberHostedService>(serviceProvider =>
                new RabbitMqSubscriberHostedService(
                    serviceName,
                    settings,
                    registrations,
                    serviceProvider.GetRequiredService<LagoVista.Core.PlatformSupport.ILogger>(),
                    serviceProvider.GetRequiredService<IServiceScopeFactory>(),
                    serviceProvider.GetRequiredService<IApplicationRuntimeState>()));

            services.AddSingleton<IHostedService>(serviceProvider =>
                serviceProvider.GetRequiredService<RabbitMqSubscriberHostedService>());

            services.AddSingleton<IHostedServiceDiagnostics>(serviceProvider =>
                serviceProvider.GetRequiredService<RabbitMqSubscriberHostedService>());

            return services;
        }

        private static void ValidateRegistrations(IList<RabbitMqSubscriberHandlerRegistration> registrations, string serviceName)
        {
            if (!registrations.Any())
                throw new InvalidOperationException($"RabbitMQ subscriber '{serviceName}' must register at least one message handler.");

            var duplicateMessageTypeNames = registrations
                .GroupBy(x => x.MessageTypeName, StringComparer.OrdinalIgnoreCase)
                .Where(x => x.Count() > 1)
                .Select(x => x.Key)
                .ToList();

            if (duplicateMessageTypeNames.Any())
                throw new InvalidOperationException($"RabbitMQ subscriber '{serviceName}' contains duplicate message class names: {String.Join(", ", duplicateMessageTypeNames)}.");

            var duplicateMessageTypes = registrations
                .GroupBy(x => x.MessageType)
                .Where(x => x.Count() > 1)
                .Select(x => x.Key.FullName)
                .ToList();

            if (duplicateMessageTypes.Any())
                throw new InvalidOperationException($"RabbitMQ subscriber '{serviceName}' contains duplicate message types: {String.Join(", ", duplicateMessageTypes)}.");
        }
    }
}
