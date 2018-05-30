using LagoVista.Core.PlatformSupport;
using Microsoft.Azure.ServiceBus;
using System;
using System.Threading.Tasks;

namespace LagoVista.Core.Networking.AsyncMessaging
{
    /// <summary>
    /// This class is used by ServiceBusRequestModerator to route the response back to the requestor via service bus.
    /// This class lives on-premises.
    /// </summary>
    public sealed class ServiceBusAsyncResponseSender: IAsyncResponseHandler
    {
        private readonly ILogger _logger;
        private readonly TopicClient _topicClient;

        public ServiceBusAsyncResponseSender(IServiceBusAsyncResponseSenderConnectionSettings settings, ILogger logger) : base()
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // Endpoint - Name
            // SharedAccessKeyName - UserName
            // SharedAccessKey - AccessKey
            // DestinationEntityPath - ResourceName
            var senderConnectionString = $"Endpoint=sb://{settings.ServiceBusAsyncResponseSender.Name}.servicebus.windows.net/;SharedAccessKeyName={settings.ServiceBusAsyncResponseSender.UserName};SharedAccessKey={settings.ServiceBusAsyncResponseSender.AccessKey};";
            var destinationEntityPath = settings.ServiceBusAsyncResponseSender.ResourceName;
            
            //todo: ML - need to set retry policy and operation timeout etc.
            _topicClient = new TopicClient(senderConnectionString, destinationEntityPath, null);
        }

        public async Task HandleResponse(IAsyncResponse response)
        {
            try
            {
                // package response in service bus message and send to topic
                var message = new Message(response.Payload)
                {
                    Label = response.Path,
                    ContentType = response.GetType().FullName,
                    MessageId = response.Id,
                    CorrelationId = response.CorrelationId
                };
                await _topicClient.SendAsync(message);
            }
            catch(Exception ex)
            {
                //todo: ML - log exception
                throw;
            }
        }
    }
}
