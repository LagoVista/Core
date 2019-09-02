using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Cloning
{
    public class CloneRequest
    {
        public string OriginalId { get; set; }
        public string NewName { get; set; }
        public string NewKey { get; set; }
    }
}
