// File: tests/LagoVista.Core.Tests/AutoMapper/EntityHeaderTextConverterTests.cs
using LagoVista.Core.AutoMapper;
using LagoVista.Core.Models;
using NUnit.Framework;

namespace LagoVista.Core.Tests.Mapping.Converters
{
    [TestFixture]
    public sealed class EntityHeaderTextConverterTests
    {
        private sealed class HasNameOnly : EntityHeader
        {
            public string Name { get; set; }
        }

        [Test]
        public void CanConvert_ReturnsTrue_ForEntityHeader_ToString()
        {
            var c = new EntityHeaderTextConverter();
            Assert.That(c.CanConvert(typeof(EntityHeader), typeof(string)), Is.True);
        }

        [Test]
        public void Convert_Null_ReturnsNull()
        {
            var c = new EntityHeaderTextConverter();
            Assert.That(c.Convert(null, typeof(string)), Is.Null);
        }

        [Test]
        public void Convert_PrefersText()
        {
            var c = new EntityHeaderTextConverter();
            var eh = new EntityHeader { Text = "hello", Id = "id" };

            var result = c.Convert(eh, typeof(string));

            Assert.That(result, Is.EqualTo("hello"));
        }

        [Test]
        public void Convert_ReturnsNull_WhenTextMissing()
        {
            var c = new EntityHeaderTextConverter();
            var eh = new EntityHeader { Text = null };

            var result = c.Convert(eh, typeof(string));

            Assert.That(result, Is.Null);
        }
        [Test]
        public void Convert_ReturnsNull_WhenNoTextOrName()
        {
            var c = new EntityHeaderTextConverter();
            var eh = new EntityHeader { Text = null };

            var result = c.Convert(eh, typeof(string));

            Assert.That(result, Is.Null);
        }
    }
}