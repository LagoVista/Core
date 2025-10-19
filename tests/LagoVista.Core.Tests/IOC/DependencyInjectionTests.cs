// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 621796463804a5f21bb5ac06d88a0937aac713a396ab4d2129423082c29ed1a0
// IndexVersion: 0
// --- END CODE INDEX META ---
using System;
using LagoVista.Core.IOC;
using LagoVista.Core.Tests.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LagoVista.Core.Tests.IOC
{
    [TestClass]
    public class DependencyInjectionTests
    {
        [TestMethod]
        public void SimpleSingletonInjectionTest()
        {
            SLWIOC.RegisterSingleton<IClassA, ClassA>();
            SLWIOC.RegisterSingleton<IClassB, ClassB>();
            SLWIOC.RegisterSingleton<IClassC, ClassC>();

            var classA = SLWIOC.Get<IClassA>();
            classA.PropertyA = "1234";

            Assert.AreEqual("1234", SLWIOC.Get<IClassA>().PropertyA);

            var classB = SLWIOC.Get<IClassB>();
            classB.PropertyA = "4567";

            Assert.AreEqual("4567", SLWIOC.Get<IClassB>().PropertyA);

            var classC = SLWIOC.Get<IClassC>();
            Assert.AreEqual(classC.ClassA.PropertyA,classA.PropertyA);
            Assert.AreEqual(classC.ClassB.PropertyA,classB.PropertyA);
        }

        [TestMethod]
        public void SimpleCreateAllTest()
        {
            SLWIOC.Register<IClassA, ClassA>();
            SLWIOC.Register<IClassB, ClassB>();
            SLWIOC.Register<IClassC, ClassC>();

            SLWIOC.Create<IClassC>();
        }
    }
}
