using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.Core.Networking.Models
{
    public class SSDPDiscoveryConfiguration
    {
        public string DeviceType { get; set; }
        public string FriendlyName { get; set; }
        public string Manufacture { get; set; }
        public string ManufactureUrl { get; set; }
        public string ModelDescription { get; set; }
        public string ModelName { get; set; }
        public string ModelNumber { get; set; }
        public string ModelUrl { get; set; }
        public string SerialNumber { get; set; }
    }
}
