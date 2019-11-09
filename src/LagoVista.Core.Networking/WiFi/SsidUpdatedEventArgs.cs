using System;

namespace LagoVista.Core.Networking.WiFi
{
    public class SsidUpdatedEventArgs : EventArgs
    {
        public SsidUpdatedEventArgs(string ssid)
        {
            if (string.IsNullOrEmpty(ssid)) throw new ArgumentNullException(nameof(ssid));
            SSID = ssid;
        }

        public string SSID { get; }
    }
}
