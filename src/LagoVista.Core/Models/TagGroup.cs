// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: d8c857b3e126af22af782ce7c2882d31c94647b6b78c6d0ac40826cf07c652fa
// IndexVersion: 2
// --- END CODE INDEX META ---
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
