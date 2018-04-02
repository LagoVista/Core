using LagoVista.Core.Attributes;
using LagoVista.Core.Interfaces;
using LagoVista.Core.IOC;
using LagoVista.Core.Models;
using LagoVista.Core.PlatformSupport;
using LagoVista.Core.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
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
        /// <summary>
        /// Validate an IValidateable object.
        /// </summary>
        /// <param name="entity">Object to validate</param>
        /// <param name="action">Action to execute: if not specified, use Any, this is normally used for custom validation routines</param>
        /// <param name="requirePopulatedEHValues">If this is set to true it will require that the entire object graphs, including traversing through EntityHeader<T>.Value will be required if the parameter is required.</param>
        /// <returns></returns>
        public static ValidationResult Validate(IValidateable entity, Actions action = Actions.Any, bool requirePopulatedEHValues = false)
        {
            var result = new ValidationResult();

            if (entity == null)
            {
                if (SLWIOC.Contains(typeof(ILogger)))
                {
                    var logger = SLWIOC.Get<ILogger>();
                    logger.AddException("Validator_Validate", new Exception("NULL Value Passed to Validate Method"));
                }

                result.AddSystemError("Null Value Provided for model on upload.");
                return result;
            }

            var methods = entity.GetType().GetTypeInfo().DeclaredMethods;
            foreach (var method in methods)
            {
                var attr = method.GetCustomAttributes(typeof(PreValidationAttribute), true).OfType<PreValidationAttribute>().FirstOrDefault();
                if (attr != null)
                {
                    CallPreValidationRoutine(entity, method, action);
                }
            }


            ValidateAuditInfo(result, entity);
            ValidateId(result, entity);

            var properties = entity.GetType().GetTypeInfo().GetAllProperties();
            foreach (var prop in properties)
            {
                var attr = prop.GetCustomAttributes(typeof(FormFieldAttribute), true).OfType<FormFieldAttribute>().FirstOrDefault();
                if (attr != null)
                {
                    ValidateProperty(attr, result, entity, prop, action);
                }
            }

            foreach (var method in methods)
            {
                var attr = method.GetCustomAttributes(typeof(CustomValidatorAttribute), true).OfType<CustomValidatorAttribute>().FirstOrDefault();
                if (attr != null)
                {
                    CallCustomValidationRoutine(result, entity, method, action);
                }
            }

            foreach (var prop in properties)
            {
                var attr = prop.GetCustomAttributes(typeof(FormFieldAttribute), true).OfType<FormFieldAttribute>().FirstOrDefault();
                var propValue = prop.GetValue(entity);
                if (propValue != null)
                {
                    if (propValue is IValidateable validateablePropValue)
                    {
                        var childResult = Validator.Validate(validateablePropValue, action);
                        result.Concat(childResult);
                    }

                    if (propValue.GetType().GetTypeInfo().IsGenericType &&
                        propValue.GetType().GetGenericTypeDefinition() == typeof(EntityHeader<>))
                    {
                        var valueProperty = propValue.GetType().GetTypeInfo().GetDeclaredProperty("Value");
                        if (valueProperty.GetValue(propValue) is IValidateable validatableChild)
                        {
                            var childResult = Validator.Validate(validatableChild, action);
                            result.Concat(childResult);
                        }
                        else
                        {
                            if (attr != null && attr.IsRequired && requirePopulatedEHValues)
                            {
                                AddRequiredFieldMissingMessage(result, attr, prop.Name);
                            }
                        }
                    }

                    if (prop.GetValue(entity) is System.Collections.IEnumerable listValues)
                    {
                        foreach (var listValue in listValues)
                        {
                            if (listValue is IValidateable validatableListValue)
                            {
                                var childResult = Validator.Validate(validatableListValue, action);
                                result.Concat(childResult);
                            }
                        }
                    }
                }


            }

            return result;
        }

        public static void CallPreValidationRoutine(IValidateable entity, MethodInfo method, Actions action)
        {
            if (method.ReturnType != typeof(void))
            {
                throw new InvalidOperationException("Custom Validation Method must not return a type, the return type must be [void]");
            }

            var parameters = method.GetParameters();

            if (parameters.Count() > 1)
            {
                throw new InvalidOperationException("Custom Validation Method most not accept more than one parameters, it may optionally [Actions action].");
            }

            if (parameters.Count() == 1 && parameters[0].ParameterType != typeof(Actions))
            {
                throw new InvalidOperationException("Custom Validation Method must optionally accept only a first parameter of [Actions action].");
            }

            if (parameters.Count() == 1)
            {
                method.Invoke(entity, new object[] { action });
            }
            else
            {
                method.Invoke(entity, new object[] {});
            }
        }

        public static void CallCustomValidationRoutine(ValidationResult result, IValidateable entity, MethodInfo method, Actions action)
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
            else if (prop.PropertyType.Name.StartsWith(nameof(EntityHeader))) /* YUCK! KDW 5/12/2017 */
            {
                ValidateEntityHeader(result, prop, attr, value as EntityHeader);
            }

            ValidateNumber(result, prop, attr, value);
        }

        private static void AddRequiredFieldMissingMessage(ValidationResult result, FormFieldAttribute attr, string propertyName)
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
                    result.AddSystemError(Resources.ValidationResource.Entity_Header_Null_System.Replace("[NAME]", propertyName));
                }
                else
                {
                    result.AddUserError(Resources.ValidationResource.PropertyIsRequired.Replace("[PROPERTYLABEL]", propertyLabel));
                }
            }

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
                    AddRequiredFieldMissingMessage(result, attr, prop.Name);
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
                    if (DateTime.TryParse(auditableModel.CreationDate, CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.AssumeUniversal, out DateTime dateTime))
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
                if (attr.FieldType == FieldTypes.Key)
                {
                    var reEx = new Regex("^[a-z0-9]{3,30}$");
                    if (!reEx.Match(value).Success)
                    {
                        if (attr.ResourceType == null)
                        {
                            throw new Exception($"Validating String - Reg Ex Validation has a resource text, but no resource type for field [{attr.LabelDisplayResource}]");
                        }

                        result.AddUserError(ValidationResource.Common_Key_Validation);
                    }
                }

                if (!String.IsNullOrEmpty(attr.RegExValidation))
                {
                    var reEx = new Regex(attr.RegExValidation);
                    if (!reEx.Match(value).Success)
                    {
                        if (attr.ResourceType == null)
                        {
                            throw new Exception($"Validating String - Reg Ex Validation was invalid, but no resource type for field [{attr.LabelDisplayResource}]");
                        }

                        if (String.IsNullOrEmpty(attr.RegExValidationMessageResource))
                        {
                            throw new Exception($"Validating String - Reg Ex Validation was invalid, [RegExValidationMessageResource] was null or empty and could not lookup error message for invalid field [{attr.LabelDisplayResource}].");
                        }
                        else {
                            var validationProperty = attr.ResourceType.GetTypeInfo().GetDeclaredProperty(attr.RegExValidationMessageResource);
                            if (validationProperty == null)
                            {
                                throw new Exception($"Validating String - Reg Ex Validation was invalid, but could not find validation message resource for field [{attr.LabelDisplayResource}]");
                            }
                            else
                            {
                                result.AddUserError((string)validationProperty.GetValue(validationProperty.DeclaringType, null));
                            }
                        }
                    }
                }



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
