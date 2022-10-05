#define TEST_SERVICE_BUS

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using System.Threading;
using LagoVista.Core.Rpc.Server.ServiceBus;
using LagoVista.Core.Rpc.Tests.Server.Utils;
using LagoVista.Core.Utils;
using LagoVista.Core.Rpc.Messages;
#if TEST_SERVICE_BUS
using LagoVista.Core.Models;
using LagoVista.Core.Rpc.Tests.Utils;
using System;
using System.Threading.Tasks;
#endif

namespace LagoVista.Core.Rpc.Tests.Server
{
    [TestClass]
    public class RequestSeverTests : RPC_TestBase
    {
#if TEST_SERVICE_BUS
        [TestMethod]
        public async Task ServiceBus_TopicClient_SendAsync()
        {

            const string INSTANCE_ID = "24603e698fa04d0cb5e4b81515ae68cc";

            var ResourceName = "rpc_request";
            var _destinationEntityPath = ResourceName;

            var topicConnectionString = System.Environment.GetEnvironmentVariable("SB_LOCAL_REQUEST_BUS");
            if(String.IsNullOrEmpty(topicConnectionString))
            {
                throw new ArgumentNullException("must define environment variable [SB_LOCAL_REQUEST_BUS]");
            }

            var message = new Rpc.Messages.Message
            {
                OrganizationId = "c8ad4589f26842e7a1aefbaefc979c9b",
                InstanceId = INSTANCE_ID,
                Id = Guid.NewGuid().ToId(),
                CorrelationId = Constants.MessageCorrelationId,
                DestinationPath = Constants.MessageDestination,
                TimeStamp = Constants.MessageTimeStamp,
                ReplyPath = Constants.MessageReplyPath
            };

            //rpc_request_c8ad4589f26842e7a1aefbaefc979c9b_9e88c7f6b5894dbfb3bc09d20736705e
            var entityPath = $"{_destinationEntityPath}_{message.InstanceId}"
                .Replace("__", "_")
                .ToLower();
            Assert.AreEqual($"rpc_request_{INSTANCE_ID}", entityPath);

            var client = new ServiceBusAdministrationClient(topicConnectionString);
            if (!await client.TopicExistsAsync(entityPath))
            {
                await client.CreateTopicAsync(entityPath);
            }

            var clientOptions = new ServiceBusClientOptions() { TransportType = ServiceBusTransportType.AmqpWebSockets };
            var sbClient = new ServiceBusClient(topicConnectionString, clientOptions);
            var sender = sbClient.CreateSender(entityPath);
            
            var receiver = sbClient.CreateProcessor(entityPath, "application");

            var receiveCount = 0;

            receiver.ProcessMessageAsync += async (ProcessMessageEventArgs arg) =>
            {
                receiveCount += 1;
                Console.WriteLine($"Message Received  {arg.Message.Subject} ");

                await arg.CompleteMessageAsync(arg.Message);
           
                
            };

            receiver.ProcessErrorAsync += (ProcessErrorEventArgs arg) => 
            {
                Console.WriteLine("got an error");
                return Task.CompletedTask;
            }
;

            await receiver.StartProcessingAsync();

            var messageOut = new ServiceBusMessage(message.Payload)
            {
                ContentType = message.GetType().FullName,
                MessageId = message.Id,
                CorrelationId = message.CorrelationId,
                To = message.DestinationPath,
                ReplyTo = message.ReplyPath,
                Subject = message.DestinationPath,
            };

         //   await sender.SendMessageAsync(messageOut);

            while(receiveCount < 1)
            {
                await Task.Delay(50);
            }
        }

        [TestMethod]
        public async Task ServerTests()
        {
            const string INSTANCE_ID = "24603e698fa04d0cb5e4b81515ae68cc";

            var logger = new TestLogger();

            var _requestBroker = new LagoVista.Core.Rpc.Server.RequestBroker();
          
            var coupler = new AsyncCoupler<IMessage>(logger, new TestUsageMetrics("rpc", "rpc", "rpc") { Version = "N/A" });
            var server = new ServiceBusRequestServer(_requestBroker, logger);
            await server.StartAsync(GetSettings(INSTANCE_ID));

            await Task.Delay(5000);
        }



#endif
    }
}