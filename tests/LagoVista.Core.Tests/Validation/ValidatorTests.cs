// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 6f34d1f6a760c7cc09d52a0fd11f8cf267bc07b2bfa9671eae4c9bf4f85bf1c6
// IndexVersion: 1
// --- END CODE INDEX META ---
using LagoVista.Core.Models;
using LagoVista.Core.Tests.Resources.Models;
using LagoVista.Core.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace LagoVista.Core.Tests.Validation
{
    [TestClass]
    public class ValidatorTests
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

        public Models.AuditableModel GetValidAuditableModel()
        {
            var auditEntity = new Models.AuditableModel();
            auditEntity.SetCreationUpdatedFields(EntityHeader.Create(Guid.NewGuid().ToId(), "SOME USER"));

            return auditEntity;
        }

        public Models.StringLengthModel GetValidStringLengthModel()
        {
            return new Models.StringLengthModel()
            {
                BetweenStringLength_5_15 = "KEVIN D WOLF", //11 chars
                MinStringLength_5 = "KEVIN D WOLF", /// 11 chars
                MaxStringLength_15 = "KEVIN D WOLF"
            };
        }

        private void WriteResults(ValidationResult result)
        {
            foreach (var error in result.Errors)
            {
                Console.WriteLine(error);
            }
        }

        [TestMethod]
        public void ConcatErrorsTest()
        {
            var result1 = new ValidationResult();
            result1.Errors.Add(new ErrorMessage("BAD"));

            var result2 = new ValidationResult();
            result2.Concat(result1);

            Assert.AreEqual(1, result2.Errors.Where(err => err.Message == "BAD").Count());
        }

        [TestMethod]
        public void Validate_PropertiesThatImplementIValidatable()
        {

        }

        [TestMethod]
        public void Validate_GenericListPropertiesThatImplementIValidatable()
        {

        }

        [TestMethod]
        public void IsRequired_Valid()
        {
            var result = Validator.Validate(GetValidModel());
            Assert.IsTrue(result.Successful);
        }

        [TestMethod]
        public void IsRequired_InValid()
        {
            var model = GetValidModel();
            model.SystemRequired = null;
            var result = Validator.Validate(model);
            Assert.IsFalse(result.Successful);
        }

        [TestMethod]
        public void IsRequired_InInvalidIfBasePropertyNotSet()
        {
            var model = GetValidModel();
            model.ParentRequiredProperty = null;
            var result = Validator.Validate(model);
            Assert.IsFalse(result.Successful);
        }

        [TestMethod]
        public void IsRequired_InValid_DefaultMessage()
        {
            var model = GetValidModel();
            model.PropertyBasedValidationMessage = null;
            var result = Validator.Validate(model);
            Assert.AreEqual(LagoVista.Core.Resources.ValidationResource.SystemMissingProperty.Replace("[PROPERTYNAME]", "PropertyBasedValidationMessage"), result.Errors.FirstOrDefault().Message);
        }

        [TestMethod]
        public void IsRequired_InValid_LabelMessage()
        {
            var model = GetValidModel();
            model.LabelBasedValidationMessage = null;
            var result = Validator.Validate(model);
            Assert.AreEqual(LagoVista.Core.Resources.ValidationResource.PropertyIsRequired.Replace("[PROPERTYLABEL]", ValidationResources.FirstNameLabel), result.Errors.FirstOrDefault().Message);
        }

        [TestMethod]
        public void IsRequired_InValid_CustomMessage()
        {
            var model = GetValidModel();
            model.CustomValidationMessage = null;
            var result = Validator.Validate(model);
            Assert.AreEqual(ValidationResources.CustomValidationMessage, result.Errors.FirstOrDefault().Message);
        }

        [TestMethod]
        public void JsonDateValidation_Valid()
        {
            var jsonDate = DateTime.Now.ToJSONString();
            Assert.IsTrue(jsonDate.SuccessfulJSONDate());
        }

        [TestMethod]
        public void JsonDateValidation_Invalid()
        {
            var jsonDate = "ABC" + DateTime.Now.ToJSONString() + "XYZ";
            Assert.IsFalse(jsonDate.SuccessfulJSONDate());
        }

        [TestMethod]
        public void IdValidation_Valid()
        {
            var jsonDate = Guid.NewGuid().ToId();
            Assert.IsTrue(jsonDate.SuccessfulId());
        }

        [TestMethod]
        public void IdValidation_Invalid()
        {
            var jsonDate = Guid.NewGuid().ToId() + "XYZ";
            Assert.IsFalse(jsonDate.SuccessfulId());
        }

        [TestMethod]
        public void IDEntityTest_Valid()
        {
            var idEntity = new Models.IdValidationModel
            {
                Id = Guid.NewGuid().ToId()
            };
            var result = Validator.Validate(idEntity);
            Assert.IsTrue(result.Successful);
        }

        [TestMethod]
        public void IDEntityTest_Invalid()
        {
            var idEntity = new Models.IdValidationModel
            {
                Id = null
            };
            var result = Validator.Validate(idEntity);
            Assert.IsFalse(result.Successful);
        }

        [TestMethod]
        public void IDEntityTest_EmptyGuid_Invalid()
        {
            var idEntity = new Models.IdValidationModel
            {
                Id = Guid.Empty.ToId()
            };
            var result = Validator.Validate(idEntity);
            Assert.IsFalse(result.Successful);
        }

        [TestMethod]
        public void IDEntityTest_ZeroId_Invalid()
        {
            var idEntity = new Models.IdValidationModel
            {
                Id = "0"
            };
            var result = Validator.Validate(idEntity);
            Assert.IsFalse(result.Successful);
        }

        [TestMethod]
        public void AuditTest_Valid()
        {
            var auditEntity = GetValidAuditableModel();

            var result = Validator.Validate(auditEntity);
            Assert.IsTrue(result.Successful, result.ErrorMessage);
        }

        [TestMethod]
        public void AuditTest_MissingCreatedBy_Invalid()
        {
            var auditEntity = GetValidAuditableModel();
            auditEntity.CreatedBy = null;
            var result = Validator.Validate(auditEntity);
            Assert.IsFalse(result.Successful);
            WriteResults(result);
            Assert.AreEqual(LagoVista.Core.Resources.ValidationResource.CreatedByNotNull, result.Errors.First().Message);
        }

        [TestMethod]
        public void AuditTest_MissingLastUpdatedBy_Invalid()
        {
            var auditEntity = GetValidAuditableModel();
            auditEntity.LastUpdatedBy = null;
            var result = Validator.Validate(auditEntity);
            Assert.IsFalse(result.Successful);
            WriteResults(result);
            Assert.AreEqual(LagoVista.Core.Resources.ValidationResource.LastUpdatedByNotNull, result.Errors.First().Message);
        }

        [TestMethod]
        public void AuditTest_MissingCreatedByNoId_Invalid()
        {
            var auditEntity = GetValidAuditableModel();
            auditEntity.CreatedBy.Id = null;
            var result = Validator.Validate(auditEntity);
            Assert.IsFalse(result.Successful);
            WriteResults(result);
            Assert.AreEqual(LagoVista.Core.Resources.ValidationResource.CreatedByIdNotNullOrEmpty, result.Errors.First().Message);
        }

        [TestMethod]
        public void AuditTest_MissingLastUpdatedByNoId_Invalid()
        {
            var auditEntity = GetValidAuditableModel();
            auditEntity.LastUpdatedBy.Id = null;
            var result = Validator.Validate(auditEntity);
            Assert.IsFalse(result.Successful);
            WriteResults(result);
            Assert.AreEqual(LagoVista.Core.Resources.ValidationResource.LastUpdatedByIdNotNullOrEmpty, result.Errors.First().Message);
        }

        [TestMethod]
        public void AuditTest_InvalidCreatedById_Invalid()
        {
            var auditEntity = GetValidAuditableModel();
            auditEntity.CreatedBy.Id += "INVALID";
            var result = Validator.Validate(auditEntity);
            Assert.IsFalse(result.Successful);
            WriteResults(result);
            Assert.AreEqual(LagoVista.Core.Resources.ValidationResource.CreatedByIdInvalidFormat, result.Errors.First().Message);
        }

        [TestMethod]
        public void AuditTest_InvalidLastUpdatedById_Invalid()
        {
            var auditEntity = GetValidAuditableModel();
            auditEntity.LastUpdatedBy.Id += "INVALID";
            var result = Validator.Validate(auditEntity);
            Assert.IsFalse(result.Successful);
            WriteResults(result);
            Assert.AreEqual(LagoVista.Core.Resources.ValidationResource.LastUpdatedByIdInvalidFormat, result.Errors.First().Message);
        }

        [TestMethod]
        public void AuditTest_InvalidCreatedByText_Invalid()
        {
            var auditEntity = GetValidAuditableModel();
            auditEntity.CreatedBy.Text = null;
            var result = Validator.Validate(auditEntity);
            Assert.IsFalse(result.Successful);
            WriteResults(result);
            Assert.AreEqual(LagoVista.Core.Resources.ValidationResource.CreatedByTextNotNullOrEmpty, result.Errors.First().Message);
        }

        [TestMethod]
        public void AuditTest_InvalidLastUpdatedByText_Invalid()
        {
            var auditEntity = GetValidAuditableModel();
            auditEntity.LastUpdatedBy.Text = null;
            var result = Validator.Validate(auditEntity);
            Assert.IsFalse(result.Successful);
            WriteResults(result);
            Assert.AreEqual(LagoVista.Core.Resources.ValidationResource.LastUpdatedByTextNotNullOrEmpty, result.Errors.First().Message);
        }

        [TestMethod]
        public void AuditTest_MissingCreateDate_Invalid()
        {
            var auditEntity = GetValidAuditableModel();
            auditEntity.CreationDate = null;
            var result = Validator.Validate(auditEntity);
            Assert.IsFalse(result.Successful);
            WriteResults(result);
            Assert.AreEqual(LagoVista.Core.Resources.ValidationResource.CreationDateRequired, result.Errors.First().Message);
        }


        [TestMethod]
        public void AuditTest_MissingLastUpdatedDate_Invalid()
        {
            var auditEntity = GetValidAuditableModel();
            auditEntity.LastUpdatedDate = null;
            var result = Validator.Validate(auditEntity);
            Assert.IsFalse(result.Successful);
            WriteResults(result);
            Assert.AreEqual(LagoVista.Core.Resources.ValidationResource.LastUpdatedDateRequired, result.Errors.First().Message);
        }

        [TestMethod]
        public void AuditTest_InvalidCreationDate_Invalid()
        {
            var auditEntity = GetValidAuditableModel();
            auditEntity.CreationDate += "INVALID";
            var result = Validator.Validate(auditEntity);
            Assert.IsFalse(result.Successful);
            WriteResults(result);
            Assert.AreEqual(LagoVista.Core.Resources.ValidationResource.CreationDateInvalidFormat + " " + auditEntity.CreationDate, result.Errors.First().Message);
        }


        [TestMethod]
        public void AuditTest_InvalidLastUpdatedDate_Invalid()
        {
            var auditEntity = GetValidAuditableModel();
            auditEntity.LastUpdatedDate += "INVALID";
            var result = Validator.Validate(auditEntity);
            WriteResults(result);
            Assert.IsFalse(result.Successful);
            Assert.AreEqual(LagoVista.Core.Resources.ValidationResource.LastUpdateDateInvalidFormat + " " + auditEntity.LastUpdatedDate, result.Errors.First().Message);
        }

        [TestMethod]
        public void StringLength_Valid()
        {
            var strModel = GetValidStringLengthModel();
            var result = Validator.Validate(strModel);
            Assert.IsTrue(result.Successful);
        }

        [TestMethod]
        public void StringLength_TooShort_Invalid()
        {
            var strModel = GetValidStringLengthModel();
            strModel.MinStringLength_5 = "FOO";
            var result = Validator.Validate(strModel);
            Assert.IsFalse(result.Successful);
            WriteResults(result);
        }

        [TestMethod]
        public void StringLength_TooLong_Invalid()
        {
            var strModel = GetValidStringLengthModel();
            strModel.MaxStringLength_15 = "KEVIN D WOLF KEVIN D WOLF";
            var result = Validator.Validate(strModel);
            Assert.IsFalse(result.Successful);
            WriteResults(result);
        }

        [TestMethod]
        public void StringLength_Between_TooShort_Invalid()
        {
            var strModel = GetValidStringLengthModel();
            strModel.BetweenStringLength_5_15 = "FOO";
            var result = Validator.Validate(strModel);
            Assert.IsFalse(result.Successful);
            WriteResults(result);
        }

        [TestMethod]
        public void StringLength_Between_TooLong_Invalid()
        {
            var strModel = GetValidStringLengthModel();
            strModel.BetweenStringLength_5_15 = "KEVIN D WOLF KEVIN D WOLF";
            var result = Validator.Validate(strModel);
            Assert.IsFalse(result.Successful);
            WriteResults(result);
        }

    }
}
