using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.Networking.Models
{
    public class UDPMessageReceivedEventArgs
    {
        public Stream InputStream { get; set; }

        public String RemoteHostName { get; set; }

        public String  RemotePort { get; set; }

    }
}
