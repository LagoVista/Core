using LagoVista.Core.Interfaces.AutoMapper;
using LagoVista.Core.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Core.AutoMapper
{
    /// <summary>
    /// Legacy encrypted mapper.
    ///
    /// This version has been simplified by extracting reflection planning into IEncryptedMapperPlanner.
    /// </summary>
    public sealed class EncryptedMapper : IEncryptedMapper
    {
        private readonly IEncryptionKeyProvider _keyProvider;
        private readonly IEncryptor _encryptor;
        private readonly IEncryptedMapperPlanner _planner;

        public EncryptedMapper(IEncryptionKeyProvider keyProvider, IEncryptedMapperPlanner planner, IEncryptor encryptor)
        {
            _keyProvider = keyProvider ?? throw new ArgumentNullException(nameof(keyProvider));
            _planner = planner ?? throw new ArgumentNullException(nameof(planner));
            _encryptor = encryptor ?? throw new ArgumentNullException(nameof(encryptor));
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

            var secretId = plan.BuildSecretId(dto, org);
            var encryptionKey = await _keyProvider.GetKeyAsync(secretId, org, user, plan.CreateIfMissing, ct).ConfigureAwait(false);

            foreach (var f in plan.Fields)
            {
                var ciphertext = f.GetCiphertext(dto);
                if (f.SkipIfEmpty && string.IsNullOrEmpty(ciphertext))
                    continue;

                var salt = f.GetSalt(dto);
                if (string.IsNullOrWhiteSpace(salt))
                    throw new InvalidOperationException($"Salt resolved empty for DTO {typeof(TDto).Name}.{f.SaltPropertyName}.");

                var plaintext = _encryptor.Decrypt(salt, ciphertext, encryptionKey);
                f.SetPlaintext(domain, plaintext);
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

            var secretId = plan.BuildSecretId(dto, org);
            var encryptionKey = await _keyProvider.GetKeyAsync(secretId, org, user, plan.CreateIfMissing, ct).ConfigureAwait(false);

            foreach (var f in plan.Fields)
            {
                var plaintext = f.GetPlaintext(domain);
                if (f.SkipIfEmpty && string.IsNullOrEmpty(plaintext))
                    continue;

                var salt = f.GetSalt(dto);
                if (string.IsNullOrWhiteSpace(salt))
                    throw new InvalidOperationException($"Salt resolved empty for DTO {typeof(TDto).Name}.{f.SaltPropertyName}.");

                var ciphertext = _encryptor.Encrypt(salt, plaintext, encryptionKey);
                f.SetCiphertext(dto, ciphertext);
            }
        }
    }
}