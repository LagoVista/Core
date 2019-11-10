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

namespace LagoVista.Core.Rpc.Server.ServiceBus
{
    public sealed class ServiceBusRequestServer : AbstractRequestServer
    {
        #region Fields        
        private IConnectionSettings _subscriberSettings;
        private string _transmitterConnectionString;
        private SubscriptionClient _subscriptionClient;
        #endregion

        public ServiceBusRequestServer(IRequestBroker requestBroker, ILogger logger) :
            base(requestBroker, logger)
        {

            // Endpoint - AccountId
            // SharedAccessKeyName - UserName
            // SharedAccessKey - AccessKey
            // DestinationEntityPath - ResourceName
        }

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

                        await ReceiveAsync(new Request(decompressedStream.ToArray()));
                        await _subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);                        
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.GetType().Name}: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                await _subscriptionClient.DeadLetterAsync(message.SystemProperties.LockToken, ex.GetType().FullName, ex.Message);
                throw;
            }
        }

        protected override void ConfigureSettings(ITransceiverConnectionSettings connectionSettings)
        {
            _subscriberSettings = connectionSettings.RpcServerReceiver;

            var transmitterSettings = connectionSettings.RpcServerTransmitter;
            _transmitterConnectionString = $"Endpoint=sb://{transmitterSettings.AccountId}.servicebus.windows.net/;SharedAccessKeyName={transmitterSettings.UserName};SharedAccessKey={transmitterSettings.AccessKey};";
        }

        protected override void UpdateSettings(ITransceiverConnectionSettings connectionSettings)
        {
            _subscriberSettings = connectionSettings.RpcServerReceiver;

            var transmitterSettings = connectionSettings.RpcServerTransmitter;
            _transmitterConnectionString = $"Endpoint=sb://{transmitterSettings.AccountId}.servicebus.windows.net/;SharedAccessKeyName={transmitterSettings.UserName};SharedAccessKey={transmitterSettings.AccessKey};";           

            Restart();
        }    

        private Task HandleException(ExceptionReceivedEventArgs e)
        {
            Console.WriteLine($"{e.Exception.GetType().Name}: {e.Exception.Message}");
            Console.WriteLine(e.Exception.StackTrace);
            //todo: ML - replace sample code from SbListener with appropriate error handling.
            // await StateChanged(Deployment.Admin.Models.PipelineModuleStatus.FatalError);
            //SendNotification(Runtime.Core.Services.Targets.WebSocket, $"Exception Starting Service Bus Listener at : {_listenerConfiguration.HostName}/{_listenerConfiguration.Queue} {ex.Exception.Message}");
            //LogException("AzureServiceBusListener_Listen", ex.Exception);
            return Task.FromResult<object>(null);
        }

        private async void Restart()
        {
            await _subscriptionClient.CloseAsync();
            await CustomStartAsync();
        }

        protected override Task CustomStartAsync()
        {
            // Endpoint - AccountId
            // SharedAccessKeyName - UserName
            // SharedAccessKey - AccessKey
            // SourceEntityPath - ResourceName
            // SubscriptionPath - Uri
            var parts = _subscriberSettings.ResourceName.Split('/');
            if (parts.Length != 2) throw new Exception("Resource Name for Server Listener must be [TOPIC]/[SUBSCRIPTION]");

            var topic = parts[0];
            var subscrption = parts[1];

            var connectionBuilder = new ServiceBusConnectionStringBuilder($"Endpoint={_subscriberSettings.Uri}")
            {
                EntityPath = topic,
                SasToken = _subscriberSettings.AccessKey,
            };

            _subscriptionClient = new SubscriptionClient(connectionBuilder, subscrption, ReceiveMode.PeekLock, new RetryExponential(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(30), 10));

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

            return Task.FromResult(default(object));
        }

        protected override async Task CustomTransmitMessageAsync(IMessage message)
        {
            // no need to create topic - we wouldn't even be here if the other side hadn't done it's part
            //new RetryExponential(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(30), 10)
            var topicClient = new TopicClient(_transmitterConnectionString, message.ReplyPath.ToLower(), null);
            try
            {
                using(var ms = new MemoryStream(message.Payload))
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
            catch (Exception ex)
            {
                _logger.AddException("Custom Transmit Message", ex, message.DestinationPath.ToKVP("destinationPath"));
                throw;
            }
            finally
            {
                await topicClient.CloseAsync();
            }
        }
    }
}