using LagoVista.Core.Models.Geo;
using System;
using Xunit;

namespace LagoVista.Core.Tests.Geo
{

    public class GeoLocationMathTests
    {

        [Fact()]
        public void GeogrpahyType_Find_North()
        {
            var start = new GeoLocation()
            {
                Longitude = -82.0,
                Latitude = 27.0
            };

            var end = new GeoLocation()
            {
                Longitude = -82.0,
                Latitude = 29.0
            };

            var heading = start.HeadingTo(end);
            Console.WriteLine($"heading => {heading}");
            Assert.Equal(0, heading, 2);
        }

        [Fact()]
        public void GeogrpahyType_Find_South()
        {
            var start = new GeoLocation()
            {
                Longitude = -82.0,
                Latitude = 27.0
            };

            var end = new GeoLocation()
            {
                Longitude = -82.0,
                Latitude = 25.0
            };

            var heading = start.HeadingTo(end);
            Console.WriteLine($"heading => {heading}");
            Assert.Equal(180, heading, 2);
        }

        [Fact()]
        public void GeogrpahyType_Find_West()
        {
            var start = new GeoLocation()
            {
                Longitude = -82.0,
                Latitude = 25.0
            };

            var end = new GeoLocation()
            {
                Longitude = -83.0,
                Latitude = 25.0
            };

            var heading = start.HeadingTo(end);
            Console.WriteLine($"heading => {heading}");
            Assert.Equal(270, Convert.ToInt32(heading));
        }


        [Fact()]
        public void GeogrpahyType_Find_East()
        {
            var start = new GeoLocation()
            {
                Longitude = -82.0,
                Latitude = 25.0
            };

            var end = new GeoLocation()
            {
                Longitude = -81.0,
                Latitude = 25.0
            };

            var heading = start.HeadingTo(end);
            Console.WriteLine($"heading => {heading}");
            Assert.Equal(Convert.ToInt32(90), Convert.ToInt32(heading));
        }

    }
}
