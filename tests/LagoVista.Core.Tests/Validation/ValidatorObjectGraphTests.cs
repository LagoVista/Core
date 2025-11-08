// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 5bfa88ea58dfb87a4eb46592029604e8519acbb07347b435c3fb022b37107aa0
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
    public class ValidatorObjectGraphTests : ValidationTestBase
    {
        public Models.ValidationModel GetValidModel()
        {
            return new Models.ValidationModel()
            {
                ParentRequiredProperty = "I AM HERE",
                SystemRequired = "KEVIN",
                CustomValidationMessage = "CUStoM",
                LabelBasedValidationMessage = "LABEL",
                PropertyBasedValidationMessage = "FOO"
            };
        }

        public Models.ValidatableObjectGraph GetValidObjectGraph()
        {
            var graph = new Models.ValidatableObjectGraph()
            {
                ChildModel = GetValidModel(),
                ChildModels = new System.Collections.Generic.List<Models.ValidationModel>()
            };

            graph.ChildModels.Add(GetValidModel());
            graph.ChildModels.Add(GetValidModel());

            return graph;
        }

        [TestMethod]
        public void Validator_ObjectGraph_Valid()
        {
            var objectGraph = GetValidObjectGraph();

            var result = Validator.Validate(objectGraph);

            AssertIsValid(result);            
        }

        [TestMethod]
        public void Validator_ObjectGraph_PropertyInvalid()
        {
            /* Test to ensure validation takes place on all child single instance objects marked as IValidatable */
            var objectGraph = GetValidObjectGraph();
            objectGraph.ChildModel.SystemRequired = null;
            var result = Validator.Validate(objectGraph);            

            AssertIsInValid(result);
        }

        [TestMethod]
        public void Validator_ObjectGraph_PropertyOnListIsInvalid()
        {
            /* Test to ensure validation takes place on all child single instance within list */
            var objectGraph = GetValidObjectGraph();
            objectGraph.ChildModels[0].SystemRequired = null;
            var result = Validator.Validate(objectGraph);

            AssertIsInValid(result);
        }
    }
}
