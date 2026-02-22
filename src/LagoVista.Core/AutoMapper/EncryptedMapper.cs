using LagoVista.Core.Attributes;
using LagoVista.Core.Interfaces.AutoMapper;
using LagoVista.Core.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Core.AutoMapper
{
    public sealed class EncryptedMapper : IEncryptedMapper
    {
        private static readonly ConcurrentDictionary<(Type Domain, Type Dto), object> _plans = new ConcurrentDictionary<(Type Domain, Type Dto), object>();

        private readonly IEncryptionKeyProvider _keyProvider;
        private readonly IEncryptor _encryptor;
        private readonly IMapValueConverterRegistry _converters;

        public EncryptedMapper(IEncryptionKeyProvider keyProvider, IMapValueConverterRegistry converters, IEncryptor encryptor)
        {
            _keyProvider = keyProvider ?? throw new ArgumentNullException(nameof(keyProvider));
            _encryptor = encryptor ?? throw new ArgumentNullException(nameof(encryptor));
            _converters = converters ?? throw new ArgumentNullException(nameof(converters));
        }

        public async Task MapDecryptAsync<TDomain, TDto>(TDomain domain, TDto dto, EntityHeader org, EntityHeader user, CancellationToken ct = default)
            where TDomain : class
            where TDto : class
        {
            if (domain == null) throw new ArgumentNullException(nameof(domain));
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (org == null) throw new ArgumentNullException(nameof(org));
            if (user == null) throw new ArgumentNullException(nameof(user));

            var plan = GetOrBuildPlan<TDomain, TDto>();

            var secretId = BuildSecretId(plan, dto, org);
            var encryptionKey = await _keyProvider.GetKeyAsync(secretId, org, user, plan.CreateIfMissing, ct).ConfigureAwait(false);

            foreach (var f in plan.Fields)
            {
                var ciphertext = f.GetCiphertext(dto);
                if (f.SkipIfEmpty && String.IsNullOrEmpty(ciphertext))
                    continue;

                var salt = f.GetSalt(dto);
                if (String.IsNullOrWhiteSpace(salt))
                    throw new InvalidOperationException($"Salt resolved empty for DTO {typeof(TDto).Name}.{f.SaltPropertyName}.");

                var plaintext = _encryptor.Decrypt(salt, ciphertext, encryptionKey);
            }
        }

        private string ConvertDomainValueToString(object value, Type domainType)
        {
            if (value == null) return null;

            if (domainType == typeof(string))
                return (string)value;

            if (_converters.TryConvert(value, typeof(string), out var converted))
                return (string)converted;

            // If no converter exists, fail loudly so we don't silently corrupt/encrypt junk.
            throw new InvalidOperationException($"No converter registered to convert {domainType.Name} to string for encryption.");
        }

        private object ConvertStringToDomainValue(string plaintext, Type domainType)
        {
            Console.WriteLine($"[SetPlainText] Converting plaintext to domain value. DomainType={domainType.Name}, Plaintext='{plaintext}'");

            if (domainType == typeof(string))
                return plaintext;

            // Handle nullable targets: empty/null -> null
            var underlying = Nullable.GetUnderlyingType(domainType);
            if (underlying != null && String.IsNullOrWhiteSpace(plaintext))
                return null;

            if (_converters.TryConvert(plaintext, domainType, out var converted))
            {
                Console.WriteLine($"[SetPlainText] Converting plaintext to domain value. DomainType={domainType.Name}, Plaintext='{plaintext}, Converted={converted};");

                return converted;
            }
            throw new InvalidOperationException($"No converter registered to convert string to {domainType.Name} for decryption.");
        }


        public async Task MapEncryptAsync<TDomain, TDto>(TDomain domain, TDto dto, EntityHeader org, EntityHeader user, CancellationToken ct = default)
            where TDomain : class
            where TDto : class
        {
            if (domain == null) throw new ArgumentNullException(nameof(domain));
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (org == null) throw new ArgumentNullException(nameof(org));
            if (user == null) throw new ArgumentNullException(nameof(user));

            var plan = GetOrBuildPlan<TDomain, TDto>();

            var secretId = BuildSecretId(plan, dto, org);
            var encryptionKey = await _keyProvider.GetKeyAsync(secretId, org, user, plan.CreateIfMissing, ct).ConfigureAwait(false);

            foreach (var f in plan.Fields)
            {
                var plaintext = f.GetPlaintext(domain);
                if (f.SkipIfEmpty && String.IsNullOrEmpty(plaintext))
                    continue;

                var salt = f.GetSalt(dto);
                if (String.IsNullOrWhiteSpace(salt))
                    throw new InvalidOperationException($"Salt resolved empty for DTO {typeof(TDto).Name}.{f.SaltPropertyName}.");

                var ciphertext = _encryptor.Encrypt(salt, plaintext, encryptionKey);
                f.SetCiphertext(dto, ciphertext);
            }
        }

        private  Plan<TDomain, TDto> GetOrBuildPlan<TDomain, TDto>()
            where TDomain : class
            where TDto : class
        {
            var key = (typeof(TDomain), typeof(TDto));
            if (!_plans.TryGetValue(key, out var planObj))
            {
                planObj = BuildPlan<TDomain, TDto>();
                _plans.TryAdd(key, planObj);
            }

            return (Plan<TDomain, TDto>)planObj;
        }

        private static string BuildSecretId<TDomain, TDto>(Plan<TDomain, TDto> plan, TDto dto, EntityHeader org)
            where TDomain : class
            where TDto : class
        {
            var idValue = plan.GetId(dto);
            if (String.IsNullOrWhiteSpace(idValue))
                throw new InvalidOperationException($"Encryption key id resolved empty for DTO {typeof(TDto).Name}.{plan.IdPropertyName}.");

            var secretId = plan.SecretIdFormat
                .Replace("{orgId}", org.Id ?? "")
                .Replace("{id}", idValue);
            return secretId;
        }

        private  Plan<TDomain, TDto> BuildPlan<TDomain, TDto>()
            where TDomain : class
            where TDto : class
        {
            var dtoType = typeof(TDto);
            var domainType = typeof(TDomain);

            var keyAttr = dtoType.GetCustomAttribute<EncryptionKeyAttribute>() ?? throw new InvalidOperationException($"DTO type {dtoType.Name} must have [EncryptionKey(...)] attribute.");

            var idProp = dtoType.GetProperty(keyAttr.IdProperty, BindingFlags.Public | BindingFlags.Instance) ?? throw new InvalidOperationException($"DTO type {dtoType.Name} missing IdProperty '{keyAttr.IdProperty}'.");

            Func<TDto, string> getId = dto => idProp.GetValue(dto)?.ToString() ?? "";

            var fields = domainType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(p => new { Prop = p, Attr = p.GetCustomAttribute<EncryptedFieldAttribute>() })
                .Where(x => x.Attr != null)
                .Select(x => BuildField<TDomain, TDto>(x.Prop, x.Attr))
                .ToArray();

            return new Plan<TDomain, TDto>(keyAttr.SecretIdFormat, keyAttr.CreateIfMissing, keyAttr.IdProperty, getId, fields);
        }

        private  FieldPlan<TDomain, TDto> BuildField<TDomain, TDto>(PropertyInfo domainProp, EncryptedFieldAttribute attr)
            where TDomain : class
            where TDto : class
        {
            if (attr == null) throw new ArgumentNullException(nameof(attr));

            if (!domainProp.CanRead || !domainProp.CanWrite)
                throw new InvalidOperationException($"{typeof(TDomain).Name}.{domainProp.Name} has [EncryptedField] but must be readable and writable.");

            var dtoType = typeof(TDto);

            var cipherProp = dtoType.GetProperty(attr.CiphertextProperty, BindingFlags.Public | BindingFlags.Instance) ?? throw new InvalidOperationException($"DTO type {dtoType.Name} missing ciphertext property '{attr.CiphertextProperty}'.");
            if (!cipherProp.CanRead || !cipherProp.CanWrite || cipherProp.PropertyType != typeof(string))
                throw new InvalidOperationException($"DTO type {dtoType.Name}.{cipherProp.Name} must be a readable/writable string property.");

            var saltProp = dtoType.GetProperty(attr.SaltProperty, BindingFlags.Public | BindingFlags.Instance) ?? throw new InvalidOperationException($"DTO type {dtoType.Name} missing salt property '{attr.SaltProperty}'.");

            Func<TDto, string> getCipher = dto => cipherProp.GetValue(dto)?.ToString();
            Action<TDto, string> setCipher = (dto, ciphertext) => cipherProp.SetValue(dto, ciphertext);

            Func<TDto, string> getSalt = dto => saltProp.GetValue(dto)?.ToString() ?? "";

            Func<TDomain, string> getPlain = domain => ConvertDomainValueToString(domainProp.GetValue(domain), domainProp.PropertyType);
            Action<TDomain, string> setPlain = (domain, plaintext) =>
            {
                var value = ConvertStringToDomainValue(plaintext, domainProp.PropertyType);
                domainProp.SetValue(domain, value);
            };
            return new FieldPlan<TDomain, TDto>(cipherProp.Name, attr.SaltProperty, attr.SkipIfEmpty, getCipher, setCipher, getSalt, getPlain, setPlain);
        }

        private sealed class Plan<TDomain, TDto>
            where TDomain : class
            where TDto : class
        {
            public string SecretIdFormat { get; }
            public bool CreateIfMissing { get; }
            public string IdPropertyName { get; }
            public Func<TDto, string> GetId { get; }
            public FieldPlan<TDomain, TDto>[] Fields { get; }

            public Plan(string secretIdFormat, bool createIfMissing, string idPropertyName, Func<TDto, string> getId, FieldPlan<TDomain, TDto>[] fields)
            {
                SecretIdFormat = secretIdFormat;
                CreateIfMissing = createIfMissing;
                IdPropertyName = idPropertyName;
                GetId = getId;
                Fields = fields;
            }
        }

        private sealed class FieldPlan<TDomain, TDto>
            where TDomain : class
            where TDto : class
        {
            public string CiphertextPropertyName { get; }
            public string SaltPropertyName { get; }
            public bool SkipIfEmpty { get; }
            public Func<TDto, string> GetCiphertext { get; }
            public Action<TDto, string> SetCiphertext { get; }
            public Func<TDto, string> GetSalt { get; }
            public Func<TDomain, string> GetPlaintext { get; }
            public Action<TDomain, string> SetPlaintext { get; }

            public FieldPlan(string ciphertextPropertyName, string saltPropertyName, bool skipIfEmpty, Func<TDto, string> getCiphertext, Action<TDto, string> setCiphertext, Func<TDto, string> getSalt, Func<TDomain, string> getPlaintext, Action<TDomain, string> setPlaintext)
            {
                CiphertextPropertyName = ciphertextPropertyName;
                SaltPropertyName = saltPropertyName;
                SkipIfEmpty = skipIfEmpty;
                GetCiphertext = getCiphertext;
                SetCiphertext = setCiphertext;
                GetSalt = getSalt;
                GetPlaintext = getPlaintext;
                SetPlaintext = setPlaintext;
            }
        }
    }
}
