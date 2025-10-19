// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 52f91dfa79fe1bb5d8412f0357513e86780a331a43e401f2337468b0007648b3
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.Tests.Operators
{
    [TestClass]
    public class EntityHeaderEqualsOperatorTests
    {
        [TestMethod]
        public void Test_Null_Equals()
        {
            EntityHeader hdr1 = null;
            EntityHeader hdr2 = null;
            Assert.IsTrue(hdr1 == hdr2);
        }

        [TestMethod]
        public void Test_Empty_Equals()
        {
            var hdr1 = new EntityHeader();
            var hdr2 = new EntityHeader();
            var isTrue = hdr1 == hdr2;

            Assert.IsTrue(isTrue);
        }

        [TestMethod]
        public void Test_Compare_To_Null()
        {
            var id = Guid.NewGuid().ToId();
            var hdr1 = EntityHeader.Create(id, "FIRST NAME");
            var isTrue = hdr1 != null;

            Assert.IsTrue(isTrue);
        }


        [TestMethod]
        public void Test_Compare_To_NotNull()
        {
            EntityHeader hdr1 = null;
            var isTrue = hdr1 == null;

            Assert.IsTrue(isTrue);
        }

        [TestMethod]
        public void Test_FirstNull_SecondNotEmpty_NotEquals()
        {
            var id = Guid.NewGuid().ToId();
            EntityHeader hdr1 = null;
            var hdr2 = EntityHeader.Create(id, "FIRST NAME");            

            Assert.IsFalse(hdr1 == hdr2);
        }

        [TestMethod]
        public void Test_SecondNull_FirstNotEmpty_NotEquals()
        {
            var id = Guid.NewGuid().ToId();
            var hdr1 = EntityHeader.Create(id, "FIRST NAME");
            EntityHeader hdr2 = null;

            Assert.IsFalse(hdr1 == hdr2);
        }

        [TestMethod]
        public void Test_FirstNull_SecondEmpty_Equals()
        {
            EntityHeader hdr1 = null;
            var hdr2 = new EntityHeader();            

            Assert.IsTrue(hdr1 == hdr2);
        }

        [TestMethod]
        public void Test_SecondNull_FirstEmpty_Equals()
        {
            var hdr1 = new EntityHeader();
            EntityHeader hdr2 = null;

            Assert.IsTrue(hdr1 == hdr2);
        }

        [TestMethod]
        public void Test_PopulatedMatch_Equals()
        {
            var id = Guid.NewGuid().ToId();
            var hdr1 = EntityHeader.Create(id, "FIRST NAME");
            var hdr2 = EntityHeader.Create(id, "FIRST NAME");

            Assert.IsTrue(hdr1 == hdr2);
        }


        [TestMethod]
        public void Test_PopulatedIdMatchTextNoMatch_NotEquals()
        {
            var id = Guid.NewGuid().ToId();
            var hdr1 = EntityHeader.Create(id, "FIRST NAME");
            var hdr2 = EntityHeader.Create(id, "LAST NAME");

            Assert.IsFalse(hdr1 == hdr2);
        }

        [TestMethod]
        public void Test_PopulatedTextMatchIdNoMatch_NotEquals()
        {
            var hdr1 = EntityHeader.Create(Guid.NewGuid().ToId(), "FIRST NAME");
            var hdr2 = EntityHeader.Create(Guid.NewGuid().ToId(), "FIRST NAME");

            Assert.IsFalse(hdr1 == hdr2);
        }


    }
}
