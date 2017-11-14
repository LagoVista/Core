using LagoVista.Core.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace LagoVista.Core.Tests.Validation
{
    public class ChildEntityHeaderTests : ValidationTestBase
    {
        private Models.EntityHeaderChildValueModels GetValidModel()
        {
            var model = new Models.EntityHeaderChildValueModels()
            {
                Required = new Core.Models.EntityHeader<Models.EhChildModel>()
                {
                    Id = "8187A93217C745BBA471D6E16722A793",
                    Text = "REQCHILD",
                    Value = new Models.EhChildModel()
                    {
                        IsRequiredProp = "has value",
                        GrandChild = new Core.Models.EntityHeader<Models.EhGrandChildModel>()
                        {
                            Id = "8187A93217C745BBA471D6E16722A793",
                            Text = "REQGC",
                            Value = new Models.EhGrandChildModel()
                            {

                            }
                        }
                    }
                }
            };

            return model;
        }

        [Fact]
        public void ValidateGrandChild()
        {
            var model = GetValidModel();
            var result = Validator.Validate(model);

            AssertIsValid(result);
        }


        [Fact]
        public void EntityHeader_GrandChild_MissingChild_Valid()
        {
            var model = GetValidModel();
            model.Required = null;
            var result = Validator.Validate(model);

            AssertIsInValid(result);
        }

        [Fact]
        public void EntityHeader_GrandChild_ChildMissingChild_Invalid()
        {
            var model = GetValidModel();
            model.Required.Value.GrandChild.Value.IsRequiredProp = null;
            var result = Validator.Validate(model);

            AssertIsInValid(result);
        }
    }
}
