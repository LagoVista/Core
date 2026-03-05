// File: tests/LagoVista.Core.Tests/AutoMapper/EntityHeaderIdConverterTests.cs
using LagoVista.Core.AutoMapper;
using LagoVista.Core.Models;
using NUnit.Framework;
using System;

namespace LagoVista.Core.Tests.Mapping.Converters
{
    [TestFixture]
    public sealed class EntityHeaderIdConverterTests
    {
        [Test]
        public void CanConvert_ReturnsFalse_WhenSourceNotEntityHeader()
        {
            var c = new EntityHeaderIdConverter();
            Assert.That(c.CanConvert(typeof(string), typeof(string)), Is.False);
        }

        [Test]
        public void CanConvert_ReturnsTrue_ForEntityHeader_ToStringGuidInt()
        {
            var c = new EntityHeaderIdConverter();

            Assert.That(c.CanConvert(typeof(EntityHeader), typeof(string)), Is.True);
            Assert.That(c.CanConvert(typeof(EntityHeader), typeof(Guid)), Is.True);
            Assert.That(c.CanConvert(typeof(EntityHeader), typeof(int)), Is.True);
        }

        [Test]
        public void Convert_NullSource_ReturnsNull()
        {
            var c = new EntityHeaderIdConverter();
            Assert.That(c.Convert(null, typeof(string)), Is.Null);
        }

        [Test]
        public void Convert_ToString_ReturnsId()
        {
            var c = new EntityHeaderIdConverter();
            var eh = new EntityHeader { Id = "abc-123", Text = "t" };

            var result = c.Convert(eh, typeof(string));

            Assert.That(result, Is.EqualTo("abc-123"));
        }

        [Test]
        public void Convert_ToGuid_ParsesGuid()
        {
            var c = new EntityHeaderIdConverter();
            var id = Guid.NewGuid().ToString("D");
            var eh = new EntityHeader { Id = id };

            var result = c.Convert(eh, typeof(Guid));

            Assert.That(result, Is.TypeOf<Guid>());
            Assert.That((Guid)result, Is.EqualTo(Guid.Parse(id)));
        }

        [Test]
        public void Convert_ToGuid_Throws_OnEmptyId_ForNonNullable()
        {
            var c = new EntityHeaderIdConverter();
            var eh = new EntityHeader { Id = "   " };

            Assert.That(() => c.Convert(eh, typeof(Guid)), Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void Convert_ToGuidNullable_ReturnsNull_OnEmptyId()
        {
            var c = new EntityHeaderIdConverter();
            var eh = new EntityHeader { Id = "" };

            var result = c.Convert(eh, typeof(Guid?));

            Assert.That(result, Is.Null);
        }

        [Test]
        public void Convert_ToInt_ParsesInt()
        {
            var c = new EntityHeaderIdConverter();
            var eh = new EntityHeader { Id = "42" };

            var result = c.Convert(eh, typeof(int));

            Assert.That(result, Is.EqualTo(42));
        }

        [Test]
        public void Convert_ToInt_Throws_OnNonNumeric()
        {
            var c = new EntityHeaderIdConverter();
            var eh = new EntityHeader { Id = "nope" };

            Assert.That(() => c.Convert(eh, typeof(int)), Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void Convert_Throws_OnUnsupportedTarget()
        {
            var c = new EntityHeaderIdConverter();
            var eh = new EntityHeader { Id = "x" };

            Assert.That(() => c.Convert(eh, typeof(DateTime)), Throws.TypeOf<InvalidOperationException>());
        }
    }
}