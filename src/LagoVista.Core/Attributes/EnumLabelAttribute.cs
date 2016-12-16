using System;

namespace LagoVista.Common.Attributes
{

    public class EnumLabelAttribute : Attribute
    {
        public EnumLabelAttribute(String labelResource = "", Type resourceType = null, String key = "", String help = "")
        {
            LabelResource = labelResource;
            Key = key;
            Help = help;
            ResourceType = resourceType;
        }

        public String LabelResource { get; private set; }
        public String Key { get; private set; }
        public String Help { get; private set; }
        public Type ResourceType { get; private set; }
    }
}
