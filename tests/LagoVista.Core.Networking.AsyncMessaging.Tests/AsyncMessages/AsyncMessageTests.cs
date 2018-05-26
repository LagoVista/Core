using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LagoVista.Core.Networking.AsyncMessaging.Tests.AsyncMessages
{
    [TestClass]
    public class AsyncMessageTests
    {
        [TestInitialize]
        public void Init()
        {
        }

        private readonly string messagePath = "TestPath";
        private readonly DateTime timeStamp = new DateTime(2018, 1, 1, 13, 0, 0);
        private readonly string messageId = Guid.NewGuid().ToString();
        private readonly string messageCorrelationId = Guid.NewGuid().ToString();

        [TestMethod]
        public void TestAsyncMessage()
        {
            IAsyncMessage message1 = new AsyncMessage()
            {
                Id = messageId,
                CorrelationId = messageCorrelationId,
                Path = messagePath,
                TimeStamp = timeStamp
            };
            Assert.AreEqual(message1.Id, messageId);
            Assert.AreEqual(message1.CorrelationId, messageCorrelationId);
            Assert.AreEqual(message1.Path, messagePath);
            Assert.AreEqual(message1.TimeStamp, timeStamp);

            byte[] marshalledData = message1.MarshalledData;
            Assert.IsTrue(message1.MarshalledData.SequenceEqual(marshalledData));

            IAsyncMessage message2 = new AsyncMessage(marshalledData);
            Assert.AreEqual(message1.Id, message2.Id);
            Assert.AreEqual(message1.CorrelationId, message2.CorrelationId);
            Assert.AreEqual(message1.Path, message2.Path);
            Assert.AreEqual(message1.TimeStamp, message2.TimeStamp);
            Assert.IsTrue(message1.MarshalledData.SequenceEqual(message2.MarshalledData));
        }

        [TestMethod]
        public void TestAsyncRequest()
        {
            IAsyncRequest message1 = new AsyncRequest()
            {
                Id = messageId,
                CorrelationId = messageCorrelationId,
                Path = messagePath,
                TimeStamp = timeStamp
            };
            Assert.AreEqual(message1.Id, messageId);
            Assert.AreEqual(message1.CorrelationId, messageCorrelationId);
            Assert.AreEqual(message1.Path, messagePath);
            Assert.AreEqual(message1.TimeStamp, timeStamp);

            var keyName = "property1";
            object value1 = "value1";
            message1.SetValue(keyName, value1);
            Assert.AreEqual(message1.GetValue(keyName), value1);
            Assert.AreEqual(message1.GetValue<string>(keyName), value1);

            object value2 = "value2";
            var argumentException = Assert.ThrowsException<ArgumentException>(() => { message1.SetValue(keyName, value2); });
            Assert.AreEqual($"key already exists {keyName}", argumentException.Message);
            message1.SetValue(keyName, value2, true);
            Assert.AreEqual(message1.GetValue(keyName), value2);
            Assert.AreEqual(message1.GetValue<string>(keyName), value2);

            byte[] marshalledData = message1.MarshalledData;
            Assert.IsTrue(message1.MarshalledData.SequenceEqual(marshalledData));

            IAsyncRequest message2 = new AsyncRequest(marshalledData);
            Assert.AreEqual(message1.Id, message2.Id);
            Assert.AreEqual(message1.CorrelationId, message2.CorrelationId);
            Assert.AreEqual(message1.Path, message2.Path);
            Assert.AreEqual(message1.TimeStamp, message2.TimeStamp);
            Assert.AreEqual(message1.GetValue(keyName), message2.GetValue(keyName));
            Assert.IsTrue(message1.MarshalledData.SequenceEqual(message2.MarshalledData));
        }

        [TestMethod]
        public void TestAsyncResponse()
        {
            var responseValue = "response value";
            // cast as object to prevent being the json constructor from being called
            IAsyncResponse message1 = new AsyncResponse((object)responseValue)
            {
                Id = messageId,
                CorrelationId = messageCorrelationId,
                Path = messagePath,
                TimeStamp = timeStamp
            };
            Assert.IsTrue(message1.Success);
            Assert.AreEqual(message1.Id, messageId);
            Assert.AreEqual(message1.CorrelationId, messageCorrelationId);
            Assert.AreEqual(message1.Path, messagePath);
            Assert.AreEqual(message1.TimeStamp, timeStamp);
            Assert.IsNotNull(message1.ReturnValue);
            Assert.AreEqual(message1.ReturnValue, responseValue);

            byte[] marshalledData = message1.MarshalledData;
            Assert.IsTrue(message1.MarshalledData.SequenceEqual(marshalledData));

            IAsyncResponse message2 = new AsyncResponse(marshalledData);
            Assert.IsTrue(message2.Success);
            Assert.AreEqual(message1.Id, message2.Id);
            Assert.AreEqual(message1.CorrelationId, message2.CorrelationId);
            Assert.AreEqual(message1.Path, message2.Path);
            Assert.AreEqual(message1.TimeStamp, message2.TimeStamp);
            Assert.IsNotNull(message2.ReturnValue);
            Assert.AreEqual(message1.ReturnValue, message2.ReturnValue);
            Assert.IsTrue(message1.MarshalledData.SequenceEqual(message2.MarshalledData));

            IAsyncResponse<string> message4 = new AsyncResponse<string>(marshalledData);
            Assert.IsTrue(message4.Success);
            Assert.AreEqual(message1.Id, message4.Id);
            Assert.AreEqual(message1.CorrelationId, message4.CorrelationId);
            Assert.AreEqual(message1.Path, message4.Path);
            Assert.AreEqual(message1.TimeStamp, message4.TimeStamp);
            Assert.IsInstanceOfType(message4.TypedReturnValue, typeof(string));
            Assert.IsNotNull(message4.TypedReturnValue);
            Assert.AreEqual(message1.ReturnValue, message4.TypedReturnValue);
            Assert.IsTrue(message1.MarshalledData.SequenceEqual(message4.MarshalledData));

            var exMessage = "fail";
            IAsyncResponse message3 = new AsyncResponse(new Exception(exMessage))
            {
                Id = messageId,
                CorrelationId = messageCorrelationId,
                Path = messagePath,
                TimeStamp = timeStamp
            };
            Assert.IsFalse(message3.Success);
            Assert.AreEqual(message3.Exception.Message, exMessage);
            var exception = Assert.ThrowsException<KeyNotFoundException>(() => { var r = message3.ReturnValue; });
            Assert.AreEqual(exception.Message, "__response");

            Assert.IsFalse(message1.MarshalledData.SequenceEqual(message3.MarshalledData));
        }
    }
}
