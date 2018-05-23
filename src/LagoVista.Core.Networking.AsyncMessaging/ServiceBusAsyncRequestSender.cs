using LagoVista.Core.PlatformSupport;
using Microsoft.Azure.ServiceBus;
using System;
using System.Threading.Tasks;

namespace LagoVista.Core.Networking.AsyncMessaging
{
    public class ServiceBusAsyncRequestSender : IAsyncRequestHandler
    {
        private readonly ISenderConnectionSettings _connectionSettings;
        private readonly ILogger _logger;
        private readonly TopicClient _topicClient;
        public ServiceBusAsyncRequestSender(ISenderConnectionSettings connectionSettings, ILogger logger)
        {
            _connectionSettings = connectionSettings ?? throw new ArgumentNullException("connectionSettings");
            _logger = logger ?? throw new ArgumentNullException("logger");

            var senderConnectionString = connectionSettings.ServiceBusConnectionString; // "Endpoint=sb://localrequestbus-dev.servicebus.windows.net/;SharedAccessKeyName=SendAccessKey;SharedAccessKey=B2bGyjZVtiNsgQ/BvjCqwtk9FgCYGdA7np99etWzHLc=;";
            var destinationEntityPath = connectionSettings.DestinationEntityPath; // "9e88c7f6b5894dbfb3bc09d20736705e_fromlocal";
            //todo: ML - need to set retry policy and operation timeout etc.
            _topicClient = new TopicClient(senderConnectionString, destinationEntityPath, null);
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
