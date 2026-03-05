using LagoVista.Core.AutoMapper;
using NUnit.Framework;
using System;

namespace LagoVista.Core.Tests.Crypto
{
    [TestFixture]
    public sealed class EncryptorTests
    {
        private const string Key32 = "0123456789ABCDEF0123456789ABCDEF"; // 32 bytes ASCII
        private const string Salt = "SALT:";

        [Test]
        public void EncryptDecrypt_RoundTrip_Works()
        {
            var sut = new Encryptor();
            var plaintext = "hello-world";

            var ciphertext = sut.Encrypt(Salt, plaintext, Key32);
            Assert.That(ciphertext, Is.Not.Null.And.Not.Empty);
            Assert.That(ciphertext, Is.Not.EqualTo(plaintext));

            var roundTrip = sut.Decrypt(Salt, ciphertext, Key32);
            Assert.That(roundTrip, Is.EqualTo(plaintext));
        }

        [Test]
        public void Decrypt_WithWrongKey_ThrowsCryptographicException()
        {
            var sut = new Encryptor();
            var plaintext = "secret";

            var ciphertext = sut.Encrypt(Salt, plaintext, Key32);

            var wrongKey32 = "FEDCBA9876543210FEDCBA9876543210";

            Assert.Throws<System.Security.Cryptography.CryptographicException>(() =>
                sut.Decrypt(Salt, ciphertext, wrongKey32));
        }

        [Test]
        public void Encrypt_WithInvalidKeyLength_ThrowsCryptographicException()
        {
            var sut = new Encryptor();

            // 10 bytes is not a valid AES key size.
            var badKey = "short-key!";

            Assert.Throws<System.Security.Cryptography.CryptographicException>(() =>
                sut.Encrypt(Salt, "data", badKey));
        }

        [Test]
        public void Decrypt_WithInvalidBase64Ciphertext_ThrowsFormatException()
        {
            var sut = new Encryptor();

            Assert.Throws<FormatException>(() =>
                sut.Decrypt(Salt, "not-base64!!!", Key32));
        }
    }
}