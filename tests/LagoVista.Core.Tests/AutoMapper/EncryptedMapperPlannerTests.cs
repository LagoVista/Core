using LagoVista.Core.Attributes;
using LagoVista.Core.AutoMapper;
using LagoVista.Core.Interfaces.AutoMapper;
using LagoVista.Core.Models;
using NUnit.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace LagoVista.Core.Tests.Mapping
{
    [TestFixture]
    public sealed class EncryptedMapperPlannerTests
    {
        // --- Minimal converter registry we can control in tests ---
        private sealed class FakeConverterRegistry : IMapValueConverterRegistry
        {
            private readonly Dictionary<(Type Source, Type Target), Func<object, object>> _maps =
                new Dictionary<(Type Source, Type Target), Func<object, object>>();

            public void Register(Type source, Type target, Func<object, object> convert)
                => _maps[(source, target)] = convert;

            public bool TryConvert(object sourceValue, Type targetType, out object convertedValue)
            {
                convertedValue = null;
                if (sourceValue == null)
                    return true;

                var st = sourceValue.GetType();
                if (_maps.TryGetValue((st, targetType), out var fn))
                {
                    convertedValue = fn(sourceValue);
                    return true;
                }

                return false;
            }

            public bool TryConvert(object sourceValue, Type targetType, Type converterType, out object convertedValue)
            {
                // Not used by EncryptedMapperPlanner
                convertedValue = null;
                return false;
            }

            public bool CanConvert(Type sourceType, Type targetType) => _maps.ContainsKey((sourceType, targetType));
            public IMapValueConverter GetConverter(Type converterType) => null;
            public IMapValueConverter GetConverter(Type sourceType, Type targetType) => null;

            public int AddRange(params IMapValueConverter[] converters)
            {
                throw new NotImplementedException();
            }

            public IEnumerable<IMapValueConverter> Converters => Array.Empty<IMapValueConverter>();
        }
        private static void ClearEncryptedMapperPlannerCache()
        {
            var field = typeof(EncryptedMapperPlanner)
                .GetField("_plans", BindingFlags.NonPublic | BindingFlags.Static);

            var dict = (ConcurrentDictionary<(Type Domain, Type Dto), object>)field!.GetValue(null)!;
            dict.Clear();
        }

        [SetUp]
        public void SetUp()
        {
            ClearEncryptedMapperPlannerCache();
        }

        [EncryptionKey("LegacyKey-{orgId}-{id}")]
        private sealed class IntOnlyDto
        {
            public Guid Id { get; set; }
            public string CipherI { get; set; }
            public string SaltI { get; set; }
        }

        private sealed class IntOnlyDomain
        {
            [EncryptedField("CipherI", SaltProperty = "SaltI")]
            public int PlainI { get; set; }
        }

        private static EntityHeader Org(string id = "org-123") => new EntityHeader { Id = id };
        private static EntityHeader User() => new EntityHeader { Id = "user-456" };

        // --- Test models ---
        [EncryptionKey("LegacyKey-{orgId}-{id}", IdProperty = "Id", CreateIfMissing = false)]
        private sealed class GoodDto
        {
            public Guid Id { get; set; }

            public string CipherA { get; set; }
            public string SaltA { get; set; }

            public string CipherB { get; set; }
            public string SaltB { get; set; }
        }

        private sealed class GoodDomain
        {
            [EncryptedField("CipherA", SaltProperty = "SaltA", SkipIfEmpty = true)]
            public string PlainA { get; set; }

            // Non-string domain type conversion paths
            [EncryptedField("CipherB", SaltProperty = "SaltB", SkipIfEmpty = false)]
            public int PlainB { get; set; }
        }

        // Missing [EncryptionKey]
        private sealed class NoKeyDto
        {
            public Guid Id { get; set; }
            public string CipherA { get; set; }
            public string SaltA { get; set; }
        }

        [EncryptionKey("LegacyKey-{orgId}-{id}", IdProperty = "MissingId")]
        private sealed class MissingIdPropDto
        {
            public Guid Id { get; set; }
        }

        [EncryptionKey("LegacyKey-{orgId}-{id}")]
        private sealed class BadCipherPropDto
        {
            public Guid Id { get; set; }
            public int CipherA { get; set; } // wrong type
            public string SaltA { get; set; }
        }

        [EncryptionKey("LegacyKey-{orgId}-{id}")]
        private sealed class MissingCipherPropDto
        {
            public Guid Id { get; set; }
            public string SaltA { get; set; }
        }

        [EncryptionKey("LegacyKey-{orgId}-{id}")]
        private sealed class MissingSaltPropDto
        {
            public Guid Id { get; set; }
            public string CipherA { get; set; }
        }

        private sealed class DomainWithNonWritable
        {
            [EncryptedField("CipherA", SaltProperty = "SaltA")]
            public string PlainA { get; } // not writable
        }

        private sealed class DomainWithNullableInt
        {
            [EncryptedField("CipherA", SaltProperty = "SaltA")]
            public int? PlainA { get; set; }
        }

        // --- Tests ---

        [Test]
        public void GetOrBuildPlan_Throws_WhenConvertersNull()
        {
            Assert.Throws<ArgumentNullException>(() => new EncryptedMapperPlanner(null));
        }

        [Test]
        public void GetOrBuildPlan_Throws_WhenDtoMissingEncryptionKeyAttribute()
        {
            var reg = new FakeConverterRegistry();
            var planner = new EncryptedMapperPlanner(reg);

            var ex = Assert.Throws<InvalidOperationException>(() => planner.GetOrBuildPlan<GoodDomain, NoKeyDto>());
            Assert.That(ex!.Message, Does.Contain("must have [EncryptionKey"));
        }

        [Test]
        public void GetOrBuildPlan_Throws_WhenDtoMissingIdProperty()
        {
            var reg = new FakeConverterRegistry();
            var planner = new EncryptedMapperPlanner(reg);

            var ex = Assert.Throws<InvalidOperationException>(() => planner.GetOrBuildPlan<GoodDomain, MissingIdPropDto>());
            Assert.That(ex!.Message, Does.Contain("missing IdProperty"));
        }

        [Test]
        public void GetOrBuildPlan_Throws_WhenCiphertextPropertyMissing()
        {
            var reg = new FakeConverterRegistry();
            var planner = new EncryptedMapperPlanner(reg);

            var ex = Assert.Throws<InvalidOperationException>(() => planner.GetOrBuildPlan<GoodDomain, MissingCipherPropDto>());
            Assert.That(ex!.Message, Does.Contain("missing ciphertext property"));
        }

        [Test]
        public void GetOrBuildPlan_Throws_WhenSaltPropertyMissing()
        {
            var reg = new FakeConverterRegistry();
            var planner = new EncryptedMapperPlanner(reg);

            var ex = Assert.Throws<InvalidOperationException>(() => planner.GetOrBuildPlan<GoodDomain, MissingSaltPropDto>());
            Assert.That(ex!.Message, Does.Contain("missing salt property"));
        }

        [Test]
        public void GetOrBuildPlan_Throws_WhenCiphertextPropertyNotStringOrNotRW()
        {
            var reg = new FakeConverterRegistry();
            var planner = new EncryptedMapperPlanner(reg);

            var ex = Assert.Throws<InvalidOperationException>(() => planner.GetOrBuildPlan<GoodDomain, BadCipherPropDto>());
            Assert.That(ex!.Message, Does.Contain("must be a readable/writable string property"));
        }

        [Test]
        public void GetOrBuildPlan_Throws_WhenDomainEncryptedFieldNotReadableWritable()
        {
            var reg = new FakeConverterRegistry();
            var planner = new EncryptedMapperPlanner(reg);

            var ex = Assert.Throws<InvalidOperationException>(() => planner.GetOrBuildPlan<DomainWithNonWritable, GoodDto>());
            Assert.That(ex!.Message, Does.Contain("must be readable and writable"));
        }

        [Test]
        public void GetOrBuildPlan_CachesPerPair()
        {
            var reg = new FakeConverterRegistry();
            reg.Register(typeof(string), typeof(string), v => v);

            var planner = new EncryptedMapperPlanner(reg);

            var p1 = planner.GetOrBuildPlan<GoodDomain, GoodDto>();
            var p2 = planner.GetOrBuildPlan<GoodDomain, GoodDto>();

            Assert.That(ReferenceEquals(p1, p2), Is.True, "planner should cache the plan instance per (Domain,Dto) pair");
        }

        [Test]
        public void BuildSecretId_ReplacesTokens_AndThrowsWhenIdEmpty()
        {
            var reg = new FakeConverterRegistry();
            reg.Register(typeof(string), typeof(string), v => v);
            reg.Register(typeof(int), typeof(string), v => ((int)v).ToString());

            var planner = new EncryptedMapperPlanner(reg);
            var plan = planner.GetOrBuildPlan<GoodDomain, GoodDto>();

            var dto = new GoodDto { Id = Guid.Parse("11111111-1111-1111-1111-111111111111") };

            var secret = plan.BuildSecretId(dto, Org("org-XYZ"));
            Assert.That(secret, Is.EqualTo("LegacyKey-org-XYZ-11111111-1111-1111-1111-111111111111"));

            // Force empty idValue by setting Id to default Guid and overriding ToString? Can't.
            // Instead, use a DTO whose IdProperty resolves to null via reflection by using a nullable ID.
            var plan2 = planner.GetOrBuildPlan<DomainWithNullableInt, DtoWithNullableId>();
            var dto2 = new DtoWithNullableId { Id = null };
            var ex = Assert.Throws<InvalidOperationException>(() => plan2.BuildSecretId(dto2, Org("org-1")));
            Assert.That(ex!.Message, Does.Contain("resolved empty"));
        }

        [EncryptionKey("LegacyKey-{orgId}-{id}", IdProperty = "Id")]
        private sealed class DtoWithNullableId
        {
            public Guid? Id { get; set; }
            public string CipherA { get; set; }
            public string SaltA { get; set; }
        }

        [Test]
        public void FieldPlan_GetSetCiphertext_AndSalt_Work()
        {
            var reg = new FakeConverterRegistry();
            reg.Register(typeof(int), typeof(string), v => ((int)v).ToString());
            reg.Register(typeof(string), typeof(int), v => int.Parse((string)v));

            var planner = new EncryptedMapperPlanner(reg);
            var plan = planner.GetOrBuildPlan<GoodDomain, GoodDto>();

            Assert.That(plan.Fields.Count, Is.EqualTo(2));

            var dto = new GoodDto
            {
                Id = Guid.NewGuid(),
                CipherA = "cipher-a",
                SaltA = "salt-a",
                CipherB = "cipher-b",
                SaltB = "salt-b"
            };

            // Field A
            var fA = plan.Fields[0];
            Assert.That(fA.CiphertextPropertyName, Is.EqualTo("CipherA"));
            Assert.That(fA.SaltPropertyName, Is.EqualTo("SaltA"));

            Assert.That(fA.GetCiphertext(dto), Is.EqualTo("cipher-a"));
            Assert.That(fA.GetSalt(dto), Is.EqualTo("salt-a"));

            fA.SetCiphertext(dto, "cipher-a-2");
            Assert.That(dto.CipherA, Is.EqualTo("cipher-a-2"));
        }

        [Test]
        public void FieldPlan_Converts_NonStringDomainType_ToAndFromString()
        {
            var registry = new MapValueConverterRegistry(new IMapValueConverter[]
            {
                new NumericStringConverter(),
            });

            var planner = new EncryptedMapperPlanner(registry);
            var plan = planner.GetOrBuildPlan<GoodDomain, GoodDto>();

            var domain = new GoodDomain { PlainB = 42 };
            var dto = new GoodDto { Id = Guid.NewGuid(), CipherB = "x", SaltB = "s" };

            var fB = plan.Fields[1];

            // Domain -> string
            var s = fB.GetPlaintext(domain);
            Assert.That(s, Is.EqualTo("42"));

            // string -> Domain
            fB.SetPlaintext(domain, "123");
            Assert.That(domain.PlainB, Is.EqualTo(123));
        }

        [Test]
        public void FieldPlan_ConvertDomainValueToString_Throws_WhenNoConverter()
        {
            var reg = new FakeConverterRegistry(); // intentionally empty
            var planner = new EncryptedMapperPlanner(reg);

            var plan = planner.GetOrBuildPlan<IntOnlyDomain, IntOnlyDto>();

            var domain = new IntOnlyDomain { PlainI = 9 };
            var f = plan.Fields[0];

            Assert.Throws<InvalidOperationException>(() => _ = f.GetPlaintext(domain));
        }

        [Test]
        public void FieldPlan_ConvertStringToDomainValue_Throws_WhenNoConverter()
        {
            var reg = new FakeConverterRegistry(); // intentionally empty
            var planner = new EncryptedMapperPlanner(reg);
            var plan = planner.GetOrBuildPlan<GoodDomain, GoodDto>();

            var domain = new GoodDomain();
            var fB = plan.Fields[1];

            var ex = Assert.Throws<InvalidOperationException>(() => fB.SetPlaintext(domain, "12"));
            Assert.That(ex!.Message, Does.Contain("No converter registered"));
        }

        [Test]
        public void FieldPlan_StringToNullableDomain_ReturnsNull_WhenPlaintextEmpty()
        {
            var reg = new FakeConverterRegistry();
            // If conversion were attempted, we'd need a converter, but empty should short-circuit to null for Nullable<T>
            var planner = new EncryptedMapperPlanner(reg);
            var plan = planner.GetOrBuildPlan<DomainWithNullableInt, DtoWithNullableIntCipher>();

            var domain = new DomainWithNullableInt { PlainA = 7 };
            var f = plan.Fields[0];

            f.SetPlaintext(domain, ""); // empty => null because domain type is int?
            Assert.That(domain.PlainA, Is.Null);
        }

        [EncryptionKey("LegacyKey-{orgId}-{id}")]
        private sealed class DtoWithNullableIntCipher
        {
            public Guid Id { get; set; }
            public string CipherA { get; set; }
            public string SaltA { get; set; }
        }
    }
}