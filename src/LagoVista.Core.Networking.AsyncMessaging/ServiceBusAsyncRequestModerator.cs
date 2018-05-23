using LagoVista.Core.PlatformSupport;
using Microsoft.Azure.ServiceBus;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Core.Networking.AsyncMessaging
{
    public sealed class ServiceBusAsyncRequestModerator : IAsyncRequestListener
    {
        private readonly SubscriptionClient _subscriptionClient;
        private readonly IAsyncResponseHandler _responseHandler;
        private readonly IAsyncRequestBroker _requestBroker;
        private readonly IListenerConnectionSettings _connectionSettings;
        private readonly ILogger _logger;

        //todo: ML - replace iconnectionsettings with correct type
        public ServiceBusAsyncRequestModerator(IAsyncResponseHandler responseHandler, IAsyncRequestBroker requestBroker, IListenerConnectionSettings connectionSettings, ILogger logger)
        {
            _connectionSettings = connectionSettings ?? throw new ArgumentNullException("connectionSettings");
            _logger = logger ?? throw new ArgumentNullException("logger");
            _requestBroker = requestBroker ?? throw new ArgumentNullException("requestBroker");
            _responseHandler = responseHandler ?? throw new ArgumentNullException("sender");

            //var receiverConnectionString = "Endpoint=sb://localrequestbus-dev.servicebus.windows.net/;SharedAccessKeyName=ListenAccessKey;SharedAccessKey=mIzxiTinXIAtX0H2XknVj6LWvkDYCjv/PdOxNfmENd8=;";
            //var sourceEntityPath = "9e88c7f6b5894dbfb3bc09d20736705e_tolocal";
            //var subscriptionPath = "devSub";
            var receiverConnectionString = connectionSettings.ServiceBusConnectionString;
            var sourceEntityPath = connectionSettings.SourceEntityPath;
            var subscriptionPath = connectionSettings.SubscriptionPath;

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
                var asyncRequest = new AsyncRequest(message.Body);
                var asyncResponse = await _requestBroker.HandleRequestAsync(asyncRequest);
                await _responseHandler.HandleResponse(asyncResponse);
                await _subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
            }
            catch (Exception ex)
            {
                await _subscriptionClient.DeadLetterAsync(message.SystemProperties.LockToken, ex.GetType().FullName, ex.Message);
                throw;
            }
        }

        private async Task HandleException(ExceptionReceivedEventArgs arg)
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