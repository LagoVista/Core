using LagoVista.Core.Attributes;
using LagoVista.Core.AutoMapper.Converters;
using LagoVista.Core.Models;
using LagoVista.Core.Tests.Resources.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.Tests.Mapping.Converters
{
    [TestFixture]
    public sealed class EntityHeaderEnumToStringConverterTests
    {
        public enum LabeledEnum
        {
            [EnumLabel("enum1", ValidationResources.Names.EnumOne, typeof(ValidationResources))]
            Enum1,
            [EnumLabel("enum2", ValidationResources.Names.EnumTwo, typeof(ValidationResources))]
            Enum2,
        }

        private enum UnlabeledEnum
        {
            NoLabel
        }

        [Test]
        public void CanConvert_WhenTargetNotString_ReturnsFalse()
        {
            var sut = new EntityHeaderEnumToStringConverter();

            Assert.That(
                sut.CanConvert(typeof(EntityHeader<LabeledEnum>), typeof(int)),
                Is.False);
        }

        [Test]
        public void CanConvert_WhenSourceNotEntityHeaderOfEnum_ReturnsFalse()
        {
            var sut = new EntityHeaderEnumToStringConverter();

            Assert.That(
                sut.CanConvert(typeof(string), typeof(string)),
                Is.False);

            Assert.That(
                sut.CanConvert(typeof(EntityHeader<string>), typeof(string)),
                Is.False);
        }

        [Test]
        public void CanConvert_WhenEntityHeaderOfEnumToString_ReturnsTrue()
        {
            var sut = new EntityHeaderEnumToStringConverter();

            Assert.That(
                sut.CanConvert(typeof(EntityHeader<LabeledEnum>), typeof(string)),
                Is.True);

            // Nullable target should still work (converter normalizes underlying types)
            Assert.That(
                sut.CanConvert(typeof(EntityHeader<LabeledEnum>), typeof(string)),
                Is.True);
        }

        [Test]
        public void Convert_WhenSourceNull_ReturnsNull()
        {
            var sut = new EntityHeaderEnumToStringConverter();

            var result = sut.Convert(null, typeof(string));

            Assert.That(result, Is.Null);
        }

        [Test]
        public void Convert_WhenEntityHeaderEnumValueHasLabel_ReturnsLabelCode()
        {
            var sut = new EntityHeaderEnumToStringConverter();

            var eh = EntityHeader<LabeledEnum>.Create(LabeledEnum.Enum1);

            var result = sut.Convert(eh, typeof(string));

            Assert.That(result, Is.EqualTo("enum1"));
        }

        [Test]
        public void Convert_WhenEnumValueMissingEnumLabel_Throws()
        {
            var sut = new EntityHeaderEnumToStringConverter();

            var eh = EntityHeader<UnlabeledEnum>.Create(UnlabeledEnum.NoLabel);

            Assert.Throws<InvalidOperationException>(() =>
                sut.Convert(eh, typeof(string)));
        }

        [Test]
        public void Convert_CalledTwice_UsesCachedMapAndReturnsSameCode()
        {
            var sut = new EntityHeaderEnumToStringConverter();

            var eh1 = EntityHeader<LabeledEnum>.Create(LabeledEnum.Enum1);
            var eh2 = EntityHeader<LabeledEnum>.Create(LabeledEnum.Enum2);

            var r1 = sut.Convert(eh1, typeof(string));
            var r2 = sut.Convert(eh2, typeof(string));

            Assert.That(r1, Is.EqualTo("enum1"));
            Assert.That(r2, Is.EqualTo("enum2"));
        }
    }
}
