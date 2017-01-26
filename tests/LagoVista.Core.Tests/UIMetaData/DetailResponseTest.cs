using LagoVista.Core.Models.UIMetaData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace LagoVista.Core.Tests.UIMetaData
{
    public class DetailResponseTest
    {
        [Fact(DisplayName ="DetailResponses_Should_Be_PopulatedFromModel")]
        public void DetailResponses_Should_Be_PopulatedFromModel()
        {
            var model1 = new Model2();
            model1.Field1 = "MY VALUE";
            var response = DetailResponse<Model2>.Create(model1);

            Assert.Equal("MY VALUE", response.View["field1"].Value);
        }

        [Fact(DisplayName = "DetailResponses_Should_Be_PopulatedFromModel_WithEnum")]
        public void DetailResponses_Should_Be_PopulatedFromModel_WithEnum()
        {
            var model1 = new Model1();
            model1.Field1 = "MY VALUE";
            var response = DetailResponse<Model1>.Create(model1);

            Assert.Equal("MY VALUE", response.View["field1"].Value);
        }
    }
}
