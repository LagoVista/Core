using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LagoVista.Core.Networking.AsyncMessaging.Tests.ServiceBusTests
{
    [TestClass]
    public class TopicInstructionsTest
    {
        [TestMethod]
        public void TopicInstructions_ImplicitOperator_FromString_ReturnsNonNull()
        {
            var json = "{\"organizationKey\": \"ok\", \"instanceKey\": \"ik\", \"instanceId\": \"ii\"}";
            ServiceBusAsyncRequestSender.TopicInstructions topicInstructions = json;
            Assert.IsNotNull(topicInstructions);
        }

        [TestMethod]
        public void TopicInstructions_ImplicitOperator_FromString_ReturnsCorrectValues()
        {
            var json = "{\"organizationKey\": \"ok\", \"instanceKey\": \"ik\", \"instanceId\": \"ii\"}";
            ServiceBusAsyncRequestSender.TopicInstructions topicInstructions = json;
            Assert.AreEqual("ok", topicInstructions.OrganizationKey);
            Assert.AreEqual("ik", topicInstructions.InstanceKey);
            Assert.AreEqual("ii", topicInstructions.InstanceId);
        }

        [TestMethod]
        public void TopicInstructions_ImplicitOperator_FromString_ReturnsNonNull_WithVar()
        {
            var json = "{\"organizationKey\": \"ok\", \"instanceKey\": \"ik\", \"instanceId\": \"ii\"}";
            var topicInstructions = (ServiceBusAsyncRequestSender.TopicInstructions)json;
            Assert.IsNotNull(topicInstructions);
        }

        [TestMethod]
        public void TopicInstructions_ImplicitOperator_FromString_ReturnsCorrectValues_WithVar()
        {
            var json = "{\"organizationKey\": \"ok\", \"instanceKey\": \"ik\", \"instanceId\": \"ii\"}";
            var topicInstructions = (ServiceBusAsyncRequestSender.TopicInstructions)json;
            Assert.AreEqual("ok", topicInstructions.OrganizationKey);
            Assert.AreEqual("ik", topicInstructions.InstanceKey);
            Assert.AreEqual("ii", topicInstructions.InstanceId);
        }

        [TestMethod]
        public void TopicInstructions_ImplicitOperator_FromString_ReturnsNull_WithVarAndEmptyString()
        {
            var json = string.Empty;
            var topicInstructions = (ServiceBusAsyncRequestSender.TopicInstructions)json;
            Assert.IsNull(topicInstructions);
        }

        [TestMethod]
        public void TopicInstructions_ImplicitOperator_ToString_UsingExplicitCast()
        {
            var json = "{\"organizationKey\": \"ok\", \"instanceKey\": \"ik\", \"instanceId\": \"ii\"}";
            var topicInstructions = (ServiceBusAsyncRequestSender.TopicInstructions)json;
            var controlString = $"_{topicInstructions.OrganizationKey}_{topicInstructions.InstanceKey}_{topicInstructions.InstanceId}";
            Assert.AreEqual(controlString, (string)topicInstructions);
        }

        [TestMethod]
        public void TopicInstructions_ImplicitOperator_ToString__UsingImplicitCast()
        {
            var json = "{\"organizationKey\": \"ok\", \"instanceKey\": \"ik\", \"instanceId\": \"ii\"}";
            var topicInstructions = (ServiceBusAsyncRequestSender.TopicInstructions)json;
            var controlString = $"[TOPIC_PREFIX]_{topicInstructions.OrganizationKey}_{topicInstructions.InstanceKey}_{topicInstructions.InstanceId}";
            Assert.AreEqual(controlString, "[TOPIC_PREFIX]" + topicInstructions);
        }
    }
}

