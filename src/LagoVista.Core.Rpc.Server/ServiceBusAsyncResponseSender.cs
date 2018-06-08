using LagoVista.Core.PlatformSupport;
using Microsoft.Azure.ServiceBus;
using System;
using System.Threading.Tasks;

namespace LagoVista.Core.Networking.Rpc.ServiceBus.Server
{
    /// <summary>
    /// This class is used by ServiceBusRequestModerator to route the response back to the requestor via service bus.
    /// This class lives on-premises.
    /// </summary>
    public sealed class ServiceBusAsyncResponseSender : IAsyncResponseHandler
    {
        private readonly ILogger _logger;
        private readonly string _senderConnectionString;
        private readonly IServiceBusAsyncResponseSenderConnectionSettings _settings;

        public ServiceBusAsyncResponseSender(IServiceBusAsyncResponseSenderConnectionSettings settings, ILogger logger) : base()
        {
            Console.WriteLine("ServiceBusAsyncResponseSender::ctor");
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // Endpoint - AccountId
            // SharedAccessKeyName - UserName
            // SharedAccessKey - AccessKey
            // DestinationEntityPath - ResourceName
            _senderConnectionString = $"Endpoint=sb://{settings.ServiceBusAsyncResponseSender.AccountId}.servicebus.windows.net/;SharedAccessKeyName={settings.ServiceBusAsyncResponseSender.UserName};SharedAccessKey={settings.ServiceBusAsyncResponseSender.AccessKey};";
        }

        public async Task HandleResponse(IAsyncResponse response)
        {
            Console.WriteLine("sending response");

            //todo: ML - need to set retry policy and operation timeout etc. https://docs.microsoft.com/en-us/dotnet/api/microsoft.azure.servicebus.retrypolicy?view=azure-dotnet
            //todo: ML - to improve performance we could keep a concurent dictionary of topic clients hashed by reply path
            var topicClient = new TopicClient(_senderConnectionString, response.ReplyPath, null);
            try
            {
                // package response in service bus message and send to topic
                var message = new Microsoft.Azure.ServiceBus.Message(response.Payload)
                {
                    Label = response.DestinationPath,
                    ContentType = response.GetType().FullName,
                    MessageId = response.Id,
                    CorrelationId = response.CorrelationId
                };
                await topicClient.SendAsync(message);
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
