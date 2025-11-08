// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 5eb409556efa556a0f9e41ad7b4dd24bc037c038d5ddf33538de86f538897159
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LagoVista.Core.Tests.Validation
{
    [TestClass]
    public class ChildEntityHeaderTests : ValidationTestBase
    {
        private Models.EntityHeaderChildValueModels GetValidModel()
        {
            var model = new Models.EntityHeaderChildValueModels()
            {
                IsRequiredTopLevelProperty = new Core.Models.EntityHeader<Models.EhChildModel>()
                {
                    Id = "8187A93217C745BBA471D6E16722A793",
                    Text = "REQCHILD",
                    Value = new Models.EhChildModel()
                    {
                        IsRequiredChildProperty = "has value",
                        GrandChild = new Core.Models.EntityHeader<Models.EhGrandChildModel>()
                        {
                            Id = "8187A93217C745BBA471D6E16722A793",
                            Text = "REQGC",
                            Value = new Models.EhGrandChildModel()
                            {
                                IsRequiredGrandChildProperty = "grandchild has value"
                            }
                        }
                    }
                }
            };

            return model;
        }

        [TestMethod]
        public void EntityHeader_ValidModel()
        {
            var model = GetValidModel();

            var result = Validator.Validate(model, requirePopulatedEHValues:true);
            
            AssertIsValid(result);
        }

        [TestMethod]
        public void EntityHeader_ChildHasEHButValueNotPopulated_And_NotRequireFullObjectGraphTraversal()
        {
            var model = GetValidModel();
            model.IsRequiredTopLevelProperty.Value = null;


            ///By default rquirePopulatedEHValues will be false, include to make a bit more clear
            var result = Validator.Validate(model, requirePopulatedEHValues : false);           

            AssertIsValid(result);
        }

        [TestMethod]
        public void EntityHeader_ChildHasEHButValueNotPopulated_And_RequireFullObjectGraphTraversal()
        {
            var model = GetValidModel();
            model.IsRequiredTopLevelProperty.Value = null;

            var result = Validator.Validate(model, requirePopulatedEHValues: true);

            AssertIsInValid(result);
        }


        [TestMethod]
        public void EntityHeader_ChildMissingInvalid()
        {
            var model = GetValidModel();
            model.IsRequiredTopLevelProperty = null;

            var result = Validator.Validate(model);
            
            AssertIsInValid(result);
        }


        [TestMethod]
        public void EntityHeader_GrandChild_MissingChild_Invalid()
        {
            var model = GetValidModel();
            model.IsRequiredTopLevelProperty = null;
            var result = Validator.Validate(model);

            AssertIsInValid(result);
        }

        [TestMethod]
        public void EntityHeader_GrandChild_ChildMissingChild_Invalid()
        {
            var model = GetValidModel();
            model.IsRequiredTopLevelProperty.Value.GrandChild.Value.IsRequiredGrandChildProperty = null;
            var result = Validator.Validate(model);
            AssertIsInValid(result);
        }
    }
}
