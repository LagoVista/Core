using LagoVista.Core.PlatformSupport;
using LagoVista.Core.Rpc.Messages;
using LagoVista.Core.Rpc.Settings;
using Microsoft.Azure.ServiceBus;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Core.Rpc.Server.ServiceBus
{
    public sealed class ServiceBusRequestServer : AbstractRequestServer
    {
        #region Fields
        private readonly string _topicConnectionString;
        private readonly string _destinationEntityPath;
        private readonly SubscriptionClient _subscriptionClient;
        #endregion

        public ServiceBusRequestServer(ITransceiverConnectionSettings connectionSettings, IRequestBroker requestBroker, ILogger logger) :
            base(connectionSettings, requestBroker, logger)
        {
            // Endpoint - AccountId
            // SharedAccessKeyName - UserName
            // SharedAccessKey - AccessKey
            // DestinationEntityPath - ResourceName
            var topicSettings = connectionSettings.RpcTransmitter;
            _topicConnectionString = $"Endpoint=sb://{topicSettings.AccountId}.servicebus.windows.net/;SharedAccessKeyName={topicSettings.UserName};SharedAccessKey={topicSettings.AccessKey};";
            _destinationEntityPath = topicSettings.ResourceName;

            // Endpoint - AccountId
            // SharedAccessKeyName - UserName
            // SharedAccessKey - AccessKey
            // SourceEntityPath - ResourceName
            // SubscriptionPath - Uri
            var subscriptionSettings = connectionSettings.RpcReceiver;
            var receiverConnectionString = $"Endpoint=sb://{subscriptionSettings.AccountId}.servicebus.windows.net/;SharedAccessKeyName={subscriptionSettings.UserName};SharedAccessKey={subscriptionSettings.AccessKey};";
            var sourceEntityPath = subscriptionSettings.ResourceName;
            var subscriptionPath = subscriptionSettings.Uri;

            //todo: ML - need to set retry policy and operation timeout etc.
            _subscriptionClient = new SubscriptionClient(receiverConnectionString, sourceEntityPath, subscriptionPath, ReceiveMode.PeekLock, null);
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

        private async Task HandleException(ExceptionReceivedEventArgs e)
        {
            //todo: ML - replace sample code from SbListener with appropriate error handling.
            // await StateChanged(Deployment.Admin.Models.PipelineModuleStatus.FatalError);
            //SendNotification(Runtime.Core.Services.Targets.WebSocket, $"Exception Starting Service Bus Listener at : {_listenerConfiguration.HostName}/{_listenerConfiguration.Queue} {ex.Exception.Message}");
            //LogException("AzureServiceBusListener_Listen", ex.Exception);
        }

        protected override void CustomStart()
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

        protected override async Task CustomTransmitMessageAsync(IMessage message)
        {
            //todo: ML - if service bus topic and subscription doesn't exist, then create it: https://github.com/Azure-Samples/service-bus-dotnet-management/tree/master/src/service-bus-dotnet-management

            //todo: ML - need to set retry policy and operation timeout etc.
            var topicClient = new TopicClient(_topicConnectionString, message.ReplyPath, null);
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