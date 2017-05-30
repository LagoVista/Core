using LagoVista.Core.Attributes;
using LagoVista.Core.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace LagoVista.Core.Models.UIMetaData
{
    public class FormField
    {
        private FormField() { }

        public string Label { get;  set; }
        public String Watermark { get;  set; }
        public bool IsRequired { get;  set; }
        public string RequiredMessage { get;  set; }
        public string Help { get;  set; }
        public string FieldType { get;  set; }
        public string RegEx { get;  set; }
        public string RegExMessage { get;  set; }
        public String Name { get;  set; }
        public String Value { get; set; }
        public String DefaultValue { get; set; }
        public bool IsUserEditable { get;  set; }
        public String DataType { get;  set; }
        public int? MinLength { get; set; }
        public int? MaxLength { get; set; }
        public bool IsVisible { get; set; }
        public List<EnumDescription> Options { get; set; }
        
        public static  FormField Create(String name, FormFieldAttribute attr)
        {
            var field = new FormField();

            if (!String.IsNullOrEmpty(attr.LabelDisplayResource))
            {
                if (attr.ResourceType == null)
                {
                    throw new Exception($"Building Metadata - label is defined, but Resource Type is not defined on {name} {attr.LabelDisplayResource}");
                }

                var labelProperty = attr.ResourceType.GetTypeInfo().GetDeclaredProperty(attr.LabelDisplayResource);
                field.Label = (string)labelProperty.GetValue(labelProperty.DeclaringType, null);
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
            field.FieldType = attr.FieldType.ToString();
            field.Name = name;
            field.MinLength = attr.MinLength;
            field.MaxLength = attr.MaxLength;

            field.Options = new List<EnumDescription>();
            if(attr.EnumType != null)
            {
                var values = Enum.GetValues(attr.EnumType);
                for (var idx = 0; idx < values.GetLength(0); ++idx)
                {
                    var value = values.GetValue(idx).ToString();

                    var enumMember = attr.EnumType.GetTypeInfo().DeclaredMembers.Where(mbr => mbr.Name == value.ToString()).FirstOrDefault();
                    var enumAttr = enumMember.GetCustomAttribute<EnumLabelAttribute>();

                    field.Options.Add(EnumDescription.Create(enumAttr, value));
                }
            }

            if(attr.FieldType == FieldTypes.NameSpace)
            {
                field.RegEx=@"^[a-z0-9]{6,30}$";
                field.RegExMessage = ValidationResource.Validation_RegEx_Namespace;
            }

            if (attr.FieldType == FieldTypes.Key)
            {
                field.RegEx = @"^[a-z0-9]{3,30}$";
                field.RegExMessage = ValidationResource.Common_Key_Validation;
            }

            if (!String.IsNullOrEmpty(attr.WaterMark))
            {
                if (attr.ResourceType == null)
                {
                    throw new Exception($"Building Metadata - watermark is defined, but Resource Type is not defined on {name} {attr.WaterMark}");
                }

                var placeholderProperty = attr.ResourceType.GetTypeInfo().GetDeclaredProperty(attr.WaterMark);
                field.Watermark = placeholderProperty == null ? String.Empty : (string)placeholderProperty.GetValue(placeholderProperty.DeclaringType, null);
            }

            if(!String.IsNullOrEmpty(attr.HelpResource))
            {
                if (attr.ResourceType == null)
                {
                    throw new Exception($"Building Metaata - watermark is defined, but Resource Type is not defined on {name} {attr.HelpResource}");
                }

                var helpProperty = attr.ResourceType.GetTypeInfo().GetDeclaredProperty(attr.HelpResource);
                field.Help = helpProperty == null ? String.Empty : (string)helpProperty.GetValue(helpProperty.DeclaringType, null);
            }

            field.IsVisible = true;

            return field;
        }
    }
}
