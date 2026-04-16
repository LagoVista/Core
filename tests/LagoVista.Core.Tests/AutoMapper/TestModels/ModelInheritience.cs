using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.Tests.AutoMapper.TestModels
{
    public class SourceInheritsBase
    {
        public NormalizedId32 Id { get; set; }
    }

    public class SourceInheritsDerived : SourceInheritsBase
    {
        public new string Id { get; set; }
    }

    public class TargetInheritsBase
    {
        public NormalizedId32 Id { get; set; }
    }
}
