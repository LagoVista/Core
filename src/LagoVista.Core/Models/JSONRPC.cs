using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.Utils
{
    public class JSONRPCMessage
    {
        public String MethodName { get; set; }
        public int ID { get; set; }
    }
}
