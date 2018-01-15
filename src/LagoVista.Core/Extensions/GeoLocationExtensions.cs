﻿using LagoVista.Core.Models.Geo;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core
{
    public static class GeoLocationExtensions
    {

        public static double ToRadians(this double degrees)
        {
            return degrees * (Math.PI / 180.0);
        }

        public static double ToDegrees(this double radians)
        {
            return (180 / Math.PI) * radians;
        }

        public static GeoLocation ToCoreLocation(this string value, GeoLocation defaultValue = null)
        {
            if (String.IsNullOrEmpty(value)) return defaultValue;
            var parts = value.Split(',');
            if (parts.Length != 2) return defaultValue;
            var strLat = parts[0];
            var strLon = parts[1];

            if (double.TryParse(strLat, out double lat) &&
                double.TryParse(strLon, out double lon))
            {
                if (lat > 90 || lat < -90) return defaultValue;
                if (lon > 180 || lon < -180) return defaultValue;

                ///NuvIoT doesn't include altitude...perhaps someday it should.
                return new GeoLocation() { Latitude = lat, Longitude = lon };
            }
            else
            {
                return defaultValue;
            }
        }
    }
}
