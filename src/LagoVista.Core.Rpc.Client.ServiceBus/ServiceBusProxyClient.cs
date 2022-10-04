using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using LagoVista.Core.Interfaces;
using LagoVista.Core.PlatformSupport;
using LagoVista.Core.Rpc.Messages;
using LagoVista.Core.Rpc.Settings;
using LagoVista.Core.Validation;
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
        private ServiceBusClient _senderClient;
        private ServiceBusSender _sender;

        private ServiceBusClient _processorClient;
        private ServiceBusProcessor _processor;

        #endregion

        private async Task CreateTopicAsync(string entityPath)
        {
            if (_topicConstructorSettings == null)
            {
                throw new ArgumentNullException(nameof(_topicConstructorSettings));
            }

            var connstr = $"Endpoint=sb://{_topicConstructorSettings.AccountId}.servicebus.windows.net/;SharedAccessKeyName={_topicConstructorSettings.UserName};SharedAccessKey={_topicConstructorSettings.AccessKey};";

            var client = new ServiceBusAdministrationClient(connstr);
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

            var clientOptions = new ServiceBusClientOptions() { TransportType = ServiceBusTransportType.AmqpWebSockets };
            _processorClient = new ServiceBusClient(receiverConnectionString, clientOptions);

            await CreateTopicAsync(_subscriberSettings.ResourceName);

            _processor = _processorClient.CreateProcessor(_subscriberSettings.ResourceName, _subscriberSettings.Uri);
            _processor.ProcessMessageAsync += _processor_ProcessMessageAsync; ;
            _processor.ProcessErrorAsync += _processor_ProcessErrorAsync; ;
            Console.WriteLine($"[ClientListenerStarting] {_subscriberSettings.ResourceName}  {_subscriberSettings.Uri}");
            await _processor.StartProcessingAsync();
            Console.WriteLine($"[ClientListenerStarted] {_subscriberSettings.ResourceName}  {_subscriberSettings.Uri}");
        }

        private Task _processor_ProcessErrorAsync(ProcessErrorEventArgs arg)
        {
            //todo: ML - replace sample code from SbListener with appropriate error handling.
            // await StateChanged(Deployment.Admin.Models.PipelineModuleStatus.FatalError);
            //SendNotification(Runtime.Core.Services.Targets.WebSocket, $"Exception Starting Service Bus Listener at : {_listenerConfiguration.HostName}/{_listenerConfiguration.Queue} {ex.Exception.Message}");
            //LogException("AzureServiceBusListener_Listen", ex.Exception);
            return Task.FromResult<object>(null);
        }

        private async Task _processor_ProcessMessageAsync(ProcessMessageEventArgs arg)
        {
            try
            {
                /* 
                 * it is possible that this may not be the client that asked for the RPC request
                 * but it could be the one that consumed the message.  If this is the case the 
                 * asyn coupler will say, nope I didn't ask for this and will not processe it.
                 * return it back to the pool to see if another client can handle it.
                 */

                Console.WriteLine($"[ReceivedOnClient]");

                using (var compressedStream = new MemoryStream(arg.Message.Body.ToArray()))
                using (var decompressorStream = new DeflateStream(compressedStream, CompressionMode.Decompress))
                using (var decompressedStream = new MemoryStream())
                {
                    decompressorStream.CopyTo(decompressedStream);
                    if ((await ReceiveAsync(new Response(decompressedStream.ToArray()))).Successful)
                    {
                        await arg.CompleteMessageAsync(arg.Message);
                        Console.WriteLine($"[Success Processed]");
                    }
                    else
                    {
                        await arg.AbandonMessageAsync(arg.Message);
                        Console.WriteLine($"[Failed Processed]");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Dead Letter] {arg.Message} {ex.Message}");
                await arg.DeadLetterMessageAsync(arg.Message, ex.Message);
                throw;
            }
        }

        #endregion

        #region Rpc Transmitter Methods

        protected override async Task<InvokeResult> CustomTransmitMessageAsync(IMessage message)
        {
            var entityPath = $"{_serverTopicPrefix}_{message.InstanceId}".ToLower();

            await CreateTopicAsync(entityPath);

            var clientOptions = new ServiceBusClientOptions() { TransportType = ServiceBusTransportType.AmqpWebSockets };
            _senderClient = new ServiceBusClient(_transmitterConnectionSettings, clientOptions);
            _sender = _senderClient.CreateSender(entityPath);

            Console.WriteLine($"[SendingFromClient]: {entityPath}");

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
                    var sbMsg = new ServiceBusMessage(buffer);
                    sbMsg.ContentType = message.GetType().FullName;
                    sbMsg.CorrelationId = message.CorrelationId;
                    sbMsg.To = message.DestinationPath;
                    sbMsg.ReplyTo = message.ReplyPath;
                    sbMsg.Subject = message.DestinationPath;
                    sbMsg.MessageId = Guid.NewGuid().ToId();
                    await _sender.SendMessageAsync(sbMsg);
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
                await _sender.DisposeAsync();
                await _senderClient.DisposeAsync();
            }
        }

        #endregion
    }
}

