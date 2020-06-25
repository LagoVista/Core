using LagoVista.Core.Interfaces;
using LagoVista.Core.PlatformSupport;
using LagoVista.Core.Rpc.Messages;
using LagoVista.Core.Rpc.Settings;
using LagoVista.Core.Validation;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using System;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Core.Rpc.Client.ServiceBus
{
    /* 
     * The client is the component that is actually making requests of a remote server (which happens
     * to be some sort of IoT runtime instance)
     */
    public sealed class ServiceBusProxyClient : AbstractProxyClient
    {
        #region Fields
        private IConnectionSettings _topicConstructorSettings;
        private IConnectionSettings _subscriberSettings;
        private string _transmitterConnectionSettings;
        private string _serverTopicPrefix;
        private SubscriptionClient _subscriptionClient;
        #endregion

        private async Task CreateTopicAsync(string entityPath)
        {
            if(_topicConstructorSettings == null)
            {
                throw new ArgumentNullException(nameof(_topicConstructorSettings));
            }

            var connstr = $"Endpoint=sb://{_topicConstructorSettings.AccountId}.servicebus.windows.net/;SharedAccessKeyName={_topicConstructorSettings.UserName};SharedAccessKey={_topicConstructorSettings.AccessKey};";

            var client = new ManagementClient(connstr);
            if (!await client.TopicExistsAsync(entityPath))
            {
                await client.CreateTopicAsync(entityPath);
                await client.CreateSubscriptionAsync(entityPath, _subscriberSettings.Uri);
            }
        }

        #region Constructors

        public ServiceBusProxyClient(
            IAsyncCoupler<IMessage> asyncCoupler,
            ILogger logger) :
            base(asyncCoupler, logger)
        {
        }

        protected override void ConfigureSettings(ITransceiverConnectionSettings settings)
        {
            _topicConstructorSettings = settings.RpcAdmin;
            _subscriberSettings = settings.RpcClientReceiver;
         
            // Endpoint - AccountId
            // SharedAccessKeyName - UserName
            // SharedAccessKey - AccessKey
            // DestinationEntityPath - ResourceName
            var transmitterSettings = settings.RpcClientTransmitter;
            _transmitterConnectionSettings = $"Endpoint=sb://{transmitterSettings.AccountId}.servicebus.windows.net/;SharedAccessKeyName={transmitterSettings.UserName};SharedAccessKey={transmitterSettings.AccessKey};";
            _serverTopicPrefix = transmitterSettings.ResourceName;
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
            var receiverConnectionString = $"Endpoint=sb://{_subscriberSettings.AccountId}.servicebus.windows.net/;SharedAccessKeyName={_subscriberSettings.UserName};SharedAccessKey={_subscriberSettings.AccessKey};";
            var sourceEntityPath = _subscriberSettings.ResourceName;
            var subscriptionPath = _subscriberSettings.Uri;

            if (_topicConstructorSettings != null)
            {
                await CreateTopicAsync(sourceEntityPath);
            }

            //new RetryExponential(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(30), 10)
            _subscriptionClient = new SubscriptionClient(receiverConnectionString, sourceEntityPath, subscriptionPath, ReceiveMode.PeekLock, null);
            Console.WriteLine($"RPC Subscription Created {sourceEntityPath} - {subscriptionPath}");

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
                using (var compressedStream = new MemoryStream(message.Body))
                using (var decompressorStream = new DeflateStream(compressedStream, CompressionMode.Decompress))
                {
                    using (var decompressedStream = new MemoryStream())
                    {
                        decompressorStream.CopyTo(decompressedStream);

                        /* 
                         * it is possible that this may not be the client that asked for the RPC request
                         * but it could be the one that consumed the message.  If this is the case the 
                         * asyn coupler will say, nope I didn't ask for this and will not processe it.
                         * return it back to the pool to see if another client can handle it.
                         */ 
                        if((await ReceiveAsync(new Response(decompressedStream.ToArray()))).Successful) 
                            await _subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
                        else
                            await _subscriptionClient.AbandonAsync(message.SystemProperties.LockToken);
                    }
                }
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

        protected override async Task<InvokeResult> CustomTransmitMessageAsync(IMessage message)
        {
            var entityPath = $"{_serverTopicPrefix}_{message.InstanceId}".ToLower();

            await CreateTopicAsync(entityPath);

            Console.WriteLine($"Sending message async entity: {_transmitterConnectionSettings} - {entityPath} - {message.DestinationPath} - {message.Payload} - {message.ReplyPath}");

            var topicClient = new TopicClient(_transmitterConnectionSettings, entityPath, new RetryExponential(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(30), 10));
            try
            {
                using (var ms = new MemoryStream(message.Payload))
                using (var mso = new MemoryStream())
                {
                    using (var ds = new DeflateStream(mso, CompressionMode.Compress))
                    {
                        ms.CopyTo(ds);
                    }

                    var buffer = mso.ToArray();

                    var messageOut = new Microsoft.Azure.ServiceBus.Message(buffer)
                    {
                        ContentType = message.GetType().FullName,
                        MessageId = message.Id,
                        CorrelationId = message.CorrelationId,
                        To = message.DestinationPath,
                        ReplyTo = message.ReplyPath,
                        Label = message.DestinationPath,
                    };

                    await topicClient.SendAsync(messageOut);
                    return InvokeResult.Success;
                }
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

