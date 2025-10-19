// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 61159f98de2e885935ddb0fb7c0d5044ba60283cdef6eaa7d64b62530d738881
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core.Models.UIMetaData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

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

        [TestMethod]
        public void ShouldBeDiscussableEntity()
        {
            var model1 = new Model1();
            var response = DetailResponse<Model1>.Create(model1);
            Assert.IsTrue(response.HasDiscussions);
        }

        [TestMethod]
        public void DetailResponse_Get_Child_View()
        {
            var model1 = new Model1();
            model1.Field5 = new Model2();

            var response = DetailResponse<Model1>.Create(model1);
            var childForm = response.View[nameof(Model1.Field5).ToLower()];
            Assert.IsNotNull(childForm.View);
            Assert.IsTrue(childForm.View.Count > 0);

            foreach (var child in childForm.View)
            {
                Console.WriteLine(child.Key);
            }
        }
    }
}
