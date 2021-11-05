#define TEST_SERVICE_BUS

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Azure.ServiceBus.Management;
#if TEST_SERVICE_BUS
using LagoVista.Core.Models;
using LagoVista.Core.Rpc.Tests.Utils;
using Microsoft.Azure.ServiceBus;
using System;
using System.Threading.Tasks;
#endif

namespace LagoVista.Core.Rpc.Tests.Server
{
    [TestClass]
    public class RequestSeverTests
    {
#if TEST_SERVICE_BUS
        [TestMethod]
        public async Task ServiceBus_TopicClient_SendAsync()
        {

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
                InstanceId = "9e88c7f6b5894dbfb3bc09d20736705e",
                Id = Guid.NewGuid().ToId(),
                CorrelationId = Constants.MessageCorrelationId,
                DestinationPath = Constants.MessageDestination,
                TimeStamp = Constants.MessageTimeStamp,
                ReplyPath = Constants.MessageReplyPath
            };

            //rpc_request_c8ad4589f26842e7a1aefbaefc979c9b_9e88c7f6b5894dbfb3bc09d20736705e
            var entityPath = $"{_destinationEntityPath}_{message.OrganizationId}_{message.InstanceId}"
                .Replace("__", "_")
                .ToLower();
            Assert.AreEqual("rpc_request_c8ad4589f26842e7a1aefbaefc979c9b_9e88c7f6b5894dbfb3bc09d20736705e", entityPath);

            var client = new ManagementClient(topicConnectionString);
            if (!await client.TopicExistsAsync(entityPath))
            {
                await client.CreateTopicAsync(entityPath);
            }

            var topicClient = new TopicClient(topicConnectionString, entityPath, RetryPolicy.Default);

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
#endif
    }
}