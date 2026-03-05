// File: tests/LagoVista.Core.Tests/AutoMapper/Converters/ToEntityHeaderConverterTests.cs
using LagoVista.Core.AutoMapper.Converters;
using LagoVista.Core.Models;
using NUnit.Framework;
using System;

namespace LagoVista.Core.Tests.Mapping.Converters
{
    [TestFixture]
    public sealed class ToEntityHeaderConverterTests
    {
        private sealed class HasToEntityHeader
        {
            public EntityHeader ToEntityHeader() => new EntityHeader { Id = "id-1", Text = "text-1" };
        }

        private sealed class HasToEntityHeader_ReturnsNull
        {
            public EntityHeader ToEntityHeader() => null;
        }

        private sealed class WrongReturnType
        {
            public string ToEntityHeader() => "nope";
        }

        private sealed class PrivateToEntityHeader
        {
            private EntityHeader ToEntityHeader() => new EntityHeader { Id = "id-x", Text = "text-x" };
        }

        [Test]
        public void CanConvert_Throws_OnNullTypes()
        {
            var c = new ToEntityHeaderConverter();
            Assert.That(() => c.CanConvert(null, typeof(EntityHeader)), Throws.TypeOf<ArgumentNullException>());
            Assert.That(() => c.CanConvert(typeof(HasToEntityHeader), null), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void CanConvert_ReturnsTrue_WhenPublicInstanceToEntityHeaderExists()
        {
            var c = new ToEntityHeaderConverter();
            Assert.That(c.CanConvert(typeof(HasToEntityHeader), typeof(EntityHeader)), Is.True);
        }

        [Test]
        public void CanConvert_ReturnsFalse_WhenMethodMissingOrWrongOrPrivate()
        {
            var c = new ToEntityHeaderConverter();

            Assert.That(c.CanConvert(typeof(object), typeof(EntityHeader)), Is.False);
            Assert.That(c.CanConvert(typeof(WrongReturnType), typeof(EntityHeader)), Is.False);
            Assert.That(c.CanConvert(typeof(PrivateToEntityHeader), typeof(EntityHeader)), Is.False);
        }

        [Test]
        public void Convert_ReturnsNull_WhenSourceNull()
        {
            var c = new ToEntityHeaderConverter();
            Assert.That(c.Convert(null, typeof(EntityHeader)), Is.Null);
        }

        [Test]
        public void Convert_Throws_WhenTargetIsNotEntityHeader()
        {
            var c = new ToEntityHeaderConverter();
            Assert.That(() => c.Convert(new HasToEntityHeader(), typeof(string)), Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void Convert_Throws_WhenNoMethod()
        {
            var c = new ToEntityHeaderConverter();
            Assert.That(() => c.Convert(new object(), typeof(EntityHeader)), Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void Convert_ReturnsEntityHeader_WhenMethodReturnsHeader()
        {
            var c = new ToEntityHeaderConverter();
            var result = c.Convert(new HasToEntityHeader(), typeof(EntityHeader));

            Assert.That(result, Is.TypeOf<EntityHeader>());
            var eh = (EntityHeader)result;
            Assert.That(eh.Id, Is.EqualTo("id-1"));
            Assert.That(eh.Text, Is.EqualTo("text-1"));
        }

        [Test]
        public void Convert_ReturnsNull_WhenMethodReturnsNull()
        {
            var c = new ToEntityHeaderConverter();
            var result = c.Convert(new HasToEntityHeader_ReturnsNull(), typeof(EntityHeader));
            Assert.That(result, Is.Null);
        }
    }
}