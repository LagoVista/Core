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
