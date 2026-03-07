using LagoVista.Core.AutoMapper;
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace LagoVista.Core.Tests.Mapping
{
    [TestFixture]
    public sealed class MappingPlanComposerTests
    {
        private sealed class FakeAtomicPlanBuilder : IAtomicPlanBuilder
        {
            private readonly InvokeResult<IReadOnlyList<AtomicMapStep>> _result;

            public FakeAtomicPlanBuilder(InvokeResult<IReadOnlyList<AtomicMapStep>> result)
            {
                _result = result;
            }

            public InvokeResult<IReadOnlyList<AtomicMapStep>> BuildAtomicSteps(Type sourceType, Type targetType) => _result;
        }

        // --- Test models used to drive Include(...) branches ---

        private sealed class ChildSource { public string Name { get; set; } }
        private sealed class ChildTarget { public string Name { get; set; } }

        private sealed class ListSource { public List<ChildSource> Items { get; set; } = new(); }
        private sealed class ListTarget { public List<ChildTarget> Items { get; set; } = new(); }

        private sealed class EhSource { public ChildSource Customer { get; set; } = new(); }
        private sealed class EhTarget { public EntityHeader<ChildTarget> Customer { get; set; } }

        private sealed class ObjSource { public ChildSource Customer { get; set; } = new(); }
        private sealed class ObjTarget { public ChildTarget Customer { get; set; } }

        private sealed class NotListSource
        {
            public ChildSource Items { get; set; } = new(); // <-- exists, but NOT a List<T>
        }


        private static InvokeResult<IReadOnlyList<AtomicMapStep>> OkAtomic()
            => InvokeResult<IReadOnlyList<AtomicMapStep>>.Create(new AtomicMapStep[0]);

        private static InvokeResult<IReadOnlyList<AtomicMapStep>> FailAtomic(string msg)
            => InvokeResult<IReadOnlyList<AtomicMapStep>>.FromErrors(new ErrorMessage { Message = msg });

        [Test]
        public void Include_ListProperty_DispatchesToIncludeList()
        {
            var composer = new MappingPlanComposer<ListSource, ListTarget>(new FakeAtomicPlanBuilder(OkAtomic()));

            composer.Include(t => t.Items);

            // We don’t need to inspect internal GraphShape strongly here; just assert the fluent call worked
            // and Build doesn’t throw due to Include(...) itself.
            var built = composer.Build();
            Assert.That(built.Successful, Is.True);
        }

        // And update ONLY this test to use NotListSource + ListTarget:
        [Test]
        public void Include_ListProperty_Throws_WhenSourceIsNotList()
        {
            var ex = Assert.Throws<InvalidOperationException>(() =>
            {
                var composer = new MappingPlanComposer<NotListSource, ListTarget>(new FakeAtomicPlanBuilder(OkAtomic()));
                composer.Include(t => t.Items);
            });

            Assert.That(ex!.Message, Does.Contain("target is List"));
            Assert.That(ex.Message, Does.Contain("but source is"));
        }

        [Test]
        public void Include_EntityHeaderProperty_DispatchesToIncludeEntityHeaderValue()
        {
            var composer = new MappingPlanComposer<EhSource, EhTarget>(new FakeAtomicPlanBuilder(OkAtomic()));

            composer.Include(t => t.Customer);

            var built = composer.Build();
            Assert.That(built.Successful, Is.True);
        }

        [Test]
        public void Include_ObjectChild_DispatchesToIncludeChild()
        {
            var composer = new MappingPlanComposer<ObjSource, ObjTarget>(new FakeAtomicPlanBuilder(OkAtomic()));

            composer.Include(t => t.Customer);

            var built = composer.Build();
            Assert.That(built.Successful, Is.True);
        }

        [Test]
        public void Include_Throws_WhenNoMatchingSourcePropertyName()
        {
            var composer = new MappingPlanComposer<ObjSource, ObjTargetNoMatch>(new FakeAtomicPlanBuilder(OkAtomic()));

            var ex = Assert.Throws<InvalidOperationException>(() => composer.Include(t => t.Missing));
            Assert.That(ex!.Message, Does.Contain("no matching source property"));
        }

        private sealed class ObjTargetNoMatch
        {
            public ChildTarget Missing { get; set; }
        }

        [Test]
        public void Build_ReturnsErrors_WhenAtomicBuilderFails()
        {
            var composer = new MappingPlanComposer<ObjSource, ObjTarget>(new FakeAtomicPlanBuilder(FailAtomic("atomic failed")));

            var res = composer.Build();

            Assert.That(res.Successful, Is.False);
            Assert.That(res.Errors, Is.Not.Null);
            Assert.That(res.Errors[0].Message, Does.Contain("atomic failed"));
        }

        [Test]
        public void Build_ReturnsErrors_WhenGraphValidatorFindsIssues()
        {
            // Easiest way to force the graph validator to complain:
            // create a cycle by mapping child types to each other and including child.
            // The validator should detect recursion/reachability issues (depending on its rules).
            var composer = new MappingPlanComposer<CycleA, CycleB>(new FakeAtomicPlanBuilder(OkAtomic()));
            composer.Include(t => t.Next); // Next exists on both, and types refer back to each other

            var res = composer.Build();

            Assert.That(res.Successful, Is.False);
            Assert.That(res.Errors, Is.Not.Null.And.Not.Empty);
        }

        private sealed class CycleA { public CycleB Next { get; set; } }
        private sealed class CycleB { public CycleA Next { get; set; } }
    }
}