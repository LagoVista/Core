using LagoVista.Core.Attributes;
using System;
using System.Reflection;

namespace LagoVista.Core.Models.UIMetaData
{
    public class EnumLabel
    {
        public String Name { get; set; }
        public String Key { get; set; }
        public String Label { get; set; }
        public String Help { get; set; }

        public static EnumLabel Create(EnumLabelAttribute attr, String name)
        {
            var enumLabel = new EnumLabel();
            enumLabel.Key = attr.Key;
            enumLabel.Name = name;

            var labelProperty = attr.ResourceType.GetTypeInfo().GetDeclaredProperty(attr.LabelResource);
            enumLabel.Label = labelProperty.GetValue(labelProperty.DeclaringType, null) as String;

            if(!String.IsNullOrEmpty(attr.HelpResource))
            {
                var helpProperty = attr.ResourceType.GetTypeInfo().GetDeclaredProperty(attr.LabelResource);
                enumLabel.Help = helpProperty.GetValue(helpProperty.DeclaringType, null) as String;
            }

            return enumLabel;
        }
    }
}
