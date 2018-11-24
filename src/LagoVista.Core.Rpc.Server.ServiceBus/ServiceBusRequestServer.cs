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
        private readonly IConnectionSettings _topicConstructorSettings;
        private readonly IConnectionSettings _receiverSettings;
        private readonly string _topicConnectionString;
        private readonly string _destinationEntityPath;
        private SubscriptionClient _subscriptionClient;
        #endregion

        private async Task CreateTopicAsync(string entityPath)
        {
            var connstr = $"Endpoint=sb://{_receiverSettings.AccountId}.servicebus.windows.net/;SharedAccessKeyName={_topicConstructorSettings.UserName};SharedAccessKey={_topicConstructorSettings.AccessKey};";

            var client = new ManagementClient(connstr);
            if (!await client.TopicExistsAsync(entityPath))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Topic does not exist, creating now: " + entityPath);
                Console.ResetColor();
                var response = await client.CreateTopicAsync(entityPath);
                if (response == null)
                {
                    throw new Exception("Could not create topic.");
                }
                await client.CreateSubscriptionAsync(entityPath, "application");
                await client.CreateSubscriptionAsync(entityPath, "admin");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Topic Exists using: " + entityPath);
                Console.ResetColor();
            }
            //return Task.FromResult<object>(null);
        }

        public ServiceBusRequestServer(ITransceiverConnectionSettings connectionSettings, IRequestBroker requestBroker, ILogger logger) :
            base(connectionSettings, requestBroker, logger)
        {
            _topicConstructorSettings = connectionSettings.RpcAdmin;
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

            _subscriptionClient = new SubscriptionClient(receiverConnectionString, sourceEntityPath, subscriptionPath, ReceiveMode.PeekLock, new RetryExponential(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(30), 10));

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
            Console.WriteLine("WIRTING OUTPUT MESSAGE 2");

            // no need to create topic - we wouldn't even be here if the other side hadn't done it's part
            //new RetryExponential(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(30), 10)
            var topicClient = new TopicClient(_topicConnectionString, message.ReplyPath.ToLower(), null);
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