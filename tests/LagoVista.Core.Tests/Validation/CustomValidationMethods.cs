// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 0971bb6162536dcc89262b5a2ef779d3253be0aa51ec98cbb795ac068cf4f1f6
// IndexVersion: 1
// --- END CODE INDEX META ---
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using LagoVista.Core.Tests.Models;
using LagoVista.Core.Validation;

namespace LagoVista.Core.Tests.Validation
{
    [TestClass]
    public class CustomValidationMethods
    {
        [TestMethod]
        public void NotCalledCount_0_Valid()
        {
            var instance = new CustomValidation_ValidModel();
            Assert.AreEqual(0, instance.Called);
        }

        [TestMethod]
        public void ResultsOnlyParameter_MethodCall_Valid()
        {
            var instance = new CustomValidation_ValidModel();
            Validator.Validate(instance);
            Assert.AreEqual(1, instance.Called);
        }

        [TestMethod]
        public void No_Comparitor_InvalidSignature_NoExcpetion()
        {
            var instance = new CustomValidation_NoCustomValidation_Valid();
            Validator.Validate(instance);
            Assert.AreEqual(0, instance.Called);
        }

        [TestMethod]
        public void Test_Valid_BothParameters_MethodCall()
        {
            var instance = new CustomValidation_BothParameter_Valid();
            Validator.Validate(instance);
            Assert.AreEqual(1, instance.Called);
        }

        [TestMethod]
        public void Test_Valid_InvalidFirstType_MethodCall()
        {
            Assert.ThrowsExactly<InvalidOperationException>(() =>
            {
                var instance = new CustomValidation_WrongType_Invalid();
                Validator.Validate(instance);
            });
        }

        [TestMethod]
        public void Test_Valid_InvalidSecondType_MethodCall()
        {
            Assert.ThrowsExactly<InvalidOperationException>(() =>
            {
                var instance = new CustomValidation_WrongSecondType_Invalid();
                Validator.Validate(instance);
            });
        }

        [TestMethod]
        public void Test_Valid_NoParamameters_MethodCall()
        {
            Assert.ThrowsExactly<InvalidOperationException>(() =>
            {
                var instance = new CustomValidation_NoParameter_Invalid();
                Validator.Validate(instance);
            });
        }

        [TestMethod]
        public void Test_Valid_TooManyTypes_MethodCall()
        {
            Assert.ThrowsExactly<InvalidOperationException>(() =>
            {
                var instance = new CustomValidation_TooManyTypes_Invalid();
                Validator.Validate(instance);
            });
        }

        [TestMethod]
        public void Test_Call_Two_ValidationMethods()
        {
            var instance = new CustomValidation_TwoMethods_Valid();
            Validator.Validate(instance);
            Assert.AreEqual(2, instance.Called);
        }

        [TestMethod]
        public void Test_CustomValidation_TestOnInsert_Valid()
        {
            var instance = new CustomValidation_TestOnInsert
            {
                Field = "FOO"
            };
            var result = Validator.Validate(instance, Actions.Create);
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(1, instance.Called);
        }

        [TestMethod]
        public void Test_CustomValidation_TestOnInsert_Null_Invalid()
        {
            var instance = new CustomValidation_TestOnInsert
            {
                Field = null
            };
            var result = Validator.Validate(instance, Actions.Create);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.AreEqual("ERROR", result.Errors.First().Message);
            Assert.AreEqual(1, instance.Called);
        }

        [TestMethod]
        public void Test_CustomValidation_TestOnInsert_Null_Ignore_WithUpdate_Valid()
        {
            var instance = new CustomValidation_TestOnInsert
            {
                Field = null
            };
            var result = Validator.Validate(instance, Actions.Update);
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(1, instance.Called);
        }

        [TestMethod]
        public void Test_CustomValidation_TestOnUpdate_Valid()
        {
            var instance = new CustomValidation_TestOnInsert
            {
                Field = "FOO"
            };
            var result = Validator.Validate(instance, Actions.Update);
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(1, instance.Called);
        }

        [TestMethod]
        public void Test_CustomValidation_TestOnUpdate_Null_Invalid()
        {
            var instance = new CustomValidation_TestOnUpdate
            {
                Field = null
            };
            var result = Validator.Validate(instance, Actions.Update);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.AreEqual("ERROR", result.Errors.First().Message);
            Assert.AreEqual(1, instance.Called);
        }

        [TestMethod]
        public void Test_CustomValidation_TestOnUpdate_Null_Ignore_WithInsert_Valid()
        {
            var instance = new CustomValidation_TestOnUpdate
            {
                Field = null
            };
            var result = Validator.Validate(instance, Actions.Create);
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(1, instance.Called);
        }

    }
}
