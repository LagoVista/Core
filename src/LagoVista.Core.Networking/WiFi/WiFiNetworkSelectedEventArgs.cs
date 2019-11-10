using System;

namespace LagoVista.Core.Networking.WiFi
{
    public class WiFiNetworkSelectedEventArgs : EventArgs
    {
        public WiFiNetworkSelectedEventArgs(WiFiAdapter wiFiAdapater) : base()
        {
            WiFiAdapater = wiFiAdapater ?? throw new ArgumentNullException(nameof(wiFiAdapater));
        }

        public WiFiAdapter WiFiAdapater { get; }
    }
}
