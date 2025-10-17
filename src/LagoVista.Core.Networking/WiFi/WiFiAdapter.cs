// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 7b9cfb965eda4bac4e30775561f617b599e8f0424f00063b668c6181af99d992
// IndexVersion: 1
// --- END CODE INDEX META ---
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
