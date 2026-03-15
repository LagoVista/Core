using LagoVista.Core.MessageQueue;
using LagoVista.Core.PlatformSupport;
using LagoVista.MessageQueue.Rabbit;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LagoVista.MessageQueue.Rabbit
{
    public static class RabbitMqServiceCollectionExtensions
    {
        public static IServiceCollection AddRabbitMqMessageQueue(this IServiceCollection services, Action<RabbitMqMessageQueueBuilder> configure)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (configure == null) throw new ArgumentNullException(nameof(configure));

            var builder = new RabbitMqMessageQueueBuilder();
            configure(builder);

            if (!builder.ServiceRegistrations.Any()) throw new InvalidOperationException("At least one RabbitMQ service must be registered.");
            if (!builder.TypeRegistrations.Any()) throw new InvalidOperationException("At least one RabbitMQ message type must be registered.");

            foreach (var registration in builder.ServiceRegistrations)
                registration.Validate();

            foreach (var registration in builder.TypeRegistrations)
                registration.Validate();

            var duplicateServiceNames = builder.ServiceRegistrations
                .GroupBy(x => x.ServiceName, StringComparer.OrdinalIgnoreCase)
                .Where(x => x.Count() > 1)
                .Select(x => x.Key)
                .ToList();

            if (duplicateServiceNames.Any())
                throw new InvalidOperationException($"Duplicate RabbitMQ service registrations were found: {String.Join(", ", duplicateServiceNames)}.");

            var duplicateMessageTypes = builder.TypeRegistrations
                .GroupBy(x => x.MessageType)
                .Where(x => x.Count() > 1)
                .Select(x => x.Key.FullName)
                .ToList();

            if (duplicateMessageTypes.Any())
                throw new InvalidOperationException($"Duplicate RabbitMQ message type registrations were found: {String.Join(", ", duplicateMessageTypes)}.");

            var knownServiceNames = new HashSet<string>(builder.ServiceRegistrations.Select(x => x.ServiceName), StringComparer.OrdinalIgnoreCase);
            var unknownServiceNames = builder.TypeRegistrations
                .Where(x => !knownServiceNames.Contains(x.ServiceName))
                .Select(x => $"{x.MessageType.FullName} -> {x.ServiceName}")
                .ToList();

            if (unknownServiceNames.Any())
                throw new InvalidOperationException($"RabbitMQ message type registrations reference unknown services: {String.Join(", ", unknownServiceNames)}.");

            services.AddSingleton<IMessageQueueTypeRegistry>(_ => new RabbitMqMessageQueueTypeRegistry(builder.TypeRegistrations, builder.ServiceRegistrations.Select(x => x.ServiceName)));

            services.AddSingleton<IMessageQueuePublisher>(serviceProvider =>
            {
                var logger = serviceProvider.GetRequiredService<ILogger>();
                var typeRegistry = serviceProvider.GetRequiredService<IMessageQueueTypeRegistry>();

                var serviceClients = new List<RabbitMqMessageQueueServiceClient>();
                foreach (var registration in builder.ServiceRegistrations)
                    serviceClients.Add(new RabbitMqMessageQueueServiceClient(registration, logger));

                return new RabbitMqMessageQueuePublisher(typeRegistry, serviceClients);
            });

            return services;
        }
    }
}