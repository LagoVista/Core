using LagoVista.Core.Attributes;
using LagoVista.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.Tests.AutoMapper.TestModels
{
    public class ChildIdMappingSource
    {
        [MapTo(nameof(ChildIdMappingTarget.GrandChildId))]
        public GrandChild GrandChild { get; set; }

    }

    public class ChildIdMappingTarget
    {
        public string GrandChildId { get; set; }
    }   

    public class GrandChild : RelationalEntityBase
    {
        public string Name { get; set; }
    }
}
