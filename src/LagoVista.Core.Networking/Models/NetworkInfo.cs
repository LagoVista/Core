using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.Common.Net.Models
{
    public class NetworkInfo
    {
        public string NetworkName { get; set; }
        public string NetworkIpv6 { get; set; }
        public string NetworkIpv4 { get; set; }
        public string NetworkStatus { get; set; }
    }
}
