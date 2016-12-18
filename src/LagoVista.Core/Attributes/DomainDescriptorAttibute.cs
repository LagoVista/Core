using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DomainDescriptorAttribute : Attribute
    {
        public DomainDescriptorAttribute()
        {
        }
    }
}
