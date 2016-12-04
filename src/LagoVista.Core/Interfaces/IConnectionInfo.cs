using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.Interfaces
{
    public interface IConnectionSettings
    {
        String Uri { get; set; }
        String AccessKey { get; set; }
        String UserName { get; set; }
        String AccountId { get; set; }
        String DeviceId { get; set; }
        String Password { get; set; }
        String Port { get; set; }
        String ResourceName { get; set; }
        bool IsSSL { get; set; }

        Func<bool> ValidationAction { get; set; }
        Func<string> GetValidationErrors { get; set; }

    }
}
