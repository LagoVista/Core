using LagoVista.Core.Models.UIMetaData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LagoVista.Core.Tests.UIMetaData
{
    [TestClass]
    public class DetailResponseTest
    {
        [TestMethod]
        public void DetailResponses_Should_Be_PopulatedFromModel()
        {
            var model1 = new Model2();
            model1.Field1 = "MY VALUE";
            var response = DetailResponse<Model2>.Create(model1);

            Assert.AreEqual("MY VALUE", response.View["field1"].DefaultValue);
        }

        [TestMethod]
        public void DetailResponses_Should_Be_PopulatedFromModel_WithEnum()
        {
            var model1 = new Model1();
            model1.Field1 = "MY VALUE";
            var response = DetailResponse<Model1>.Create(model1);

            Assert.AreEqual("MY VALUE", response.View["field1"].DefaultValue);
        }
    }
}
