using LagoVista;
using LagoVista.Core.Attributes;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Interfaces.AutoMapper;
using LagoVista.Core.Interfaces.Crypto;
using LagoVista.Core.Models;
using LagoVista.Core.Models.Crypto;
using System;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Core.AutoMapper
{
    /// <summary>
    /// Encrypted mapper with dual-read (legacy + modern) and modern-write.
    ///
    /// - Decrypt: modern if ciphertext is envelope (enc;...), else legacy
    /// - Encrypt: always modern (writes envelope into ciphertext column)
    ///
    /// Legacy encryptor/key-provider remain for backward compatibility during migration.
    /// </summary>
    [CriticalCoverage]
    public sealed class EncryptedMapper : IEncryptedMapper
    {
        private const string ModernEnvelopePrefix = "enc;";
        private const int DefaultKv = 1; // kv resolver currently returns 1

        private readonly IEncryptionKeyProvider _keyProvider; // legacy (cached provider)
        private readonly IEncryptor _encryptor; // legacy
        private readonly IEncryptedMapperPlanner _planner;

        private readonly IModernEncryption _modernEncryption;
        private readonly IModernKeyIdBuilder _modernKeyIdBuilder;

        public EncryptedMapper(
            IEncryptionKeyProvider keyProvider,
            IEncryptedMapperPlanner planner,
            IEncryptor encryptor,
            IModernEncryption modernEncryption,
            IModernKeyIdBuilder modernKeyIdBuilder)
        {
            _keyProvider = keyProvider ?? throw new ArgumentNullException(nameof(keyProvider));
            _planner = planner ?? throw new ArgumentNullException(nameof(planner));
            _encryptor = encryptor ?? throw new ArgumentNullException(nameof(encryptor));
            _modernEncryption = modernEncryption ?? throw new ArgumentNullException(nameof(modernEncryption));
            _modernKeyIdBuilder = modernKeyIdBuilder ?? throw new ArgumentNullException(nameof(modernKeyIdBuilder));
        }

        public async Task MapDecryptAsync<TDomain, TDto>(
            TDomain domain,
            TDto dto,
            EntityHeader org,
            EntityHeader user,
            CancellationToken ct = default)
            where TDomain : class
            where TDto : class
        {
            if (domain == null) throw new ArgumentNullException(nameof(domain));
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (org == null) throw new ArgumentNullException(nameof(org));
            if (user == null) throw new ArgumentNullException(nameof(user));


            var plan = _planner.GetOrBuildPlan<TDomain, TDto>();

            // Only compute modern KeyId if we encounter any modern envelopes.
            string modernKeyId = null;

            var orgId36 = CoerceOrgIdToGuidString36(org.Id, "org.Id");
            var recId36 = CoerceDtoIdToGuidString36(dto);

            // Legacy secret/key is only needed if we encounter any legacy ciphertexts.
            string legacyKey = null;

            foreach (var f in plan.Fields)
            {
                var ciphertext = f.GetCiphertext(dto);
                if (f.SkipIfEmpty && string.IsNullOrEmpty(ciphertext))
                    continue;

                try
                {
         
                    // Modern envelope path
                    if (ciphertext.StartsWith(ModernEnvelopePrefix, StringComparison.Ordinal))
                    {
                        if (modernKeyId == null)
                        {
                            var modernAttr = typeof(TDto).GetCustomAttribute<ModernKeyIdAttribute>()
                                ?? throw new InvalidOperationException(
                                    $"DTO type {typeof(TDto).Name} must have [ModernKeyId(...)] attribute for modern encryption.");

                            modernKeyId = await _modernKeyIdBuilder.BuildKeyGuiIdAsync(dto, modernAttr, org, user, ct).ConfigureAwait(false);
                        }

                        var req = new DecryptStringRequest
                        {
                            OrgId = orgId36,
                            Org = org,
                            User = user,
                            RecId = recId36,
                            FieldName = f.CiphertextPropertyName.ToLowerInvariant(),
                            KeyId = modernKeyId,
                            Envelope = ciphertext
                        };

                        var plaintext = await _modernEncryption.DecryptAsync(req, ct).ConfigureAwait(false);
                        f.SetPlaintext(domain, plaintext);
                        continue;
                    }

                    // Legacy path
                    if (legacyKey == null)
                    {
                        var legacySecretId = plan.BuildSecretId(dto, org);
                        legacyKey = await _keyProvider.GetKeyAsync(legacySecretId, org, user, plan.CreateIfMissing, ct).ConfigureAwait(false);
                    }

                    var salt = f.GetSalt(dto);
                    if (string.IsNullOrWhiteSpace(salt))
                        throw new InvalidOperationException($"Salt resolved empty for DTO {typeof(TDto).Name}.{f.SaltPropertyName}.");

                    var legacyPlaintext = _encryptor.Decrypt(salt, ciphertext, legacyKey);
                    f.SetPlaintext(domain, legacyPlaintext);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{this.Tag()} - Field: {typeof(TDto).Name}.{f.CiphertextPropertyName}, {modernKeyId}, {ciphertext} {ex.Message}");
                }
            }
        }

        public async Task MapEncryptAsync<TDomain, TDto>(
            TDomain domain,
            TDto dto,
            EntityHeader org,
            EntityHeader user,
            CancellationToken ct = default)
            where TDomain : class
            where TDto : class
        {
            if (domain == null) throw new ArgumentNullException(nameof(domain));
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (org == null) throw new ArgumentNullException(nameof(org));
            if (user == null) throw new ArgumentNullException(nameof(user));

            var plan = _planner.GetOrBuildPlan<TDomain, TDto>();

            var modernAttr = typeof(TDto).GetCustomAttribute<ModernKeyIdAttribute>()
                ?? throw new InvalidOperationException($"DTO type {typeof(TDto).Name} must have [ModernKeyId(...)] attribute for modern encryption.");

            var modernKeyId = await _modernKeyIdBuilder.BuildKeyGuiIdAsync(dto, modernAttr, org, user, ct).ConfigureAwait(false);

            var orgId36 = CoerceOrgIdToGuidString36(org.Id, "org.Id");
            var recId36 = CoerceDtoIdToGuidString36(dto);

            foreach (var f in plan.Fields)
            {
                var plaintext = f.GetPlaintext(domain);
                if (f.SkipIfEmpty && string.IsNullOrEmpty(plaintext))
                    continue;

                var req = new EncryptStringRequest
                {
                    OrgId = orgId36,
                    Org = org,
                    User = user,
                    RecId = recId36,
                    FieldName = f.CiphertextPropertyName.ToLowerInvariant(),
                    KeyId = modernKeyId,
                    Kv = DefaultKv,
                    Plaintext = plaintext
                };

                var envelope = await _modernEncryption.EncryptAsync(req, ct).ConfigureAwait(false);

                // Always write modern envelope into ciphertext column
                f.SetCiphertext(dto, envelope);

                // Intentionally ignore salt for modern writes.
                // Legacy reads are gated by the "enc;" prefix check.
            }
        }

        private static GuidString36 CoerceOrgIdToGuidString36(string value, string label)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new InvalidOperationException($"{label} resolved empty.");

            // Most strict/cheap check first (already canonical GuidString36)
            if (GuidString36.IsStrictLowerD(value))
                return new GuidString36(value);

            // Org ids may come through as NormalizedId32 (uppercase 32 chars)
            if (NormalizedId32.TryCreate(value, out var n32))
                return n32.ToGuidString36();

            throw new InvalidOperationException(
                $"{label} must be GuidString36 (lowercase D) or NormalizedId32 (32 uppercase). Value='{value}'.");
        }

        private static GuidString36 CoerceDtoIdToGuidString36<TDto>(TDto dto)
            where TDto : class
        {
            var idProp = typeof(TDto).GetProperty("Id", BindingFlags.Public | BindingFlags.Instance);
            if (idProp == null)
                throw new InvalidOperationException($"DTO type {typeof(TDto).Name} must expose a public instance property named 'Id'.");

            var idType = idProp.PropertyType;
            var idValue = idProp.GetValue(dto);
            if (idValue == null)
                throw new InvalidOperationException($"DTO type {typeof(TDto).Name}.Id resolved null.");

            // Most of your relational DTO ids are Guid (uniqueidentifier)
            if (idValue is Guid g)
                return new GuidString36(g.ToString("D", CultureInfo.InvariantCulture).ToLowerInvariant());

            // Some systems use GuidString36 directly
            if (idValue is GuidString36 gs)
                return gs;

            // If it's a string, accept strict GuidString36 and (optionally) NormalizedId32.
            // Keeping this tolerant reduces surprise if a DTO uses a string Id.
            if (idValue is string s)
            {
                if (GuidString36.IsStrictLowerD(s))
                    return new GuidString36(s);

                if (NormalizedId32.TryCreate(s, out var n32))
                    return n32.ToGuidString36();

                throw new InvalidOperationException(
                    $"DTO type {typeof(TDto).Name}.Id string value must be GuidString36 (lowercase D) or NormalizedId32. Value='{s}'.");
            }

            throw new InvalidOperationException(
                $"DTO type {typeof(TDto).Name}.Id must be Guid, GuidString36, or string. Actual type '{idType.Name}'.");
        }
    }
}