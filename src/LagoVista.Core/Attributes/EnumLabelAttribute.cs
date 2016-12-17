using System;

namespace LagoVista.Common.Attributes
{

    public class EnumLabelAttribute : Attribute
    {
        public EnumLabelAttribute(String labelResource = "", Type resourceType = null, String id = "", String help = "")
        {
            LabelResource = labelResource;
            Id = id;
            Help = help;
            ResourceType = resourceType;
        }

        public String LabelResource { get; private set; }
        public String Id { get; private set; }
        public String Help { get; private set; }
        public Type ResourceType { get; private set; }
    }

    [EntityDescription(domain: null)]
    public class MyFoo
    {

    }

}
