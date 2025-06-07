using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Models
{
    public class TagGroup
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> Tags { get; set; }
    }
}
