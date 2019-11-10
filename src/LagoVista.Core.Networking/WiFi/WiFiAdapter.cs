using System;

namespace LagoVista.Core.Networking.WiFi
{

    public class WiFiAdapter
    {
        public WiFiAdapter() { }

        public WiFiAdapter(string name, string id)
        {
            Name = name;
            Id = id;
        }

        public string Name { get; }
        public string Id { get; }

        public override string ToString()
        {
            return $"{Name}:[{Id}]";
        }
    }
}
