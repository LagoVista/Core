using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace LagoVista.Core.Tests.Extensions
{    
    public class DateTimeExtensions
    {
        [Fact()]
        public void DateValidation_TestDate_Len20()
        {
            var dateStr = "2017-08-03T14:44:32Z";
            var date = dateStr.ToDateTime();

            Assert.Equal(2017, date.ToUniversalTime().Year);
            Assert.Equal(08, date.ToUniversalTime().Month);
            Assert.Equal(03, date.ToUniversalTime().Day);
            Assert.Equal(14, date.ToUniversalTime().Hour);
            Assert.Equal(44, date.ToUniversalTime().Minute);
            Assert.Equal(32, date.ToUniversalTime().Second);
        }

        [Fact()]
        public void DateValidation_TestDate_Len21()
        {
            var dateStr = "2017-08-03T14:44:32.Z";
            var date = dateStr.ToDateTime();

            Assert.Equal(2017, date.ToUniversalTime().Year);
            Assert.Equal(08, date.ToUniversalTime().Month);
            Assert.Equal(03, date.ToUniversalTime().Day);
            Assert.Equal(14, date.ToUniversalTime().Hour);
            Assert.Equal(44, date.ToUniversalTime().Minute);
            Assert.Equal(32, date.ToUniversalTime().Second);
        }


        [Fact()]
        public void DateValidation_TestDate_Len22()
        {
            var dateStr = "2017-08-03T14:44:32.0Z";
            var date = dateStr.ToDateTime();

            Assert.Equal(2017, date.ToUniversalTime().Year);
            Assert.Equal(08, date.ToUniversalTime().Month);
            Assert.Equal(03, date.ToUniversalTime().Day);
            Assert.Equal(14, date.ToUniversalTime().Hour);
            Assert.Equal(44, date.ToUniversalTime().Minute);
            Assert.Equal(32, date.ToUniversalTime().Second);
        }


        [Fact()]
        public void DateValidation_TestDate_Len23()
        {
            var dateStr = "2017-08-03T14:44:32.00Z";
            var date = dateStr.ToDateTime();

            Assert.Equal(2017, date.ToUniversalTime().Year);
            Assert.Equal(08, date.ToUniversalTime().Month);
            Assert.Equal(03, date.ToUniversalTime().Day);
            Assert.Equal(14, date.ToUniversalTime().Hour);
            Assert.Equal(44, date.ToUniversalTime().Minute);
            Assert.Equal(32, date.ToUniversalTime().Second);
        }

        [Fact()]
        public void DateValidation_TestDate_Len24()
        {
            var dateStr = "2017-08-03T14:44:32.000Z";
            var date = dateStr.ToDateTime();

            Assert.Equal(2017, date.ToUniversalTime().Year);
            Assert.Equal(08, date.ToUniversalTime().Month);
            Assert.Equal(03, date.ToUniversalTime().Day);
            Assert.Equal(14, date.ToUniversalTime().Hour);
            Assert.Equal(44, date.ToUniversalTime().Minute);
            Assert.Equal(32, date.ToUniversalTime().Second);
        }

        [Fact()]
        public void DateValidation_TestDateLOTS()
        {
            var dateStr = "2017-08-03T14:44:32.0034251451140Z";
            var date = dateStr.ToDateTime();

            Assert.Equal(2017, date.ToUniversalTime().Year);
            Assert.Equal(08, date.ToUniversalTime().Month);
            Assert.Equal(03, date.ToUniversalTime().Day);
            Assert.Equal(14, date.ToUniversalTime().Hour);
            Assert.Equal(44, date.ToUniversalTime().Minute);
            Assert.Equal(32, date.ToUniversalTime().Second);
        }

        [Fact()]
        public void DateValidation_ValidFormat()
        {
            var dateStr = "2017-08-03T14:44:32.0034251451140Z";
            Assert.True(dateStr.SuccessfulJSONDate());
        }

        [Fact()]
        public void DateValidation_ValidFormat_OutOfRangeDate_Invalid()
        {
            var dateStr = "2017-08-03T14:99:32.0034251451140Z";
            Assert.False(dateStr.SuccessfulJSONDate());
        }


    }
}
