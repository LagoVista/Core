using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.Core.Networking.Models
{
    public class UPNPConfiguration
    {
        public UPNPConfiguration()
        {
            Services = new List<UPNPService>();
        }

        public string DeviceType { get; set; }
        public string PresentationUrl { get; set; }
        public string FriendlyName { get; set; }
        public string Manufacture { get; set; }
        public string ManufactureUrl { get; set; }
        public string ModelDescription { get; set; }
        public string ModelName { get; set; }
        public string ModelNumber { get; set; }
        public string ModelUrl { get; set; }
        public string SerialNumber { get; set; }

        public string DefaultPageHtml { get; set; }

        public List<UPNPService> Services { get; private set; }
    }

    public class UPNPService
    {
        public string ServiceType { get; set; }
        public string ServiceId { get; set; }
        public string ControlUrl { get; set; }
        public string EventUrl { get; set; }
    }
}
