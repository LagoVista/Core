// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: ae4b03000fecc10afb2e84361f36ce313944cab3b9de74e0f4c8c584bdb52f72
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.Tests.Models
{
    [TestClass]
    public class EntityHeaderCreate
    {
        public class MyClass : IIDEntity, INamedEntity
        {
            public string Name { get; set; }

            public string Id { get; set; }
        }

        [TestMethod]
        public void Should_Create_EH_With_Value()
        {
            var eh = EntityHeader<MyClass>.Create(new MyClass()
            {
                Id = "Kevin",
                Name = "Wolf"
            });

            Assert.AreEqual("Kevin", eh.Id);
            Assert.AreEqual("Wolf", eh.Text);
            Assert.AreEqual("Kevin", eh.Value.Id);
            Assert.AreEqual("Wolf", eh.Value.Name);
        }


        [TestMethod]
        public void Should_Create_EH_WithOut_Value()
        {
            var eh = EntityHeader<MyClass>.Create("TheId", "TheText");

            Assert.AreEqual("TheId", eh.Id);
            Assert.AreEqual("TheText", eh.Text);
        }


    }
}
