using LagoVista.Crypto.Modern;
using NUnit.Framework;
using System;

namespace LagoVista.Core.Tests.Crypto.Modern
{
    [TestFixture]
    public class Base64UrlTests
    {
        [Test]
        public void EncodeDecode_RoundTrip_Works()
        {
            var bytes = new byte[] { 0, 1, 2, 3, 250, 251, 252, 253, 254, 255 };
            var encoded = Base64Url.Encode(bytes);
            var decoded = Base64Url.Decode(encoded);

            Assert.That(decoded, Is.EqualTo(bytes));
        }

        [Test]
        public void Decode_InvalidLength_Throws()
        {
            Assert.Throws<FormatException>(() => Base64Url.Decode("abcde"));
        }

        [Test]
        public void Encode_DoesNotContainPadding()
        {
            var bytes = new byte[] { 1, 2, 3, 4, 5 };
            var encoded = Base64Url.Encode(bytes);
            Assert.That(encoded.Contains("="), Is.False);
        }
    }
}
