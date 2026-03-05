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
    public sealed class StringToEntityHeaderEnumConverterTests
    {
        private enum LabeledEnum
        {
            [EnumLabel("alpha", ValidationResources.Names.EnumOne, typeof(ValidationResources))]
            Alpha,
            [EnumLabel("beta", ValidationResources.Names.EnumTwo, typeof(ValidationResources))]
            Beta,
        }

        private enum DuplicateCodeEnum
        {
            [EnumLabel("dup", ValidationResources.Names.EnumOne, typeof(ValidationResources))]
            One,
            [EnumLabel("dup", ValidationResources.Names.EnumTwo, typeof(ValidationResources))]
            Two,

        }

        [Test]
        public void CanConvert_WhenSourceNotString_ReturnsFalse()
        {
            var sut = new StringToEntityHeaderEnumConverter();

            Assert.That(
                sut.CanConvert(typeof(int), typeof(EntityHeader<LabeledEnum>)),
                Is.False);
        }

        [Test]
        public void CanConvert_WhenTargetNotEntityHeaderEnum_ReturnsFalse()
        {
            var sut = new StringToEntityHeaderEnumConverter();

            Assert.That(
                sut.CanConvert(typeof(string), typeof(string)),
                Is.False);

            Assert.That(
                sut.CanConvert(typeof(string), typeof(EntityHeader<string>)),
                Is.False);
        }

        [Test]
        public void CanConvert_WhenStringToEntityHeaderEnum_ReturnsTrue()
        {
            var sut = new StringToEntityHeaderEnumConverter();

            Assert.That(
                sut.CanConvert(typeof(string), typeof(EntityHeader<LabeledEnum>)),
                Is.True);
        }

        [Test]
        public void Convert_WhenSourceNull_ReturnsNull()
        {
            var sut = new StringToEntityHeaderEnumConverter();

            var result = sut.Convert(null, typeof(EntityHeader<LabeledEnum>));

            Assert.That(result, Is.Null);
        }

        [Test]
        public void Convert_WhenCodeWhitespace_ReturnsNull()
        {
            var sut = new StringToEntityHeaderEnumConverter();

            var result = sut.Convert("   ", typeof(EntityHeader<LabeledEnum>));

            Assert.That(result, Is.Null);
        }

        [Test]
        public void Convert_WhenKnownCode_ReturnsEntityHeaderWithEnumValue()
        {
            var sut = new StringToEntityHeaderEnumConverter();

            var result = sut.Convert("alpha", typeof(EntityHeader<LabeledEnum>));

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<EntityHeader<LabeledEnum>>());

            var eh = (EntityHeader<LabeledEnum>)result;
            Assert.That(eh.Value, Is.EqualTo(LabeledEnum.Alpha));
        }

        [Test]
        public void Convert_WhenKnownCodeDifferentCase_StillMapsBecauseIgnoreCase()
        {
            var sut = new StringToEntityHeaderEnumConverter();

            var result = sut.Convert("BeTa", typeof(EntityHeader<LabeledEnum>));

            var eh = (EntityHeader<LabeledEnum>)result;
            Assert.That(eh.Value, Is.EqualTo(LabeledEnum.Beta));
        }

        [Test]
        public void Convert_WhenUnknownCode_Throws()
        {
            var sut = new StringToEntityHeaderEnumConverter();

            Assert.Throws<InvalidOperationException>(() =>
                sut.Convert("nope", typeof(EntityHeader<LabeledEnum>)));
        }

        [Test]
        public void Convert_CalledTwice_UsesCachedMapAndReturnsSameEnum()
        {
            var sut = new StringToEntityHeaderEnumConverter();

            var r1 = (EntityHeader<LabeledEnum>)sut.Convert("alpha", typeof(EntityHeader<LabeledEnum>));
            var r2 = (EntityHeader<LabeledEnum>)sut.Convert("alpha", typeof(EntityHeader<LabeledEnum>));

            Assert.That(r1.Value, Is.EqualTo(LabeledEnum.Alpha));
            Assert.That(r2.Value, Is.EqualTo(LabeledEnum.Alpha));
        }

        [Test]
        public void Convert_WhenDuplicateEnumLabelCodesExist_Throws()
        {
            var sut = new StringToEntityHeaderEnumConverter();

            // Building the enum map should explode due to duplicate label codes.
            Assert.Throws<InvalidOperationException>(() =>
                sut.Convert("dup", typeof(EntityHeader<DuplicateCodeEnum>)));
        }
    }
}
