using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class EntityDescriptionAttribute : Attribute
    {
        Type _resourceType;
        
        private String _descriptionResource;
        private String _domain;
        private String _titleResource;
        private String _userHelpResource;

        public EntityDescriptionAttribute(String Domain, String TitleResource, String UserHelpResource, String DescriptionResource, Type ResourceType)
        {
            _descriptionResource = DescriptionResource;
            _titleResource = TitleResource;
            _userHelpResource = UserHelpResource;
            _domain = Domain;
            _resourceType = ResourceType;
        }
        
        public String DescriptionResource { get { return _descriptionResource; } }
        public String Domain { get { return _domain; } }
        public String UserHelpResource { get { return _userHelpResource; } }
        public String TitleResource { get { return _titleResource; } }
        public Type ResourceType { get { return _resourceType; } }
    }
}
