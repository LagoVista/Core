// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: de6ba4458de944f78e04f381ee2c8af2e5d18a4e526e7bcd74619c7a40bc7d29
// IndexVersion: 1
// --- END CODE INDEX META ---
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
