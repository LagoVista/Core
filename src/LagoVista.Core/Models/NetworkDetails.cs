using System;

namespace LagoVista.Core.Models
{ 
    public class NetworkDetails
    {
        public bool IsWireless { get; set; }
        public String SSID { get; set; }
        public String Name { get; set; }
        public String IPAddress { get; set; }
        public String Gateway { get; set; }
        public String Connectivity { get; set; }
    }
}
