using LagoVista.Core.Interfaces;
using LagoVista.Core.PlatformSupport;
using Microsoft.Azure.ServiceBus;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Core.Networking.AsyncMessaging
{
    /// <summary>
    /// Listens to service bus for responses to requests handled on-premises by ServiceBusRequestModerator. 
    /// When a response is received it is routed to the class waiting via the AsyncCoupler registry.
    /// This is done by calling CompleteAsync() on the coupler.
    /// This class lives where ever NuvIoT core management rest api is hosted.
    /// </summary>
    public class ServiceBusAsyncResponseListener : IAsyncResponseListener
    {
        private readonly SubscriptionClient _subscriptionClient;
        private readonly IAsyncCoupler<IAsyncResponse> _asyncCoupler;
        private readonly ILogger _logger;

        public ServiceBusAsyncResponseListener(
            IAsyncCoupler<IAsyncResponse> asyncCoupler,
            IServiceBusAsyncResponseListenerConnectionSettings settings,
            ILogger logger)
        {
            _asyncCoupler = asyncCoupler ?? throw new ArgumentNullException(nameof(asyncCoupler));
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // Endpoint - Name
            // SharedAccessKeyName - UserName
            // SharedAccessKey - AccessKey
            // SourceEntityPath - ResourceName
            // SubscriptionPath - Uri
            var receiverConnectionString = $"Endpoint=sb://{settings.ServiceBusAsyncResponseListener.Name}.servicebus.windows.net/;SharedAccessKeyName={settings.ServiceBusAsyncResponseListener.UserName};SharedAccessKey={settings.ServiceBusAsyncResponseListener.AccessKey};";
            var sourceEntityPath = settings.ServiceBusAsyncResponseListener.ResourceName;
            var subscriptionPath = settings.ServiceBusAsyncResponseListener.Uri;

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
                var response = new AsyncResponse(message.Body);
                var invokeResult = await _asyncCoupler.CompleteAsync(response.CorrelationId, response);
                if (!invokeResult.Successful)
                {
                    //todo: ML - handle unsuccessful attempt to complete the request
                }
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
            //todo: ML - need to pass exception back to azure side proxy
            //throw new NotImplementedException();
        }
    }
}
