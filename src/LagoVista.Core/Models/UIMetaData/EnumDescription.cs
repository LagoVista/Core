using LagoVista.Core.Attributes;
using LagoVista.Core.Resources;
using System;
using System.Reflection;

namespace LagoVista.Core.Models.UIMetaData
{
    public class EnumDescription
    {
        public String Name { get; set; }
        public String Key { get; set; }
        public int SortOrder { get; set; }
        public String Label { get; set; }
        public String Help { get; set; }

        public string Id { get; set; }

        public string Text { get; set; }


        public static EnumDescription CreateSelect(String selectText = null)
        {
            return new EnumDescription()
            {
                Id = "-1",
                Key = "-1",
                Text = selectText ?? LagoVistaCommonStrings.Common_Select,
                Name = selectText ?? LagoVistaCommonStrings.Common_Select,
                Label = selectText ?? LagoVistaCommonStrings.Common_Select,                 
            };
        }

        public static EnumDescription Create(EnumLabelAttribute attr, String name, int value)
        {
            var enumLabel = new EnumDescription();
            enumLabel.Key = attr.Key;
            enumLabel.SortOrder = attr.SortOrder != -1 ? attr.SortOrder : value;
            enumLabel.Name = name;
            enumLabel.Id = attr.Key;

            var labelProperty = attr.ResourceType.GetTypeInfo().GetDeclaredProperty(attr.LabelResource);
            enumLabel.Label = labelProperty.GetValue(labelProperty.DeclaringType, null) as String;
            enumLabel.Text = enumLabel.Label;

            if(!String.IsNullOrEmpty(attr.HelpResource))
            {
                var helpProperty = attr.ResourceType.GetTypeInfo().GetDeclaredProperty(attr.HelpResource);
                enumLabel.Help = helpProperty.GetValue(helpProperty.DeclaringType, null) as String;
            }

            return enumLabel;
        }

        public static EnumDescription Create(string value, string name)
        {
            return new EnumDescription()
            {
                Id = value,
                Name = name,
                Label = name,
                Text = name,
                Key = value
            };
        }

        public static EnumDescription Create(string value, string key, string name)
        {
            return new EnumDescription()
            {
                Id = value,
                Name = name,
                Text = name,
                Label = name,
                Key = key
            };
        }

        public EntityHeader ToEntityHeader()
        {
            return new EntityHeader()
            {
               Id = Id,
               Key = Key,
               Text = Text,
            };
        }
    }
}
