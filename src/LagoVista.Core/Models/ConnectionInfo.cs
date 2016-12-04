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
        public String Uri { get; set; }
        public String AccessKey { get; set; }
        public String AccountId { get; set; }
        public String UserName { get; set; }
        public String Password { get; set; }
        public String Port { get; set; }
        public String DeviceId { get; set; }
        public String ResourceName { get; set; }
        public bool IsSSL { get; set; }
        public Func<bool> ValidationAction { get; set; }
        public Func<string> GetValidationErrors { get; set; }       
    }
}
