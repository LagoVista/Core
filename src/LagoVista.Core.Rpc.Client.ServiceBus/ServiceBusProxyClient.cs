using LagoVista.Core.Interfaces;
using LagoVista.Core.PlatformSupport;
using LagoVista.Core.Rpc.Messages;
using LagoVista.Core.Rpc.Settings;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using System;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Core.Rpc.Client.ServiceBus
{
    public sealed class ServiceBusProxyClient : AbstractProxyClient
    {
        #region Fields
        private IConnectionSettings _topicConstructorSettings;
        private IConnectionSettings _receiverSettings;
        private string _transmitterConnectionSettings;
        private string _serverTopicPrefix;
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
        }

        #region Constructors

        public ServiceBusProxyClient(
            IAsyncCoupler<IMessage> asyncCoupler,
            ILogger logger) :
            base(asyncCoupler, logger)
        {
            _logger = logger;

        }

        protected override void ConfigureSettings(ITransceiverConnectionSettings settings)
        {
            _topicConstructorSettings = settings.RpcAdmin;
            _receiverSettings = settings.RpcClientReceiver;
         
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
                using (var compressedStream = new MemoryStream(message.Body))
                using (var decompressorStream = new DeflateStream(compressedStream, CompressionMode.Decompress))
                {
                    using (var decompressedStream = new MemoryStream())
                    {
                        decompressorStream.CopyTo(decompressedStream);

                        await ReceiveAsync(new Response(decompressedStream.ToArray()));
                        await _subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
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

        protected override async Task CustomTransmitMessageAsync(IMessage message)
        {
            var entityPath = $"{_serverTopicPrefix}_{message.InstanceId}"
                .Replace("__", "_")
                .ToLower();

            await CreateTopicAsync(entityPath);

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

                    Console.Write($"Original message: {message.Payload.Length}  Compressed: {buffer.Length}");

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

