// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 907cf45d03957a40ca584e0e98fe148f64553aac760ba345d945c2e47c49ee99
// IndexVersion: 1
// --- END CODE INDEX META ---
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.Networking.Models
{
    public class JSONRPCMessage
    {
        public String MethodName { get; set; }
        public int ID { get; set; }
    }
}
