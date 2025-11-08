// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 674fecac8dbc4e2360823b6889ba864e28755c22760a666deae73da616e70bd3
// IndexVersion: 2
// --- END CODE INDEX META ---
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.Tests.Validation
{
    [TestClass]
    public class TimeTests
    {
        [TestMethod]
        public void Test_Valid_Time_Strings()
        {
            Assert.IsTrue("12:32".IsValidTime());
            Assert.IsTrue("23:12".IsValidTime());
            Assert.IsTrue("03:02".IsValidTime());
            Assert.IsTrue("00:00".IsValidTime());
            Assert.IsTrue("23:59".IsValidTime());
        }


        [TestMethod]
        public void Test_InValid_Time_Strings()
        {
            Assert.IsFalse("".IsValidTime());
            Assert.IsFalse("32:32".IsValidTime());
            Assert.IsFalse("3:12".IsValidTime());
            Assert.IsFalse("03:60".IsValidTime());
            Assert.IsFalse("A7:60".IsValidTime());
            Assert.IsFalse("03:B3".IsValidTime());
        }

    }
}
