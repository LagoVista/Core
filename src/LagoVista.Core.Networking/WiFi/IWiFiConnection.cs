using System;

namespace LagoVista.Core.Networking.WiFi
{
    public interface IWiFiConnection
    {
        String SSID { get; }
        byte Signal {get;}
    }
}
