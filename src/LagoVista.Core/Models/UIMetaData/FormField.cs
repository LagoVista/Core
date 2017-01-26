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

        public string Label { get; private set; }
        public String Watermark { get; private set; }
        public bool IsRequired { get; private set; }
        public string RequiredMessage { get; private set; }
        public string Help { get; private set; }
        public string FieldType { get; private set; }
        public string RegEx { get; private set; }
        public string RegExMessage { get; private set; }
        public String Name { get; private set; }
        public String Value { get; set; }
        public bool IsUserEditable { get; private set; }
        public String DataType { get; private set; }
        public int? MinLength { get; set; }
        public int? MaxLength { get; set; }

        public List<EntityHeader> Options { get; set; }
        
        public static  FormField Create(String name, FormFieldAttribute attr)
        {
            var field = new FormField();

            if (!String.IsNullOrEmpty(attr.LabelDisplayResource))
            {
                var labelProperty = attr.ResourceType.GetTypeInfo().GetDeclaredProperty(attr.LabelDisplayResource);
                field.Label = (string)labelProperty.GetValue(labelProperty.DeclaringType, null);
            }

            field.IsRequired = attr.IsRequired;
            if (field.IsRequired)
            {
                if (!String.IsNullOrEmpty(attr.RequiredMessageResource))
                {
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

            field.Options = new List<EntityHeader>();
            if(attr.EnumType != null)
            {
                var values = Enum.GetValues(attr.EnumType);
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
                var placeholderProperty = attr.ResourceType.GetTypeInfo().GetDeclaredProperty(attr.WaterMark);
                field.Watermark = placeholderProperty == null ? String.Empty : (string)placeholderProperty.GetValue(placeholderProperty.DeclaringType, null);
            }

            if(!String.IsNullOrEmpty(attr.HelpResource))
            {
                var helpProperty = attr.ResourceType.GetTypeInfo().GetDeclaredProperty(attr.HelpResource);
                field.Help = helpProperty == null ? String.Empty : (string)helpProperty.GetValue(helpProperty.DeclaringType, null);
            }

            return field;
        }
    }
}
