using LagoVista.Core.Interfaces;
using LagoVista.Core.PlatformSupport;
using Microsoft.Azure.ServiceBus;
using System;
using System.Threading.Tasks;

namespace LagoVista.Core.Networking.AsyncMessaging
{
    public sealed class ServiceBusResponseHandler: IAsyncResponseHandler
    {
        private readonly IConnectionSettings _connectionSettings;
        private readonly ILogger _logger;
        private readonly TopicClient _topicClient;

        public ServiceBusResponseHandler(IConnectionSettings connectionSettings, ILogger logger) : base()
        {
            _connectionSettings = connectionSettings ?? throw new ArgumentNullException("connectionSettings");
            _logger = logger ?? throw new ArgumentNullException("logger");

            var senderConnectionString = "Endpoint=sb://localrequestbus-dev.servicebus.windows.net/;SharedAccessKeyName=SendAccessKey;SharedAccessKey=B2bGyjZVtiNsgQ/BvjCqwtk9FgCYGdA7np99etWzHLc=;";
            var destinationEntityPath = "9e88c7f6b5894dbfb3bc09d20736705e_fromlocal";
            //todo: ML - need to set retry policy and operation timeout etc.
            _topicClient = new TopicClient(senderConnectionString, destinationEntityPath, null);
        }

        public async Task HandleResponse(IAsyncResponse response)
        {
            // package response in service bus message and send to topic
            var messageOut = new Message(response.MarshalledData)
            {
                Label = response.Path,
                ContentType = typeof(IAsyncResponse).FullName,
                MessageId = response.Id,
                CorrelationId = response.CorrelationId
            };
            await _topicClient.SendAsync(messageOut);
        }
    }
}
