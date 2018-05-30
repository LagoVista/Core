using LagoVista.Core.Interfaces;
using LagoVista.Core.PlatformSupport;
using Microsoft.Azure.ServiceBus;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Core.Networking.AsyncMessaging
{
    public class ServiceBusAsyncResponseListener : IAsyncResponseListener
    {
        private readonly SubscriptionClient _subscriptionClient;
        private readonly IAsyncCoupler<IAsyncResponse> _asyncCoupler;
        private readonly IListenerConnectionSettings _connectionSettings;
        private readonly ILogger _logger;

        public ServiceBusAsyncResponseListener(
            IAsyncCoupler<IAsyncResponse> asyncCoupler, 
            IListenerConnectionSettings connectionSettings, 
            ILogger logger)
        {
            _asyncCoupler = asyncCoupler ?? throw new ArgumentNullException(nameof(asyncCoupler));
            _connectionSettings = connectionSettings ?? throw new ArgumentNullException(nameof(connectionSettings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

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
                var response = new AsyncResponse(message.Body);
                await _asyncCoupler.CompleteAsync(response.CorrelationId, response);
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
