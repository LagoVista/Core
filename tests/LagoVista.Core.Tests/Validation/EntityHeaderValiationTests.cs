using LagoVista.Core.Models;
using LagoVista.Core.Tests.Models;
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
    public class EntityHeaderValiationTests
    {

        public Models.EntityHeaderModel GetValidEntityHeader()
        {
            return new Models.EntityHeaderModel()
            {
                NotRequired = EntityHeader.Create(Guid.NewGuid().ToId(), "SOME USER"),
                SystemRequired = EntityHeader.Create(Guid.NewGuid().ToId(), "SOME USER"),
                UserRequired = EntityHeader.Create(Guid.NewGuid().ToId(), "SOME USER"),
                UseRequiredCustomMessage = EntityHeader.Create(Guid.NewGuid().ToId(), "SOME USER"),
                PropWithEnum = new EntityHeader<EnumFortTesting>() { Id = "value1", Text = "Value1" }
            };
        }

        [TestMethod]
        public void EntityHeaderTest_Valid()
        {
            var entityHeader = GetValidEntityHeader();
            var result = Validator.Validate(entityHeader);
            Assert.IsTrue(result.Successful);
        }

        [TestMethod]
        public void EntityHeaderTest_NonRquiredMissing_Valid()
        {
            var entityHeader = GetValidEntityHeader();
            entityHeader.NotRequired = null;
            var result = Validator.Validate(entityHeader);
            Assert.IsTrue(result.Successful);
        }

        [TestMethod]
        public void EntityHeaderTest_NullMissingSystem_Valid()
        {
            var entityHeader = GetValidEntityHeader();
            entityHeader.SystemRequired = null;
            var result = Validator.Validate(entityHeader);
            Assert.IsFalse(result.Successful);

            Assert.AreEqual(LagoVista.Core.Resources.ValidationResource.Entity_Header_Null_System.Replace("[NAME]", nameof(entityHeader.SystemRequired)), result.Errors.First().Message);
            Assert.IsTrue(result.Errors.First().SystemError);
        }

        [TestMethod]
        public void EntityHeaderTest_BothMissingSystem_Valid()
        {
            var entityHeader = GetValidEntityHeader();
            entityHeader.SystemRequired.Id = null;
            entityHeader.SystemRequired.Text = null;
            var result = Validator.Validate(entityHeader);
            Assert.IsFalse(result.Successful);

            Assert.AreEqual(LagoVista.Core.Resources.ValidationResource.Entity_Header_Null_System.Replace("[NAME]", nameof(entityHeader.SystemRequired)), result.Errors.First().Message);
            Assert.IsTrue(result.Errors.First().SystemError);
        }


        [TestMethod]
        public void EntityHeaderTest_IdMissingSystem_Valid()
        {
            var entityHeader = GetValidEntityHeader();
            entityHeader.SystemRequired.Id = null;
            var result = Validator.Validate(entityHeader);
            Assert.IsFalse(result.Successful);

            Assert.AreEqual(LagoVista.Core.Resources.ValidationResource.Entity_Header_MissingId_System.Replace("[NAME]", nameof(entityHeader.SystemRequired)), result.Errors.First().Message);
            Assert.IsTrue(result.Errors.First().SystemError);
        }


        [TestMethod]
        public void EntityHeaderTest_TextMissingSystem_Valid()
        {
            var entityHeader = GetValidEntityHeader();
            entityHeader.SystemRequired.Text = null;
            var result = Validator.Validate(entityHeader);
            Assert.IsFalse(result.Successful);

            Assert.AreEqual(LagoVista.Core.Resources.ValidationResource.Entity_Header_MissingText_System.Replace("[NAME]", nameof(entityHeader.SystemRequired)), result.Errors.First().Message);
            Assert.IsTrue(result.Errors.First().SystemError);
        }

        [TestMethod]
        public void EntityHeaderTest_NullPropertyMessage_Valid()
        {
            var entityHeader = GetValidEntityHeader();
            entityHeader.UserRequired = null;
            var result = Validator.Validate(entityHeader);
            Assert.IsFalse(result.Successful);

            Assert.AreEqual(LagoVista.Core.Resources.ValidationResource.PropertyIsRequired.Replace("[PROPERTYLABEL]", ValidationResources.FirstNameLabel), result.Errors.First().Message);
            Assert.IsFalse(result.Errors.First().SystemError);
        }

        [TestMethod]
        public void EntityHeaderTest_BothMissingPropertyMessage_Valid()
        {
            var entityHeader = GetValidEntityHeader();
            entityHeader.UserRequired.Id = null;
            entityHeader.UserRequired.Text = null;
            var result = Validator.Validate(entityHeader);
            Assert.IsFalse(result.Successful);

            Assert.AreEqual(LagoVista.Core.Resources.ValidationResource.PropertyIsRequired.Replace("[PROPERTYLABEL]", ValidationResources.FirstNameLabel), result.Errors.First().Message);
            Assert.IsFalse(result.Errors.First().SystemError);
        }


        [TestMethod]
        public void EntityHeaderTest_MissingIdPropertyMessage_Valid()
        {
            var entityHeader = GetValidEntityHeader();
            entityHeader.UserRequired.Id = null;
            var result = Validator.Validate(entityHeader);
            Assert.IsFalse(result.Successful);

            Assert.AreEqual(LagoVista.Core.Resources.ValidationResource.Entity_Header_MissingId_System.Replace("[NAME]", nameof(entityHeader.UserRequired)), result.Errors.First().Message);
            Assert.IsTrue(result.Errors.First().SystemError);
        }

        [TestMethod]
        public void EntityHeaderTest_MissingTextPropertyMessage_Valid()
        {
            var entityHeader = GetValidEntityHeader();
            entityHeader.UserRequired.Text = null;
            var result = Validator.Validate(entityHeader);
            Assert.IsFalse(result.Successful);

            Assert.AreEqual(LagoVista.Core.Resources.ValidationResource.Entity_Header_MissingText_System.Replace("[NAME]", nameof(entityHeader.UserRequired)), result.Errors.First().Message);
            Assert.IsTrue(result.Errors.First().SystemError);
        }

        [TestMethod]
        public void EntityHeaderTest_NullCustomMessage_Valid()
        {
            var entityHeader = GetValidEntityHeader();
            entityHeader.UseRequiredCustomMessage = null;
            var result = Validator.Validate(entityHeader);
            Assert.IsFalse(result.Successful);

            Assert.AreEqual(ValidationResources.CustomValidationMessage, result.Errors.First().Message);
            Assert.IsFalse(result.Errors.First().SystemError);
        }

        [TestMethod]
        public void EntityHeaderTest_BothMissingCustomMessage_Valid()
        {
            var entityHeader = GetValidEntityHeader();
            entityHeader.UseRequiredCustomMessage.Id = null;
            entityHeader.UseRequiredCustomMessage.Text = null;
            var result = Validator.Validate(entityHeader);
            Assert.IsFalse(result.Successful);

            Assert.AreEqual(ValidationResources.CustomValidationMessage, result.Errors.First().Message);
            Assert.IsFalse(result.Errors.First().SystemError);
        }


        [TestMethod]
        public void EntityHeaderTest_MissingIdPCustomMessage_Invalid()
        {
            var entityHeader = GetValidEntityHeader();
            entityHeader.UseRequiredCustomMessage.Id = null;
            var result = Validator.Validate(entityHeader);
            Assert.IsFalse(result.Successful);

            Assert.AreEqual(LagoVista.Core.Resources.ValidationResource.Entity_Header_MissingId_System.Replace("[NAME]", nameof(entityHeader.UseRequiredCustomMessage)), result.Errors.First().Message);
            Assert.IsTrue(result.Errors.First().SystemError);
        }

        [TestMethod]
        public void EntityHeaderTest_MissingTextCustomMessage_Invalid()
        {
            var entityHeader = GetValidEntityHeader();
            entityHeader.UseRequiredCustomMessage.Text = null;
            var result = Validator.Validate(entityHeader);
            Assert.IsFalse(result.Successful);

            Assert.AreEqual(LagoVista.Core.Resources.ValidationResource.Entity_Header_MissingText_System.Replace("[NAME]", nameof(entityHeader.UseRequiredCustomMessage)), result.Errors.First().Message);
            Assert.IsTrue(result.Errors.First().SystemError);
        }

        [TestMethod]
        public void EntityHeader_TestNullEnumType_Invalid()
        {
            var entityHeader = GetValidEntityHeader();
            entityHeader.PropWithEnum = null;
            var result = Validator.Validate(entityHeader);
            Assert.IsFalse(result.Successful);
        }

        [TestMethod]
        public void EntityHeader_TestMissingId_Invalid()
        {
            var entityHeader = GetValidEntityHeader();
            entityHeader.PropWithEnum.Text = String.Empty;
            var result = Validator.Validate(entityHeader);
            Assert.IsFalse(result.Successful);
        }

        [TestMethod]
        public void EntityHeader_TestMissingText_Invalid()
        {
            var entityHeader = GetValidEntityHeader();
            entityHeader.PropWithEnum.Id = String.Empty;
            var result = Validator.Validate(entityHeader);
            Assert.IsFalse(result.Successful);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void EntityHeader_TestInvalidEnum_Invalid()
        {
            var entityHeader = GetValidEntityHeader();
            entityHeader.PropWithEnum.Id = "thisisntenumvalue";
        }

    }
}
