using LagoVista.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class DomainDescriptionAttribute : Attribute
    {
        private String _key;

        public DomainDescriptionAttribute(String Key)
        {
            _key = Key;
        }

        public String Key { get; private set; }
    }
}
