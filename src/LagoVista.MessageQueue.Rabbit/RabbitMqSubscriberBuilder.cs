using LagoVista.Core.MessageQueue;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.MessageQueue.Rabbit
{
    public sealed class RabbitMqSubscriberBuilder
    {
        internal IList<RabbitMqSubscriberHandlerRegistration> Registrations { get; }
            = new List<RabbitMqSubscriberHandlerRegistration>();

        public RabbitMqSubscriberBuilder AddHandler<TMessage, THandler>()
            where THandler : class, IMessageQueueHandler<TMessage>
        {
            Registrations.Add(new RabbitMqSubscriberHandlerRegistration
            {
                MessageTypeName = typeof(TMessage).Name,
                MessageType = typeof(TMessage),
                HandlerType = typeof(THandler),
                Dispatcher = new RabbitMqMessageDispatcher<TMessage>()
            });

            return this;
        }
    }

    internal sealed class RabbitMqSubscriberHandlerRegistration
    {
        public string MessageTypeName { get; set; }
        public Type MessageType { get; set; }
        public Type HandlerType { get; set; }
        public IRabbitMqMessageDispatcher Dispatcher { get; set; }
    }

    internal interface IRabbitMqMessageDispatcher
    {
        Task DispatchAsync(object payload, BasicDeliverEventArgs args, IServiceProvider serviceProvider, CancellationToken cancellationToken);
    }

    internal sealed class RabbitMqMessageDispatcher<TMessage> : IRabbitMqMessageDispatcher
    {
        public async Task DispatchAsync(object payload, BasicDeliverEventArgs args, IServiceProvider serviceProvider, CancellationToken cancellationToken)
        {
            var handler = serviceProvider.GetRequiredService<IMessageQueueHandler<TMessage>>();

            var context = new MessageQueueContext<TMessage>
            {
                Payload = (TMessage)payload,
                MessageId = args.BasicProperties?.MessageId,
                MessageType = typeof(TMessage).Name,
                ReceivedAtUtc = DateTime.UtcNow,
                Headers = ConvertHeaders(args.BasicProperties?.Headers)
            };

            await handler.HandleAsync(context, cancellationToken).ConfigureAwait(false);
        }

        private static IReadOnlyDictionary<string, string> ConvertHeaders(IDictionary<string, object> headers)
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (headers == null) return result;

            foreach (var header in headers)
            {
                if (header.Value is byte[] bytes)
                    result[header.Key] = Encoding.UTF8.GetString(bytes);
                else if (header.Value != null)
                    result[header.Key] = header.Value.ToString();
            }

            return result;
        }
    }
}
