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
        private readonly string _topicConnectionString;
        private readonly string _destinationEntityPath;

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
            _topicConnectionString = $"Endpoint=sb://{settings.ServiceBusAsyncRequestSender.Name}.servicebus.windows.net/;SharedAccessKeyName={settings.ServiceBusAsyncRequestSender.UserName};SharedAccessKey={settings.ServiceBusAsyncRequestSender.AccessKey};";
            _destinationEntityPath = settings.ServiceBusAsyncRequestSender.ResourceName;
        }

        public async Task HandleRequest(IAsyncRequest request, string destination)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (string.IsNullOrEmpty(destination)) throw new ArgumentNullException(nameof(destination));

            //todo: ML - need to set retry policy and operation timeout etc.
            var topicClient = new TopicClient(_topicConnectionString, _destinationEntityPath.Replace("[instance_id]", destination), null);
            try
            {
                // package response in service bus message and send to topic
                var messageOut = new Message(request.Payload)
                {
                    Label = request.Path,
                    ContentType = request.GetType().FullName,
                    MessageId = request.Id,
                    CorrelationId = request.CorrelationId
                };
                await topicClient.SendAsync(messageOut);
            }
            catch (Exception ex)
            {
                //todo: ML - log exception
                throw;
            }
            finally
            {
                await topicClient.CloseAsync();
            }
        }
    }
}
