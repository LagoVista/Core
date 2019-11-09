using System;

namespace LagoVista.Core.Networking.WiFi
{
    public class WiFiNetworkSelectedEventArgs : EventArgs
    {
        public WiFiNetworkSelectedEventArgs(WiFiAdapterId wiFiAdapaterId) : base()
        {
            WiFiAdapaterId = wiFiAdapaterId ?? throw new ArgumentNullException(nameof(wiFiAdapaterId));
        }

        public WiFiAdapterId WiFiAdapaterId { get; }
    }
}
