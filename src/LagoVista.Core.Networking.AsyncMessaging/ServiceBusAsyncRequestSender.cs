using LagoVista.Core.PlatformSupport;
using Microsoft.Azure.ServiceBus;
using System;
using System.Threading.Tasks;

namespace LagoVista.Core.Networking.AsyncMessaging
{
    /// <summary>
    /// "handles" requests by sending them to service bus. They get picked up on "the other side" by 
    /// ServiceBusAsyncRequestModerator which routes requests and returns responses via service bus.
    /// This class lives where ever NuvIoT core management rest api is hosted.
    /// </summary>
    public class ServiceBusAsyncRequestSender : IAsyncRequestHandler
    {
        private readonly ILogger _logger;
        private readonly TopicClient _topicClient;
        public ServiceBusAsyncRequestSender(IServiceBusAsyncRequestSenderConnectionSettings settings, ILogger logger)
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
            var topicConnectionString = $"Endpoint=sb://{settings.ServiceBusAsyncRequestSender.Name}.servicebus.windows.net/;SharedAccessKeyName={settings.ServiceBusAsyncRequestSender.UserName};SharedAccessKey={settings.ServiceBusAsyncRequestSender.AccessKey};";
            var destinationEntityPath = settings.ServiceBusAsyncRequestSender.ResourceName;

            //todo: ML - need to set retry policy and operation timeout etc.
            _topicClient = new TopicClient(topicConnectionString, destinationEntityPath, null);
        }

        public async Task HandleRequest(IAsyncRequest request)
        {
            try
            {
                // package response in service bus message and send to topic
                var messageOut = new Message(request.MarshalledData)
                {
                    Label = request.Path,
                    ContentType = request.GetType().FullName,
                    MessageId = request.Id,
                    CorrelationId = request.CorrelationId
                };
                await _topicClient.SendAsync(messageOut);
            }
            catch (Exception ex)
            {
                //todo: ML - log exception
                throw;
            }
        }
    }
}
