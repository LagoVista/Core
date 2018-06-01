using LagoVista.Core.PlatformSupport;
using Microsoft.Azure.ServiceBus;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Core.Networking.AsyncMessaging
{
    /// <summary>
    /// Listens to service bus subscription for async requests, 
    /// invokes requests through the request broker, 
    /// returns responses to the requestor via service bus (response sender).
    /// This class lives response side.
    /// </summary>
    public sealed class ServiceBusAsyncRequestModerator : IAsyncRequestListener
    {
        private readonly IAsyncResponseHandler _responseSender;
        private readonly IAsyncRequestBroker _requestBroker;
        private readonly ILogger _logger;
        private readonly SubscriptionClient _subscriptionClient;

        public ServiceBusAsyncRequestModerator(
            IServiceBusAsyncRequestModeratorConnectionSettings settings,
            IAsyncRequestBroker requestBroker,
            IAsyncResponseHandler responseSender, 
            ILogger logger)
        {
            _responseSender = responseSender ?? throw new ArgumentNullException(nameof(responseSender));
            _requestBroker = requestBroker ?? throw new ArgumentNullException(nameof(requestBroker));
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // Endpoint - AccountId
            // SharedAccessKeyName - UserName
            // SharedAccessKey - AccessKey
            // SourceEntityPath - ResourceName
            // SubscriptionPath - Uri
            var receiverConnectionString = $"Endpoint=sb://{settings.ServiceBusAsyncRequestModerator.AccountId}.servicebus.windows.net/;SharedAccessKeyName={settings.ServiceBusAsyncRequestModerator.UserName};SharedAccessKey={settings.ServiceBusAsyncRequestModerator.AccessKey};";
            var sourceEntityPath = settings.ServiceBusAsyncRequestModerator.ResourceName;
            var subscriptionPath = settings.ServiceBusAsyncRequestModerator.Uri;

            //todo: ML - need to set retry policy and operation timeout etc.
            _subscriptionClient = new SubscriptionClient(receiverConnectionString, sourceEntityPath, subscriptionPath, ReceiveMode.PeekLock, null);
        }

        public void Start()
        {
            var options = new MessageHandlerOptions(HandleException)
            {
                AutoComplete = false,
#if DEBUG
                MaxConcurrentCalls = 1,
#else
                MaxConcurrentCalls = 100,
#endif
            };
            _subscriptionClient.RegisterMessageHandler(MessageReceived, options);
        }

        private async Task MessageReceived(Message message, CancellationToken cancelationToken)
        {
            try
            {
                var request = new AsyncRequest(message.Body);
                var response = await _requestBroker.HandleRequestAsync(request);
                await _responseSender.HandleResponse(response);
                await _subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
            }
            catch (Exception ex)
            {
                await _subscriptionClient.DeadLetterAsync(message.SystemProperties.LockToken, ex.GetType().FullName, ex.Message);
                throw;
            }
        }

        private async Task HandleException(ExceptionReceivedEventArgs e)
        {
            //todo: ML - replace sample code from SbListener with appropriate error handling.
            // await StateChanged(Deployment.Admin.Models.PipelineModuleStatus.FatalError);
            //SendNotification(Runtime.Core.Services.Targets.WebSocket, $"Exception Starting Service Bus Listener at : {_listenerConfiguration.HostName}/{_listenerConfiguration.Queue} {ex.Exception.Message}");
            //LogException("AzureServiceBusListener_Listen", ex.Exception);
        }
    }
}