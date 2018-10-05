using LagoVista.Core.Interfaces;
using LagoVista.Core.PlatformSupport;
using LagoVista.Core.Rpc.Messages;
using LagoVista.Core.Rpc.Settings;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Core.Rpc.Server.ServiceBus
{
    public sealed class ServiceBusRequestServer : AbstractRequestServer
    {
        #region Fields        
        private readonly IConnectionSettings _topicConstructorSettings;
        private readonly IConnectionSettings _receiverSettings;
        private readonly string _topicConnectionString;
        private readonly string _destinationEntityPath;
        private SubscriptionClient _subscriptionClient;
        #endregion

        private Task CreateTopicAsync(string entityPath)
        {
            //var connstr = $"Endpoint=sb://{_receiverSettings.AccountId}.servicebus.windows.net/;SharedAccessKeyName={_topicConstructorSettings.UserName};SharedAccessKey={_topicConstructorSettings.AccessKey};";

            //var client = new ManagementClient(connstr);
            //if (!await client.TopicExistsAsync(entityPath))
            //{
            //    await client.CreateTopicAsync(entityPath);
            //    await client.CreateSubscriptionAsync(entityPath, "application");
            //    await client.CreateSubscriptionAsync(entityPath, "admin");
            //}
            return Task.FromResult<object>(null);
        }

        public ServiceBusRequestServer(ITransceiverConnectionSettings connectionSettings, IRequestBroker requestBroker, ILogger logger) :
            base(connectionSettings, requestBroker, logger)
        {
            _topicConstructorSettings = connectionSettings.RpcTopicConstructor;
            _receiverSettings = connectionSettings.RpcReceiver;

            // Endpoint - AccountId
            // SharedAccessKeyName - UserName
            // SharedAccessKey - AccessKey
            // DestinationEntityPath - ResourceName
            var topicSettings = connectionSettings.RpcTransmitter;
            _topicConnectionString = $"Endpoint=sb://{topicSettings.AccountId}.servicebus.windows.net/;SharedAccessKeyName={topicSettings.UserName};SharedAccessKey={topicSettings.AccessKey};";
            _destinationEntityPath = topicSettings.ResourceName;
        }

        private async Task MessageReceived(Microsoft.Azure.ServiceBus.Message message, CancellationToken cancelationToken)
        {
            try
            {
                await ReceiveAsync(new Request(message.Body));
                await _subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
            }
            catch (Exception ex)
            {
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

        protected override async Task CustomStartAsync()
        {
            // Endpoint - AccountId
            // SharedAccessKeyName - UserName
            // SharedAccessKey - AccessKey
            // SourceEntityPath - ResourceName
            // SubscriptionPath - Uri
            var receiverConnectionString = $"Endpoint=sb://{_receiverSettings.AccountId}.servicebus.windows.net/;SharedAccessKeyName={_receiverSettings.UserName};SharedAccessKey={_receiverSettings.AccessKey};";
            var sourceEntityPath = _receiverSettings.ResourceName;
            var subscriptionPath = "application";

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

        protected override async Task CustomTransmitMessageAsync(IMessage message)
        {
            // no need to create topic - we wouldn't even be here if the other side hadn't done it's part
            //new RetryExponential(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(30), 10)
            var topicClient = new TopicClient(_topicConnectionString, message.ReplyPath.ToLower(), null);
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
            //catch (Exception ex)
            //{
            //    //todo: ML - log exception
            //    throw;
            //}
            finally
            {
                await topicClient.CloseAsync();
            }
        }
    }
}