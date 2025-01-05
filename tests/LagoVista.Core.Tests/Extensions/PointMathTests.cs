using LagoVista.Core.Models.Drawing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.Tests.Extensions
{
    [TestClass]
    public class PointMathTests
    {

        [TestMethod]
        public void SubtractPointFloatTest()
        {
            var p1 = new Point2D<float>(2.4f, 1.7f);
            var p2 = new Point2D<float>(4.5f, 4.5f);

            var result = p2 - p1;
            Assert.AreEqual(2.1, result.X);
            Assert.AreEqual(2.8, result.Y);
        }
    }
}
