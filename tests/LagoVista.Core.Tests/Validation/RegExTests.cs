using LagoVista.Core.Tests.Resources.Models;
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
    public class RegExTests
    {

        [TestMethod]
        public void RegEx_Valid()
        {
            var regExModel = new Models.RegExModel()
            {
                Key = "abc123",
                RegExValue_2_20_lower_case = "ab"
            };

            var validationResult = Validator.Validate(regExModel);
            Assert.IsTrue(validationResult.Successful);
        }

        [TestMethod]
        public void RegEx_ManualRegEx_Invalid()
        {
            var regExModel = new Models.RegExModel()
            {
                Key = "abc123",
                RegExValue_2_20_lower_case = "XX"
            };

            var validationResult = Validator.Validate(regExModel);
            Assert.IsFalse(validationResult.Successful);
            Assert.AreEqual(1, validationResult.Errors.Where(err => err.Message == ValidationResources.RegExMessage).Count());
        }

        [TestMethod]
        public void RegEx_Key_Invalid()
        {
            var regExModel = new Models.RegExModel()
            {
                Key = "AXXXX",
                RegExValue_2_20_lower_case = "aa"
            };

            var validationResult = Validator.Validate(regExModel);
            Assert.IsFalse(validationResult.Successful);
            Assert.AreEqual(1, validationResult.Errors.Where(err => err.Message == LagoVista.Core.Resources.ValidationResource.Common_Key_Validation).Count());
        }
    }
}
