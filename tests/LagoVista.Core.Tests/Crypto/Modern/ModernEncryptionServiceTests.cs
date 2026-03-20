using LagoVista.Core.Models;
using LagoVista.Core.Models.Crypto;
using LagoVista.Crypto.Modern;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Crypto.Tests.Modern
{
    [TestFixture]
    public sealed class ModernEncryptionServiceTests
    {
        private static EntityHeader OrgEH() => new EntityHeader { Id = "ORG" };
        private static EntityHeader UserEH() => new EntityHeader { Id = "USER" };

        private static ModernEncryptionService NewSut(FakeKeyMaterialStore store = null)
        {
            store ??= new FakeKeyMaterialStore();

            return new ModernEncryptionService(
                aadBuilder: new AadBuilderV1(),
                envelopeCodec: new EnvelopeCodecV2(),
                keyMaterialStore: store,
                aead: new AesGcmEncryptorNet9());
        }

        [Test]
        public async Task EncryptDecrypt_RoundTrip_Works()
        {
            var sut = NewSut();

            var orgId = GuidString36.Factory();
            var recId = GuidString36.Factory();

            var encReq = new EncryptStringRequest
            {
                OrgId = orgId,
                RecId = recId,
                FieldName = "EncryptedBalance",  // intentionally mixed case
                KeyId = "customer-3f2504e04f8911d39a0c0305e82c3301:v2",
                Kv = 1,
                Plaintext = "77.01"
            };

            var envelope = await sut.EncryptAsync(encReq, CancellationToken.None);

            Assert.That(envelope, Does.StartWith("enc;v=2;alg=a256gcm;kv=1;aad=v1;data="));
            Assert.That(envelope, Does.EndWith(";"));

            var decReq = new DecryptStringRequest
            {
                OrgId = orgId,
                RecId = recId,
                FieldName = "encryptedbalance", // different case, should still work
                KeyId = encReq.KeyId,
                Envelope = envelope
            };

            var plaintext = await sut.DecryptAsync(decReq, CancellationToken.None);

            Assert.That(plaintext, Is.EqualTo("77.01"));
        }

        [Test]
        public void EncryptAsync_WhenKvInvalid_Throws()
        {
            var sut = NewSut();

            var req = new EncryptStringRequest
            {
                OrgId = GuidString36.Factory(),
                RecId = GuidString36.Factory(),
                FieldName = "x",
                KeyId = "customer-abc:v2",
                Kv = 0,
                Plaintext = "hi"
            };

            Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
                await sut.EncryptAsync(req, CancellationToken.None));
        }

        [Test]
        public void EncryptAsync_WhenPlaintextMissing_Throws()
        {
            var sut = NewSut();

            var req = new EncryptStringRequest
            {
                OrgId = GuidString36.Factory(),
                RecId = GuidString36.Factory(),
                FieldName = "x",
                KeyId = "customer-abc:v2",
                Kv = 1,
                Plaintext = "   "
            };

            Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await sut.EncryptAsync(req, CancellationToken.None));
        }

        [Test]
        public void DecryptAsync_WhenEnvelopeMissing_Throws()
        {
            var sut = NewSut();

            var req = new DecryptStringRequest
            {
                OrgId = GuidString36.Factory(),
                RecId = GuidString36.Factory(),
                FieldName = "x",
                KeyId = "customer-abc:v2",
                Envelope = ""
            };

            Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await sut.DecryptAsync(req, CancellationToken.None));
        }

        [Test]
        public async Task DecryptAsync_WhenKeyIdDoesNotMatchAAD_ThrowsCryptographicException()
        {
            var sut = NewSut();

            var orgId = GuidString36.Factory();
            var recId = GuidString36.Factory();

            var encReq = new EncryptStringRequest
            {
                OrgId = orgId,
                RecId = recId,
                FieldName = "EncryptedBalance",
                KeyId = "customer-11111111111111111111111111111111:v2",
                Kv = 1,
                Plaintext = "secret"
            };

            var envelope = await sut.EncryptAsync(encReq, CancellationToken.None);

            // Same envelope but wrong KeyId => AAD differs => auth fail
            var decReq = new DecryptStringRequest
            {
                OrgId = orgId,
                RecId = recId,
                FieldName = "EncryptedBalance",
                KeyId = "customer-22222222222222222222222222222222:v2",
                Envelope = envelope
            };

            Assert.ThrowsAsync<AuthenticationTagMismatchException>(async () =>
                await sut.DecryptAsync(decReq, CancellationToken.None));
        }

        [Test]
        public async Task DecryptAsync_UsesKvFromEnvelope_WhenFetchingKeyMaterial()
        {
            var store = new FakeKeyMaterialStore();
            var sut = NewSut(store);

            var orgId = GuidString36.Factory();
            var recId = GuidString36.Factory();

            // Encrypt with kv=2
            var encReq = new EncryptStringRequest
            {
                OrgId = orgId,
                RecId = recId,
                FieldName = "Tag",
                KeyId = "customer-aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa:v2",
                Kv = 2,
                Plaintext = "kv-test"
            };

            var envelope = await sut.EncryptAsync(encReq, CancellationToken.None);

            var decReq = new DecryptStringRequest
            {
                OrgId = orgId,
                RecId = recId,
                FieldName = "Tag",
                KeyId = encReq.KeyId,
                Envelope = envelope
            };

            var plaintext = await sut.DecryptAsync(decReq, CancellationToken.None);
            Assert.That(plaintext, Is.EqualTo("kv-test"));

            Assert.That(store.LastKv, Is.EqualTo(2), "Decrypt should fetch key material using parts.Kv from the envelope.");
        }

        private sealed class FakeKeyMaterialStore : IKeyMaterialStore
        {
            private readonly Dictionary<(string keyId, int kv), byte[]> _keys = new();

            public int? LastKv { get; private set; }

            public Task<byte[]> GetOrCreateKey32Async(EntityHeader org, EntityHeader user, string keyId, int kv, CancellationToken ct = default)
            {
                LastKv = kv;

                var k = (keyId, kv);
                if (!_keys.TryGetValue(k, out var key32))
                {
                    key32 = RandomNumberGenerator.GetBytes(32);
                    _keys[k] = key32;
                }

                return Task.FromResult(key32);
            }
        }
    }
}