// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 0521c51d117202584db5231f0dd0dc9d35e8071557d3a88f4f3399f5714c560b
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.Models
{
    public class ConnectionSettings : IConnectionSettings
    {
        private Dictionary<string, string> _settings = new Dictionary<string, string>();

        public string Name { get; set; }
        public string Uri { get; set; }
        public string Baud { get; set; }
        public string IPAddressV4 { get; set; }
        public string IPAddressV6 { get; set; }
        public string AccessKey { get; set; }
        public string AccountId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Port { get; set; }
        public string DeviceId { get; set; }
        public string ResourceName { get; set; }
        public string ValidThrough { get; set; }
        public bool IsSSL { get; set; }
        public Func<bool> ValidationAction { get; set; }
        public Func<string> GetValidationErrors { get; set; }
        public Dictionary<string, string> Settings { get { return _settings; } set { _settings = value; } }

        public int TimeoutInSeconds { get; set; }
    }
}
