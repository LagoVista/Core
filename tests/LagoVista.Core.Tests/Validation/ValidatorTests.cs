using LagoVista.Core.Models;
using LagoVista.Core.Tests.Resources.Models;
using LagoVista.Core.Validation;
using Xunit;
using System;
using System.Linq;

namespace LagoVista.Core.Tests.Validation
{
    public class ValidatorTests
    {
        public Models.ValidationModel GetValidModel()
        {
            return new Models.ValidationModel()
            {
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

        [Fact]
        public void ConcatErrorsTest()
        {
            var result1 = new ValidationResult();
            result1.Errors.Add(new ErrorMessage("BAD"));

            var result2 = new ValidationResult();
            result2.Concat(result1);

            Assert.Equal(1,result2.Errors.Where(err => err.Message == "BAD").Count());
        }

        [Fact]
        public void IsRequired_Valid()
        {
            var result = Validator.Validate(GetValidModel());
            Assert.True(result.Successful);
        }

        [Fact]
        public void IsRequired_InValid()
        {
            var model = GetValidModel();
            model.SystemRequired = null;
            var result = Validator.Validate(model);
            Assert.False(result.Successful);
        }

        [Fact]
        public void IsRequired_InValid_DefaultMessage()
        {
            var model = GetValidModel();
            model.PropertyBasedValidationMessage = null;
            var result = Validator.Validate(model);
            Assert.Equal(LagoVista.Core.Resources.ValidationResource.SystemMissingProperty.Replace("[PROPERTYNAME]", "PropertyBasedValidationMessage"), result.Errors.FirstOrDefault().Message);
        }

        [Fact]
        public void IsRequired_InValid_LabelMessage()
        {
            var model = GetValidModel();
            model.LabelBasedValidationMessage = null;
            var result = Validator.Validate(model);
            Assert.Equal(LagoVista.Core.Resources.ValidationResource.PropertyIsRequired.Replace("[PROPERTYLABEL]", ValidationResources.FirstNameLabel), result.Errors.FirstOrDefault().Message);
        }

        [Fact]
        public void IsRequired_InValid_CustomMessage()
        {
            var model = GetValidModel();
            model.CustomValidationMessage = null;
            var result = Validator.Validate(model);
            Assert.Equal(ValidationResources.CustomValidationMessage, result.Errors.FirstOrDefault().Message);
        }

        [Fact]
        public void JsonDateValidation_Valid()
        {
            var jsonDate = DateTime.Now.ToJSONString();
            Assert.True(jsonDate.SuccessfulJSONDate());
        }

        [Fact]
        public void JsonDateValidation_Invalid()
        {
            var jsonDate = "ABC" + DateTime.Now.ToJSONString() + "XYZ";
            Assert.False(jsonDate.SuccessfulJSONDate());
        }

        [Fact]
        public void IdValidation_Valid()
        {
            var jsonDate = Guid.NewGuid().ToId();
            Assert.True(jsonDate.SuccessfulId());
        }

        [Fact]
        public void IdValidation_Invalid()
        {
            var jsonDate = Guid.NewGuid().ToId() + "XYZ";
            Assert.False(jsonDate.SuccessfulId());
        }

        [Fact]
        public void IDEntityTest_Valid()
        {
            var idEntity = new Models.IdValidationModel();
            idEntity.Id = Guid.NewGuid().ToId();
            var result = Validator.Validate(idEntity);
            Assert.True(result.Successful);
        }

        [Fact]
        public void IDEntityTest_Invalid()
        {
            var idEntity = new Models.IdValidationModel();
            idEntity.Id = null;
            var result = Validator.Validate(idEntity);
            Assert.False(result.Successful);
        }

        [Fact]
        public void IDEntityTest_EmptyGuid_Invalid()
        {
            var idEntity = new Models.IdValidationModel();
            idEntity.Id = Guid.Empty.ToId();
            var result = Validator.Validate(idEntity);
            Assert.False(result.Successful);
        }

        [Fact]
        public void IDEntityTest_ZeroId_Invalid()
        {
            var idEntity = new Models.IdValidationModel();
            idEntity.Id = "0";
            var result = Validator.Validate(idEntity);
            Assert.False(result.Successful);
        }

        [Fact]
        public void AuditTest_Valid()
        {
            var auditEntity = GetValidAuditableModel();

            var result = Validator.Validate(auditEntity);
            Assert.True(result.Successful);
        }

        [Fact]
        public void AuditTest_MissingCreatedBy_Invalid()
        {
            var auditEntity = GetValidAuditableModel();
            auditEntity.CreatedBy = null;
            var result = Validator.Validate(auditEntity);
            Assert.False(result.Successful);
            WriteResults(result);
            Assert.Equal(LagoVista.Core.Resources.ValidationResource.CreatedByNotNull, result.Errors.First().Message);
        }

        [Fact]
        public void AuditTest_MissingLastUpdatedBy_Invalid()
        {
            var auditEntity = GetValidAuditableModel();
            auditEntity.LastUpdatedBy = null;
            var result = Validator.Validate(auditEntity);
            Assert.False(result.Successful);
            WriteResults(result);
            Assert.Equal(LagoVista.Core.Resources.ValidationResource.LastUpdatedByNotNull, result.Errors.First().Message);
        }

        [Fact]
        public void AuditTest_MissingCreatedByNoId_Invalid()
        {
            var auditEntity = GetValidAuditableModel();
            auditEntity.CreatedBy.Id = null;
            var result = Validator.Validate(auditEntity);
            Assert.False(result.Successful);
            WriteResults(result);
            Assert.Equal(LagoVista.Core.Resources.ValidationResource.CreatedByIdNotNullOrEmpty, result.Errors.First().Message);
        }

        [Fact]
        public void AuditTest_MissingLastUpdatedByNoId_Invalid()
        {
            var auditEntity = GetValidAuditableModel();
            auditEntity.LastUpdatedBy.Id = null;
            var result = Validator.Validate(auditEntity);
            Assert.False(result.Successful);
            WriteResults(result);
            Assert.Equal(LagoVista.Core.Resources.ValidationResource.LastUpdatedByIdNotNullOrEmpty, result.Errors.First().Message);
        }

        [Fact]
        public void AuditTest_InvalidCreatedById_Invalid()
        {
            var auditEntity = GetValidAuditableModel();
            auditEntity.CreatedBy.Id += "INVALID";
            var result = Validator.Validate(auditEntity);
            Assert.False(result.Successful);
            WriteResults(result);
            Assert.Equal(LagoVista.Core.Resources.ValidationResource.CreatedByIdInvalidFormat, result.Errors.First().Message);
        }

        [Fact]
        public void AuditTest_InvalidLastUpdatedById_Invalid()
        {
            var auditEntity = GetValidAuditableModel();
            auditEntity.LastUpdatedBy.Id += "INVALID";
            var result = Validator.Validate(auditEntity);
            Assert.False(result.Successful);
            WriteResults(result);
            Assert.Equal(LagoVista.Core.Resources.ValidationResource.LastUpdatedByIdInvalidFormat, result.Errors.First().Message);
        }

        [Fact]
        public void AuditTest_InvalidCreatedByText_Invalid()
        {
            var auditEntity = GetValidAuditableModel();
            auditEntity.CreatedBy.Text = null;
            var result = Validator.Validate(auditEntity);
            Assert.False(result.Successful);
            WriteResults(result);
            Assert.Equal(LagoVista.Core.Resources.ValidationResource.CreatedByTextNotNullOrEmpty, result.Errors.First().Message);
        }

        [Fact]
        public void AuditTest_InvalidLastUpdatedByText_Invalid()
        {
            var auditEntity = GetValidAuditableModel();
            auditEntity.LastUpdatedBy.Text = null;
            var result = Validator.Validate(auditEntity);
            Assert.False(result.Successful);
            WriteResults(result);
            Assert.Equal(LagoVista.Core.Resources.ValidationResource.LastUpdatedByTextNotNullOrEmpty, result.Errors.First().Message);
        }

        [Fact]
        public void AuditTest_MissingCreateDate_Invalid()
        {
            var auditEntity = GetValidAuditableModel();
            auditEntity.CreationDate = null;
            var result = Validator.Validate(auditEntity);
            Assert.False(result.Successful);
            WriteResults(result);
            Assert.Equal(LagoVista.Core.Resources.ValidationResource.CreationDateRequired, result.Errors.First().Message);
        }


        [Fact]
        public void AuditTest_MissingLastUpdatedDate_Invalid()
        {
            var auditEntity = GetValidAuditableModel();
            auditEntity.LastUpdatedDate = null;
            var result = Validator.Validate(auditEntity);
            Assert.False(result.Successful);
            WriteResults(result);
            Assert.Equal(LagoVista.Core.Resources.ValidationResource.LastUpdatedDateRequired, result.Errors.First().Message);
        }

        [Fact]
        public void AuditTest_InvalidCreationDate_Invalid()
        {
            var auditEntity = GetValidAuditableModel();
            auditEntity.CreationDate += "INVALID";
            var result = Validator.Validate(auditEntity);
            Assert.False(result.Successful);
            WriteResults(result);
            Assert.Equal(LagoVista.Core.Resources.ValidationResource.CreationDateInvalidFormat + " " + auditEntity.CreationDate, result.Errors.First().Message);
        }


        [Fact]
        public void AuditTest_InvalidLastUpdatedDate_Invalid()
        {
            var auditEntity = GetValidAuditableModel();
            auditEntity.LastUpdatedDate += "INVALID";
            var result = Validator.Validate(auditEntity);
            Assert.False(result.Successful);
            WriteResults(result);
            Assert.Equal(LagoVista.Core.Resources.ValidationResource.LastUpdateDateInvalidFormat + " " + auditEntity.LastUpdatedDate, result.Errors.First().Message);
        }

        [Fact]
        public void StringLength_Valid()
        {
            var strModel = GetValidStringLengthModel();
            var result = Validator.Validate(strModel);
            Assert.True(result.Successful);
        }

        [Fact]
        public void StringLength_TooShort_Invalid()
        {
            var strModel = GetValidStringLengthModel();
            strModel.MinStringLength_5 = "FOO";
            var result = Validator.Validate(strModel);
            Assert.False(result.Successful);
            WriteResults(result);
        }

        [Fact]
        public void StringLength_TooLong_Invalid()
        {
            var strModel = GetValidStringLengthModel();
            strModel.MaxStringLength_15 = "KEVIN D WOLF KEVIN D WOLF";
            var result = Validator.Validate(strModel);
            Assert.False(result.Successful);
            WriteResults(result);
        }

        [Fact]
        public void StringLength_Between_TooShort_Invalid()
        {
            var strModel = GetValidStringLengthModel();
            strModel.BetweenStringLength_5_15 = "FOO";
            var result = Validator.Validate(strModel);
            Assert.False(result.Successful);
            WriteResults(result);
        }

        [Fact]
        public void StringLength_Between_TooLong_Invalid()
        {
            var strModel = GetValidStringLengthModel();
            strModel.BetweenStringLength_5_15 = "KEVIN D WOLF KEVIN D WOLF";
            var result = Validator.Validate(strModel);
            Assert.False(result.Successful);
            WriteResults(result);
        }

    }
}
