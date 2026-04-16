using LagoVista.Core.AutoMapper;
using LagoVista.Core.Interfaces.AutoMapper;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace LagoVista.Core.Tests.Mapping
{
    [TestFixture]
    public sealed class MapValueConverterRegistryTests
    {
        private sealed class IntToStringConverter : IMapValueConverter
        {
            public bool CanConvert(Type sourceType, Type targetType)
            {
                var st = Nullable.GetUnderlyingType(sourceType) ?? sourceType;
                var tt = Nullable.GetUnderlyingType(targetType) ?? targetType;
                return st == typeof(int) && tt == typeof(string);
            }

            public object Convert(object sourceValue, Type targetType)
            {
                return ((int)sourceValue).ToString();
            }
        }

        private sealed class StringToIntConverter : IMapValueConverter
        {
            public bool CanConvert(Type sourceType, Type targetType)
            {
                var st = Nullable.GetUnderlyingType(sourceType) ?? sourceType;
                var tt = Nullable.GetUnderlyingType(targetType) ?? targetType;
                return st == typeof(string) && tt == typeof(int);
            }

            public object Convert(object sourceValue, Type targetType)
            {
                return int.Parse((string)sourceValue);
            }
        }

        [Test]
        public void Ctor_AllowsNullEnumerable_ConvertersIsEmpty()
        {
            var reg = new MapValueConverterRegistry(null);

            Assert.That(reg.Converters, Is.Not.Null);
            Assert.That(new List<IMapValueConverter>(reg.Converters).Count, Is.EqualTo(0));
        }

        [Test]
        public void GetConverter_ByType_ReturnsInstanceOrNull()
        {
            var a = new IntToStringConverter();
            var reg = new MapValueConverterRegistry(new[] { a });

            var found = reg.GetConverter(typeof(IntToStringConverter));
            var missing = reg.GetConverter(typeof(StringToIntConverter));

            Assert.That(found, Is.SameAs(a));
            Assert.That(missing, Is.Null);
        }

        [Test]
        public void TryConvert_NullSourceValue_ReturnsTrueAndNullResult()
        {
            var reg = new MapValueConverterRegistry(new IMapValueConverter[] { new IntToStringConverter() });

            var ok = reg.TryConvert(null, typeof(string), out var converted);

            Assert.That(ok, Is.True);
            Assert.That(converted, Is.Null);
        }

        [Test]
        public void TryConvert_FindsFirstMatchingConverter_AndConverts()
        {
            var reg = new MapValueConverterRegistry(new IMapValueConverter[]
            {
                new IntToStringConverter(), // should match
                new StringToIntConverter()  // should not be used
            });

            var ok = reg.TryConvert(42, typeof(string), out var converted);

            Assert.That(ok, Is.True);
            Assert.That(converted, Is.EqualTo("42"));
        }

        [Test]
        public void TryConvert_WithExplicitConverterType_FailsIfConverterNotRegisteredOrCannotConvert()
        {
            var reg = new MapValueConverterRegistry(new IMapValueConverter[] { new IntToStringConverter(), new StringToIntConverter() });

            // Not registered
            var okMissing = reg.TryConvert(42, typeof(string), typeof(StringToIntConverter), out var convertedMissing);
            Assert.That(okMissing, Is.False);
            Assert.That(convertedMissing, Is.Null);

            // Registered but cannot convert this requested pair
            var okCannot = reg.TryConvert("42", typeof(int), typeof(IntToStringConverter), out var convertedCannot);
            Assert.That(okCannot, Is.False);
            Assert.That(convertedCannot, Is.Null);
        }

        [Test]
        public void CanConvert_And_GetConverter_HandleNullableUnwrap()
        {
            var reg = new MapValueConverterRegistry(new IMapValueConverter[] { new IntToStringConverter() });

            Assert.That(reg.CanConvert(typeof(int?), typeof(string)), Is.True);

            var c = reg.GetConverter(typeof(int?), typeof(string));
            Assert.That(c, Is.Not.Null);
            Assert.That(c.GetType(), Is.EqualTo(typeof(IntToStringConverter)));
        }

        [Test]
        public void AddRange_SkipsDuplicatesByType_And_DeduplicateRemovesDupesKeepingFirst()
        {
            var a1 = new IntToStringConverter();
            var a2 = new IntToStringConverter(); // same type, different instance
            var b1 = new StringToIntConverter();

            var reg = new MapValueConverterRegistry(new IMapValueConverter[] { a1, a2, b1 });

            // Constructor can include duplicates; Deduplicate should keep first per type.
            reg.Deduplicate();
            var afterDedup = new List<IMapValueConverter>(reg.Converters);
            Assert.That(afterDedup.Count, Is.EqualTo(2));
            Assert.That(afterDedup[0].GetType(), Is.EqualTo(typeof(IntToStringConverter)));
            Assert.That(afterDedup[1].GetType(), Is.EqualTo(typeof(StringToIntConverter)));
            Assert.That(afterDedup[0], Is.SameAs(a1)); // keeps first instance

            // AddRange should skip duplicates by type
            var added = reg.AddRange(new IMapValueConverter[] { new IntToStringConverter(), new StringToIntConverter() });
            Assert.That(added, Is.EqualTo(0));
        }
    }
}