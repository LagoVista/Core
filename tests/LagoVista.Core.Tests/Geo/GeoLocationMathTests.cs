using LagoVista.Core.Models.Geo;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LagoVista.Core.Tests.Geo
{

    [TestClass]
    public class GeoLocationMathTests
    {

        [TestMethod]
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
            Assert.AreEqual(0, heading, 2);
        }

        [TestMethod]
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
            Assert.AreEqual(180, heading, 2);
        }

        [TestMethod]
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
            Assert.AreEqual(270, Convert.ToInt32(heading));
        }


        [TestMethod]
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
            Assert.AreEqual(Convert.ToInt32(90), Convert.ToInt32(heading));
        }

    }
}
