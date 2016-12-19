using System;

namespace LagoVista.Core.Attributes
{

    public class EnumLabelAttribute : Attribute
    {
        private Type _resourceType;
        private String _labelResource;
        private String _key;
        private String _helpResource;

        public EnumLabelAttribute(String Key, String LabelResource, Type ResourceType, String HelpResource = "")
        {
            _labelResource = LabelResource;
            _key = Key;
            _resourceType = ResourceType;
            _helpResource = HelpResource;
        }

        public String LabelResource { get { return _labelResource; } }
        public String Key { get { return _key; } }
        public String HelpResource { get { return _helpResource; } }
        public Type ResourceType { get { return _resourceType; } }
    }
}
