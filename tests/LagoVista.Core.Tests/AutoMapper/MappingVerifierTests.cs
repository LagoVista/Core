using LagoVista.Core.Attributes;
using LagoVista.Core.AutoMapper;
using NUnit.Framework;
using System;

namespace LagoVista.Core.Tests.Mapping
{
    [TestFixture]
    public sealed class MappingVerifierTests
    {
        // 1) Should throw when a writable target property is not covered by any atomic step
        private sealed class MissingPropSource
        {
            public string A { get; set; }
        }

        private sealed class MissingPropTarget
        {
            public string A { get; set; }

            // Not in source, writable => should be reported missing
            public string B { get; set; }
        }

        [Test]
        public void Verify_Throws_When_TargetHasUnmappedWritableProperty()
        {
            var ex = Assert.Throws<MappingVerifier.MappingVerificationException>(() =>
                MappingVerifier.Verify<MissingPropSource, MissingPropTarget>());

            Assert.That(ex!.Message, Does.Contain("Mapping verification failed"));
            Assert.That(ex.Message, Does.Contain("Missing target properties"));
            Assert.That(ex.Message, Does.Contain("B"));
        }

        // 2) IgnoreOnMapTo should cause the builder to plan this as ignored (still "covered" via TargetProperty),
        // so verifier should not report it as missing.
        private sealed class IgnoredPropSource
        {
            public string A { get; set; }
        }

        private sealed class IgnoredPropTarget
        {
            public string A { get; set; }

            [IgnoreOnMapTo]
            public string B { get; set; }
        }

        [Test]
        public void Verify_DoesNotThrow_When_TargetProperty_Is_IgnoreOnMapTo()
        {
            Assert.DoesNotThrow(() =>
                MappingVerifier.Verify<IgnoredPropSource, IgnoredPropTarget>());
        }

        // 3) Read-only target properties must NOT be required (GetRequiredTargetProperties filters CanWrite)
        private sealed class ReadOnlyTargetSource
        {
            public string A { get; set; }
        }

        private sealed class ReadOnlyTargetTarget
        {
            public string A { get; set; }

            // get-only => should not be required/validated
            public string Computed => "x";
        }

        [Test]
        public void Verify_DoesNotRequire_ReadOnlyTargetProperties()
        {
            Assert.DoesNotThrow(() =>
                MappingVerifier.Verify<ReadOnlyTargetSource, ReadOnlyTargetTarget>());
        }

        // 4) MapFrom should allow non-matching names to be treated as mapped
        private sealed class MapFromSource
        {
            public string SourceName { get; set; }
        }

        private sealed class MapFromTarget
        {
            [MapFrom("SourceName")]
            public string TargetName { get; set; }
        }

        [Test]
        public void Verify_Allows_MapFrom_NameMismatch()
        {
            Assert.DoesNotThrow(() =>
                MappingVerifier.Verify<MapFromSource, MapFromTarget>());
        }
    }
}