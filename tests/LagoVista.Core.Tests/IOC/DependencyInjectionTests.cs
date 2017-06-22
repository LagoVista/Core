using System;
using LagoVista.Core.IOC;
using LagoVista.Core.Tests.Models;
using Xunit;

namespace LagoVista.Core.Tests.IOC
{
    public class DependencyInjectionTests
    {
        [Fact]
        public void SimpleSingletonInjectionTest()
        {
            SLWIOC.RegisterSingleton<IClassA, ClassA>();
            SLWIOC.RegisterSingleton<IClassB, ClassB>();
            SLWIOC.RegisterSingleton<IClassC, ClassC>();

            var classA = SLWIOC.Get<IClassA>();
            classA.PropertyA = "1234";

            Assert.Equal("1234", SLWIOC.Get<IClassA>().PropertyA);

            var classB = SLWIOC.Get<IClassB>();
            classB.PropertyA = "4567";

            Assert.Equal("4567", SLWIOC.Get<IClassB>().PropertyA);

            var classC = SLWIOC.Get<IClassC>();
            Assert.Equal(classC.ClassA.PropertyA,classA.PropertyA);
            Assert.Equal(classC.ClassB.PropertyA,classB.PropertyA);
        }

        [Fact]
        public void SimpleCreateAllTest()
        {
            SLWIOC.Register<IClassA, ClassA>();
            SLWIOC.Register<IClassB, ClassB>();
            SLWIOC.Register<IClassC, ClassC>();

            SLWIOC.Create<IClassC>();
        }
    }
}
