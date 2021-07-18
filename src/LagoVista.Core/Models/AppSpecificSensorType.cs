using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Models
{
    public enum SensorTechnology
    {
        ADC,
        IO,
        Bluetooth
    }

    public class AppSpecificSensorTypes
    {
        public string Id { get; set; }

        public SensorTechnology Technology { get; set; }

        public string Key { get; set; }

        public string Name { get; set; }

        public string IconKey { get; set; }

        public string QRCode { get; set; }
        public string Description { get; set; }
        public string WebLink { get; set; }
        public int SensorConfigId { get; set; }

        public bool HasConfigurableThreshold { get; set; } 

        public double DefaultLowTolerance { get; set; }
        public double DefaultHighTolerance { get; set; }

        public string DefaultLowValueErrorCode { get; set; }
        public string DefaultHighValueErrorCode { get; set; }
    }
}
