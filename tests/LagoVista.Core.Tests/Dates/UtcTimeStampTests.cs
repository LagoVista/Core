using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.Tests.Dates
{

    [TestFixture]
    public class UtcTimeStampTests
    {
        private class Simple
        {
            public UtcTimestamp? Timestamp { get; set; }
        }


        [Test]
        public void Should_Assign_To_Nullable()
        {
            var simple = new Simple { Timestamp = null };
            Assert.That(!simple.Timestamp.HasValue);   
        }



        [Test]
        [Obsolete]
        public void Should_Assign_To_Nullable_In_True_Ternary_Expression()
        {
            var simple = new Simple();
            simple.Timestamp = true ? UtcTimestamp.Now : null;
            Assert.That(simple.Timestamp.HasValue);
        }

        [Test]
        public void Should_Assign_To_Nullable_In_False_Ternary_Expression()
        {
            var simple = new Simple();
            simple.Timestamp = false ? UtcTimestamp.Now : (UtcTimestamp?)null;
            Assert.That(!simple.Timestamp.HasValue);
        }




    }
}
