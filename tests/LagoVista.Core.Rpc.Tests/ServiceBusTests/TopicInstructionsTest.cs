using Microsoft.Azure.ServiceBus.Management;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace LagoVista.Core.Rpc.Tests.ServiceBusTests
{
    [TestClass]
    public class TopicInstructionsTest
    {

        [TestMethod]
        public async Task CreateTopicAsync()
        {
            string entityPath = "runtime-creation";
            var connstr = $"Endpoint=sb://localrequestbus-dev.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=fVoIfvx0cI0oY7Zkni4dknP6FbZbjh3DiUh7Ci4rrfI=;";

            var client = new ManagementClient(connstr);
            if (!await client.TopicExistsAsync(entityPath))
            {
                await client.CreateTopicAsync(entityPath);
                await client.CreateSubscriptionAsync(entityPath, "application");
                await client.CreateSubscriptionAsync(entityPath, "admin");
            }
        }

        //        private readonly string _json = "{\"organizationId\": \"ok\", \"instanceId\": \"ii\"}";
        //        [TestMethod]
        //        public void TopicInstructions_ImplicitOperator_FromString_ReturnsNonNull()
        //        {
        //            ServiceBusRequestSender.TopicInstructions topicInstructions = _json;
        //            Assert.IsNotNull(topicInstructions);
        //        }

        //        [TestMethod]
        //        public void TopicInstructions_ImplicitOperator_FromString_ReturnsCorrectValues()
        //        {
        //            ServiceBusRequestSender.TopicInstructions topicInstructions = _json;
        //            Assert.AreEqual("ok", topicInstructions.OrganizationId);
        //            Assert.AreEqual("ii", topicInstructions.InstanceId);
        //        }

        //        [TestMethod]
        //        public void TopicInstructions_ImplicitOperator_FromString_ReturnsNonNull_WithVar()
        //        {
        //            var topicInstructions = (ServiceBusRequestSender.TopicInstructions)_json;
        //            Assert.IsNotNull(topicInstructions);
        //        }

        //        [TestMethod]
        //        public void TopicInstructions_ImplicitOperator_FromString_ReturnsCorrectValues_WithVar()
        //        {
        //            var topicInstructions = (ServiceBusRequestSender.TopicInstructions)_json;
        //            Assert.AreEqual("ok", topicInstructions.OrganizationId);
        //            Assert.AreEqual("ii", topicInstructions.InstanceId);
        //        }

        //        [TestMethod]
        //        public void TopicInstructions_ImplicitOperator_FromString_ReturnsNull_WithVarAndEmptyString()
        //        {
        //            var json = string.Empty;
        //            var topicInstructions = (ServiceBusRequestSender.TopicInstructions)json;
        //            Assert.IsNull(topicInstructions);
        //        }

        //        [TestMethod]
        //        public void TopicInstructions_ImplicitOperator_ToString_UsingExplicitCast()
        //        {
        //            var topicInstructions = (ServiceBusRequestSender.TopicInstructions)_json;
        //            //var controlString = $"_{topicInstructions.organizationId}_{topicInstructions.InstanceKey}_{topicInstructions.InstanceId}";
        //            var controlString = $"_{topicInstructions.OrganizationId}_{topicInstructions.InstanceId}";
        //            Assert.AreEqual(controlString, (string)topicInstructions);
        //        }

        //        [TestMethod]
        //        public void TopicInstructions_ImplicitOperator_ToString__UsingImplicitCast()
        //        {
        //            var topicInstructions = (ServiceBusRequestSender.TopicInstructions)_json;
        //            //var controlString = $"[TOPIC_PREFIX]_{topicInstructions.organizationId}_{topicInstructions.InstanceKey}_{topicInstructions.InstanceId}";
        //            var controlString = $"[TOPIC_PREFIX]_{topicInstructions.OrganizationId}_{topicInstructions.InstanceId}";
        //            Assert.AreEqual(controlString, "[TOPIC_PREFIX]" + topicInstructions);
        //        }
    }
}

