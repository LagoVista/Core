using LagoVista.Core.Networking.AsyncMessaging.Tests.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LagoVista.Core.Networking.AsyncMessaging.Tests.AsyncMessages
{
    [TestClass]
    public class AsyncMessageTests : TestBase
    {
        private readonly string messagePath = "TestPath";
        private readonly DateTime messageTimeStamp = new DateTime(2018, 1, 1, 13, 0, 0);
        private readonly string messageId = Guid.NewGuid().ToString();
        private readonly string messageCorrelationId = Guid.NewGuid().ToString();

        private IAsyncMessage CreateControlMessage()
        {
            return new AsyncMessage()
            {
                Id = messageId,
                CorrelationId = messageCorrelationId,
                Path = messagePath,
                TimeStamp = messageTimeStamp
            };
        }

        [TestMethod]
        public void AsyncMessage_Constructor_Generic()
        {
            var message = new AsyncMessage()
            {
                Id = messageId,
                CorrelationId = messageCorrelationId,
                Path = messagePath,
                TimeStamp = messageTimeStamp
            };

            // tests constructor and InternalSetValue and InternalGetValue
            Assert.AreEqual(messageId, message.Id);
            Assert.AreEqual(messageCorrelationId, message.CorrelationId);
            Assert.AreEqual(messagePath, message.Path);
            Assert.AreEqual(messageTimeStamp, message.TimeStamp);
        }

        [TestMethod]
        public void AsyncMessage_Constructor_MarshalledData()
        {
            var controlMessage = CreateControlMessage();

            var message = new AsyncMessage(controlMessage.MarshalledData);

            Assert.AreEqual(controlMessage.Id, message.Id);
            Assert.AreEqual(controlMessage.CorrelationId, message.CorrelationId);
            Assert.AreEqual(controlMessage.Path, message.Path);
            Assert.AreEqual(controlMessage.TimeStamp, message.TimeStamp);
            Assert.AreEqual(controlMessage.Json, message.Json);
            Assert.IsTrue(controlMessage.MarshalledData.SequenceEqual(message.MarshalledData));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AsyncMessage_Constructor_MarshalledData_ArgumentNull()
        {
            byte[] marshalledData = null;
            var message = new AsyncMessage(marshalledData);
        }

        [TestMethod]
        public void AsyncMessage_SetValue()
        {
            var controlMessage = CreateControlMessage();
            controlMessage.SetValue("key", "value");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AsyncMessage_SetValue_KeyAlreadyExistsError()
        {
            var controlMessage = CreateControlMessage();
            var key = "key";
            controlMessage.SetValue(key, "value1");
            controlMessage.SetValue(key, "value2");
        }

        [TestMethod]
        public void AsyncMessage_SetValue_OverwriteValueWithExistingKey()
        {
            var controlMessage = CreateControlMessage();
            var key = "key";
            controlMessage.SetValue(key, "value1");
            controlMessage.SetValue(key, "value2", true);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AsyncMessage_SetValue_KeyNullArgument()
        {
            string key = string.Empty;
            var controlMessage = CreateControlMessage();
            controlMessage.SetValue(key, "value1");
        }

        [TestMethod]
        public void AsyncMessage_SetValue_NullValueArgument()
        {
            string value = string.Empty;
            var controlMessage = CreateControlMessage();
            controlMessage.SetValue("key", value);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AsyncMessage_SetValue_IllegalKeyPrefix()
        {
            var controlMessage = CreateControlMessage();
            controlMessage.SetValue("__key", "value");
        }

        [TestMethod]
        public void AsyncMessage_GetValue()
        {
            var controlMessage = CreateControlMessage();

            var key = "key";
            var value = "value";
            controlMessage.SetValue(key, value);

            Assert.AreEqual(value, controlMessage.GetValue(key));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AsyncMessage_GetValue_KeyNullArgument()
        {
            var controlMessage = CreateControlMessage();

            var key = string.Empty;
            var v = controlMessage.GetValue(key);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AsyncMessage_GetValue_IllegalKeyPrefix()
        {
            var controlMessage = CreateControlMessage();
            var v = controlMessage.GetValue("__key");
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void AsyncMessage_GetValue_KeyNotFound()
        {
            var controlMessage = CreateControlMessage();
            var v = controlMessage.GetValue("key");
        }

        [TestMethod]
        public void AsyncMessage_GetValue_T()
        {
            var controlMessage = CreateControlMessage();

            var key = "key";
            var value = new ProxySubject();
            controlMessage.SetValue(key, value);

            Assert.AreEqual(value, controlMessage.GetValue<ProxySubject>(key));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void AsyncMessage_GetValue_T_InvalidType()
        {
            var controlMessage = CreateControlMessage();

            var key = "key";
            var value = new object();
            controlMessage.SetValue(key, value);

            var v = controlMessage.GetValue<ProxySubject>("key");
        }
    }
}
