using Xunit;
using System;
using System.Linq;
using LagoVista.Core.Tests.Models;
using LagoVista.Core.Validation;

namespace LagoVista.Core.Tests.Validation
{
    public class CustomValidationMethods
    {
        [Fact]
        public void NotCalledCount_0_Valid()
        {
            var instance = new CustomValidation_ValidModel();
            Assert.Equal(0, instance.Called);
        }

        [Fact]
        public void ResultsOnlyParameter_MethodCall_Valid()
        {
            var instance = new CustomValidation_ValidModel();
            Validator.Validate(instance);
            Assert.Equal(1, instance.Called);
        }

        [Fact]
        public void No_Comparitor_InvalidSignature_NoExcpetion()
        {
            var instance = new CustomValidation_NoCustomValidation_Valid();
            Validator.Validate(instance);
            Assert.Equal(0, instance.Called);
        }

        [Fact]
        public void Test_Valid_BothParameters_MethodCall()
        {
            var instance = new CustomValidation_BothParameter_Valid();
            Validator.Validate(instance);
            Assert.Equal(1, instance.Called);
        }

        [Fact]
        public void Test_Valid_InvalidFirstType_MethodCall()
        {
            var instance = new CustomValidation_WrongType_Invalid();
            Assert.Throws<InvalidOperationException>(() => Validator.Validate(instance));
        }

        [Fact]
        public void Test_Valid_InvalidSecondType_MethodCall()
        {
            var instance = new CustomValidation_WrongSecondType_Invalid();
            Assert.Throws<InvalidOperationException>(() => Validator.Validate(instance));
        }

        [Fact]
        public void Test_Valid_NoParamameters_MethodCall()
        {
            var instance = new CustomValidation_NoParameter_Invalid();
            Assert.Throws<InvalidOperationException>(() => Validator.Validate(instance));
        }

        [Fact]
        public void Test_Valid_TooManyTypes_MethodCall()
        {
            var instance = new CustomValidation_TooManyTypes_Invalid();
            Assert.Throws<InvalidOperationException>(() => Validator.Validate(instance));
        }

        [Fact]
        public void Test_Call_Two_ValidationMethods()
        {
            var instance = new CustomValidation_TwoMethods_Valid();
            Validator.Validate(instance);
            Assert.Equal(2, instance.Called);
        }

        [Fact]
        public void Test_CustomValidation_TestOnInsert_Valid()
        {
            var instance = new CustomValidation_TestOnInsert
            {
                Field = "FOO"
            };
            var result = Validator.Validate(instance, Actions.Create);
            Assert.Empty(result.Errors);
            Assert.Equal(1, instance.Called);
        }

        [Fact]
        public void Test_CustomValidation_TestOnInsert_Null_Invalid()
        {
            var instance = new CustomValidation_TestOnInsert
            {
                Field = null
            };
            var result = Validator.Validate(instance, Actions.Create);
            Assert.Single(result.Errors);
            Assert.Equal("ERROR", result.Errors.First().Message);
            Assert.Equal(1, instance.Called);
        }

        [Fact]
        public void Test_CustomValidation_TestOnInsert_Null_Ignore_WithUpdate_Valid()
        {
            var instance = new CustomValidation_TestOnInsert
            {
                Field = null
            };
            var result = Validator.Validate(instance, Actions.Update);
            Assert.Empty(result.Errors);
            Assert.Equal(1, instance.Called);
        }

        [Fact]
        public void Test_CustomValidation_TestOnUpdate_Valid()
        {
            var instance = new CustomValidation_TestOnInsert
            {
                Field = "FOO"
            };
            var result = Validator.Validate(instance, Actions.Update);
            Assert.Empty(result.Errors);
            Assert.Equal(1, instance.Called);
        }

        [Fact]
        public void Test_CustomValidation_TestOnUpdate_Null_Invalid()
        {
            var instance = new CustomValidation_TestOnUpdate
            {
                Field = null
            };
            var result = Validator.Validate(instance, Actions.Update);
            Assert.Single(result.Errors);
            Assert.Equal("ERROR", result.Errors.First().Message);
            Assert.Equal(1, instance.Called);
        }

        [Fact]
        public void Test_CustomValidation_TestOnUpdate_Null_Ignore_WithInsert_Valid()
        {
            var instance = new CustomValidation_TestOnUpdate
            {
                Field = null
            };
            var result = Validator.Validate(instance, Actions.Create);
            Assert.Empty(result.Errors);
            Assert.Equal(1, instance.Called);
        }

    }
}
