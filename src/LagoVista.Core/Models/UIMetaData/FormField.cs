using LagoVista.Core.Attributes;
using LagoVista.Core.Commanding;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LagoVista.Core.Models.UIMetaData
{
    public enum YearOptionsEnum
    {

        TwoThousand
    }

    public class FormField
    {
        public const string FieldType_MarkDown = "MarkDown";
        public const string FieldType_CheckBox = "CheckBox";
        public const string FieldType_Picker = "Picker";
        public const string FieldType_Time = "Time";
        public const string FieldType_Date = "Date";
        public const string FieldType_DateTime = "DateTime";
        public const string FeildType_EntityHeaderPicker = "EntityHeaderPicker";
        public const string FieldType_Text = "Text";
        public const string FieldType_Key = "Key";
        public const string FieldType_LinkButton = "LinkButton";
        public const string FieldType_ChildList = "ChildList";
        public const string FieldType_ChildView = "ChildView";
        public const string FieldType_ChildItem = "ChildItem";
        public const string FieldType_Decimal = "Decimal";
        public const string FieldType_Integer = "Integer";
        public const string FieldType_Bool = "Bool";
        public const string FieldType_NameSpace = "NameSpace";
        public const string FieldType_Password = "Password";
        public const string FieldType_MultilineText = "MultiLineText";
        public const string FieldType_FileUpload = "FileUpload";
        public const string FieldType_MediaResourceUpload = "MediaResourceUpload";

        private FormField() { }

        public string Label { get; set; }
        public String Watermark { get; set; }
        public bool IsRequired { get; set; }
        public string RequiredMessage { get; set; }
        public string Help { get; set; }
        public string FieldType { get; set; }
        public string RegEx { get; set; }
        public string RegExMessage { get; set; }
        public String Name { get; set; }
        public String Value { get; set; }
        public String Display { get; set; }
        public String DefaultValue { get; set; }
        public bool IsUserEditable { get; set; }
        public bool IsEnabled { get; set; }
        public String DataType { get; set; }
        public bool? AllowAddChild { get; set; }
        public int? MinLength { get; set; }
        public int? MaxLength { get; set; }
        public bool IsVisible { get; set; }
        public bool IsMarkDown { get; set; }
        public RelayCommand Command { get; set; }
        public List<EnumDescription> Options { get; set; }
        public FormConditionals ConditionalFields { get; set; }

        public IDictionary<string, FormField> View { get; set; }
        public List<string> FormFields { get; set; }
        public string ModelTitle { get; set; }
        public string ModelHelp { get; set; }

        public static List<EnumDescription> GetEnumOptions<Type>()
        {
            var options = new List<EnumDescription>();
            if (typeof(Type) == typeof(YearOptionsEnum))
            {
                for (int idx = 2020; idx < DateTime.Now.Year + 3; ++idx)
                {
                    options.Add(new EnumDescription()
                    {
                        Id = idx.ToString(),
                        Key = idx.ToString(),
                        Label = idx.ToString(),
                        Name = idx.ToString(),
                        SortOrder = idx,
                        Text = idx.ToString()
                    });
                }

                return options;
            }

            var enumType = typeof(Type);

            new List<EnumDescription>();
            var values = Enum.GetValues(enumType);
            for (var idx = 0; idx < values.GetLength(0); ++idx)
            {
                var value = values.GetValue(idx).ToString();

                var enumMember = enumType.GetTypeInfo().DeclaredMembers.Where(mbr => mbr.Name == value.ToString()).FirstOrDefault();
                var enumAttr = enumMember.GetCustomAttribute<EnumLabelAttribute>();

                if (enumAttr.IsActive)
                {
                    options.Add(EnumDescription.Create(enumAttr, value, idx));
                }
            }

            return options.OrderBy(opt => opt.SortOrder).ToList();
        }


        public static FormField Create(String name, FormFieldAttribute attr, PropertyInfo property)
        {
            var field = new FormField();
            field.Name = name;
            field.FieldType = attr.FieldType.ToString();

            if (!String.IsNullOrEmpty(attr.LabelDisplayResource))
            {
                if (attr.ResourceType == null)
                {
                    throw new Exception($"Building Metadata - label is defined, but Resource Type is not defined on {name} {attr.LabelDisplayResource}");
                }

                var labelProperty = attr.ResourceType.GetTypeInfo().GetDeclaredProperty(attr.LabelDisplayResource);
                field.Label = (string)labelProperty.GetValue(labelProperty.DeclaringType, null);
            }

            if (field.FieldType == FormField.FieldType_ChildView)
            {
                field.View = new Dictionary<string, FormField>();

                var childProperties = property.PropertyType.GetRuntimeProperties();
                foreach (var childProperty in childProperties)
                {
                    var fieldAttributes = childProperty.GetCustomAttributes<FormFieldAttribute>();
                    if (fieldAttributes.Any())
                    {
                        var camelCaseName = childProperty.Name.Substring(0, 1).ToLower() + childProperty.Name.Substring(1);
                        var childField = FormField.Create(camelCaseName, fieldAttributes.First(), childProperty);
                        field.View.Add(camelCaseName, childField);
                    }
                }
                return field;
            }

            field.IsRequired = attr.IsRequired;
            if (field.IsRequired)
            {
                if (!String.IsNullOrEmpty(attr.RequiredMessageResource))
                {
                    if (attr.ResourceType == null)
                    {
                        throw new Exception($"Building Metadata - required message is defined, but Resource Type is not defined on {name} {attr.LabelDisplayResource}");
                    }

                    var validationProperty = attr.ResourceType.GetTypeInfo().GetDeclaredProperty(attr.RequiredMessageResource);
                    field.RequiredMessage = (string)validationProperty.GetValue(validationProperty.DeclaringType, null);
                }
                else
                {
                    field.RequiredMessage = ValidationResource.PropertyIsRequired.Replace(Tokens.PROPERTY_LABEL, field.Label);
                }
            }

            field.IsUserEditable = attr.IsUserEditable;
            field.IsEnabled = true;
            field.IsVisible = true;


            if (!String.IsNullOrEmpty(attr.WaterMark))
            {
                if (attr.ResourceType == null)
                {
                    throw new Exception($"Building Metadata - watermark is defined, but Resource Type is not defined on {name} {attr.WaterMark}");
                }

                var placeholderProperty = attr.ResourceType.GetTypeInfo().GetDeclaredProperty(attr.WaterMark);
                field.Watermark = placeholderProperty == null ? String.Empty : (string)placeholderProperty.GetValue(placeholderProperty.DeclaringType, null);
            }

            if (!String.IsNullOrEmpty(attr.HelpResource))
            {
                if (attr.ResourceType == null)
                {
                    throw new Exception($"Building Metaata - watermark is defined, but Resource Type is not defined on {name} {attr.HelpResource}");
                }

                var helpProperty = attr.ResourceType.GetTypeInfo().GetDeclaredProperty(attr.HelpResource);
                field.Help = helpProperty == null ? String.Empty : (string)helpProperty.GetValue(helpProperty.DeclaringType, null);
            }


            if (attr.FieldType == FieldTypes.ChildList || attr.FieldType == FieldTypes.ChildListInline)
            {
                var childListProperty = property.PropertyType;

                var childType = childListProperty.GenericTypeArguments.FirstOrDefault();
                var entityDescription = childType.GetTypeInfo().CustomAttributes.FirstOrDefault(eda => eda.AttributeType == typeof(EntityDescriptionAttribute));

                if (attr.FieldType == FieldTypes.ChildListInline)
                    field.AllowAddChild = attr.AllowAddChild;

                if (childType != null && entityDescription != null)
                {
                    var childTypeAttr = childType.GetTypeInfo().GetCustomAttributes<EntityDescriptionAttribute>().FirstOrDefault();
                    var entity = EntityDescription.Create(childType, childTypeAttr);
                    field.ModelHelp = entity.UserHelp;
                    field.ModelTitle = entity.Title;

                    var childInstance = Activator.CreateInstance(childType) as IFormDescriptor;
                    if (childInstance != null)
                        field.FormFields = childInstance.GetFormFields().Select(fld => $"{fld.Substring(0, 1).ToLower()}{fld.Substring(1)}").ToList();

                    var fieldConditionalInstance = Activator.CreateInstance(childType) as IFormConditionalFields;
                    if (fieldConditionalInstance is IFormConditionalFields)
                    {
                        var conditionalFields = (fieldConditionalInstance as IFormConditionalFields).GetConditionalFields();
                        field.ConditionalFields = conditionalFields.ValuesAsCamelCase();
                    }

                    field.View = new Dictionary<string, FormField>();

                    var childProperties = childType.GetRuntimeProperties();
                    foreach (var childProperty in childProperties)
                    {
                        var fieldAttributes = childProperty.GetCustomAttributes<FormFieldAttribute>();
                        if (fieldAttributes.Any())
                        {
                            var camelCaseName = childProperty.Name.Substring(0, 1).ToLower() + childProperty.Name.Substring(1);
                            var childField = FormField.Create(camelCaseName, fieldAttributes.First(), childProperty);
                            field.View.Add(camelCaseName, childField);
                        }
                    }
                    return field;
                }
            }

            field.MinLength = attr.MinLength;
            field.MaxLength = attr.MaxLength;
            field.IsMarkDown = attr.IsMarkDown;

            field.Options = new List<EnumDescription>();
            if (attr.EnumType != null)
            {
                var options = new List<EnumDescription>();
                if (attr.EnumType == typeof(YearOptionsEnum))
                {
                    for (int idx = 2020; idx < DateTime.Now.Year + 3; ++idx)
                    {
                        options.Add(new EnumDescription()
                        {
                            Id = idx.ToString(),
                            Key = idx.ToString(),
                            Label = idx.ToString(),
                            Name = idx.ToString(),
                            SortOrder = idx,
                            Text = idx.ToString()
                        });
                    }
                }
                else
                {
                    var values = Enum.GetValues(attr.EnumType);
                    for (var idx = 0; idx < values.GetLength(0); ++idx)
                    {
                        var value = values.GetValue(idx).ToString();

                        var enumMember = attr.EnumType.GetTypeInfo().DeclaredMembers.Where(mbr => mbr.Name == value.ToString()).FirstOrDefault();
                        var enumAttr = enumMember.GetCustomAttribute<EnumLabelAttribute>();

                        if (enumAttr.IsActive)
                        {
                            options.Add(EnumDescription.Create(enumAttr, value, idx));
                        }
                    }
                }

                field.Options = options.OrderBy(opt => opt.SortOrder).ToList();
            }

            if (attr.FieldType == FieldTypes.NameSpace)
            {
                field.RegEx = @"^[a-z][a-z0-9]{5,30}$";
                field.RegExMessage = ValidationResource.Validation_RegEx_Namespace;
            }
            else if (attr.FieldType == FieldTypes.Key)
            {
                field.RegEx = @"^[a-z][a-z0-9]{2,50}$";
                field.RegExMessage = ValidationResource.Common_Key_Validation;
            }
            else
            {
                field.RegEx = attr.RegExValidation;
                if (!String.IsNullOrEmpty(attr.RegExValidationMessageResource))
                {
                    if (attr.ResourceType == null)
                    {
                        throw new Exception($"Building Metadata - Reg Ex Validation has a resource nae, but no resource type on {name} {attr.LabelDisplayResource}");
                    }

                    var validationProperty = attr.ResourceType.GetTypeInfo().GetDeclaredProperty(attr.RegExValidationMessageResource);
                    field.RegExMessage = (string)validationProperty.GetValue(validationProperty.DeclaringType, null);
                }
            }


            return field;
        }
    }
}
