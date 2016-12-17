using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class DomainDescriptionAttribute : Attribute
    {
        public DomainDescriptionAttribute(String name, String description)
        {
            Name = name;
            Description = description;
        }

        public String Name { get; private set; }
        public String Description { get; private set; }
    }
}
