#define TEST_SERVICE_BUS

using LagoVista.Core.Models;
using LagoVista.Core.Rpc.Messages;
using Microsoft.Azure.ServiceBus;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.Rpc.Tests.Server
{
    [TestClass]
    public class RequestSeverTests
    {
        [TestMethod]
        public async Task ServiceBus_TopicClient_SendAsync()
        {
            var topicSettings = new ConnectionSettings
            {
                AccountId = "localrequestbus-dev",
                UserName = "SendAccessKey",
                AccessKey = "B2bGyjZVtiNsgQ/BvjCqwtk9FgCYGdA7np99etWzHLc=",
                ResourceName = "rcp_request"
            };
            var _destinationEntityPath = topicSettings.ResourceName;

            var _topicConnectionString = $"Endpoint=sb://{topicSettings.AccountId}.servicebus.windows.net/;SharedAccessKeyName={topicSettings.UserName};SharedAccessKey={topicSettings.AccessKey};";

            var message = new Rpc.Messages.Message
            {
                OrganizationId = "c8ad4589f26842e7a1aefbaefc979c9b",
                InstanceId = "9e88c7f6b5894dbfb3bc09d20736705e"
            };

            //rpc_request_c8ad4589f26842e7a1aefbaefc979c9b_9e88c7f6b5894dbfb3bc09d20736705e
            var entityPath = (_destinationEntityPath + $"_{message.OrganizationId}_{message.InstanceId}").Replace("__", "_").ToLower();
            var topicClient = new TopicClient(_topicConnectionString, entityPath, RetryPolicy.Default);

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
    }
}