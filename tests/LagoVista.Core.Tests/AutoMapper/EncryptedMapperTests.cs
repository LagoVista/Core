using LagoVista.Core.Attributes;
using LagoVista.Core.AutoMapper;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Interfaces.AutoMapper;
using LagoVista.Core.Interfaces.Crypto;
using LagoVista.Core.Models;
using LagoVista.Core.Models.Crypto;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Core.Tests.Mapping
{
    [TestFixture]
    public sealed class EncryptedMapperTests
    {
        private sealed class Domain
        {
            public string Secret1 { get; set; }
            public string Secret2 { get; set; }
        }

        [ModernKeyId("RateKey-{id}")]
        private sealed class DtoGuidId
        {
            public Guid Id { get; set; }
            public string Salt1 { get; set; } = "salt-1";
            public string Salt2 { get; set; } = "salt-2";
            public string Cipher1 { get; set; }
            public string Cipher2 { get; set; }
        }

        [ModernKeyId("RateKey-{id}")]
        private sealed class DtoStringId
        {
            public string Id { get; set; }
            public string Salt1 { get; set; } = "salt-1";
            public string Cipher1 { get; set; }
        }

        // No [ModernKeyId] on purpose
        private sealed class DtoNoModernAttr
        {
            public Guid Id { get; set; }
            public string Salt1 { get; set; } = "salt-1";
            public string Cipher1 { get; set; }
        }

        private sealed class Plan<TDomain, TDto> : IEncryptedMapPlan<TDomain, TDto>
            where TDomain : class
            where TDto : class
        {
            public bool CreateIfMissing { get; set; } = true;
            public string LegacySecretId { get; set; } = "legacy:secret:id";
            public IReadOnlyList<IEncryptedFieldPlan<TDomain, TDto>> Fields { get; set; } =
                Array.Empty<IEncryptedFieldPlan<TDomain, TDto>>();

            public string BuildSecretId(TDto dto, EntityHeader org) => LegacySecretId;
        }

        private sealed class Field<TDomain, TDto> : IEncryptedFieldPlan<TDomain, TDto>
            where TDomain : class
            where TDto : class
        {
            public string CiphertextPropertyName { get; set; }
            public string SaltPropertyName { get; set; }
            public bool SkipIfEmpty { get; set; }

            public Func<TDto, string> GetCiphertext { get; set; }
            public Action<TDto, string> SetCiphertext { get; set; }

            public Func<TDto, string> GetSalt { get; set; }

            public Func<TDomain, string> GetPlaintext { get; set; }
            public Action<TDomain, string> SetPlaintext { get; set; }
        }

        private sealed class Planner : IEncryptedMapperPlanner
        {
            private readonly object _plan;
            public Planner(object plan) => _plan = plan;

            public IEncryptedMapPlan<TDomain, TDto> GetOrBuildPlan<TDomain, TDto>()
                where TDomain : class
                where TDto : class
                => (IEncryptedMapPlan<TDomain, TDto>)_plan;
        }

        private sealed class CapturingModernKeyIdBuilder : IModernKeyIdBuilder
        {
            public int Calls { get; private set; }
            public readonly List<(Type DtoType, string Format)> Seen = new();
            public string NextKeyId { get; set; } = "ModernKey-123";

            public Task<string> BuildKeyGuiIdAsync<TDto>(TDto dto, ModernKeyIdAttribute attr, EntityHeader org, EntityHeader user, CancellationToken ct = default)
                where TDto : class
            {
                Calls++;
                Seen.Add((typeof(TDto), attr.KeyIdFormat));
                return Task.FromResult(NextKeyId);
            }
        }

        private sealed class CapturingModernEncryption : IModernEncryption
        {
            public int EncryptCalls { get; private set; }
            public int DecryptCalls { get; private set; }

            public EncryptStringRequest LastEncrypt { get; private set; }
            public DecryptStringRequest LastDecrypt { get; private set; }

            public string NextEnvelope { get; set; } = "enc;v=2;alg=a256gcm;kv=1;aad=v1;data=AA;";
            public string NextPlaintext { get; set; } = "modern-plaintext";

            public Task<string> EncryptAsync(EncryptStringRequest request, CancellationToken ct = default)
            {
                EncryptCalls++;
                LastEncrypt = request;
                return Task.FromResult(NextEnvelope);
            }

            public Task<string> DecryptAsync(DecryptStringRequest request, CancellationToken ct = default)
            {
                DecryptCalls++;
                LastDecrypt = request;
                return Task.FromResult(NextPlaintext);
            }
        }

        private sealed class CapturingLegacyKeyProvider : IEncryptionKeyProvider
        {
            public int Calls { get; private set; }
            public (string SecretId, bool CreateIfMissing) Last { get; private set; }
            public string NextKey { get; set; } = "0123456789ABCDEF0123456789ABCDEF"; // 32 chars

            public Task<string> GetKeyAsync(string secretId, EntityHeader org, EntityHeader user, bool createIfMissing, CancellationToken ct = default)
            {
                Calls++;
                Last = (secretId, createIfMissing);
                return Task.FromResult(NextKey);
            }
        }

        private sealed class CapturingLegacyEncryptor : IEncryptor
        {
            public int EncryptCalls { get; private set; }
            public int DecryptCalls { get; private set; }

            public (string Salt, string Plain, string Key) LastEncrypt { get; private set; }
            public (string Salt, string Cipher, string Key) LastDecrypt { get; private set; }

            public string NextCiphertext { get; set; } = "legacy-cipher";
            public string NextPlaintext { get; set; } = "legacy-plain";

            public string Decrypt(string salt, string ciphertext, string key)
            {
                DecryptCalls++;
                LastDecrypt = (salt, ciphertext, key);
                return NextPlaintext;
            }

            public string Encrypt(string salt, string plaintext, string key)
            {
                EncryptCalls++;
                LastEncrypt = (salt, plaintext, key);
                return NextCiphertext;
            }
        }

        private static EntityHeader Org(string id) => new EntityHeader { Id = id };
        private static EntityHeader User(string id) => new EntityHeader { Id = id };

        private static string GuidLowerD(Guid g) => g.ToString("D", CultureInfo.InvariantCulture).ToLowerInvariant();

        private static string NormalizedId32FromGuid(Guid g) => g.ToString("N", CultureInfo.InvariantCulture).ToUpperInvariant();

        [Test]
        public async Task MapDecrypt_ModernEnvelope_UsesModernAndBuildsKeyIdOnce_ForMultipleFields()
        {
            var dtoId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var dto = new DtoGuidId
            {
                Id = dtoId,
                Cipher1 = "enc;v=2;alg=a256gcm;kv=1;aad=v1;data=AA;",
                Cipher2 = "enc;v=2;alg=a256gcm;kv=1;aad=v1;data=BB;",
                Salt1 = "salt-1",
                Salt2 = "salt-2"
            };

            var domain = new Domain();

            var field1 = new Field<Domain, DtoGuidId>
            {
                CiphertextPropertyName = nameof(DtoGuidId.Cipher1),
                SaltPropertyName = nameof(DtoGuidId.Salt1),
                SkipIfEmpty = false,
                GetCiphertext = d => d.Cipher1,
                SetCiphertext = (d, v) => d.Cipher1 = v,
                GetSalt = d => d.Salt1,
                GetPlaintext = d => d.Secret1,
                SetPlaintext = (d, v) => d.Secret1 = v
            };

            var field2 = new Field<Domain, DtoGuidId>
            {
                CiphertextPropertyName = nameof(DtoGuidId.Cipher2),
                SaltPropertyName = nameof(DtoGuidId.Salt2),
                SkipIfEmpty = false,
                GetCiphertext = d => d.Cipher2,
                SetCiphertext = (d, v) => d.Cipher2 = v,
                GetSalt = d => d.Salt2,
                GetPlaintext = d => d.Secret2,
                SetPlaintext = (d, v) => d.Secret2 = v
            };

            var plan = new Plan<Domain, DtoGuidId> { Fields = new[] { field1, field2 } };
            var planner = new Planner(plan);

            var keyProvider = new CapturingLegacyKeyProvider();
            var legacy = new CapturingLegacyEncryptor();

            var modernEnc = new CapturingModernEncryption { NextPlaintext = "modern-plaintext" };
            var modernKeyId = new CapturingModernKeyIdBuilder { NextKeyId = "ModernKey-XYZ" };

            var sut = new EncryptedMapper(keyProvider, planner, legacy, modernEnc, modernKeyId);

            // org comes as NormalizedId32 (uppercase 32 chars)
            var orgGuid = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
            var org = Org(NormalizedId32FromGuid(orgGuid));
            var user = User("user-1");

            await sut.MapDecryptAsync(domain, dto, org, user, CancellationToken.None);

            Assert.That(modernKeyId.Calls, Is.EqualTo(1), "KeyId builder should be computed once per mapping run.");
            Assert.That(modernEnc.DecryptCalls, Is.EqualTo(2), "Both fields are modern envelopes.");
            Assert.That(keyProvider.Calls, Is.EqualTo(0), "Legacy key provider should not be touched on modern-only DTOs.");
            Assert.That(legacy.DecryptCalls, Is.EqualTo(0), "Legacy decryptor should not be used on modern envelopes.");

            Assert.That(domain.Secret1, Is.EqualTo("modern-plaintext"));
            Assert.That(domain.Secret2, Is.EqualTo("modern-plaintext"));

            Assert.That(modernEnc.LastDecrypt.OrgId.Value, Is.EqualTo(GuidLowerD(orgGuid)));
            Assert.That(modernEnc.LastDecrypt.RecId.Value, Is.EqualTo(GuidLowerD(dtoId)));
            Assert.That(modernEnc.LastDecrypt.KeyId, Is.EqualTo("ModernKey-XYZ"));
            Assert.That(modernEnc.LastDecrypt.FieldName, Is.EqualTo(nameof(DtoGuidId.Cipher2).ToLowerInvariant()));
        }

        [Test]
        public void MapDecrypt_ModernEnvelope_Throws_WhenDtoMissingModernKeyIdAttribute()
        {
            var dto = new DtoNoModernAttr
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Cipher1 = "enc;v=2;alg=a256gcm;kv=1;aad=v1;data=AA;",
                Salt1 = "salt-1"
            };

            var domain = new Domain();

            var field1 = new Field<Domain, DtoNoModernAttr>
            {
                CiphertextPropertyName = nameof(DtoNoModernAttr.Cipher1),
                SaltPropertyName = nameof(DtoNoModernAttr.Salt1),
                SkipIfEmpty = false,
                GetCiphertext = d => d.Cipher1,
                SetCiphertext = (d, v) => d.Cipher1 = v,
                GetSalt = d => d.Salt1,
                GetPlaintext = d => d.Secret1,
                SetPlaintext = (d, v) => d.Secret1 = v
            };

            var plan = new Plan<Domain, DtoNoModernAttr> { Fields = new[] { field1 } };
            var planner = new Planner(plan);

            var sut = new EncryptedMapper(
                new CapturingLegacyKeyProvider(),
                planner,
                new CapturingLegacyEncryptor(),
                new CapturingModernEncryption(),
                new CapturingModernKeyIdBuilder());

            var org = Org(GuidLowerD(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa")));
            var user = User("user-1");

            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await sut.MapDecryptAsync(domain, dto, org, user, CancellationToken.None));

            Assert.That(ex!.Message, Does.Contain("must have [ModernKeyId"));
        }

        [Test]
        public async Task MapDecrypt_LegacyCipher_UsesLegacyKeyProviderOnce_AndDecryptor()
        {
            var dtoId = Guid.Parse("33333333-3333-3333-3333-333333333333");
            var dto = new DtoGuidId
            {
                Id = dtoId,
                Cipher1 = "legacy-ciphertext",
                Salt1 = "salt-123"
            };

            var domain = new Domain();

            var field = new Field<Domain, DtoGuidId>
            {
                CiphertextPropertyName = nameof(DtoGuidId.Cipher1),
                SaltPropertyName = nameof(DtoGuidId.Salt1),
                SkipIfEmpty = false,
                GetCiphertext = d => d.Cipher1,
                SetCiphertext = (d, v) => d.Cipher1 = v,
                GetSalt = d => d.Salt1,
                GetPlaintext = d => d.Secret1,
                SetPlaintext = (d, v) => d.Secret1 = v
            };

            var plan = new Plan<Domain, DtoGuidId>
            {
                CreateIfMissing = true,
                LegacySecretId = "legacy:sec:1",
                Fields = new[] { field }
            };

            var planner = new Planner(plan);
            var kp = new CapturingLegacyKeyProvider { NextKey = "0123456789ABCDEF0123456789ABCDEF" };
            var legacy = new CapturingLegacyEncryptor { NextPlaintext = "legacy-plain" };

            var modernEnc = new CapturingModernEncryption();
            var modernKid = new CapturingModernKeyIdBuilder();

            var sut = new EncryptedMapper(kp, planner, legacy, modernEnc, modernKid);

            var org = Org(GuidLowerD(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa")));
            var user = User("user-1");

            await sut.MapDecryptAsync(domain, dto, org, user, CancellationToken.None);

            Assert.That(kp.Calls, Is.EqualTo(1));
            Assert.That(kp.Last.SecretId, Is.EqualTo("legacy:sec:1"));
            Assert.That(kp.Last.CreateIfMissing, Is.True);

            Assert.That(legacy.DecryptCalls, Is.EqualTo(1));
            Assert.That(legacy.LastDecrypt.Salt, Is.EqualTo("salt-123"));
            Assert.That(legacy.LastDecrypt.Cipher, Is.EqualTo("legacy-ciphertext"));

            Assert.That(modernEnc.DecryptCalls, Is.EqualTo(0));
            Assert.That(modernKid.Calls, Is.EqualTo(0));

            Assert.That(domain.Secret1, Is.EqualTo("legacy-plain"));
        }

        [Test]
        public void MapDecrypt_LegacyCipher_Throws_WhenSaltEmpty()
        {
            var dto = new DtoGuidId
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                Cipher1 = "legacy-ciphertext",
                Salt1 = "" // empty => should throw
            };

            var domain = new Domain();

            var field = new Field<Domain, DtoGuidId>
            {
                CiphertextPropertyName = nameof(DtoGuidId.Cipher1),
                SaltPropertyName = nameof(DtoGuidId.Salt1),
                SkipIfEmpty = false,
                GetCiphertext = d => d.Cipher1,
                SetCiphertext = (d, v) => d.Cipher1 = v,
                GetSalt = d => d.Salt1,
                GetPlaintext = d => d.Secret1,
                SetPlaintext = (d, v) => d.Secret1 = v
            };

            var plan = new Plan<Domain, DtoGuidId> { Fields = new[] { field } };
            var planner = new Planner(plan);

            var sut = new EncryptedMapper(
                new CapturingLegacyKeyProvider(),
                planner,
                new CapturingLegacyEncryptor(),
                new CapturingModernEncryption(),
                new CapturingModernKeyIdBuilder());

            var org = Org(GuidLowerD(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa")));
            var user = User("user-1");

            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await sut.MapDecryptAsync(domain, dto, org, user, CancellationToken.None));

            Assert.That(ex!.Message, Does.Contain("Salt resolved empty"));
        }

        [Test]
        public async Task MapDecrypt_SkipIfEmpty_SkipsField()
        {
            var dto = new DtoGuidId
            {
                Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                Cipher1 = "", // empty
                Salt1 = "salt-1"
            };

            var domain = new Domain { Secret1 = "unchanged" };

            var field = new Field<Domain, DtoGuidId>
            {
                CiphertextPropertyName = nameof(DtoGuidId.Cipher1),
                SaltPropertyName = nameof(DtoGuidId.Salt1),
                SkipIfEmpty = true,
                GetCiphertext = d => d.Cipher1,
                SetCiphertext = (d, v) => d.Cipher1 = v,
                GetSalt = d => d.Salt1,
                GetPlaintext = d => d.Secret1,
                SetPlaintext = (d, v) => d.Secret1 = v
            };

            var plan = new Plan<Domain, DtoGuidId> { Fields = new[] { field } };
            var planner = new Planner(plan);

            var kp = new CapturingLegacyKeyProvider();
            var legacy = new CapturingLegacyEncryptor();
            var modernEnc = new CapturingModernEncryption();
            var modernKid = new CapturingModernKeyIdBuilder();

            var sut = new EncryptedMapper(kp, planner, legacy, modernEnc, modernKid);

            var org = Org(GuidLowerD(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa")));
            var user = User("user-1");

            await sut.MapDecryptAsync(domain, dto, org, user, CancellationToken.None);

            Assert.That(domain.Secret1, Is.EqualTo("unchanged"));
            Assert.That(kp.Calls, Is.EqualTo(0));
            Assert.That(legacy.DecryptCalls, Is.EqualTo(0));
            Assert.That(modernEnc.DecryptCalls, Is.EqualTo(0));
            Assert.That(modernKid.Calls, Is.EqualTo(0));
        }

        [Test]
        public async Task MapEncrypt_WritesModernEnvelope_AndIgnoresSalt()
        {
            var dtoId = Guid.Parse("66666666-6666-6666-6666-666666666666");
            var dto = new DtoGuidId
            {
                Id = dtoId,
                Salt1 = "", // should not matter for modern write
                Cipher1 = null
            };

            var domain = new Domain { Secret1 = "hello" };

            var field = new Field<Domain, DtoGuidId>
            {
                CiphertextPropertyName = nameof(DtoGuidId.Cipher1),
                SaltPropertyName = nameof(DtoGuidId.Salt1),
                SkipIfEmpty = false,
                GetCiphertext = d => d.Cipher1,
                SetCiphertext = (d, v) => d.Cipher1 = v,
                GetSalt = d => d.Salt1,
                GetPlaintext = d => d.Secret1,
                SetPlaintext = (d, v) => d.Secret1 = v
            };

            var plan = new Plan<Domain, DtoGuidId> { Fields = new[] { field } };
            var planner = new Planner(plan);

            var modernEnc = new CapturingModernEncryption { NextEnvelope = "enc;v=2;alg=a256gcm;kv=1;aad=v1;data=ZZ;" };
            var modernKid = new CapturingModernKeyIdBuilder { NextKeyId = "ModernKey-ABC" };

            var sut = new EncryptedMapper(
                new CapturingLegacyKeyProvider(),
                planner,
                new CapturingLegacyEncryptor(),
                modernEnc,
                modernKid);

            var orgGuid = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
            var org = Org(NormalizedId32FromGuid(orgGuid)); // exercise NormalizedId32 -> GuidString36
            var user = User("user-1");

            await sut.MapEncryptAsync(domain, dto, org, user, CancellationToken.None);

            Assert.That(dto.Cipher1, Is.EqualTo("enc;v=2;alg=a256gcm;kv=1;aad=v1;data=ZZ;"));

            Assert.That(modernKid.Calls, Is.EqualTo(1));
            Assert.That(modernEnc.EncryptCalls, Is.EqualTo(1));

            Assert.That(modernEnc.LastEncrypt.OrgId.Value, Is.EqualTo(GuidLowerD(orgGuid)));
            Assert.That(modernEnc.LastEncrypt.RecId.Value, Is.EqualTo(GuidLowerD(dtoId)));
            Assert.That(modernEnc.LastEncrypt.KeyId, Is.EqualTo("ModernKey-ABC"));
            Assert.That(modernEnc.LastEncrypt.FieldName, Is.EqualTo(nameof(DtoGuidId.Cipher1).ToLowerInvariant()));
            Assert.That(modernEnc.LastEncrypt.Kv, Is.EqualTo(1));
            Assert.That(modernEnc.LastEncrypt.Plaintext, Is.EqualTo("hello"));
        }

        [Test]
        public async Task MapEncrypt_SkipIfEmpty_SkipsFieldAndDoesNotCallModernEncryption()
        {
            var dto = new DtoGuidId
            {
                Id = Guid.Parse("77777777-7777-7777-7777-777777777777"),
                Cipher1 = null
            };

            var domain = new Domain { Secret1 = "" }; // empty + SkipIfEmpty

            var field = new Field<Domain, DtoGuidId>
            {
                CiphertextPropertyName = nameof(DtoGuidId.Cipher1),
                SaltPropertyName = nameof(DtoGuidId.Salt1),
                SkipIfEmpty = true,
                GetCiphertext = d => d.Cipher1,
                SetCiphertext = (d, v) => d.Cipher1 = v,
                GetSalt = d => d.Salt1,
                GetPlaintext = d => d.Secret1,
                SetPlaintext = (d, v) => d.Secret1 = v
            };

            var plan = new Plan<Domain, DtoGuidId> { Fields = new[] { field } };
            var planner = new Planner(plan);

            var modernEnc = new CapturingModernEncryption();
            var modernKid = new CapturingModernKeyIdBuilder();

            var sut = new EncryptedMapper(
                new CapturingLegacyKeyProvider(),
                planner,
                new CapturingLegacyEncryptor(),
                modernEnc,
                modernKid);

            var org = Org(GuidLowerD(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa")));
            var user = User("user-1");

            await sut.MapEncryptAsync(domain, dto, org, user, CancellationToken.None);

            Assert.That(modernKid.Calls, Is.EqualTo(1), "KeyId is built before field loop (by design).");
            Assert.That(modernEnc.EncryptCalls, Is.EqualTo(0));
            Assert.That(dto.Cipher1, Is.Null);
        }

        [Test]
        public void MapDecrypt_Throws_WhenOrgIdInvalid()
        {
            var dto = new DtoGuidId
            {
                Id = Guid.Parse("88888888-8888-8888-8888-888888888888"),
                Cipher1 = "legacy-ciphertext",
                Salt1 = "salt-1"
            };

            var domain = new Domain();

            var field = new Field<Domain, DtoGuidId>
            {
                CiphertextPropertyName = nameof(DtoGuidId.Cipher1),
                SaltPropertyName = nameof(DtoGuidId.Salt1),
                SkipIfEmpty = false,
                GetCiphertext = d => d.Cipher1,
                SetCiphertext = (d, v) => d.Cipher1 = v,
                GetSalt = d => d.Salt1,
                GetPlaintext = d => d.Secret1,
                SetPlaintext = (d, v) => d.Secret1 = v
            };

            var plan = new Plan<Domain, DtoGuidId> { Fields = new[] { field } };
            var planner = new Planner(plan);

            var sut = new EncryptedMapper(
                new CapturingLegacyKeyProvider(),
                planner,
                new CapturingLegacyEncryptor(),
                new CapturingModernEncryption(),
                new CapturingModernKeyIdBuilder());

            var org = Org("not-a-guid-or-n32");
            var user = User("user-1");

            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await sut.MapDecryptAsync(domain, dto, org, user, CancellationToken.None));

            Assert.That(ex!.Message, Does.Contain("org.Id must be GuidString36"));
        }

        [Test]
        public void MapDecrypt_Throws_WhenDtoIdStringInvalid()
        {
            var dto = new DtoStringId
            {
                Id = "not-a-guid-or-n32",
                Cipher1 = "enc;v=2;alg=a256gcm;kv=1;aad=v1;data=AA;",
                Salt1 = "salt-1"
            };

            var domain = new Domain();

            var field = new Field<Domain, DtoStringId>
            {
                CiphertextPropertyName = nameof(DtoStringId.Cipher1),
                SaltPropertyName = nameof(DtoStringId.Salt1),
                SkipIfEmpty = false,
                GetCiphertext = d => d.Cipher1,
                SetCiphertext = (d, v) => d.Cipher1 = v,
                GetSalt = d => d.Salt1,
                GetPlaintext = d => d.Secret1,
                SetPlaintext = (d, v) => d.Secret1 = v
            };

            var plan = new Plan<Domain, DtoStringId> { Fields = new[] { field } };
            var planner = new Planner(plan);

            var sut = new EncryptedMapper(
                new CapturingLegacyKeyProvider(),
                planner,
                new CapturingLegacyEncryptor(),
                new CapturingModernEncryption(),
                new CapturingModernKeyIdBuilder());

            var org = Org(GuidLowerD(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa")));
            var user = User("user-1");

            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await sut.MapDecryptAsync(domain, dto, org, user, CancellationToken.None));

            Assert.That(ex!.Message, Does.Contain(".Id string value must be GuidString36"));
        }
    }
}