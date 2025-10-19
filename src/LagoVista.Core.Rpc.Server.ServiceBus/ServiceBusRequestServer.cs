// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 3d947227e2e361bdd6cf5058526654d333b24f128690ca263f6260fed8ac4138
// IndexVersion: 0
// --- END CODE INDEX META ---
using Azure;
using Azure.Messaging.ServiceBus;
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

namespace LagoVista.Core.Rpc.Server.ServiceBus
{
    /* 
     * In this case, the server is the component that is accepting requests to be processed, it is 
     * running on the instance.
     */
    public sealed class ServiceBusRequestServer : AbstractRequestServer
    {
        #region Fields        
        private AzureSasCredential _subscriberSASSettings;

        private string _subscriberConnectionString;
        private string _transmitterConnectionString;

        private IConnectionSettings _subscriberSettings;
        private IConnectionSettings _transmitterSettings;

        private ServiceBusClient _processorClient;
        private ServiceBusProcessor _processor;

        #endregion

        public ServiceBusRequestServer(IRequestBroker requestBroker, ILogger logger) :
            base(requestBroker, logger)
        {

            // Endpoint - AccountId
            // SharedAccessKeyName - UserName
            // SharedAccessKey - AccessKey
            // DestinationEntityPath - ResourceName
        }


        protected override void ConfigureSettings(ITransceiverConnectionSettings connectionSettings)
        {
            _subscriberSettings = connectionSettings.RpcServerReceiver;
            _transmitterSettings = connectionSettings.RpcServerTransmitter;

            if (_subscriberSettings.AccessKey.StartsWith("SharedAccessSignature"))
            {
                _subscriberSASSettings = new AzureSasCredential(_subscriberSettings.AccessKey);
                _subscriberConnectionString = null;
            }
            else
            {
                _subscriberConnectionString = $"Endpoint=sb://{_subscriberSettings.AccountId}.servicebus.windows.net/;SharedAccessKeyName={_subscriberSettings.UserName};SharedAccessKey={_subscriberSettings.AccessKey};";
                _subscriberSASSettings = null;
            }

            _transmitterConnectionString = $"Endpoint=sb://{_transmitterSettings.AccountId}.servicebus.windows.net/;SharedAccessKeyName={_transmitterSettings.UserName};SharedAccessKey={_transmitterSettings.AccessKey};";
            Console.WriteLine($"Subscriber Settings : {_subscriberConnectionString}");
            Console.WriteLine($"Transmitter Settings: {_transmitterConnectionString}");
        }

        protected override void UpdateSettings(ITransceiverConnectionSettings connectionSettings)
        {
            _subscriberSettings = connectionSettings.RpcServerReceiver;
            _transmitterSettings = connectionSettings.RpcServerTransmitter;

            _subscriberSASSettings = new AzureSasCredential(_subscriberSettings.AccessKey);

            _transmitterConnectionString = $"Endpoint=sb://{_transmitterSettings.AccountId}.servicebus.windows.net/;SharedAccessKeyName={_transmitterSettings.UserName};SharedAccessKey={_transmitterSettings.AccessKey};";

            Restart();
        }


        private async void Restart()
        {
            await _processor.CloseAsync();
            await _processor.DisposeAsync();

            await _processorClient.DisposeAsync();
            await CustomStartAsync();
        }

        protected override async Task CustomStartAsync()
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

            

            var clientOptions = new ServiceBusClientOptions() { TransportType = ServiceBusTransportType.AmqpWebSockets };

            if (_subscriberSASSettings != null)
            {
                Console.Write("Using SubScriberSASSettings");
                var fqASBName = $"{_subscriberSettings.AccountId}.servicebus.windows.net";
                _processorClient = new ServiceBusClient(fqASBName, _subscriberSASSettings, clientOptions);
            }
            else
            {
                Console.Write("Using Connection String");
                _processorClient = new ServiceBusClient(_subscriberConnectionString, clientOptions);
            }

            _processor = _processorClient.CreateProcessor(topic.ToLower(), subscrption.ToLower());
            _processor.ProcessMessageAsync += _processor_ProcessMessageAsync;
            _processor.ProcessErrorAsync += _processor_ProcessErrorAsync;

            Console.WriteLine($"[Starting] ServiceBusRequestServer - {_subscriberSettings.AccountId} - topic: {topic}, subs: {subscrption}");
            await _processor.StartProcessingAsync();
            Console.WriteLine($"[Started] ServiceBusRequestServer - {_subscriberSettings.AccountId} - topic: {topic}, subs: {subscrption}");
            if (_processorClient.IsClosed)
            {
                throw new InvalidOperationException("Processor Client Not Open");
            }
        }

        private Task _processor_ProcessErrorAsync(ProcessErrorEventArgs arg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[ServiceBusRequestServer__ProcessError] - " + arg.Exception.Message);
            if(arg.Exception.InnerException != null)
                Console.WriteLine("[ServiceBusRequestServer__ProcessError] - " + arg.Exception.InnerException.Message);

            Console.ResetColor();

            return Task.CompletedTask;
        }

        private async Task _processor_ProcessMessageAsync(ProcessMessageEventArgs arg)
        {
            try
            {
                using (var compressedStream = new MemoryStream(arg.Message.Body.ToArray()))
                using (var decompressorStream = new DeflateStream(compressedStream, CompressionMode.Decompress))
                {
                    using (var decompressedStream = new MemoryStream())
                    {
                        decompressorStream.CopyTo(decompressedStream);
                        Console.WriteLine($"[Received]: {arg.Message.Subject}");

                        await ReceiveAsync(new Request(decompressedStream.ToArray()));
                        await arg.CompleteMessageAsync(arg.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor=ConsoleColor.Red;
                Console.WriteLine($"[ServiceBusRequestServer__ProcessMessage] {ex.GetType().Name}: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                Console.ResetColor();

                await arg.DeadLetterMessageAsync(arg.Message, ex.GetType().FullName, ex.Message);
                throw;
            }
        }

        protected override async Task<InvokeResult> CustomTransmitMessageAsync(IMessage message)
        {
            // no need to create topic - we wouldn't even be here if the other side hadn't done it's part
            //new RetryExponential(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(30), 10)

            var clientOptions = new ServiceBusClientOptions() { TransportType = ServiceBusTransportType.AmqpWebSockets };
            var senderClient = new ServiceBusClient(_transmitterConnectionString, clientOptions);
            var sender = senderClient.CreateSender(message.ReplyPath.ToLower());

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

                    Console.WriteLine($"[SendingFromServer]: {message.ReplyPath} {message.Payload.Length}  Compressed: {buffer.Length}");

                    var messageOut = new ServiceBusMessage(buffer)
                    {
                        ContentType = message.GetType().FullName,
                        MessageId = message.Id,
                        CorrelationId = message.CorrelationId,
                        To = message.DestinationPath,
                        ReplyTo = message.ReplyPath,
                        Subject = message.DestinationPath,
                    };

                    await sender.SendMessageAsync(messageOut);
                    return InvokeResult.Success;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Client Send  Message] {ex.GetType().Name}: {ex.Message}");
                _logger.AddException("Custom Transmit Message", ex, message.DestinationPath.ToKVP("destinationPath"));
                throw;
            }
            finally
            {
                await sender.CloseAsync();
                await sender.DisposeAsync();

                await senderClient.DisposeAsync();
            }
        }
    }
}