using LagoVista.Core.Attributes;
using LagoVista.Core.AutoMapper;
using LagoVista.Core.AutoMapper.LagoVista.Core.AutoMapper;
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LagoVista.Core.Tests.Mapping
{
    [TestFixture]
    public sealed class ReflectionMappingPlanGraphValidatorTests
    {
        private sealed class AtomicOkBuilder : IAtomicPlanBuilder
        {
            public InvokeResult<IReadOnlyList<AtomicMapStep>> BuildAtomicSteps(Type sourceType, Type targetType)
                => InvokeResult<IReadOnlyList<AtomicMapStep>>.Create(Array.Empty<AtomicMapStep>());
        }

        private sealed class AtomicThrowBuilder : IAtomicPlanBuilder
        {
            public InvokeResult<IReadOnlyList<AtomicMapStep>> BuildAtomicSteps(Type sourceType, Type targetType)
                => throw new MappingPlanBuildException(new[] { "boom" });
        }

        // ---------- Models for cycle detection ----------
        private sealed class CycleA { public CycleB Next { get; set; } }
        private sealed class CycleB { public CycleA Next { get; set; } }

        [Test]
        public void Validate_ReturnsCycleError_WhenGraphCycles()
        {
            var v = new ReflectionMappingPlanGraphValidator(new AtomicOkBuilder());

            var errs = v.Validate(typeof(CycleA), typeof(CycleB));

            Assert.That(errs, Is.Not.Null.And.Not.Empty);
            Assert.That(errs.Any(e => e.Contains("Cycle detected", StringComparison.OrdinalIgnoreCase)), Is.True);
        }

        [Test]
        public void EnumeratePairs_ThrowsMappingPlanBuildException_WhenGraphCycles()
        {
            var v = new ReflectionMappingPlanGraphValidator(new AtomicOkBuilder());

            var ex = Assert.Throws<MappingPlanBuildException>(() =>
            {
                // force enumeration
                _ = v.EnumeratePairs(typeof(CycleA), typeof(CycleB)).ToList();
            });

            Assert.That(ex!.Errors, Is.Not.Null.And.Not.Empty);
            Assert.That(ex.Errors[0], Does.Contain("Cycle detected"));
        }

        // ---------- Dictionary unsupported ----------
        private sealed class DictSource { public Dictionary<string, string> Data { get; set; } = new(); }
        private sealed class DictTarget { public Dictionary<string, string> Data { get; set; } = new(); }

        [Test]
        public void Validate_ReturnsError_WhenDictionaryMappingEncountered()
        {
            var v = new ReflectionMappingPlanGraphValidator(new AtomicOkBuilder());

            var errs = v.Validate(typeof(DictSource), typeof(DictTarget));

            Assert.That(errs, Is.Not.Null.And.Not.Empty);
            Assert.That(errs.Any(e => e.Contains("Dictionary mapping not supported", StringComparison.OrdinalIgnoreCase)), Is.True);
        }

        // ---------- IgnoreOnMapTo skips recursion ----------
        private sealed class IgnoreSource { public IgnoreChildSource Child { get; set; } }
        private sealed class IgnoreChildSource { public CycleB Next { get; set; } }

        private sealed class IgnoreTarget
        {
            [IgnoreOnMapTo]
            public IgnoreChildTarget Child { get; set; }
        }

        private sealed class IgnoreChildTarget { public CycleA Next { get; set; } }

        [Test]
        public void Validate_DoesNotRecurse_WhenTargetHasIgnoreOnMapTo()
        {
            var v = new ReflectionMappingPlanGraphValidator(new AtomicOkBuilder());

            // If it recursed into Child, we'd hit the CycleA/CycleB cycle. It must not.
            var errs = v.Validate(typeof(IgnoreSource), typeof(IgnoreTarget));

            Assert.That(errs, Is.Empty);
        }

        // ---------- EntityHeader<T> is leaf ----------
        private sealed class EhTSource { public EntityHeader<CycleA> Ref { get; set; } }
        private sealed class EhTTarget { public EntityHeader<CycleB> Ref { get; set; } }

        [Test]
        public void Validate_TreatsEntityHeaderOfT_AsLeaf_NoCycle()
        {
            var v = new ReflectionMappingPlanGraphValidator(new AtomicOkBuilder());

            // Even though CycleA/CycleB would cycle, EntityHeader<T> is leaf so no recursion, no cycle error.
            var errs = v.Validate(typeof(EhTSource), typeof(EhTTarget));

            Assert.That(errs, Is.Empty);
        }

        // ---------- Non-generic EntityHeader recurses ----------
        private sealed class EhSource { public EntityHeader EH { get; set; } }
        private sealed class EhTarget { public EntityHeader EH { get; set; } }

        [Test]
        public void Validate_RecurseOnNonGenericEntityHeader()
        {
            var v = new ReflectionMappingPlanGraphValidator(new AtomicOkBuilder());

            // Non-generic EntityHeader is treated as child edge (EntityHeaderValue) and recurses.
            // It should at least validate without blowing up; may add errors only if atomic builder does.
            var errs = v.Validate(typeof(EhSource), typeof(EhTarget));

            Assert.That(errs, Is.Empty);
        }

        // ---------- Lists recurse on element type ----------
        private sealed class ListSource { public List<CycleA> Items { get; set; } = new(); }
        private sealed class ListTarget { public List<CycleB> Items { get; set; } = new(); }

        [Test]
        public void Validate_RecurseOnListElementType_AndDetectsCycles()
        {
            var v = new ReflectionMappingPlanGraphValidator(new AtomicOkBuilder());

            // Items(Collection) leads to CycleA->CycleB recursion and then cycle via Next.
            var errs = v.Validate(typeof(ListSource), typeof(ListTarget));

            Assert.That(errs, Is.Not.Empty);
            Assert.That(errs.Any(e => e.Contains("Cycle detected", StringComparison.OrdinalIgnoreCase)), Is.True);
        }

        // ---------- Atomic builder exceptions are captured into errors ----------
        private sealed class AtomicErrSource { public string Name { get; set; } }
        private sealed class AtomicErrTarget { public string Name { get; set; } }

        [Test]
        public void Validate_CollectsAtomicBuilderErrors_WhenBuilderThrowsMappingPlanBuildException()
        {
            var v = new ReflectionMappingPlanGraphValidator(new AtomicThrowBuilder());

            var errs = v.Validate(typeof(AtomicErrSource), typeof(AtomicErrTarget));

            Assert.That(errs, Is.Not.Empty);
            Assert.That(errs.Any(e => e.Contains("boom", StringComparison.OrdinalIgnoreCase)), Is.True);
        }

        // ---------- EnumeratePairs yields root pair ----------
        [Test]
        public void EnumeratePairs_YieldsRootPair_First()
        {
            var v = new ReflectionMappingPlanGraphValidator(new AtomicOkBuilder());

            var first = v.EnumeratePairs(typeof(AtomicErrSource), typeof(AtomicErrTarget)).First();

            Assert.That(first.SourceType, Is.EqualTo(typeof(AtomicErrSource)));
            Assert.That(first.TargetType, Is.EqualTo(typeof(AtomicErrTarget)));
            Assert.That(first.Depth, Is.EqualTo(0));
            Assert.That(first.Path, Does.Contain("AtomicErrSource->AtomicErrTarget"));
        }
    }
}