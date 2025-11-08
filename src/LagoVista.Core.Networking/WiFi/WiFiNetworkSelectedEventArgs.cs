// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 95b92e91540926d82162945143bd178ee79a5214a8dc6993ee529588cd6ccce3
// IndexVersion: 2
// --- END CODE INDEX META ---
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
