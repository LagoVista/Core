//using Microsoft.VisualStudio.TestTools.UnitTesting;

//namespace LagoVista.Core.Rpc.Tests.ServiceBusTests
//{
//    [TestClass]
//    public class TopicInstructionsTest
//    {
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
//    }
//}

