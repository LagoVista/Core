using LagoVista.Core.MessageQueue;
using LagoVista.Core.PlatformSupport;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LagoVista.MessageQueue.Rabbit
{
    public static class RabbitMqPublisherServiceCollectionExtensions
    {
        public static IServiceCollection AddRabbitMqPublisher<TMessage, TPublisher, TImplementation>(this IServiceCollection services, IConfiguration configuration, string sectionName)
            where TPublisher : class
            where TImplementation : class, TPublisher
        {
            if(services == null) throw new ArgumentNullException(nameof(services));
            if(configuration == null) throw new ArgumentNullException(nameof(configuration));
            if(String.IsNullOrEmpty(sectionName)) throw new ArgumentNullException(nameof(sectionName));

            var settings = RabbitMqPublisherSettings.Read(configuration, sectionName);
            return services.AddRabbitMqPublisher<TMessage, TPublisher, TImplementation>(settings, sectionName);
        }

        public static IServiceCollection AddRabbitMqPublisher<TMessage, TPublisher, TImplementation>(this IServiceCollection services, RabbitMqPublisherSettings settings, string serviceName)
            where TPublisher : class
            where TImplementation : class, TPublisher
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (settings == null) throw new ArgumentNullException(nameof(settings));
            if (String.IsNullOrEmpty(serviceName)) throw new ArgumentNullException(nameof(serviceName));
            settings.Validate(serviceName);

            var topology = new SingleMessageTopology<TMessage>(new MessageQueuePublishRoute
            {
                DestinationName = settings.ExchangeName,
                RouteKey = settings.RouteKey,
                ContentType = settings.ContentType,
                Persistent = settings.Persistent
            });

            services.AddSingleton(new RabbitMqServiceRegistration
            {
                ServiceName = serviceName,
                ConnectionSettings = settings.ToConnectionSettings(),
                Topology = topology
            });

            services.AddSingleton(new RabbitMqTypeRegistration
            {
                MessageType = typeof(TMessage),
                ServiceName = serviceName
            });

            services.AddTransient<TPublisher, TImplementation>();

            EnsurePublisherInfrastructureRegistered(services);

            return services;
        }

        private static void EnsurePublisherInfrastructureRegistered(IServiceCollection services)
        {
            services.TryAddSingleton<IMessageQueueTypeRegistry>(serviceProvider =>
            {
                var serviceRegistrations = serviceProvider.GetServices<RabbitMqServiceRegistration>().ToList();
                var typeRegistrations = serviceProvider.GetServices<RabbitMqTypeRegistration>().ToList();

                ValidateRegistrations(serviceRegistrations, typeRegistrations);

                return new RabbitMqMessageQueueTypeRegistry(typeRegistrations, serviceRegistrations.Select(x => x.ServiceName));
            });

            services.TryAddSingleton<IMessageQueuePublisher>(serviceProvider =>
            {
                var logger = serviceProvider.GetRequiredService<ILogger>();
                var typeRegistry = serviceProvider.GetRequiredService<IMessageQueueTypeRegistry>();
                var serviceRegistrations = serviceProvider.GetServices<RabbitMqServiceRegistration>().ToList();

                var serviceClients = new List<RabbitMqMessageQueueServiceClient>();
                foreach (var registration in serviceRegistrations)
                    serviceClients.Add(new RabbitMqMessageQueueServiceClient(registration, logger));

                return new RabbitMqMessageQueuePublisher(typeRegistry, serviceClients);
            });
        }

        private static void ValidateRegistrations(IReadOnlyList<RabbitMqServiceRegistration> serviceRegistrations, IReadOnlyList<RabbitMqTypeRegistration> typeRegistrations)
        {
            if (serviceRegistrations == null) throw new ArgumentNullException(nameof(serviceRegistrations));
            if (typeRegistrations == null) throw new ArgumentNullException(nameof(typeRegistrations));

            if (!serviceRegistrations.Any()) throw new InvalidOperationException("At least one RabbitMQ service must be registered.");
            if (!typeRegistrations.Any()) throw new InvalidOperationException("At least one RabbitMQ message type must be registered.");

            foreach (var registration in serviceRegistrations)
                registration.Validate();

            foreach (var registration in typeRegistrations)
                registration.Validate();

            var duplicateServiceNames = serviceRegistrations
                .GroupBy(x => x.ServiceName, StringComparer.OrdinalIgnoreCase)
                .Where(x => x.Count() > 1)
                .Select(x => x.Key)
                .ToList();

            if (duplicateServiceNames.Any())
                throw new InvalidOperationException($"Duplicate RabbitMQ service registrations were found: {String.Join(", ", duplicateServiceNames)}.");

            var duplicateMessageTypes = typeRegistrations
                .GroupBy(x => x.MessageType)
                .Where(x => x.Count() > 1)
                .Select(x => x.Key.FullName)
                .ToList();

            if (duplicateMessageTypes.Any())
                throw new InvalidOperationException($"Duplicate RabbitMQ message type registrations were found: {String.Join(", ", duplicateMessageTypes)}.");

            var knownServiceNames = new HashSet<string>(serviceRegistrations.Select(x => x.ServiceName), StringComparer.OrdinalIgnoreCase);
            var unknownServiceNames = typeRegistrations
                .Where(x => !knownServiceNames.Contains(x.ServiceName))
                .Select(x => $"{x.MessageType.FullName} -> {x.ServiceName}")
                .ToList();

            if (unknownServiceNames.Any())
                throw new InvalidOperationException($"RabbitMQ message type registrations reference unknown services: {String.Join(", ", unknownServiceNames)}.");
        }
    }
}
