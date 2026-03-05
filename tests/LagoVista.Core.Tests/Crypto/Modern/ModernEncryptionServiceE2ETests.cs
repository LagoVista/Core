using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Models.Crypto;
using LagoVista.Crypto.Modern;
using LagoVista.Core.Validation;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Threading.Tasks;
using static LagoVista.Core.Tests.Mapping.LagoVistaAutoMapperV1Tests;

namespace LagoVista.Core.Tests.Crypto.Modern
{
    [TestFixture]
    public class ModernEncryptionServiceE2ETests
    {
        private static EntityHeader OrgHeader => EntityHeader.Create(Guid.NewGuid().ToString(), "org");
        private static EntityHeader UserHeader => EntityHeader.Create(Guid.NewGuid().ToString(), "user");


        private static IModernEncryption BuildService(ISecureStorage secureStorage, EntityHeader org, EntityHeader user)
        {
            var aad = new AadBuilderV1();
            var env = new EnvelopeCodecV2();
            var store = new SecureStorageKeyMaterialStore(secureStorage);
            var aead = new AesGcmEncryptorNet9();

            return new ModernEncryptionService(aad, env, store, aead, org, user);
        }

        [Test]
        public async Task EncryptDecrypt_RoundTrip_Works()
        {
            var secrets = new ConcurrentDictionary<string, string>(StringComparer.Ordinal);
            var secureStorage = new FakeSecureStorage();

            var org = OrgHeader;
            var user = UserHeader;
            var svc = BuildService(secureStorage, org, user);

            var req = new EncryptStringRequest
            {
                OrgId = new GuidString36(("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa")),
                RecId = new GuidString36(("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb")),
                FieldName = "EncryptedBalance",
                KeyId = "account-0123456789abcdef0123456789abcdef:v2",
                Kv = 1,
                Plaintext = "142.44"
            };

            var envelope = await svc.EncryptAsync(req);
            Assert.That(envelope, Does.StartWith("enc;v=2;"));

            var decReq = new DecryptStringRequest
            {
                OrgId = req.OrgId,
                RecId = req.RecId,
                FieldName = req.FieldName,
                KeyId = req.KeyId,
                Envelope = envelope
            };

            var plaintext = await svc.DecryptAsync(decReq);
            Assert.That(plaintext, Is.EqualTo("142.44"));
        }

        [Test]
        public async Task Encrypt_SamePlaintextTwice_ProducesDifferentEnvelope()
        {
            var secrets = new ConcurrentDictionary<string, string>(StringComparer.Ordinal);
            var secureStorage = new FakeSecureStorage();

            var org = OrgHeader;
            var user = UserHeader;
            var svc = BuildService(secureStorage, org, user);

            var req = new EncryptStringRequest
            {
                OrgId = new GuidString36(("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa")),
                RecId = new GuidString36(("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb")),
                FieldName = "EncryptedBalance",
                KeyId = "account-0123456789abcdef0123456789abcdef:v2",
                Kv = 1,
                Plaintext = "142.44"
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
            var svc = BuildService(secureStorage, org, user);

            var req = new EncryptStringRequest
            {
                OrgId = new GuidString36(("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa")),
                RecId = new GuidString36(("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb")),
                FieldName = "EncryptedBalance",
                KeyId = "account-0123456789abcdef0123456789abcdef:v2",
                Kv = 1,
                Plaintext = "142.44"
            };

            var envelope = await svc.EncryptAsync(req);

            var bad = new DecryptStringRequest
            {
                OrgId = req.OrgId,
                RecId = req.RecId,
                FieldName = "EncryptedOnlineBalance", // different => AAD mismatch
                KeyId = req.KeyId,
                Envelope = envelope
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
            var svc = BuildService(secureStorage, org, user);

            var req = new EncryptStringRequest
            {
                OrgId = GuidString36.Factory(),
                RecId = GuidString36.Factory(),
                FieldName = "EncryptedBalance",
                KeyId = "account-0123456789abcdef0123456789abcdef:v2",
                Kv = 1,
                Plaintext = "   "
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
            var svc = BuildService(secureStorage, org, user);

            var req = new DecryptStringRequest
            {
                OrgId = GuidString36.Factory(),
                RecId = GuidString36.Factory(),
                FieldName = "EncryptedBalance",
                KeyId = "account-0123456789abcdef0123456789abcdef:v2",
                Envelope = ""
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
            var svc = BuildService(secureStorage, org, user);

            var req = new DecryptStringRequest
            {
                OrgId = GuidString36.Factory(),
                RecId = GuidString36.Factory(),
                FieldName = "EncryptedBalance",
                KeyId = "account-0123456789abcdef0123456789abcdef:v2",
                Envelope = "legacybase64=="
            };

            Assert.ThrowsAsync<FormatException>(async () => await svc.DecryptAsync(req));
        }
    }
}
