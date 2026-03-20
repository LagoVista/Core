using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Models.Crypto;
using LagoVista.Core.Validation;
using LagoVista.Crypto.Modern;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static LagoVista.Core.Tests.Mapping.LagoVistaAutoMapperV1Tests;

namespace LagoVista.Core.Tests.Crypto.Modern
{
    [TestFixture]
    public class ModernEncryptionServiceE2ETests
    {
        private static EntityHeader OrgHeader => EntityHeader.Create(Guid.NewGuid().ToString(), "org");
        private static EntityHeader UserHeader => EntityHeader.Create(Guid.NewGuid().ToString(), "user");


        private static IModernEncryption BuildService(ISecureStorage secureStorage)
        {
            var aad = new AadBuilderV1();
            var env = new EnvelopeCodecV2();
            var store = new SecureStorageKeyMaterialStore(secureStorage);
            var aead = new AesGcmEncryptorNet9();

            return new ModernEncryptionService(aad, env, store, aead);
        }

        [Test]
        public async Task EncryptDecrypt_RoundTrip_Works()
        {
            var secrets = new ConcurrentDictionary<string, string>(StringComparer.Ordinal);
            var secureStorage = new FakeSecureStorage();

            var org = OrgHeader;
            var user = UserHeader;
            var svc = BuildService(secureStorage);

            var req = new EncryptStringRequest
            {
                OrgId = new GuidString36(("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa")),
                RecId = new GuidString36(("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb")),
                FieldName = "EncryptedBalance",
                KeyId = "account-0123456789abcdef0123456789abcdef:v2",
                Kv = 1,
                Plaintext = "142.44",
                Org = org,
                User = user,
            };

            var envelope = await svc.EncryptAsync(req);
            Assert.That(envelope, Does.StartWith("enc;v=2;"));

            var decReq = new DecryptStringRequest
            {
                OrgId = req.OrgId,
                RecId = req.RecId,
                FieldName = req.FieldName,
                KeyId = req.KeyId,
                Envelope = envelope,
                Org = org,
                User = user,
            };

            var plaintext = await svc.DecryptAsync(decReq);
            Assert.That(plaintext, Is.EqualTo("142.44"));
        }

        [Test]
        public void EncryptDecrypt_RoundTrip_Works2()
        {
            var sut = new AesGcmEncryptorNet9();

            var key32 = RandomNumberGenerator.GetBytes(32);
            var nonce12 = RandomNumberGenerator.GetBytes(12);
            var aad = Encoding.UTF8.GetBytes("aad-v1");
            var plaintext = Encoding.UTF8.GetBytes("hello secure world");

            var ciphertext = new byte[plaintext.Length];
            var tag16 = new byte[16];

            sut.Encrypt(key32, nonce12, plaintext, aad, ciphertext, tag16);

            Assert.That(ciphertext, Is.Not.EqualTo(plaintext)); // sanity: should differ for non-empty input

            var roundTrip = sut.Decrypt(key32, nonce12, ciphertext, tag16, aad);

            Assert.That(roundTrip, Is.EqualTo(plaintext));
        }

        [Test]
        public async Task Encrypt_SamePlaintextTwice_ProducesDifferentEnvelope()
        {
            var secrets = new ConcurrentDictionary<string, string>(StringComparer.Ordinal);
            var secureStorage = new FakeSecureStorage();

            var org = OrgHeader;
            var user = UserHeader;
            var svc = BuildService(secureStorage);

            var req = new EncryptStringRequest
            {
                OrgId = new GuidString36(("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa")),
                RecId = new GuidString36(("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb")),
                FieldName = "EncryptedBalance",
                KeyId = "account-0123456789abcdef0123456789abcdef:v2",
                Kv = 1,
                Plaintext = "142.44",
                Org = org,
                User = user,
            };

            var e1 = await svc.EncryptAsync(req);
            var e2 = await svc.EncryptAsync(req);

            Assert.That(e1, Is.Not.EqualTo(e2));
        }

        [Test]
        public async Task Decrypt_WithWrongFieldName_FailsAuthentication()
        {
            var secrets = new ConcurrentDictionary<string, string>(StringComparer.Ordinal);
            var secureStorage = new FakeSecureStorage();

            var org = OrgHeader;
            var user = UserHeader;
            var svc = BuildService(secureStorage);

            var req = new EncryptStringRequest
            {
                OrgId = new GuidString36(("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa")),
                RecId = new GuidString36(("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb")),
                FieldName = "EncryptedBalance",
                KeyId = "account-0123456789abcdef0123456789abcdef:v2",
                Kv = 1,
                Plaintext = "142.44",
                Org = org,
                User = user,
            };

            var envelope = await svc.EncryptAsync(req);

            var bad = new DecryptStringRequest
            {
                OrgId = req.OrgId,
                RecId = req.RecId,
                FieldName = "EncryptedOnlineBalance", // different => AAD mismatch
                KeyId = req.KeyId,
                Envelope = envelope,
                Org = org,
                User = user,
            };

            Assert.ThrowsAsync<AuthenticationTagMismatchException>(async () => await svc.DecryptAsync(bad));
        }

        [Test]
        public void Encrypt_BadPlaintext_ThrowsArgumentNullException()
        {
            var secrets = new ConcurrentDictionary<string, string>(StringComparer.Ordinal);
            var secureStorage = new FakeSecureStorage();

            var org = OrgHeader;
            var user = UserHeader;
            var svc = BuildService(secureStorage);

            var req = new EncryptStringRequest
            {
                OrgId = GuidString36.Factory(),
                RecId = GuidString36.Factory(),
                FieldName = "EncryptedBalance",
                KeyId = "account-0123456789abcdef0123456789abcdef:v2",
                Kv = 1,
                Plaintext = "   ",
                Org = org,
                User = user,
            };

            Assert.ThrowsAsync<ArgumentNullException>(async () => await svc.EncryptAsync(req));
        }

