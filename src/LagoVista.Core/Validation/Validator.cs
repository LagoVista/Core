using LagoVista.Core.Attributes;
using LagoVista.Core.Interfaces;
using LagoVista.Core.IOC;
using LagoVista.Core.Models;
using LagoVista.Core.PlatformSupport;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.Validation
{
    public enum Actions
    {
        Any,
        Create,
        Read,
        Update,
        Delete,
        CreateOrUpdate
    }

    public class Validator
    {
        public static ValidationResult Validate(IValidateable entity, Actions action = Actions.Any)
        {
            var result = new ValidationResult();
        
            if (entity == null)
            {
                if(SLWIOC.Contains(typeof(ILogger)))
                {
                    var logger = SLWIOC.Get<ILogger>();
                    logger.AddException("Validator_Validate", new Exception("NULL Value Passed to Validate Method"));
                }

                result.AddSystemError("Null Value Provided for model on upload.");
                return result;
            }

            ValidateAuditInfo(result, entity);
            ValidateId(result, entity);

            var properties = entity.GetType().GetTypeInfo().DeclaredProperties;
            foreach (var prop in properties)
            {
                var attr = prop.GetCustomAttributes(typeof(FormFieldAttribute), true).OfType<FormFieldAttribute>().FirstOrDefault();
                if (attr != null)
                {
                    ValidateProperty(attr, result, entity, prop, action);
                }
            }

            var methods = entity.GetType().GetTypeInfo().DeclaredMethods;
            foreach (var method in methods)
            {
                var attr = method.GetCustomAttributes(typeof(CustomValidatorAttribute), true).OfType<CustomValidatorAttribute>().FirstOrDefault();
                if (attr != null)
                {
                    CallCustomValidationRoutine(attr, result, entity, method, action);
                }
            }

            return result;
        }

        public static void CallCustomValidationRoutine(CustomValidatorAttribute attr, ValidationResult result, IValidateable entity, MethodInfo method, Actions action)
        {
            if (method.ReturnType != typeof(void))
            {
                throw new InvalidOperationException("Custom Validation Method must not return a type, the return type must be [void]");
            }

            var parameters = method.GetParameters();
            if (parameters.Count() == 0)
            {
                throw new InvalidOperationException("Custom Validation Method must accept at a minimum one parameter, [ValidationResult result]");
            }

            if (parameters[0].ParameterType != typeof(ValidationResult))
            {
                throw new InvalidOperationException("Custom Validation Method must accept the first parameter of [ValidationResult result]");
            }

            if (parameters.Count() > 2)
            {
                throw new InvalidOperationException("Custom Validation Method most not accept more than two parameters, it must accept at minimum [ValidationResult result] and optionally [Actions action].");
            }

            if (parameters.Count() == 2 && parameters[1].ParameterType != typeof(Actions))
            {
                throw new InvalidOperationException("Custom Validation Method most optionally accept only a second parameter of [Actions action].");
            }

            if (parameters.Count() == 2)
            {
                method.Invoke(entity, new object[] { result, action });
            }
            else
            {
                method.Invoke(entity, new object[] { result });
            }

        }

        private static void ValidateProperty(FormFieldAttribute attr, ValidationResult result, IValidateable entity, PropertyInfo prop, Actions action)
        {
            var value = prop.GetValue(entity);
            if (prop.PropertyType == typeof(string))
            {
                ValidateString(result, prop, attr, value as String);
            }
            /// TODO: Find better approeach for detecting generic type entity headers.
            else if (prop.PropertyType.Name.StartsWith(nameof(EntityHeader))) /* YUCK! KDW 5/12/207 */
            {
                ValidateEntityHeader(result, prop, attr, value as EntityHeader);
            }

            ValidateNumber(result, prop, attr, value);
        }



        private static void ValidateEntityHeader(ValidationResult result, PropertyInfo prop, FormFieldAttribute attr, EntityHeader value)
        {
            if (value != null && String.IsNullOrEmpty(value.Id) && !String.IsNullOrEmpty(value.Text))
            {
                result.AddSystemError(Resources.ValidationResource.Entity_Header_MissingId_System.Replace("[NAME]", prop.Name));
            }
            else if (value != null && String.IsNullOrEmpty(value.Text) && !String.IsNullOrEmpty(value.Id))
            {
                result.AddSystemError(Resources.ValidationResource.Entity_Header_MissingText_System.Replace("[NAME]", prop.Name));
            }
            else if (attr.IsRequired)
            {
                if (value == null || value.IsEmpty())
                {
                    if (!String.IsNullOrEmpty(attr.RequiredMessageResource) && attr.ResourceType != null)
                    {
                        var validationProperty = attr.ResourceType.GetTypeInfo().GetDeclaredProperty(attr.RequiredMessageResource);
                        var validationMessage = (string)validationProperty.GetValue(validationProperty.DeclaringType, null);
                        result.AddUserError(validationMessage);
                    }
                    else
                    {
                        var propertyLabel = GetLabel(attr);
                        if (String.IsNullOrEmpty(propertyLabel))
                        {
                            result.AddSystemError(Resources.ValidationResource.Entity_Header_Null_System.Replace("[NAME]", prop.Name));
                        }
                        else
                        {
                            result.AddUserError(Resources.ValidationResource.PropertyIsRequired.Replace("[PROPERTYLABEL]", propertyLabel));
                        }
                    }
                }
            }
        }

        private static void ValidateId(ValidationResult result, IValidateable entity)
        {
            if (entity is IIDEntity)
            {
                var idModel = entity as IIDEntity;
                if (String.IsNullOrEmpty(idModel.Id))
                {
                    result.AddSystemError(Resources.ValidationResource.IDIsRequired);
                }
                else
                {

                    if (idModel.Id == Guid.Empty.ToId()) result.AddSystemError(Resources.ValidationResource.IDMustNotBeEmptyGuid);
                    if (idModel.Id == "0") result.AddSystemError(Resources.ValidationResource.IDMustNotBeZero);
                }
            }
        }

        private static void ValidateAuditInfo(ValidationResult result, IValidateable entity)
        {
            if (entity is IAuditableEntity)
            {
                var auditableModel = entity as IAuditableEntity;
                if (String.IsNullOrEmpty(auditableModel.CreationDate))
                {
                    result.AddSystemError(Resources.ValidationResource.CreationDateRequired);
                }
                else if (!auditableModel.CreationDate.SuccessfulJSONDate())
                {
                    if (DateTime.TryParse(auditableModel.CreationDate,CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.AssumeUniversal, out DateTime dateTime))
                    {
                        auditableModel.CreationDate = dateTime.ToJSONString();
                    }
                    else
                    {
                        result.AddSystemError(Resources.ValidationResource.CreationDateInvalidFormat + " " + auditableModel.CreationDate);
                    }
                }

                if (String.IsNullOrEmpty(auditableModel.LastUpdatedDate))
                {

                    result.AddSystemError(Resources.ValidationResource.LastUpdatedDateRequired);
                }
                else if (!auditableModel.LastUpdatedDate.SuccessfulJSONDate())
                {
                    if (DateTime.TryParse(auditableModel.LastUpdatedDate, CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.AssumeUniversal, out DateTime dateTime))
                    {
                        auditableModel.LastUpdatedDate = dateTime.ToJSONString();
                    }
                    else
                    {
                        result.AddSystemError(Resources.ValidationResource.LastUpdateDateInvalidFormat + " " + auditableModel.LastUpdatedDate);
                    }                    
                }

                if (auditableModel.LastUpdatedBy == null)
                {
                    result.AddSystemError(Resources.ValidationResource.LastUpdatedByNotNull);
                }
                else
                {
                    if (String.IsNullOrEmpty(auditableModel.LastUpdatedBy.Id))
                    {
                        result.AddSystemError(Resources.ValidationResource.LastUpdatedByIdNotNullOrEmpty);
                    }
                    else if (!auditableModel.LastUpdatedBy.Id.SuccessfulId())
                    {
                        result.AddSystemError(Resources.ValidationResource.LastUpdatedByIdInvalidFormat);
                    }

                    if (String.IsNullOrEmpty(auditableModel.LastUpdatedBy.Text))
                    {
                        result.AddSystemError(Resources.ValidationResource.LastUpdatedByTextNotNullOrEmpty);
                    }
                }

                if (auditableModel.CreatedBy == null)
                {
                    result.AddSystemError(Resources.ValidationResource.CreatedByNotNull);
                }
                else
                {
                    if (String.IsNullOrEmpty(auditableModel.CreatedBy.Id))
                    {
                        result.AddSystemError(Resources.ValidationResource.CreatedByIdNotNullOrEmpty);
                    }
                    else if (!auditableModel.CreatedBy.Id.SuccessfulId())
                    {
                        result.AddSystemError(Resources.ValidationResource.CreatedByIdInvalidFormat);
                    }

                    if (String.IsNullOrEmpty(auditableModel.CreatedBy.Text))
                    {
                        result.AddSystemError(Resources.ValidationResource.CreatedByTextNotNullOrEmpty);
                    }
                }
            }
        }

        private static string GetLabel(FormFieldAttribute attr)
        {
            if (attr.ResourceType != null && !String.IsNullOrEmpty(attr.LabelDisplayResource))
            {
                var labelProperty = attr.ResourceType.GetTypeInfo().GetDeclaredProperty(attr.LabelDisplayResource);
                return (labelProperty != null) ? (string)labelProperty.GetValue(labelProperty.DeclaringType, null) : (string)null;
            }
            else
            {
                return null;
            }
        }

        private static void ValidateNumber(ValidationResult result, PropertyInfo propertyInfo, FormFieldAttribute attr, object value)
        {


        }

        private static void ValidateString(ValidationResult result, PropertyInfo propertyInfo, FormFieldAttribute attr, String value)
        {
            var propertyLabel = GetLabel(attr);
            if (String.IsNullOrEmpty(propertyLabel))
                propertyLabel = propertyInfo.Name;

            if (String.IsNullOrEmpty(value) && attr.IsRequired)
            {
                var validationMessage = String.Empty;

                if (attr.FieldType == FieldTypes.Hidden)
                {
                    result.AddSystemError(Resources.ValidationResource.SystemMissingProperty.Replace("[PROPERTYNAME]", propertyInfo.Name));
                }
                else
                {
                    if (!String.IsNullOrEmpty(attr.RequiredMessageResource) && attr.ResourceType != null)
                    {
                        var validationProperty = attr.ResourceType.GetTypeInfo().GetDeclaredProperty(attr.RequiredMessageResource);
                        result.AddUserError((string)validationProperty.GetValue(validationProperty.DeclaringType, null));
                    }
                    else if (!String.IsNullOrEmpty(attr.LabelDisplayResource))
                    {
                        validationMessage = Resources.ValidationResource.PropertyIsRequired.Replace("[PROPERTYLABEL]", propertyLabel);
                        result.AddUserError(validationMessage);
                    }
                    else
                    {
                        result.AddSystemError(Resources.ValidationResource.SystemMissingProperty.Replace("[PROPERTYNAME]", propertyInfo.Name));
                    }
                }
            }

            if (!String.IsNullOrEmpty(value))
            {
                if (attr.MinLength.HasValue && attr.MaxLength.HasValue)
                {
                    if (value.Length < attr.MinLength || value.Length > attr.MaxLength)
                    {
                        result.AddUserError(Resources.ValidationResource.ValueLength_Between.Replace("[PROPERTY]", propertyLabel).Replace("[MIN]", attr.MinLength.ToString()).Replace("[MAX]", attr.MaxLength.ToString()));
                    }
                }
                else if (attr.MaxLength.HasValue)
                {
                    if (value.Length > attr.MaxLength)
                    {
                        result.AddUserError(Resources.ValidationResource.ValueLength_TooLong.Replace("[PROPERTY]", propertyLabel).Replace("[MAX]", attr.MaxLength.ToString()));
                    }
                }
                else if (attr.MinLength.HasValue)
                {
                    if (value.Length < attr.MinLength)
                    {
                        result.AddUserError(Resources.ValidationResource.ValueLength_TooShort.Replace("[PROPERTY]", propertyLabel).Replace("[MIN]", attr.MinLength.ToString()));
                    }
                }
            }

        }
    }
}
