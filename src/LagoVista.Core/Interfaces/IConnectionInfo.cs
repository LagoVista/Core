using System;
using System.Collections.Generic;

namespace LagoVista.Core.Interfaces
{
    public interface IConnectionSettings
    {
        String Name { get; set; }
        String Uri { get; set; }
        String Baud { get; set; }
        String IPAddressV4 { get; set; }
        String IPAddressV6 { get; set; }
        String AccessKey { get; set; }
        String UserName { get; set; }
        String AccountId { get; set; }
        String DeviceId { get; set; }
        String Password { get; set; }
        String Port { get; set; }
        String ResourceName { get; set; }
        string ValidThrough { get; set; }
        bool IsSSL { get; set; }
        Func<bool> ValidationAction { get; set; }
        Func<string> GetValidationErrors { get; set; }
        Dictionary<string, string> Settings { get; set; }
        int TimeoutInSeconds { get; set; }
    }
}
