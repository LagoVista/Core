using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.Tests.AutoMapper.TestModels
{
    public class SourceWithNullableGuid
    {
        public GuidString36? Prop1 { get; set; }
    }

    public class TargetWithNullableGuid
    {
        public Guid? Prop1 { get; set; }

    }   
}
