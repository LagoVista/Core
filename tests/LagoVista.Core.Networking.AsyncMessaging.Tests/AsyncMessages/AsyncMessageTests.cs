using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LagoVista.Core.Networking.AsyncMessaging.Tests.AsyncMessages
{
    [TestClass]
    public class AsyncMessageTests
    {
        [TestInitialize]
        public void Init()
        {
        }

        [TestMethod]
        public void TestAsyncMessage()
        {
            var messagePath = "TestPath";
            var timeStamp = new DateTime(2018, 1, 1, 13, 0, 0);
            var messageId = Guid.NewGuid().ToString();
            var messageCorrelationId= Guid.NewGuid().ToString();
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

        }

        [TestMethod]
        public void TestAsyncResponse()
        {

        }
    }
}
