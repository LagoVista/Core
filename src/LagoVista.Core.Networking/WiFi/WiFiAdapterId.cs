using System;

namespace LagoVista.Core.Networking.WiFi
{

    public class WiFiAdapterId
    {
        public WiFiAdapterId() { }

        public WiFiAdapterId(string name, string id)
        {
            Name = name;
            Id = id;
        }

        public string Name { get; }
        public string Id { get; }

        public string Bssid { get; set; }

        public double SignalStrengthDB { get; set; }

        public override string ToString()
        {
            return $"{Name}:[{Id}]";
        }
    }
}
