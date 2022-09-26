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
        private IConnectionSettings _subscriberSettings;
        private string _transmitterConnectionString;
        
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

     
        private async void Restart()
        {
            await _processor.CloseAsync();
            await _processor.DisposeAsync();            
        
            await _processorClient.DisposeAsync();
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

            var receiverConnectionString = $"Endpoint=sb://{_subscriberSettings.AccountId}.servicebus.windows.net/;SharedAccessKeyName={_subscriberSettings.UserName};SharedAccessKey={_subscriberSettings.AccessKey};";

            var clientOptions = new ServiceBusClientOptions() { TransportType = ServiceBusTransportType.AmqpWebSockets };
            _processorClient = new ServiceBusClient(receiverConnectionString, clientOptions);
            _processor = _processorClient.CreateProcessor(_subscriberSettings.ResourceName, new ServiceBusProcessorOptions());
            _processor.ProcessMessageAsync += _processor_ProcessMessageAsync;
            _processor.ProcessErrorAsync += _processor_ProcessErrorAsync;

            Console.WriteLine($"Starting ServiceBusRequestServer - {_subscriberSettings.Uri} - {_subscriberSettings.ResourceName}");

            return Task.CompletedTask;
        }

        private Task _processor_ProcessErrorAsync(ProcessErrorEventArgs arg)
        {
            throw new NotImplementedException();
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

                        await ReceiveAsync(new Request(decompressedStream.ToArray()));
                        await arg.CompleteMessageAsync(arg.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.GetType().Name}: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                await  arg.DeadLetterMessageAsync(arg.Message, ex.GetType().FullName, ex.Message);
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
                using(var ms = new MemoryStream(message.Payload))
                using (var mso = new MemoryStream())
                {
                    using (var ds = new DeflateStream(mso, CompressionMode.Compress))
                    {
                        ms.CopyTo(ds);
                    }

                    var buffer = mso.ToArray();

                    Console.Write($"Original message: {message.Payload.Length}  Compressed: {buffer.Length}");

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