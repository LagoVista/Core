// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 9fa3b84eb4d60a97788ee3eef973fdd08dc570119dbd65ac5957dec49f0a179f
// IndexVersion: 1
// --- END CODE INDEX META ---
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
