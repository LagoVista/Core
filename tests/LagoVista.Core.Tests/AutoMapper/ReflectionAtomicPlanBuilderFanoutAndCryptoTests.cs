using LagoVista.Core.AutoMapper;
using LagoVista.Core.AutoMapper.Converters;
using LagoVista.Core.Attributes;
using NUnit.Framework;
using System;
using System.Linq;

namespace LagoVista.Core.Tests.AutoMapper
{
    [TestFixture]
    public class ReflectionAtomicPlanBuilderFanoutAndCryptoTests
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

            // MapTo fanout test uses this (no same-name mapping exists on target)
            [MapTo(nameof(Dst.FanoutOnly))]
            public string FanoutSource { get; set; }

            public int IntValue { get; set; }

            public DateTime When { get; set; }
        }

        private sealed class Dst
        {
            public string Name { get; set; }

            public string FanoutOnly { get; set; }

            public int? IntValue { get; set; }

            public string When { get; set; } // requires DateTime -> string converter
        }

        [Test]
        public void BuildAtomicSteps_MapToFanout_UsesMapToFanoutAssignKind()
        {
            var result = _builder.BuildAtomicSteps(typeof(Src), typeof(Dst));
            Assert.That(result.Successful, Is.True);

            var steps = result.Result;
            Assert.That(steps.Any(s => s.TargetProperty.Name == nameof(Dst.FanoutOnly)
                                       && s.SourceProperty.Name == nameof(Src.FanoutSource)
                                       && s.Kind == AtomicMapStepKind.MapToFanoutAssign), Is.True);
        }

        [EncryptionKey("secret-1")]
        private sealed class DtoWithKey
        {
            public string Id { get; set; }
        }

        private sealed class DomainWithEncryptedField
        {
            [EncryptedField("EncryptedValue")]
            public string Value { get; set; }
        }

        [Test]
        public void BuildAtomicSteps_WhenDecryptCryptoApplies_EmitsCryptoStepForEncryptedTargetProperty()
        {
            var result = _builder.BuildAtomicSteps(typeof(DtoWithKey), typeof(DomainWithEncryptedField));
            Assert.That(result.Successful, Is.True);

            var steps = result.Result;
            Assert.That(steps.Any(s => s.TargetProperty.Name == nameof(DomainWithEncryptedField.Value)
                                       && s.Kind == AtomicMapStepKind.Crypto), Is.True);
        }
    }
}