        [Test]
        public void Decrypt_BadEnvelope_ThrowsArgumentNullException()
        {
            var secrets = new ConcurrentDictionary<string, string>(StringComparer.Ordinal);
            var secureStorage = new FakeSecureStorage();

            var org = OrgHeader;
            var user = UserHeader;
            var svc = BuildService(secureStorage);

            var req = new DecryptStringRequest
            {
                OrgId = GuidString36.Factory(),
                RecId = GuidString36.Factory(),
                FieldName = "EncryptedBalance",
                KeyId = "account-0123456789abcdef0123456789abcdef:v2",
                Envelope = "",
                Org = org,
                User = user,
            };

            Assert.ThrowsAsync<ArgumentNullException>(async () => await svc.DecryptAsync(req));
        }

        [Test]
        public void Decrypt_NonModernEnvelope_ThrowsFormatException()
        {
            var secrets = new ConcurrentDictionary<string, string>(StringComparer.Ordinal);
            var secureStorage = new FakeSecureStorage();

            var org = OrgHeader;
            var user = UserHeader;
            var svc = BuildService(secureStorage);

            var req = new DecryptStringRequest
            {
                OrgId = GuidString36.Factory(),
                RecId = GuidString36.Factory(),
                FieldName = "EncryptedBalance",
                KeyId = "account-0123456789abcdef0123456789abcdef:v2",
                Envelope = "legacybase64==",
                Org = org,
                User = user,
            };

            Assert.ThrowsAsync<FormatException>(async () => await svc.DecryptAsync(req));
        }
        
        [Test]
        public void Encrypt_WhenTagBufferWrongLength_Throws()
        {
            var sut = new AesGcmEncryptorNet9();

            var key32 = RandomNumberGenerator.GetBytes(32);
            var nonce12 = RandomNumberGenerator.GetBytes(12);
            var aad = Array.Empty<byte>();
            var plaintext = new byte[10];

            var ciphertext = new byte[plaintext.Length];
            var tagWrong = new byte[15]; // wrong

            var ex = Assert.Throws<ArgumentOutOfRangeException>(() =>
                sut.Encrypt(key32, nonce12, plaintext, aad, ciphertext, tagWrong));

            Assert.That(ex.ParamName, Is.EqualTo("tag16Out"));
        }

        [Test]
        public void Decrypt_WhenKeyWrongLength_Throws()
        {
            var sut = new AesGcmEncryptorNet9();

            var keyWrong = RandomNumberGenerator.GetBytes(31); // wrong
            var nonce12 = RandomNumberGenerator.GetBytes(12);
            var aad = Array.Empty<byte>();
            var ciphertext = new byte[1];
            var tag16 = new byte[16];

            var ex = Assert.Throws<ArgumentOutOfRangeException>(() =>
                sut.Decrypt(keyWrong, nonce12, ciphertext, tag16, aad));

            Assert.That(ex.ParamName, Is.EqualTo("key32"));
        }




        [Test]
            public void Encrypt_WhenCiphertextBufferWrongSize_Throws()
            {
                var sut = new AesGcmEncryptorNet9();

                var key32 = RandomNumberGenerator.GetBytes(32);
                var nonce12 = RandomNumberGenerator.GetBytes(12);
                var aad = Array.Empty<byte>();
                var plaintext = new byte[10];

                var ciphertextWrong = new byte[9]; // wrong
                var tag16 = new byte[16];

                var ex = Assert.Throws<ArgumentOutOfRangeException>(() =>
                    sut.Encrypt(key32, nonce12, plaintext, aad, ciphertextWrong, tag16));

                Assert.That(ex.ParamName, Is.EqualTo("ciphertextOut"));
            }

            [Test]
            public void Decrypt_WhenNonceWrongLength_Throws()
            {
                var sut = new AesGcmEncryptorNet9();

                var key32 = RandomNumberGenerator.GetBytes(32);
                var nonceWrong = RandomNumberGenerator.GetBytes(11); // wrong
                var aad = Array.Empty<byte>();
                var ciphertext = new byte[1];
                var tag16 = new byte[16];

                var ex = Assert.Throws<ArgumentOutOfRangeException>(() =>
                    sut.Decrypt(key32, nonceWrong, ciphertext, tag16, aad));

                Assert.That(ex.ParamName, Is.EqualTo("nonce12"));
            }

            [Test]
            public void Decrypt_WhenTagTampered_ThrowsCryptographicException()
            {
                var sut = new AesGcmEncryptorNet9();

                var key32 = RandomNumberGenerator.GetBytes(32);
                var nonce12 = RandomNumberGenerator.GetBytes(12);
                var aad = Encoding.UTF8.GetBytes("aad-v1");
                var plaintext = Encoding.UTF8.GetBytes("attack at dawn");

                var ciphertext = new byte[plaintext.Length];
                var tag16 = new byte[16];

                sut.Encrypt(key32, nonce12, plaintext, aad, ciphertext, tag16);

                // Tamper the tag (auth should fail)
                tag16[0] ^= 0xFF;

                Assert.Throws<AuthenticationTagMismatchException>(() =>
                    sut.Decrypt(key32, nonce12, ciphertext, tag16, aad));
            }
        }
    }


