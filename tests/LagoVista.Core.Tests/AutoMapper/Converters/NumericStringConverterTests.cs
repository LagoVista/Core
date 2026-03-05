// File: tests/LagoVista.Core.Tests/AutoMapper/NumericStringConverterTests.cs
using LagoVista.Core.AutoMapper;
using NUnit.Framework;
using System;

namespace LagoVista.Core.Tests.Mapping.Converters
{
    [TestFixture]
    public sealed class NumericStringConverterTests
    {
        [Test]
        public void CanConvert_True_ForDecimalAndDoubleToString_AndBack()
        {
            var c = new NumericStringConverter();

            Assert.That(c.CanConvert(typeof(decimal), typeof(string)), Is.True);
            Assert.That(c.CanConvert(typeof(double), typeof(string)), Is.True);
            Assert.That(c.CanConvert(typeof(string), typeof(decimal)), Is.True);
            Assert.That(c.CanConvert(typeof(string), typeof(double)), Is.True);
        }

        [Test]
        public void Convert_DecimalToString_IsInvariant()
        {
            var c = new NumericStringConverter();
            var result = (string)c.Convert(1234.56m, typeof(string));
            Assert.That(result, Is.EqualTo("1234.56"));
        }

        [Test]
        public void Convert_DoubleToString_IsInvariant()
        {
            var c = new NumericStringConverter();
            var result = (string)c.Convert(12.5d, typeof(string));
            Assert.That(result, Is.EqualTo("12.5"));
        }

        [Test]
        public void Convert_StringToDecimal_ParsesInvariant()
        {
            var c = new NumericStringConverter();
            var result = (decimal)c.Convert("77.01", typeof(decimal));
            Assert.That(result, Is.EqualTo(77.01m));
        }

        [Test]
        public void Convert_StringToDouble_ParsesInvariant()
        {
            var c = new NumericStringConverter();
            var result = (double)c.Convert("77.01", typeof(double));
            Assert.That(result, Is.EqualTo(77.01d).Within(0.0000001));
        }

        [Test]
        public void Convert_EmptyString_ToNullableDecimal_ReturnsNull()
        {
            var c = new NumericStringConverter();
            var result = c.Convert("   ", typeof(decimal?));
            Assert.That(result, Is.Null);
        }

        [Test]
        public void Convert_EmptyString_ToNonNullableDecimal_Throws()
        {
            var c = new NumericStringConverter();
            Assert.That(() => c.Convert("", typeof(decimal)), Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void Convert_Unsupported_Throws()
        {
            var c = new NumericStringConverter();
            var result = (int)c.Convert("1024", typeof(int));
            Assert.That(result, Is.EqualTo(1024));
        }
    }
}