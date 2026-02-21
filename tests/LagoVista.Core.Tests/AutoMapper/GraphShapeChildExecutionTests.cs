using LagoVista.Core.AutoMapper;
using LagoVista.Core.Interfaces.AutoMapper;
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace LagoVista.Core.Tests.AutoMapper
{
    [TestFixture]
    public class GraphShapeChildExecutionTests
    {
        public class GrandChildDto
        {
            public string Name { get; set; }
            public EntityHeader ToEntityHeader() => EntityHeader.Create(Guid.NewGuid().ToString(), Name);
        }

        public class ChildDto
        {
            public string Value { get; set; }
            public GrandChildDto GrandChild1 { get; set; }
            public GrandChildDto GrandChild2 { get; set; }
            public EntityHeader ToEntityHeader() => EntityHeader.Create(Guid.NewGuid().ToString(), Value);
        }

        public class ParentDto
        {
            public ChildDto Child1 { get; set; }
            public ChildDto Child2 { get; set; }
            public List<ChildDto> Child3 { get; set; }
        }

        public class GrandChild
        {
            public string Name { get; set; }
        }

        public class Child
        {
            public string Value { get; set; }
            public GrandChild GrandChild1 { get; set; }
            public EntityHeader<GrandChild> GrandChild2 { get; set; }
        }

        public class Parent
        {
            public Child Child1 { get; set; }
            public EntityHeader<Child> Child2 { get; set; }
            public List<Child> Child3 { get; set; }
        }

        [Test]
        public async Task WhenNoShapeConfigured_DoesNotMapChildren()
        {
            var atomicBuilder = new Mock<LagoVista.Core.AutoMapper.LagoVista.Core.AutoMapper.IAtomicPlanBuilder>();
            atomicBuilder
                .Setup(b => b.BuildAtomicSteps(It.IsAny<Type>(), It.IsAny<Type>()))
                .Returns((Type s, Type t) => InvokeResult<IReadOnlyList<AtomicMapStep>>.Create(BuildDirectSteps(s, t)));

            var mapper = new LagoVistaAutoMapper(
                encryptedMapper: new Mock<IEncryptedMapper>().Object,
                atomicBuilder: atomicBuilder.Object,
                converters: new Mock<IMapValueConverterRegistry>().Object);

            var src = new ParentDto { Child1 = new ChildDto { Value = "x" } };
            var dst = new Parent();

            await mapper.MapAsync(src, dst, EntityHeader.Create("o", "o"), EntityHeader.Create("u", "u"));

            Assert.That(dst.Child1, Is.Null);
        }

        [Test]
        public async Task WhenShapeConfigured_MapsObjectListAndEntityHeaderValue_SummaryOnly()
        {
            var atomicBuilder = new Mock<LagoVista.Core.AutoMapper.LagoVista.Core.AutoMapper.IAtomicPlanBuilder>();
            atomicBuilder
                .Setup(b => b.BuildAtomicSteps(It.IsAny<Type>(), It.IsAny<Type>()))
                .Returns((Type s, Type t) => InvokeResult<IReadOnlyList<AtomicMapStep>>.Create(BuildDirectSteps(s, t)));

            var mapper = new LagoVistaAutoMapper(
                encryptedMapper: new Mock<IEncryptedMapper>().Object,
                atomicBuilder: atomicBuilder.Object,
                converters: new Mock<IMapValueConverterRegistry>().Object);

            var src = new ParentDto
            {
                Child1 = new ChildDto
                {
                    Value = "c1",
                    GrandChild1 = new GrandChildDto { Name = "gc1" },
                    GrandChild2 = new GrandChildDto { Name = "gc2" },
                },
                Child2 = new ChildDto { Value = "c2" },
                Child3 = new List<ChildDto> { new ChildDto { Value = "l1" }, new ChildDto { Value = "l2" } }
            };

            var dst = new Parent();

            await mapper.MapAsync(src, dst, EntityHeader.Create("o", "o"), EntityHeader.Create("u", "u"), plan =>
            {
                plan.IncludeChild(p => p.Child1, s => s.Child1, child =>
                {
                    child.IncludeChild(p => p.GrandChild1, s => s.GrandChild1);
                });

                // No nested children => Value should NOT be mapped.
                plan.IncludeEntityHeaderValue(p => p.Child2, s => s.Child2);

                plan.IncludeList(p => p.Child3, s => s.Child3);
            });

            Assert.That(dst.Child1, Is.Not.Null);
            Assert.That(dst.Child1.Value, Is.EqualTo("c1"));
            Assert.That(dst.Child1.GrandChild1, Is.Not.Null);
            Assert.That(dst.Child1.GrandChild1.Name, Is.EqualTo("gc1"));

            Assert.That(dst.Child2, Is.Not.Null);
            Assert.That(dst.Child2.Text, Is.EqualTo("c2"));
            Assert.That(dst.Child2.Value, Is.Null);

            Assert.That(dst.Child3, Is.Not.Null);
            Assert.That(dst.Child3.Count, Is.EqualTo(2));
            Assert.That(dst.Child3[0].Value, Is.EqualTo("l1"));
            Assert.That(dst.Child3[1].Value, Is.EqualTo("l2"));
        }

        [Test]
        public async Task WhenEntityHeaderValueHasChildren_MapsValue()
        {
            var atomicBuilder = new Mock<LagoVista.Core.AutoMapper.LagoVista.Core.AutoMapper.IAtomicPlanBuilder>();
            atomicBuilder
                .Setup(b => b.BuildAtomicSteps(It.IsAny<Type>(), It.IsAny<Type>()))
                .Returns((Type s, Type t) => InvokeResult<IReadOnlyList<AtomicMapStep>>.Create(BuildDirectSteps(s, t)));

            var mapper = new LagoVistaAutoMapper(
                encryptedMapper: new Mock<IEncryptedMapper>().Object,
                atomicBuilder: atomicBuilder.Object,
                converters: new Mock<IMapValueConverterRegistry>().Object);

            var src = new ParentDto
            {
                Child2 = new ChildDto { Value = "c2" }
            };

            var dst = new Parent();

            await mapper.MapAsync(src, dst, EntityHeader.Create("o", "o"), EntityHeader.Create("u", "u"), plan =>
            {
                // Force Children.Count > 0 by including at least one child edge on the value mapping.
                plan.IncludeEntityHeaderValue(p => p.Child2, s => s.Child2, nested =>
                {
                    nested.IncludeChild(v => v.GrandChild1, v => v.GrandChild1);
                });
            });

            Assert.That(dst.Child2, Is.Not.Null);
            Assert.That(dst.Child2.Text, Is.EqualTo("c2"));
            Assert.That(dst.Child2.Value, Is.Not.Null);
            Assert.That(dst.Child2.Value.Value, Is.EqualTo("c2"));
        }

        private static IReadOnlyList<AtomicMapStep> BuildDirectSteps(Type sourceType, Type targetType)
        {
            var steps = new List<AtomicMapStep>();
            foreach (var tp in targetType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!tp.CanWrite) continue;

                var sp = sourceType.GetProperty(tp.Name, BindingFlags.Public | BindingFlags.Instance);
                if (sp == null) continue;

                if (tp.PropertyType != sp.PropertyType) continue;

                steps.Add(new AtomicMapStep(tp, sp, AtomicMapStepKind.DirectAssign));
            }

            return steps;
        }
    }
}
