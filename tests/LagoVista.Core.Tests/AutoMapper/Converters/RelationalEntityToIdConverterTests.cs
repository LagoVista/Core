// File: tests/LagoVista.Core.Tests/AutoMapper/Converters/RelationalEntityToIdConverterTests.cs
using LagoVista.Core.AutoMapper.Converters;
using LagoVista.Core.Models;
using NUnit.Framework;
using System;

namespace LagoVista.Core.Tests.Mapping.Converters

{
    [TestFixture]
    public sealed class RelationalEntityToIdConverterTests
    {
        private sealed class TestRelationalEntity : RelationalEntityBase
        {
            // RelationalEntityBase in your codebase already has Id as string (used by converter).
            // If it's not settable publicly, adjust test construction accordingly.
        }

        private static TestRelationalEntity Entity(string id)
        {
            return new TestRelationalEntity { Id = id };
        }

        [Test]
        public void CanConvert_ReturnsFalse_WhenSourceIsNotRelationalEntity()
        {
            var c = new RelationalEntityToIdConverter();
            Assert.That(c.CanConvert(typeof(string), typeof(Guid)), Is.False);
        }

        [Test]
        public void CanConvert_ShouldOnlyBeTrue_ForGuidGuidNullableOrString()
        {
            var c = new RelationalEntityToIdConverter();

            Assert.That(c.CanConvert(typeof(TestRelationalEntity), typeof(Guid)), Is.True);
            Assert.That(c.CanConvert(typeof(TestRelationalEntity), typeof(Guid?)), Is.True);
            Assert.That(c.CanConvert(typeof(TestRelationalEntity), typeof(string)), Is.True);

            // This SHOULD be false, and this test will catch the current bug.
            Assert.That(c.CanConvert(typeof(TestRelationalEntity), typeof(int)), Is.False);
        }

        [Test]
        public void Convert_NullSource_ToGuidNullable_ReturnsNull()
        {
            var c = new RelationalEntityToIdConverter();
            var result = c.Convert(null, typeof(Guid?));
            Assert.That(result, Is.Null);
        }

        [Test]
        public void Convert_NullSource_ToString_ReturnsNull()
        {
            var c = new RelationalEntityToIdConverter();
            var result = c.Convert(null, typeof(string));
            Assert.That(result, Is.Null);
        }

        [Test]
        public void Convert_NullSource_ToGuid_Throws()
        {
            var c = new RelationalEntityToIdConverter();
            Assert.That(() => c.Convert(null, typeof(Guid)), Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void Convert_ToGuid_ParsesEntityId()
        {
            var c = new RelationalEntityToIdConverter();
            var id = Guid.NewGuid();
            var entity = Entity(id.ToString("D"));

            var result = c.Convert(entity, typeof(Guid));

            Assert.That(result, Is.TypeOf<Guid>());
            Assert.That((Guid)result, Is.EqualTo(id));
        }

        [Test]
        public void Convert_ToGuidNullable_ParsesEntityId()
        {
            var c = new RelationalEntityToIdConverter();
            var id = Guid.NewGuid();
            var entity = Entity(id.ToString("D"));

            var result = c.Convert(entity, typeof(Guid?));

            Assert.That(result, Is.TypeOf<Guid>());
            Assert.That((Guid)result, Is.EqualTo(id));
        }

        [Test]
        public void Convert_ToString_ReturnsEntityId()
        {
            var c = new RelationalEntityToIdConverter();
            var id = Guid.NewGuid().ToString("D");
            var entity = Entity(id);

            var result = c.Convert(entity, typeof(string));

            Assert.That(result.ToString(), Is.EqualTo(id));
        }
    }
}

/*
FIX for RelationalEntityToIdConverter.CanConvert:

Replace:
    if ((targetType == typeof(Guid) || targetType != typeof(Guid?) || targetType == typeof(string)))
        return true;

With:
    if (targetType == typeof(Guid) || targetType == typeof(Guid?) || targetType == typeof(string))
        return true;
*/