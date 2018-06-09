using LagoVista.Core.Rpc.Messages;
using LagoVista.Core.Rpc.Tests.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LagoVista.Core.Rpc.Tests.Messages
{
    [TestClass]
    public class MessageTests 
    {
        private readonly string _destination = "Test.Destination.Path";
        private readonly string _messageId = Guid.Parse("{C4CE5957-F9D7-4727-A20D-4C51AB5C6745}").ToString();
        private readonly string _messageCorrelationId = Guid.Parse("{1C2FC03B-3D21-42A3-97F3-1756177DE2CB}").ToString();
        private readonly string _organizationId = Guid.Parse("{8AF59E47-E473-41D1-AA86-8B557813EEFB}").ToString();
        private readonly string _instanceId = Guid.Parse("{EC0E2AE5-7B17-4C0D-9355-1903E3284FBE}").ToString();
        private readonly string _replyPath = "Test.ReplyTo.Path";
        private readonly DateTime _messageTimeStamp = new DateTime(2018, 1, 1, 13, 30, 30);

        private IMessage CreateControlMessage()
        {
            return new Message()
            {
                Id = _messageId,
                CorrelationId = _messageCorrelationId,
                DestinationPath = _destination,
                TimeStamp = _messageTimeStamp,
                InstanceId = _instanceId,
                OrganizationId = _organizationId,
                ReplyPath = _replyPath
            };
        }

        [TestMethod]
        public void Message_PropertyAssignments()
        {
            var message = new Message()
            {
                Id = _messageId,
                CorrelationId = _messageCorrelationId,
                DestinationPath = _destination,
                TimeStamp = _messageTimeStamp,
                InstanceId = _instanceId,
                OrganizationId = _organizationId,
                ReplyPath= _replyPath
            };

            // tests constructor, InternalSetValue, InternalGetValue and public property assignments
            Assert.AreEqual(_messageId, message.Id);
            Assert.AreEqual(_messageCorrelationId, message.CorrelationId);
            Assert.AreEqual(_destination, message.DestinationPath);
            Assert.AreEqual(_messageTimeStamp, message.TimeStamp);
            Assert.AreEqual(_instanceId, message.InstanceId);
            Assert.AreEqual(_organizationId, message.OrganizationId);
            Assert.AreEqual(_replyPath, message.ReplyPath);
        }

        [TestMethod]
        public void Message_Constructor()
        {
            var message = new Message();

            // tests that all public properties are accessible even if the values aren't explicitly set 
            Assert.AreEqual(string.Empty, message.Id);
            Assert.AreEqual(string.Empty, message.CorrelationId);
            Assert.AreEqual(string.Empty, message.DestinationPath);
            Assert.AreEqual(message.TimeStamp, message.TimeStamp);
            Assert.AreEqual(string.Empty, message.InstanceId);
            Assert.AreEqual(string.Empty, message.OrganizationId);
            Assert.AreEqual(string.Empty, message.ReplyPath);
        }

        [TestMethod]
        public void Message_Constructor_MarshalledData()
        {
            var controlMessage = CreateControlMessage();
            var message = new Message(controlMessage.Payload);

            // tests marshalled data constructor
            Assert.AreEqual(controlMessage.Id, message.Id);
            Assert.AreEqual(controlMessage.CorrelationId, message.CorrelationId);
            Assert.AreEqual(controlMessage.DestinationPath, message.DestinationPath);
            Assert.AreEqual(controlMessage.TimeStamp, message.TimeStamp);
            Assert.AreEqual(controlMessage.ReplyPath, message.ReplyPath);
            Assert.AreEqual(controlMessage.OrganizationId, message.OrganizationId);
            Assert.AreEqual(controlMessage.InstanceId, message.InstanceId);
            Assert.AreEqual(controlMessage.Json, message.Json);
            Assert.IsTrue(controlMessage.Payload.SequenceEqual(message.Payload));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Message_Constructor_MarshalledData_ArgumentNull()
        {
            byte[] marshalledData = null;
            var message = new Message(marshalledData);
        }

        [TestMethod]
        public void Message_SetValue()
        {
            var controlMessage = CreateControlMessage();
            // tests public set value
            controlMessage.SetValue("key", "value");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Message_SetValue_KeyAlreadyExistsError()
        {
            var controlMessage = CreateControlMessage();
            var key = "key";
            // tests duplicate key exception
            controlMessage.SetValue(key, "value1");
            controlMessage.SetValue(key, "value2");
        }

        [TestMethod]
        public void Message_SetValue_OverwriteValueWithExistingKey()
        {
            var controlMessage = CreateControlMessage();
            var key = "key";
            // tests duplicate key overwrite
            controlMessage.SetValue(key, "value1");
            controlMessage.SetValue(key, "value2", true);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Message_SetValue_KeyEmptyArgument()
        {
            string key = string.Empty;
            var controlMessage = CreateControlMessage();
            // tests passing empty key value
            controlMessage.SetValue(key, "value1");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Message_SetValue_KeyNullArgument()
        {
            string key = null;
            var controlMessage = CreateControlMessage();
            // tests passing null key value
            controlMessage.SetValue(key, "value1");
        }

        [TestMethod]
        public void Message_SetValue_EmptyValueArgument()
        {
            var value = string.Empty;
            var controlMessage = CreateControlMessage();
            // tests passing empty string value
            controlMessage.SetValue("key", value);
        }

        [TestMethod]
        public void Message_SetValue_NullValueArgument()
        {
            object value = null;
            var controlMessage = CreateControlMessage();
            // tests passing null object value
            controlMessage.SetValue("key", value);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Message_SetValue_IllegalKeyPrefix()
        {
            var controlMessage = CreateControlMessage();
            // tests using an illegal key prefix
            controlMessage.SetValue("__key", "value");
        }

        [TestMethod]
        public void Message_GetValue()
        {
            var key = "key";
            var value = "value";
            var controlMessage = CreateControlMessage();
            controlMessage.SetValue(key, value);

            // tests public set and get value methods
            Assert.AreEqual(value, controlMessage.GetValue(key));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Message_GetValue_KeyEmptyArgument()
        {
            var key = string.Empty;
            var controlMessage = CreateControlMessage();
            // tests passing empty key 
            var v = controlMessage.GetValue(key);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Message_GetValue_KeyNullArgument()
        {
            string key = null;
            var controlMessage = CreateControlMessage();
            // tests passing null key
            var v = controlMessage.GetValue(key);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Message_GetValue_IllegalKeyPrefix()
        {
            var controlMessage = CreateControlMessage();
            // tests using an illegal key prefix
            var v = controlMessage.GetValue("__key");
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void Message_GetValue_KeyNotFound()
        {
            var controlMessage = CreateControlMessage();
            // tests key that hasn't been set
            var v = controlMessage.GetValue("key");
        }

        [TestMethod]
        public void Message_GetValue_T()
        {
            var key = "key";
            var value = new FakeProxySubject();
            var controlMessage = CreateControlMessage();
            // tests setting/getting non-null typed object
            controlMessage.SetValue(key, value);
            Assert.AreEqual(value, controlMessage.GetValue<FakeProxySubject>(key));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void Message_GetValue_T_InvalidType()
        {
            var key = "key";
            var value = new object();
            var controlMessage = CreateControlMessage();
            // tests typed get value method with wrong type
            controlMessage.SetValue(key, value);
            var v = controlMessage.GetValue<FakeProxySubject>("key");
        }
    }
}
