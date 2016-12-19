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

        private String _name;
        private String _description;
        private String _domain;
        private String _title;
        private String _userHelp;

        public EntityDescriptionAttribute(String Name = "", String Description = "", String Domain = null, String Title="", String UserHelp="", Type ResourceType = null)
        {
            _name = Name;
            _description = Description;
            _title = Title;
            _userHelp = UserHelp;
            _domain = Domain;
            _resourceType = ResourceType;
        }

        public String Name { get { return _name; } }
        public String Description { get { return _description; } }
        public String Domain { get { return _domain; } }
        public String UserHelp { get { return _userHelp; } }
        public String Title { get { return _title; } }
        public Type ResourceType { get { return _resourceType; } }
    }
}
