using LagoVista.Core.Geo;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Models.Geo
{
    public enum DistanceUnits
    {
        NauticalMiles,
        Miles,
        Kilometers,
        Meters
    }

    public class GeoLocation : IGeoLocation
    {
        public GeoLocation()
        {
            LastUpdated = DateTime.UtcNow.ToJSONString();
        }

        public GeoLocation(double latitude, double longitude)
        {
            this.Latitude = latitude;
            this.Longitude = longitude;
            this.LastUpdated = DateTime.UtcNow.ToJSONString();
        }

        public GeoLocation(double latitude, double longitude, double? altitude) : this(latitude, longitude)
        {
            Altitude = altitude;
        }

        public GeoLocation(double latitude, double longitude, double? altitude, double? heading) : this(latitude, longitude, altitude)
        {
            Heading = heading;
        }

        public GeoLocation(double latitude, double longitude, double? altitude, double? heading, double? hdop, double? vdop) : this(latitude, longitude, altitude, heading)
        {
            Hdop = hdop;
            Vdop = vdop;
        }

        public GeoLocation(double latitude, double longitude, double? altitude, double? heading, double? hdop, double? vdop, double? accuracyM) : this(latitude, longitude, altitude, heading, hdop, vdop)
        {
            AccuracyMeters = accuracyM;
        }

        public GeoLocation(double latitude, double longitude, int numberSatellites, double? altitude, double? heading, double? hdop, double? vdop, double? accuracyM) : this(latitude, longitude, altitude, heading, hdop, vdop, accuracyM)
        {
            NumberSatellites = numberSatellites;
        }

        public double? Altitude { get; set; }
        public string LastUpdated { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public double? AccuracyMeters { get; set; }

        public double? Hdop { get; set; }

        public double? Vdop { get; set; }

        public bool HasLocation => Latitude.HasValue && Longitude.HasValue;

        public double? Heading { get; set; }

        public int? NumberSatellites { get; set; }

        /// <summary>
        /// Get distance between two points
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public double DistanceFrom(GeoLocation location, DistanceUnits distanceUnits = DistanceUnits.NauticalMiles)
        {
            if (!HasLocation)
            {
                return 0;
            }

            double phi_s = Latitude.Value.ToRadians(),
                   lamda_s = Longitude.Value.ToRadians(),
                   phi_f = location.Latitude.Value.ToRadians(),
                   lamda_f = location.Longitude.Value.ToRadians();

            // using vincenty formula
            double y = Math.Sqrt(Math.Pow((Math.Cos(phi_f) * Math.Sin(lamda_s - lamda_f)), 2) + Math.Pow((Math.Cos(phi_s) *
                           Math.Sin(phi_f) - Math.Sin(phi_s) * Math.Cos(phi_f) * Math.Cos(lamda_s - lamda_f)), 2));
            double x = Math.Sin(phi_s) * Math.Sin(phi_f) + Math.Cos(phi_s) * Math.Cos(phi_f) * Math.Cos(lamda_s - lamda_f);
            double delta = Math.Atan2(y, x);
            var nm = delta.ToDegrees() * 60;

            switch (distanceUnits)
            {
                case DistanceUnits.Kilometers: return nm * 1.852;
                case DistanceUnits.Meters: return nm * 1852;
                case DistanceUnits.Miles: return nm * 1.15077945;
            }

            return nm;
        }

        public double HeadingTo(GeoLocation location)
        {
            var deltaLon = location.Longitude.Value.ToRadians() - Longitude.Value.ToRadians();

            var heading = Math.Atan2(deltaLon, Math.Cos(Latitude.Value.ToRadians()) * Math.Sin(location.Latitude.Value.ToRadians()) - Math.Sin(Latitude.Value.ToRadians()) * Math.Cos(location.Latitude.Value.ToRadians()) * Math.Cos(deltaLon)) * 180 / Math.PI;
            return (heading < 0) ? heading + 360.0 : heading;
        }

        public string ToNuvIoTFormat()
        {
            if (Latitude.HasValue && Longitude.HasValue)
            {
                return $"{Latitude.Value:0.0000000},{Longitude.Value:0.0000000}";
            }
            else
            {
                return $"0.0000000,0.0000000";
            }
        }

        public override string ToString()
        {
            if (Latitude.HasValue && Longitude.HasValue)
            {
                return $"Latitude = {Latitude.Value:0.0000000}; Longitude = {Longitude.Value:0.0000000}";
            }
            else
            {
                return $"Latitude = ?; Longitude = ?";
            }

        }
    }
}
