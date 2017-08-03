using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.Tests.Extensions
{
    [TestClass]
    public class DateTimeExtensions
    {
        [TestMethod()]
        public void TestDate20()
        {
            var dateStr = "2017-08-03T14:44:32Z";
            var date = dateStr.ToDateTime();

            Assert.AreEqual(2017, date.ToUniversalTime().Year);
            Assert.AreEqual(08, date.ToUniversalTime().Month);
            Assert.AreEqual(03, date.ToUniversalTime().Day);
            Assert.AreEqual(14, date.ToUniversalTime().Hour);
            Assert.AreEqual(44, date.ToUniversalTime().Minute);
            Assert.AreEqual(32, date.ToUniversalTime().Second);
        }

        [TestMethod()]
        public void TestDate21()
        {
            var dateStr = "2017-08-03T14:44:32.Z";
            var date = dateStr.ToDateTime();

            Assert.AreEqual(2017, date.ToUniversalTime().Year);
            Assert.AreEqual(08, date.ToUniversalTime().Month);
            Assert.AreEqual(03, date.ToUniversalTime().Day);
            Assert.AreEqual(14, date.ToUniversalTime().Hour);
            Assert.AreEqual(44, date.ToUniversalTime().Minute);
            Assert.AreEqual(32, date.ToUniversalTime().Second);
        }


        [TestMethod()]
        public void TestDate22()
        {
            var dateStr = "2017-08-03T14:44:32.0Z";
            var date = dateStr.ToDateTime();

            Assert.AreEqual(2017, date.ToUniversalTime().Year);
            Assert.AreEqual(08, date.ToUniversalTime().Month);
            Assert.AreEqual(03, date.ToUniversalTime().Day);
            Assert.AreEqual(14, date.ToUniversalTime().Hour);
            Assert.AreEqual(44, date.ToUniversalTime().Minute);
            Assert.AreEqual(32, date.ToUniversalTime().Second);
        }


        [TestMethod()]
        public void TestDate23()
        {
            var dateStr = "2017-08-03T14:44:32.00Z";
            var date = dateStr.ToDateTime();

            Assert.AreEqual(2017, date.ToUniversalTime().Year);
            Assert.AreEqual(08, date.ToUniversalTime().Month);
            Assert.AreEqual(03, date.ToUniversalTime().Day);
            Assert.AreEqual(14, date.ToUniversalTime().Hour);
            Assert.AreEqual(44, date.ToUniversalTime().Minute);
            Assert.AreEqual(32, date.ToUniversalTime().Second);
        }

        [TestMethod()]
        public void TestDate24()
        {
            var dateStr = "2017-08-03T14:44:32.000Z";
            var date = dateStr.ToDateTime();

            Assert.AreEqual(2017, date.ToUniversalTime().Year);
            Assert.AreEqual(08, date.ToUniversalTime().Month);
            Assert.AreEqual(03, date.ToUniversalTime().Day);
            Assert.AreEqual(14, date.ToUniversalTime().Hour);
            Assert.AreEqual(44, date.ToUniversalTime().Minute);
            Assert.AreEqual(32, date.ToUniversalTime().Second);
        }

        [TestMethod()]
        public void TestDateLOTS()
        {
            var dateStr = "2017-08-03T14:44:32.0034251451140Z";
            var date = dateStr.ToDateTime();

            Assert.AreEqual(2017, date.ToUniversalTime().Year);
            Assert.AreEqual(08, date.ToUniversalTime().Month);
            Assert.AreEqual(03, date.ToUniversalTime().Day);
            Assert.AreEqual(14, date.ToUniversalTime().Hour);
            Assert.AreEqual(44, date.ToUniversalTime().Minute);
            Assert.AreEqual(32, date.ToUniversalTime().Second);
        }


    }
}
