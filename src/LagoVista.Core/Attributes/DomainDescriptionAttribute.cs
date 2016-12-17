using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class DomainDescriptionAttribute : Attribute
    {
        private String _name;
        private String _description;

        public DomainDescriptionAttribute(String Name = "", String Description = "")
        {
            _name = Name;
            _description = Description;
        }

        public String Name { get; private set; }
        public String Description { get; private set; }
    }
}
