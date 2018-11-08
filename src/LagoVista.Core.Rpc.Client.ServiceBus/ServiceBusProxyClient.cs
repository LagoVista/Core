using LagoVista.Core.Interfaces;
using LagoVista.Core.PlatformSupport;
using LagoVista.Core.Rpc.Messages;
using LagoVista.Core.Rpc.Settings;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Core.Rpc.Client.ServiceBus
{
    public sealed class ServiceBusProxyClient : AbstractProxyClient
    {
        #region Fields
        private readonly IConnectionSettings _topicConstructorSettings;
        private readonly IConnectionSettings _receiverSettings;
        private readonly string _topicConnectionString;
        private readonly string _destinationEntityPath;
        private SubscriptionClient _subscriptionClient;
        ILogger _logger;
        #endregion

        private async Task CreateTopicAsync(string entityPath)
        {
            var connstr = $"Endpoint=sb://{_receiverSettings.AccountId}.servicebus.windows.net/;SharedAccessKeyName={_topicConstructorSettings.UserName};SharedAccessKey={_topicConstructorSettings.AccessKey};";

            var client = new ManagementClient(connstr);
            if (!await client.TopicExistsAsync(entityPath))
            {
                await client.CreateTopicAsync(entityPath);
                await client.CreateSubscriptionAsync(entityPath, "application");
                await client.CreateSubscriptionAsync(entityPath, "admin");
            }
            //return Task.FromResult<object>(null);
        }

        #region Constructors

        public ServiceBusProxyClient(
            ITransceiverConnectionSettings connectionSettings,
            IAsyncCoupler<IMessage> asyncCoupler,
            ILogger logger) :
            base(connectionSettings, asyncCoupler, logger)
        {
            _topicConstructorSettings = connectionSettings.RpcTopicConstructor;
            _receiverSettings = connectionSettings.RpcReceiver;
            _logger = logger;

            // Endpoint - AccountId
            // SharedAccessKeyName - UserName
            // SharedAccessKey - AccessKey
            // DestinationEntityPath - ResourceName
            var topicSettings = connectionSettings.RpcTransmitter;
            _topicConnectionString = $"Endpoint=sb://{topicSettings.AccountId}.servicebus.windows.net/;SharedAccessKeyName={topicSettings.UserName};SharedAccessKey={topicSettings.AccessKey};";
            _destinationEntityPath = topicSettings.ResourceName;
        }

        #endregion

        #region rpc Receiver Methods

        protected override async Task CustomStartAsync()
        {
            // Endpoint - AccountId
            // SharedAccessKeyName - UserName
            // SharedAccessKey - AccessKey
            // SourceEntityPath - ResourceName
            // SubscriptionPath - Uri
            var receiverConnectionString = $"Endpoint=sb://{_receiverSettings.AccountId}.servicebus.windows.net/;SharedAccessKeyName={_receiverSettings.UserName};SharedAccessKey={_receiverSettings.AccessKey};";
            var sourceEntityPath = _receiverSettings.ResourceName;
            var subscriptionPath = _receiverSettings.Uri;
            
            await CreateTopicAsync(sourceEntityPath);
            //new RetryExponential(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(30), 10)
            _subscriptionClient = new SubscriptionClient(receiverConnectionString, sourceEntityPath, subscriptionPath, ReceiveMode.PeekLock, null);

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

        #endregion

        #region ServiceBus Subscription Methods

        private async Task MessageReceived(Microsoft.Azure.ServiceBus.Message message, CancellationToken cancelationToken)
        {
            try
            {
                var response = new Response(message.Body);
                await ReceiveAsync(response);
                await _subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
            }
            catch (Exception ex)
            {
                message.Label = ex.Message;
                await _subscriptionClient.DeadLetterAsync(message.SystemProperties.LockToken, ex.GetType().FullName, ex.Message);
                throw;
            }
        }

        private Task HandleException(ExceptionReceivedEventArgs e)
        {
            //todo: ML - replace sample code from SbListener with appropriate error handling.
            // await StateChanged(Deployment.Admin.Models.PipelineModuleStatus.FatalError);
            //SendNotification(Runtime.Core.Services.Targets.WebSocket, $"Exception Starting Service Bus Listener at : {_listenerConfiguration.HostName}/{_listenerConfiguration.Queue} {ex.Exception.Message}");
            //LogException("AzureServiceBusListener_Listen", ex.Exception);
            return Task.FromResult<object>(null);
        }

        #endregion

        #region Rpc Transmitter Methods

        protected override async Task CustomTransmitMessageAsync(IMessage message)
        {
            var entityPath = $"{_destinationEntityPath}_{message.InstanceId}"
                .Replace("__", "_")
                .ToLower();

            await CreateTopicAsync(entityPath);

            var topicClient = new TopicClient(_topicConnectionString, entityPath, new RetryExponential(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(30), 10));
            try
            {
                // package response in service bus message and send to topic
                var messageOut = new Microsoft.Azure.ServiceBus.Message(message.Payload)
                {
                    ContentType = message.GetType().FullName,
                    MessageId = message.Id,
                    CorrelationId = message.CorrelationId,
                    To = message.DestinationPath,
                    ReplyTo = message.ReplyPath,
                    Label = message.DestinationPath,
                };
                await topicClient.SendAsync(messageOut);
            }
            catch //(Exception ex)
            {
                //todo: ML - log exception
                throw;
            }
            finally
            {
                await topicClient.CloseAsync();
            }
        }

        #endregion
    }
}

