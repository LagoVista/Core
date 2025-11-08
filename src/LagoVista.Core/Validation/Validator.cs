// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 5c19bded4d62fc71f78004fcd25a0341a317738848478301e9321b325c760ba9
// IndexVersion: 2
// --- END CODE INDEX META ---
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


            if(entity is IAuditableEntity draft)
            {
                if(draft.IsDraft)
                // Drafts are not validated.
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
                                AddRequiredFieldMissingMessage(result, attr, prop.Name, entity);
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
                ValidateString(result, prop, attr, value as String, entity);
            }
            /// TODO: Find better approeach for detecting generic type entity headers.
            else if (prop.PropertyType.Name.StartsWith(nameof(EntityHeader))) /* YUCK! KDW 5/12/2017 */
            {
                ValidateEntityHeader(result, prop, attr, value as EntityHeader, entity);
            }

            ValidateNumber(result, prop, attr, value);
        }

        private static void AddRequiredFieldMissingMessage(ValidationResult result, FormFieldAttribute attr, string propertyName, IValidateable entity)
        {
            var context = entity.GetType().Name;
            if (entity is INamedEntity namedEntity)
            {
                context += " - " + namedEntity.Name;
            }

            if (!String.IsNullOrEmpty(attr.RequiredMessageResource) && attr.ResourceType != null)
            {
                var validationProperty = attr.ResourceType.GetTypeInfo().GetDeclaredProperty(attr.RequiredMessageResource);
                var validationMessage = (string)validationProperty.GetValue(validationProperty.DeclaringType, null);

                result.AddUserError(validationMessage, context);
            }
            else
            {
                var propertyLabel = GetLabel(attr);
                if (String.IsNullOrEmpty(propertyLabel))
                {
                    result.AddSystemError(Resources.ValidationResource.Entity_Header_Null_System.Replace("[NAME]", propertyName), context);
                }
                else
                {
                    result.AddUserError(Resources.ValidationResource.PropertyIsRequired.Replace(Tokens.PROPERTY_LABEL, propertyLabel), context);
                }
            }

        }


        private static void ValidateEntityHeader(ValidationResult result, PropertyInfo prop, FormFieldAttribute attr, EntityHeader value, IValidateable entity)
        {
            var context = prop.Name;
            if (entity is INamedEntity namedEntity)
            {
                context += " - " + namedEntity.Name;
            }

            if (value != null && String.IsNullOrEmpty(value.Id) && !String.IsNullOrEmpty(value.Text))
            {
                result.AddSystemError(Resources.ValidationResource.Entity_Header_MissingId_System.Replace("[NAME]", context));
            }
            else if (value != null && String.IsNullOrEmpty(value.Text) && !String.IsNullOrEmpty(value.Id))
            {
                result.AddSystemError(Resources.ValidationResource.Entity_Header_MissingText_System.Replace("[NAME]", context));
            }
            else if (attr.IsRequired)
            {
                if (value == null || value.IsEmpty())
                {
                    AddRequiredFieldMissingMessage(result, attr, prop.Name, entity);
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
                    result.AddSystemError(Resources.ValidationResource.IDIsRequired, entity.GetType().Name);
                }
                else
                {

                    if (idModel.Id == Guid.Empty.ToId()) result.AddSystemError(Resources.ValidationResource.IDMustNotBeEmptyGuid, entity.GetType().Name);
                    if (idModel.Id == "0") result.AddSystemError(Resources.ValidationResource.IDMustNotBeZero, entity.GetType().Name);
                }
            }
        }

        private static void ValidateAuditInfo(ValidationResult result, IValidateable entity)
        {
            var context = entity.GetType().Name;
            if (entity is INamedEntity namedEntity)
            {
                context += " - " + namedEntity.Name;
            }

            if (entity is IAuditableEntity)
            {
                var auditableModel = entity as IAuditableEntity;
                if (String.IsNullOrEmpty(auditableModel.CreationDate))
                {
                    result.AddSystemError(Resources.ValidationResource.CreationDateRequired, context);
                }
                else if (!auditableModel.CreationDate.SuccessfulJSONDate())
                {
                    if (DateTime.TryParse(auditableModel.CreationDate, CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.AssumeUniversal, out DateTime dateTime))
                    {
                        auditableModel.CreationDate = dateTime.ToJSONString();
                    }
                    else
                    {
                        result.AddSystemError(Resources.ValidationResource.CreationDateInvalidFormat + " " + auditableModel.CreationDate, context);
                    }
                }

                if (String.IsNullOrEmpty(auditableModel.LastUpdatedDate))
                {

                    result.AddSystemError(Resources.ValidationResource.LastUpdatedDateRequired, context);
                }
                else if (!auditableModel.LastUpdatedDate.SuccessfulJSONDate())
                {
                    if (DateTime.TryParse(auditableModel.LastUpdatedDate, CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.AssumeUniversal, out DateTime dateTime))
                    {
                        auditableModel.LastUpdatedDate = dateTime.ToJSONString();
                    }
                    else
                    {
                        result.AddSystemError(Resources.ValidationResource.LastUpdateDateInvalidFormat + " " + auditableModel.LastUpdatedDate, context);
                    }
                }

                if (auditableModel.LastUpdatedBy == null)
                {
                    result.AddSystemError(Resources.ValidationResource.LastUpdatedByNotNull, context);
                }
                else
                {
                    if (String.IsNullOrEmpty(auditableModel.LastUpdatedBy.Id))
                    {
                        result.AddSystemError(Resources.ValidationResource.LastUpdatedByIdNotNullOrEmpty, context);
                    }
                    else if (!auditableModel.LastUpdatedBy.Id.SuccessfulId())
                    {
                        result.AddSystemError(Resources.ValidationResource.LastUpdatedByIdInvalidFormat, context);
                    }

                    if (String.IsNullOrEmpty(auditableModel.LastUpdatedBy.Text))
                    {
                        result.AddSystemError(Resources.ValidationResource.LastUpdatedByTextNotNullOrEmpty, context);
                    }
                }

                if (auditableModel.CreatedBy == null)
                {
                    result.AddSystemError(Resources.ValidationResource.CreatedByNotNull, context);
                }
                else
                {
                    if (String.IsNullOrEmpty(auditableModel.CreatedBy.Id))
                    {
                        result.AddSystemError(Resources.ValidationResource.CreatedByIdNotNullOrEmpty, context);
                    }
                    else if (!auditableModel.CreatedBy.Id.SuccessfulId())
                    {
                        result.AddSystemError(Resources.ValidationResource.CreatedByIdInvalidFormat, context);
                    }

                    if (String.IsNullOrEmpty(auditableModel.CreatedBy.Text))
                    {
                        result.AddSystemError(Resources.ValidationResource.CreatedByTextNotNullOrEmpty, context);
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

        private static void ValidateString(ValidationResult result, PropertyInfo propertyInfo, FormFieldAttribute attr, String value, IValidateable entity)
        {
            var context = entity.GetType().Name;
            if (entity is INamedEntity namedEntity)
            {
                context += " - " + namedEntity.Name;
            }

            var propertyLabel = GetLabel(attr);
            if (String.IsNullOrEmpty(propertyLabel))
                propertyLabel = propertyInfo.Name;

            if (String.IsNullOrEmpty(value) && attr.IsRequired)
            {
                if (attr.FieldType == FieldTypes.Hidden)
                {
                    result.AddSystemError(Resources.ValidationResource.SystemMissingProperty.Replace("[PROPERTYNAME]", propertyInfo.Name), context);
                }
                else
                {
                    if (!String.IsNullOrEmpty(attr.RequiredMessageResource) && attr.ResourceType != null)
                    {
                        var validationProperty = attr.ResourceType.GetTypeInfo().GetDeclaredProperty(attr.RequiredMessageResource);
                        result.AddUserError((string)validationProperty.GetValue(validationProperty.DeclaringType, null), context);
                    }
                    else if (!String.IsNullOrEmpty(attr.LabelDisplayResource))
                    {
                        var validationMessage = Resources.ValidationResource.PropertyIsRequired.Replace(Tokens.PROPERTY_LABEL, propertyLabel);
                        result.AddUserError(validationMessage, context);
                    }
                    else
                    {
                        result.AddSystemError(Resources.ValidationResource.SystemMissingProperty.Replace("[PROPERTYNAME]", propertyInfo.Name), context);
                    }
                }
            }

            if (!String.IsNullOrEmpty(value))
            {
                if (attr.FieldType == FieldTypes.Key)
                {
                    var reEx = new Regex("^[a-z0-9]{3,128}$");
                    if (!reEx.Match(value).Success)
                    {
                        if (attr.ResourceType == null)
                        {
                            throw new Exception($"Validating String - Reg Ex Validation has a resource text, but no resource type for field [{attr.LabelDisplayResource}]");
                        }

                        result.AddUserError(ValidationResource.Common_Key_Validation, context + " Value: " + value ?? "[EMPTY]");
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
                                result.AddUserError((string)validationProperty.GetValue(validationProperty.DeclaringType, null), context);
                            }
                        }
                    }
                }



                if (attr.MinLength.HasValue && attr.MaxLength.HasValue)
                {
                    if (value.Length < attr.MinLength || value.Length > attr.MaxLength)
                    {
                        result.AddUserError(Resources.ValidationResource.ValueLength_Between.Replace("[PROPERTY]", propertyLabel).Replace("[MIN]", attr.MinLength.ToString()).Replace("[MAX]", attr.MaxLength.ToString()), context);
                    }
                }
                else if (attr.MaxLength.HasValue)
                {
                    if (value.Length > attr.MaxLength)
                    {
                        result.AddUserError(Resources.ValidationResource.ValueLength_TooLong.Replace("[PROPERTY]", propertyLabel).Replace("[MAX]", attr.MaxLength.ToString()), context);
                    }
                }
                else if (attr.MinLength.HasValue)
                {
                    if (value.Length < attr.MinLength)
                    {
                        result.AddUserError(Resources.ValidationResource.ValueLength_TooShort.Replace("[PROPERTY]", propertyLabel).Replace("[MIN]", attr.MinLength.ToString()), context);
                    }
                }
            }

        }
    }
}
