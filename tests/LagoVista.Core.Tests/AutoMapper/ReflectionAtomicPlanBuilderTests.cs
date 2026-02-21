using LagoVista.Core.AutoMapper;
using LagoVista.Core.AutoMapper.Converters;
using LagoVista.Core.Attributes;
using NUnit.Framework;
using System;
using System.Linq;

namespace LagoVista.Core.Tests.AutoMapper
{
    [TestFixture]
    public class ReflectionAtomicPlanBuilderTests
    {
        private ReflectionAtomicPlanBuilder _builder;

        [SetUp]
        public void Setup()
        {
            _builder = new ReflectionAtomicPlanBuilder(ConvertersRegistration.DefaultConverterRegistery);
        }

        private sealed class Src
        {
            public string Name { get; set; }

            [MapTo(nameof(Dst.ExternalProviderId))]
            public string ExternalProvider { get; set; }

            public string IgnoredOnTargetSource { get; set; }

            public int IntValue { get; set; }

            public int? NullableIntValue { get; set; }

            public DateTime When { get; set; }
        }

        private sealed class Dst
        {
            public string Name { get; set; }

            [MapFrom(nameof(Src.ExternalProvider))]
            public string ExternalProviderId { get; set; }

            [IgnoreOnMapTo]
            public string IgnoredOnTarget { get; set; }

            public int? IntValue { get; set; }

            public int IntValue2 { get; set; }

            public string When { get; set; } // requires DateTime -> string converter
        }

        [Test]
        public void BuildAtomicSteps_Direct_And_Nullable_And_MapFrom_And_MapTo_And_Ignore_Works()
        {
            var result = _builder.BuildAtomicSteps(typeof(Src), typeof(Dst));
            Assert.That(result.Successful, Is.True, String.Join("; ", result.Errors.Select(e => e.Message)));

            var steps = result.Result;

            // IgnoreOnMapTo => Ignored
            Assert.That(steps.Any(s => s.TargetProperty.Name == nameof(Dst.IgnoredOnTarget) && s.Kind == AtomicMapStepKind.Ignored), Is.True);

            // Direct assign Name
            Assert.That(steps.Any(s => s.TargetProperty.Name == nameof(Dst.Name) && s.SourceProperty.Name == nameof(Src.Name)), Is.True);

            // MapFrom on target
            Assert.That(steps.Any(s => s.TargetProperty.Name == nameof(Dst.ExternalProviderId) && s.SourceProperty.Name == nameof(Src.ExternalProvider)), Is.True);

            // MapTo fanout from source
            Assert.That(steps.Any(s => s.TargetProperty.Name == nameof(Dst.ExternalProviderId) && s.Kind == AtomicMapStepKind.MapToFanoutAssign)
                        || steps.Any(s => s.TargetProperty.Name == nameof(Dst.ExternalProviderId)), Is.True);

            // Nullable assign int -> int?
            Assert.That(steps.Any(s => s.TargetProperty.Name == nameof(Dst.IntValue) && s.SourceProperty.Name == nameof(Src.IntValue)), Is.True);

            // Converter assign DateTime -> string
            Assert.That(steps.Any(s => s.TargetProperty.Name == nameof(Dst.When) && s.SourceProperty.Name == nameof(Src.When) && s.Kind == AtomicMapStepKind.ConverterAssign), Is.True);
        }

        private sealed class SrcMissingConverter
        {
            public Guid Id { get; set; }
            public DateTime When { get; set; }
        }

        private sealed class DstMissingConverter
        {
            public int Id { get; set; } // Guid -> int no converter
            public int When { get; set; } // DateTime -> int no converter
        }

        [Test]
        public void BuildAtomicSteps_WhenConverterRequiredAndMissing_ThrowsAggregatedException()
        {
            var ex = Assert.Throws<MappingPlanBuildException>(() => _builder.BuildAtomicSteps(typeof(SrcMissingConverter), typeof(DstMissingConverter)));
            Assert.That(ex.Errors.Count, Is.GreaterThanOrEqualTo(2));
        }
    }
}
