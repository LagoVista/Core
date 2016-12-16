using LagoVista.Common.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.DocBuilder.Tests.TestModels
{
    [EntityDescription(name: "The First Model1", description: "This model is the first one used for testing", domain: "Domain1")]
    public class Model1
    {
        public String Field2 { get; set; }
        public String Field1 { get; set; }
        public String Attr1 { get; set; }
        public int Attr2 { get; set; }
    }
}
