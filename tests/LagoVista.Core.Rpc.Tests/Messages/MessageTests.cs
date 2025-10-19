// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: c83dde54fc0375c5359a32fab1133eb2ae1c3a19db3c486111307fcb690f32cd
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core.Rpc.Messages;
using LagoVista.Core.Rpc.Tests.Models;
using LagoVista.Core.Rpc.Tests.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LagoVista.Core.Rpc.Tests.Messages
{
    [TestClass]
    public class MessageTests 
    {
        public T NullTest<T>()
        {
            object value = null;
            return value != null ? (T)value : default(T);
        }

        [TestMethod] 
        public void TestNull()
        {
            var i = NullTest<int>();
        }

        private IMessage CreateControlMessage()
        {
            return new Message()
            {
                Id = Constants.MessageId,
                CorrelationId = Constants.MessageCorrelationId,
                DestinationPath = Constants.MessageDestination,
                TimeStamp = Constants.MessageTimeStamp,
                InstanceId = Constants.InstanceId,
                OrganizationId = Constants.OrganizationId,
                ReplyPath = Constants.MessageReplyPath
            };
        }

        [TestMethod]
        public void Message_PropertyAssignments()
        {
            var message = new Message()
            {
                Id = Constants.MessageId,
                CorrelationId = Constants.MessageCorrelationId,
                DestinationPath = Constants.MessageDestination,
                TimeStamp = Constants.MessageTimeStamp,
                InstanceId = Constants.InstanceId,
                OrganizationId = Constants.OrganizationId,
                ReplyPath= Constants.MessageReplyPath
            };

            // tests constructor, InternalSetValue, InternalGetValue and public property assignments
            Assert.AreEqual(Constants.MessageId, message.Id);
            Assert.AreEqual(Constants.MessageCorrelationId, message.CorrelationId);
            Assert.AreEqual(Constants.MessageDestination, message.DestinationPath);
            Assert.AreEqual(Constants.MessageTimeStamp, message.TimeStamp);
            Assert.AreEqual(Constants.InstanceId, message.InstanceId);
            Assert.AreEqual(Constants.OrganizationId, message.OrganizationId);
            Assert.AreEqual(Constants.MessageReplyPath, message.ReplyPath);
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
        public void Message_Constructor_MarshalledData_ArgumentNull()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() =>
            {
                byte[] marshalledData = null;
                var message = new Message(marshalledData);
            });
        }

        [TestMethod]
        public void Message_SetValue()
        {
            var controlMessage = CreateControlMessage();
            // tests public set value
            controlMessage.SetValue("key", "value");
        }

        [TestMethod]
        public void Message_SetValue_KeyAlreadyExistsError()
        {
            Assert.ThrowsExactly<ArgumentException>(() =>
            {
                var controlMessage = CreateControlMessage();
                var key = "key";
                // tests duplicate key exception
                controlMessage.SetValue(key, "value1");
                controlMessage.SetValue(key, "value2");
            });
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
        public void Message_SetValue_KeyEmptyArgument()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() =>
            {
                string key = string.Empty;
                var controlMessage = CreateControlMessage();
                // tests passing empty key value
                controlMessage.SetValue(key, "value1");
            });
        }

        [TestMethod]
        public void Message_SetValue_KeyNullArgument()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() =>
            {
                string key = null;
                var controlMessage = CreateControlMessage();
                // tests passing null key value
                controlMessage.SetValue(key, "value1");
            });
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
        public void Message_SetValue_IllegalKeyPrefix()
        {
            Assert.ThrowsExactly<ArgumentException>(() =>
            {
                var controlMessage = CreateControlMessage();
                // tests using an illegal key prefix
                controlMessage.SetValue("__key", "value");
            });
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
        public void Message_GetValue_KeyEmptyArgument()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() =>
            {
                var key = string.Empty;
                var controlMessage = CreateControlMessage();
                // tests passing empty key 
                var v = controlMessage.GetValue(key);
            });
        }

        [TestMethod]
        public void Message_GetValue_KeyNullArgument()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() =>
            {
                string key = null;
            var controlMessage = CreateControlMessage();
            // tests passing null key
            var v = controlMessage.GetValue(key);
        });
        }

        [TestMethod]
        public void Message_GetValue_IllegalKeyPrefix()
        {
            Assert.ThrowsExactly<ArgumentException>(() =>
            {
                var controlMessage = CreateControlMessage();
                // tests using an illegal key prefix
                var v = controlMessage.GetValue("__key");
            });
        }

        [TestMethod]
        public void Message_GetValue_KeyNotFound()
        {
            Assert.ThrowsExactly<KeyNotFoundException>(() =>
            {
                var controlMessage = CreateControlMessage();
            // tests key that hasn't been set
                var v = controlMessage.GetValue("key");
            }); 
        }

        [TestMethod]
        public void Message_GetValue_T()
        {
            var key = "key";
            var value = new ProxySubject();
            var controlMessage = CreateControlMessage();
            // tests setting/getting non-null typed object
            controlMessage.SetValue(key, value);
            Assert.AreEqual(value, controlMessage.GetValue<ProxySubject>(key));
        }

        [TestMethod]
        public void Message_GetValue_T_InvalidType()
        {
            Assert.ThrowsExactly<InvalidCastException>(() =>
            {
                var key = "key";
                var value = new object();
                var controlMessage = CreateControlMessage();
                // tests typed get value method with wrong type
                controlMessage.SetValue(key, value);
                var v = controlMessage.GetValue<ProxySubject>("key");
            });
        }
    }
}
