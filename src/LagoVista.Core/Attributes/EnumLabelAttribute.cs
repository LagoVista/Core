using System;

namespace LagoVista.Core.Attributes
{

    public class EnumLabelAttribute : Attribute
    {
        private Type _resourceType;
        private String _labelResource;
        private String _id;
        private String _help;

        public EnumLabelAttribute(String LabelResource = "", Type ResourceType = null, String Id = "", String HelpResource = "")
        {
            _labelResource = LabelResource;
            _id = Id;
            _resourceType = ResourceType;
            _help = Help;
        }

        public String LabelResource { get { return _labelResource; } }
        public String Id { get { return _id; } }
        public String Help { get { return _help; } }
        public Type ResourceType { get { return _resourceType; } }
    }
}
