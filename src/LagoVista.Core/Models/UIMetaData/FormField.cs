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
        public bool IsFileUploadImage { get; set; }
        public bool IsEnabled { get; set; }
        public string UploadUrl { get; set; }
        public String DataType { get; set; }
        public bool? AllowAddChild { get; set; }
        public int? MinLength { get; set; }
        public int? MaxLength { get; set; }
        public bool IsVisible { get; set; }

        public bool IsMarkDown { get; set; }
        public string CustomFieldType { get; set; }
        public string HelpUrl { get; set; }

        public string PickerFor { get; set; }

        public bool IsReferenceField { get; set; }

        public bool InPlaceEditing { get; set; }
        public string ScriptTemplateName { get; set; }
        public string SecureIdFieldName { get; set; }
        public RelayCommand Command { get; set; }
        public List<EnumDescription> Options { get; set; }
        public FormConditionals ConditionalFields { get; set; }

        public string ChildListDisplayMember { get; set; }
        public string CustomCategoryType { get; set; }

        public string[] ChildListDisplayMembers { get; set; }

        public IDictionary<string, FormField> View { get; set; }
        public List<string> FormFields { get; set; }
        public List<string> FormFieldsCol2 { get; set; }
        public List<string> FormFieldAdvanced { get; set; }
        public List<string> FormFieldAdvancedCol2 { get; set; }
        public List<FormAdditionalAction> FormAdditionalActions { get; set; }

        public bool OpenByDefault { get; set; }

        public string ModelTitle { get; set; }
        public string ModelHelp { get; set; }

        public string FactoryUrl { get; set; }
        public string GetUrl { get; set; }
        public string EntityHeaderPickerUrl { get; set; }

        public string AiChatPrompt { get; set; }
        public string GeneratedImageSize { get; set; }
        public string DisplayImageSize { get; set; }

        public string EditorPath { get; set; }

        public string SharedContentKey { get; set; }
        public bool PrivateFileUpload { get; set; }

        public int Rows { get; set; }
        public bool ImageUpload { get; set; }

        public List<ReplaceableTag> Tags { get; set; }

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

        public static List<EnumDescription> GetMonths()
        {
            var options = new List<EnumDescription>();
            options.Add(EnumDescription.Create("-1", LagoVistaCommonStrings.Common_Month_Select));
            options.Add(new EnumDescription() { Id = "1", Label = LagoVistaCommonStrings.Month_January, Name = LagoVistaCommonStrings.Month_January, Text = LagoVistaCommonStrings.Month_January, Key = "1", SortOrder = 1 });
            options.Add(new EnumDescription() { Id = "2", Label = LagoVistaCommonStrings.Month_February, Name = LagoVistaCommonStrings.Month_February, Text = LagoVistaCommonStrings.Month_February, Key = "2", SortOrder = 2 });
            options.Add(new EnumDescription() { Id = "3", Label = LagoVistaCommonStrings.Month_March, Name = LagoVistaCommonStrings.Month_March, Text = LagoVistaCommonStrings.Month_March, Key = "3", SortOrder = 3 });
            options.Add(new EnumDescription() { Id = "4", Label = LagoVistaCommonStrings.Month_April, Name = LagoVistaCommonStrings.Month_April, Text = LagoVistaCommonStrings.Month_April, Key = "4", SortOrder = 4 });
            options.Add(new EnumDescription() { Id = "5", Label = LagoVistaCommonStrings.Month_May, Name = LagoVistaCommonStrings.Month_May, Text = LagoVistaCommonStrings.Month_May, Key = "5", SortOrder = 5 });
            options.Add(new EnumDescription() { Id = "6", Label = LagoVistaCommonStrings.Month_June, Name = LagoVistaCommonStrings.Month_June, Text = LagoVistaCommonStrings.Month_June, Key = "6", SortOrder = 6 });
            options.Add(new EnumDescription() { Id = "7", Label = LagoVistaCommonStrings.Month_July, Name = LagoVistaCommonStrings.Month_July, Text = LagoVistaCommonStrings.Month_July, Key = "7", SortOrder = 7 });
            options.Add(new EnumDescription() { Id = "8", Label = LagoVistaCommonStrings.Month_August, Name = LagoVistaCommonStrings.Month_August, Text = LagoVistaCommonStrings.Month_August, Key = "8", SortOrder = 8 });
            options.Add(new EnumDescription() { Id = "9", Label = LagoVistaCommonStrings.Month_September, Name = LagoVistaCommonStrings.Month_September, Text = LagoVistaCommonStrings.Month_September, Key = "9", SortOrder = 9 });
            options.Add(new EnumDescription() { Id = "10", Label = LagoVistaCommonStrings.Month_October, Name = LagoVistaCommonStrings.Month_October, Text = LagoVistaCommonStrings.Month_October, Key = "10", SortOrder = 10 });
            options.Add(new EnumDescription() { Id = "11", Label = LagoVistaCommonStrings.Month_November, Name = LagoVistaCommonStrings.Month_November, Text = LagoVistaCommonStrings.Month_November, Key = "11", SortOrder = 11 });
            options.Add(new EnumDescription() { Id = "12", Label = LagoVistaCommonStrings.Month_December, Name = LagoVistaCommonStrings.Month_December, Text = LagoVistaCommonStrings.Month_December, Key = "12", SortOrder = 12 });
            return options;
        }

        public static FormField Create(String name, FormFieldAttribute attr, PropertyInfo property)
        {
            var field = new FormField();
            field.Name = name;
            field.FieldType = attr.FieldType.ToString();
            if(!String.IsNullOrEmpty(attr.SecureIdFieldName))
                field.SecureIdFieldName = attr.SecureIdFieldName.CamelCase();

            field.UploadUrl = attr.UploadUrl;
            field.FactoryUrl = attr.FactoryUrl;
            field.GetUrl = attr.GetUrl;
            field.IsReferenceField = attr.IsReferenceField;
            field.PickerFor = attr.PickerFor;
            field.EntityHeaderPickerUrl = attr.EntityHeaderUrl;
            field.HelpUrl = attr.Helpurl;
            field.ScriptTemplateName = attr.ScriptTemplateName;
            field.InPlaceEditing = attr.InPlaceEditing;
            field.IsFileUploadImage = attr.IsFileUploadImage;
            field.EditorPath = attr.EditorPath;
            field.OpenByDefault = attr.OpenByDefault;
            field.AiChatPrompt = attr.AiChatPrompt;
            field.GeneratedImageSize = attr.GeneratedImageSize;
            field.DisplayImageSize = attr.DisplayImageSize;
            field.SharedContentKey = attr.SharedContentKey;
            field.PrivateFileUpload = attr.PrivateFileUpload;
            field.Rows = attr.Rows;
            field.ImageUpload = attr.ImageUpload;

            field.Options = new List<EnumDescription>();
            if(!String.IsNullOrEmpty(attr.Tags))
            {
                field.Tags = new List<ReplaceableTag>();
                var parts = attr.Tags.Split(';');
                foreach(var tag in parts)
                {
                    var tagInfoParts = tag.Split('-');
                    if (tagInfoParts.Length != 2)
                        throw new Exception($"On field {field.Name} replacement tags should be formatted like Title 1-tag1;Title 2-tag2 - {attr.Tags}");

                    field.Tags.Add(new ReplaceableTag()
                    {
                        Title = tagInfoParts[0],
                        Tag = tagInfoParts[1]
                    });

                }
            }
            

            if (!String.IsNullOrEmpty(attr.LabelDisplayResource))
            {
                if (attr.ResourceType == null)
                {
                    throw new Exception($"Building Metadata - label is defined, but Resource Type is not defined on {name} {attr.LabelDisplayResource}");
                }

                var labelProperty = attr.ResourceType.GetTypeInfo().GetDeclaredProperty(attr.LabelDisplayResource);
                field.Label = (string)labelProperty.GetValue(labelProperty.DeclaringType, null);
            }

            field.CustomFieldType = attr.CustomFieldType;

            if(attr.FieldType == FieldTypes.Year)
            {
                field.Options = new List<EnumDescription>();
                field.FieldType = nameof(FieldTypes.Picker);
                field.Watermark = LagoVistaCommonStrings.Common_Year_Select;
                var currentYear = DateTime.Now.Year;
                if (String.IsNullOrEmpty(field.Label))
                {
                    field.Label = LagoVistaCommonStrings.Common_Year;
                }

                field.Options.Add(EnumDescription.Create("-1", LagoVistaCommonStrings.Common_Year_Select));
                for (var idx = 2015; idx < currentYear + 10; ++idx)
                {
                    field.Options.Add(new EnumDescription() { Id = idx.ToString(), Label = idx.ToString(), Key = idx.ToString(), Name = idx.ToString(), Text = idx.ToString(), SortOrder = idx });
                }
            }

            if(attr.FieldType == FieldTypes.Month)
            {
                field.FieldType = nameof(FieldTypes.Picker);
                field.Watermark = LagoVistaCommonStrings.Common_Month_Select;
                if (String.IsNullOrEmpty(field.Label))
                {
                    field.Label = LagoVistaCommonStrings.Common_Month;
                }

                field.Options = GetMonths();
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

            field.CustomCategoryType = attr.CustomerCategoryType;
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


            if (attr.FieldType == FieldTypes.ChildList || attr.FieldType == FieldTypes.ChildListInline || attr.FieldType == FieldTypes.ChildListInlinePicker)
            {
                var childListProperty = property.PropertyType;

                var childType = childListProperty.GenericTypeArguments.FirstOrDefault();
                var entityDescription = childType.GetTypeInfo().CustomAttributes.FirstOrDefault(eda => eda.AttributeType == typeof(EntityDescriptionAttribute));

                if(!String.IsNullOrEmpty(attr.ChildListDisplayMember))
                    field.ChildListDisplayMember = attr.ChildListDisplayMember.CamelCase();

                if (!string.IsNullOrEmpty(attr.ChildListDisplayMembers))
                {
                    var members = attr.ChildListDisplayMembers.Split(',');
                    field.ChildListDisplayMembers = members.Select(cldm => cldm.CamelCase()).ToArray();
                }

                if (attr.FieldType == FieldTypes.ChildListInline || attr.FieldType == FieldTypes.ChildListInlinePicker)
                    field.AllowAddChild = attr.AllowAddChild;

                if (childType != null && entityDescription != null)
                {
                    var childTypeAttr = childType.GetTypeInfo().GetCustomAttributes<EntityDescriptionAttribute>().FirstOrDefault();
                    var entity = EntityDescription.Create(childType, childTypeAttr);
                    field.ModelHelp = entity.UserHelp;
                    field.ModelTitle = entity.Title;
                    field.FactoryUrl = entity.FactoryUrl;
                    field.GetUrl = entity.GetUrl;

                    var childInstance = Activator.CreateInstance(childType) as IFormDescriptor;
                    if (childInstance != null)
                    {
                        field.FormFields = childInstance.GetFormFields().Select(fld => $"{fld.Substring(0, 1).ToLower()}{fld.Substring(1)}").ToList();
                    }

                    var childInstanceCol2 = Activator.CreateInstance(childType) as IFormDescriptorCol2;
                    if (childInstanceCol2 != null)
                    {
                        field.FormFieldsCol2 = childInstanceCol2.GetFormFieldsCol2().Select(fld => $"{fld.Substring(0, 1).ToLower()}{fld.Substring(1)}").ToList();
                    }

                    var childInstanceAdvanced = Activator.CreateInstance(childType) as IFormDescriptorAdvanced;
                    if (childInstanceAdvanced != null)
                    {
                        field.FormFieldAdvanced = childInstanceAdvanced.GetAdvancedFields().Select(fld => $"{fld.Substring(0, 1).ToLower()}{fld.Substring(1)}").ToList();
                    }

                    var childInstanceAdvancedCol2 = Activator.CreateInstance(childType) as IFormDescriptorAdvancedCol2;
                    if (childInstanceAdvancedCol2 != null)
                    {
                        field.FormFieldAdvancedCol2 = childInstanceAdvancedCol2.GetAdvancedFieldsCol2().Select(fld => $"{fld.Substring(0, 1).ToLower()}{fld.Substring(1)}").ToList();
                    }

                    var additionalActions = Activator.CreateInstance(childType) as IFormAdditionalActions;
                    if (additionalActions != null)
                    {
                        field.FormAdditionalActions = additionalActions.GetAdditionalActions().ToList();
                    }

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

                if(attr.SortEnums)
                    field.Options = options.OrderBy(opt => opt.Label).ToList();
                else
                    field.Options = options.OrderBy(opt => opt.SortOrder).ToList();

                if (attr.AddEnumSelect)
                    field.Options.Insert(0, new EnumDescription()
                    {
                        Id = "-1",
                        Key = "-1",
                        Text = LagoVistaCommonStrings.Common_Select,
                        Name = "select",
                        Label = LagoVistaCommonStrings.Common_Select
                    });

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
