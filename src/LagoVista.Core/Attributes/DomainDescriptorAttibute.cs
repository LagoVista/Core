using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DomainDescriptorAttibute : Attribute
    {
        private String _name;
        private String _description;
        
        /// <summary>
        /// This class will be used to decorate a class that contains static methods
        /// that contain information about the domains used to categorize data.  The
        /// name and description probably don't provide too much value but can be used
        /// to help describe the domains if ncessary
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Description"></param>
        public DomainDescriptorAttibute(String Name, String Description)
        {
            _name = Name;
            _description = Description;
        }

        public String Name { get { return _name; } }
        public String Description { get { return _description; } }
    }
}
