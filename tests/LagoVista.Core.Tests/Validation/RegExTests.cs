using LagoVista.Core.Tests.Resources.Models;
using LagoVista.Core.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace LagoVista.Core.Tests.Validation
{
    
    public class RegExTests
    {

        [Fact]
        public void RegEx_Valid()
        {
            var regExModel = new Models.RegExModel()
            {
                Key = "abc123",
                RegExValue_2_20_lower_case = "ab"
            };

            var validationResult = Validator.Validate(regExModel);
            Assert.True(validationResult.Successful);
        }

        [Fact]
        public void RegEx_ManualRegEx_Invalid()
        {
            var regExModel = new Models.RegExModel()
            {
                Key = "abc123",
                RegExValue_2_20_lower_case = "XX"
            };

            var validationResult = Validator.Validate(regExModel);
            Assert.False(validationResult.Successful);
            Assert.Single(validationResult.Errors.Where(err => err.Message == ValidationResources.RegExMessage));
        }

        [Fact]
        public void RegEx_Key_Invalid()
        {
            var regExModel = new Models.RegExModel()
            {
                Key = "AXXXX",
                RegExValue_2_20_lower_case = "aa"
            };

            var validationResult = Validator.Validate(regExModel);
            Assert.False(validationResult.Successful);
            Assert.Single(validationResult.Errors.Where(err => err.Message == LagoVista.Core.Resources.ValidationResource.Common_Key_Validation));
        }
    }
}
