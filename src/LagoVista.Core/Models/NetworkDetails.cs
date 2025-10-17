// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: e909561797f56e3d7565cbce409cf9735d67436a999f44e9f9090f1db250121d
// IndexVersion: 1
// --- END CODE INDEX META ---
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
