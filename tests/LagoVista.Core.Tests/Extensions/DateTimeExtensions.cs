using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LagoVista.Core.Tests.Extensions
{
    [TestClass]
    public class DateTimeExtensions
    {
        [TestMethod]
        public void DateValidation_TestDate_Len20()
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

        [TestMethod]
        public void DateValidation_TestDate_Len21()
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


        [TestMethod]
        public void DateValidation_TestDate_Len22()
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


        [TestMethod]
        public void DateValidation_TestDate_Len23()
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

        [TestMethod]
        public void DateValidation_TestDate_Len24()
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

        [TestMethod]
        public void DateValidation_TestDateLOTS()
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

        [TestMethod]
        public void DateValidation_DateOnly()
        {
            var dateStr = "2020-08-03";
            var date = dateStr.ToDateTime();

            Assert.AreEqual(2020, date.Year);
            Assert.AreEqual(08, date.Month);
            Assert.AreEqual(03, date.Day);
        }

        [TestMethod]
        public void DateValidation_DateOnly1()
        {
            var dateStr = "2020-8-3";
            var date = dateStr.ToDateTime();

            Assert.AreEqual(2020, date.Year);
            Assert.AreEqual(08, date.Month);
            Assert.AreEqual(03, date.Day);
        }

        [TestMethod]
        public void DateValidation_DateOnly2()
        {
            var dateStr = "2020-10-3";
            var date = dateStr.ToDateTime();

            Assert.AreEqual(2020, date.Year);
            Assert.AreEqual(10, date.Month);
            Assert.AreEqual(03, date.Day);
        }

        [TestMethod]
        public void DateValidation_DateOnly3()
        {
            var dateStr = "2020/8/3";
            var date = dateStr.ToDateTime();

            Assert.AreEqual(2020, date.Year);
            Assert.AreEqual(08, date.Month);
            Assert.AreEqual(03, date.Day);
        }

        [TestMethod]
        public void DateValidation_ValidFormat()
        {
            var dateStr = "2017-08-03T14:44:32.0034251451140Z";
            Assert.IsTrue(dateStr.SuccessfulJSONDate());
        }

        [TestMethod]
        public void DateValidation_ValidFormat_OutOfRangeDate_Invalid()
        {
            var dateStr = "2017-08-03T14:99:32.0034251451140Z";
            Assert.IsFalse(dateStr.SuccessfulJSONDate());
        }


    }
}
