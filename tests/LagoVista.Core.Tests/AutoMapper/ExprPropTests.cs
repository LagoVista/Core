// File: tests/LagoVista.Core.Tests/AutoMapper/ExprPropTests.cs
using LagoVista.Core.AutoMapper;
using NUnit.Framework;
using System;

namespace LagoVista.Core.Tests.Mapping
{
    [TestFixture]
    public sealed class ExprPropTests
    {
        private sealed class Obj
        {
            public string Name { get; set; }
            public int Count { get; set; }
            public Obj Child { get; set; }
        }

        [Test]
        public void Get_ReturnsPropertyInfo_ForSimpleProperty()
        {
            var pi = ExprProp.Get<Obj, string>(x => x.Name);
            Assert.That(pi.Name, Is.EqualTo(nameof(Obj.Name)));
        }

        [Test]
        public void Get_AllowsUnaryConvert_ForValueType()
        {
            // (object)x.Count introduces a Convert node
            var pi = ExprProp.Get<Obj, object>(x => (object)x.Count);
            Assert.That(pi.Name, Is.EqualTo(nameof(Obj.Count)));
        }

        [Test]
        public void Get_Throws_WhenNotMemberAccess()
        {
            Assert.That(() => ExprProp.Get<Obj, string>(x => x.ToString()), Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void Get_Throws_WhenNotDirectPropertyAccess()
        {
            Assert.That(() => ExprProp.Get<Obj, string>(x => x.Child.Name), Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void Get_Throws_WhenExprNull()
        {
            Assert.That(() => ExprProp.Get<Obj, string>(null), Throws.TypeOf<ArgumentNullException>());
        }
    }
}