// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 807a46f7d1dbd3cbc530b1c443e1c3e097da8109a1afcec370fcc4868e435e14
// IndexVersion: 2
// --- END CODE INDEX META ---
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.Core.Models
{
    public class SerialPortInfo
    {
        public String Id { get; set; }
        public String Name { get; set; }

        public int BaudRate { get; set; }
        public bool Parity { get; set; }
        public int DataBits { get; set; }
        public int StopBits { get; set; }
    }
}
