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

        [Test]
        public void EncodeDecode_RoundTrip_Works_ForArbitraryBytes()
        {
            var bytes = new byte[] { 0, 1, 2, 3, 4, 5, 250, 251, 252, 253, 254, 255 };

            var encoded = Base64Url.Encode(bytes);
            Assert.That(encoded, Is.Not.Null.And.Not.Empty);
            Assert.That(encoded, Does.Not.Contain("="), "Base64Url should not contain padding.");

            var decoded = Base64Url.Decode(encoded);
            Assert.That(decoded, Is.EqualTo(bytes));
        }

        [Test]
        public void Decode_AcceptsInputsThatNeedPaddingFixup()
        {
            // This specifically targets the common Base64Url decode branch:
            // length % 4 == 2 or 3 => padding is added internally.
            var bytes = new byte[] { 1 }; // base64 would end with "==", base64url trims it.
            var encoded = Base64Url.Encode(bytes);

            // Sanity: should be unpadded
            Assert.That(encoded, Does.Not.Contain("="));

            var decoded = Base64Url.Decode(encoded);
            Assert.That(decoded, Is.EqualTo(bytes));
        }

        [Test]
        public void Decode_WhenInputContainsInvalidCharacters_Throws()
        {
            // '*' is not valid in base64/base64url
            Assert.Throws<FormatException>(() => Base64Url.Decode("abc*def"));
        }

        [Test]
        public void EncodeDecode_RoundTrip_Length12_Hits_NoPaddingBranch()
        {
            // 12 bytes => base64 length is multiple of 4, and no "=" padding is used.
            var bytes = new byte[12];
            for (var i = 0; i < bytes.Length; i++) bytes[i] = (byte)i;

            var encoded = Base64Url.Encode(bytes);

            Assert.That(encoded, Does.Not.Contain("="), "base64url output must not include padding.");

            var decoded = Base64Url.Decode(encoded);
            Assert.That(decoded, Is.EqualTo(bytes));
        }

        [Test]
        public void Decode_RoundTrip_Length1_Hits_DoublePaddingBranch()
        {
            // 1 byte => base64 would end with "==", Encode trims, Decode must add "=="
            var bytes = new byte[] { 0xAB };

            var encoded = Base64Url.Encode(bytes);

            Assert.That(encoded, Does.Not.Contain("="), "base64url output must not include padding.");

            var decoded = Base64Url.Decode(encoded);
            Assert.That(decoded, Is.EqualTo(bytes));
        }

        [Test]
        public void Decode_RoundTrip_Length2_Hits_SinglePaddingBranch()
        {
            // 2 bytes => base64 would end with "=", Encode trims, Decode must add "="
            var bytes = new byte[] { 0x01, 0x02 };

            var encoded = Base64Url.Encode(bytes);

            Assert.That(encoded, Does.Not.Contain("="), "base64url output must not include padding.");

            var decoded = Base64Url.Decode(encoded);
            Assert.That(decoded, Is.EqualTo(bytes));
        }

        [Test]
        public void Decode_WhenLengthMod4Is1_ThrowsFormatException()
        {
            // Any string whose length % 4 == 1 should throw "Invalid base64url length."
            var ex = Assert.Throws<FormatException>(() => Base64Url.Decode("A"));
            Assert.That(ex.Message, Does.Contain("Invalid base64url length."));
        }
    }
}
