using LagoVista.Core.Attributes;
using LagoVista.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.Tests.AutoMapper.TestModels
{
    public class SimpleManual
    {
        public string Id { get; set; }

        [ManualMapping]
        public EntityHeader Prop1 { get; set; }
    }

    public class SimpleManualDto
    {
        public string Id { get; set; }

        [ManualMapping]
        public string Prop1Id { get; set; }
        [ManualMapping]
        public string Prop1Text { get; set; }
    }
}
