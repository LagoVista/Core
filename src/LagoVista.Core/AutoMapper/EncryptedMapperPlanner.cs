using LagoVista.Core.Attributes;
using LagoVista.Core.Interfaces.AutoMapper;
using LagoVista.Core.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LagoVista.Core.AutoMapper
{
    /// <summary>
    /// Builds and caches encryption mapping plans per (Domain, DTO) pair.
    /// </summary>
    public sealed class EncryptedMapperPlanner : IEncryptedMapperPlanner
    {
        private static readonly ConcurrentDictionary<(Type Domain, Type Dto), object> _plans =
            new ConcurrentDictionary<(Type Domain, Type Dto), object>();

        private readonly IMapValueConverterRegistry _converters;

        public EncryptedMapperPlanner(IMapValueConverterRegistry converters)
        {
            _converters = converters ?? throw new ArgumentNullException(nameof(converters));
        }

        public IEncryptedMapPlan<TDomain, TDto> GetOrBuildPlan<TDomain, TDto>()
            where TDomain : class
            where TDto : class
        {
            var key = (typeof(TDomain), typeof(TDto));
            if (!_plans.TryGetValue(key, out var planObj))
            {
                planObj = BuildPlan<TDomain, TDto>();
                _plans.TryAdd(key, planObj);
            }

            return (IEncryptedMapPlan<TDomain, TDto>)planObj;
        }

        private string ConvertDomainValueToString(object value, Type domainType)
        {
            if (value == null) return null;

            if (domainType == typeof(string))
                return (string)value;

            if (_converters.TryConvert(value, typeof(string), out var converted))
                return (string)converted;

            throw new InvalidOperationException($"No converter registered to convert {domainType.Name} to string for encryption.");
        }

        private object ConvertStringToDomainValue(string plaintext, Type domainType)
        {
            if (domainType == typeof(string))
                return plaintext;

            var underlying = Nullable.GetUnderlyingType(domainType);
            if (underlying != null && string.IsNullOrWhiteSpace(plaintext))
                return null;

            if (_converters.TryConvert(plaintext, domainType, out var converted))
                return converted;

            throw new InvalidOperationException($"No converter registered to convert string to {domainType.Name} for decryption.");
        }

        private IEncryptedMapPlan<TDomain, TDto> BuildPlan<TDomain, TDto>()
            where TDomain : class
            where TDto : class
        {
            var dtoType = typeof(TDto);
            var domainType = typeof(TDomain);

            var keyAttr = dtoType.GetCustomAttribute<EncryptionKeyAttribute>()
                ?? throw new InvalidOperationException($"DTO type {dtoType.Name} must have [EncryptionKey(...)] attribute.");

            var idProp = dtoType.GetProperty(keyAttr.IdProperty, BindingFlags.Public | BindingFlags.Instance)
                ?? throw new InvalidOperationException($"DTO type {dtoType.Name} missing IdProperty '{keyAttr.IdProperty}'.");

            Func<TDto, string> getId = dto => idProp.GetValue(dto)?.ToString() ?? "";

            var fields = domainType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(p => new { Prop = p, Attr = p.GetCustomAttribute<EncryptedFieldAttribute>() })
                .Where(x => x.Attr != null)
                .Select(x => BuildField<TDomain, TDto>(x.Prop, x.Attr))
                .ToArray();

            return new EncryptedMapPlan<TDomain, TDto>(keyAttr.SecretIdFormat, keyAttr.CreateIfMissing, keyAttr.IdProperty, getId, fields);
        }

        private IEncryptedFieldPlan<TDomain, TDto> BuildField<TDomain, TDto>(PropertyInfo domainProp, EncryptedFieldAttribute attr)
            where TDomain : class
            where TDto : class
        {
            if (!domainProp.CanRead || !domainProp.CanWrite)
                throw new InvalidOperationException($"{typeof(TDomain).Name}.{domainProp.Name} has [EncryptedField] but must be readable and writable.");

            var dtoType = typeof(TDto);

            var cipherProp = dtoType.GetProperty(attr.CiphertextProperty, BindingFlags.Public | BindingFlags.Instance)
                ?? throw new InvalidOperationException($"DTO type {dtoType.Name} missing ciphertext property '{attr.CiphertextProperty}'.");

            if (!cipherProp.CanRead || !cipherProp.CanWrite || cipherProp.PropertyType != typeof(string))
                throw new InvalidOperationException($"DTO type {dtoType.Name}.{cipherProp.Name} must be a readable/writable string property.");

            var saltProp = dtoType.GetProperty(attr.SaltProperty, BindingFlags.Public | BindingFlags.Instance)
                ?? throw new InvalidOperationException($"DTO type {dtoType.Name} missing salt property '{attr.SaltProperty}'.");

            Func<TDto, string> getCipher = dto => cipherProp.GetValue(dto)?.ToString();
            Action<TDto, string> setCipher = (dto, ciphertext) => cipherProp.SetValue(dto, ciphertext);

            Func<TDto, string> getSalt = dto => saltProp.GetValue(dto)?.ToString() ?? "";

            Func<TDomain, string> getPlain = domain => ConvertDomainValueToString(domainProp.GetValue(domain), domainProp.PropertyType);
            Action<TDomain, string> setPlain = (domain, plaintext) =>
            {
                var value = ConvertStringToDomainValue(plaintext, domainProp.PropertyType);
                domainProp.SetValue(domain, value);
            };

            return new EncryptedFieldPlan<TDomain, TDto>(cipherProp.Name, attr.SaltProperty, attr.SkipIfEmpty, getCipher, setCipher, getSalt, getPlain, setPlain);
        }

        private sealed class EncryptedMapPlan<TDomain, TDto> : IEncryptedMapPlan<TDomain, TDto>
            where TDomain : class
            where TDto : class
        {
            private readonly string _secretIdFormat;
            private readonly Func<TDto, string> _getId;
            private readonly string _idPropertyName;

            public bool CreateIfMissing { get; }
            public IReadOnlyList<IEncryptedFieldPlan<TDomain, TDto>> Fields { get; }

            public EncryptedMapPlan(
                string secretIdFormat,
                bool createIfMissing,
                string idPropertyName,
                Func<TDto, string> getId,
                IEncryptedFieldPlan<TDomain, TDto>[] fields)
            {
                _secretIdFormat = secretIdFormat;
                CreateIfMissing = createIfMissing;
                _idPropertyName = idPropertyName;
                _getId = getId;
                Fields = fields;
            }

            public string BuildSecretId(TDto dto, EntityHeader org)
            {
                var idValue = _getId(dto);
                if (string.IsNullOrWhiteSpace(idValue))
                    throw new InvalidOperationException($"Encryption key id resolved empty for DTO {typeof(TDto).Name}.{_idPropertyName}.");

                return _secretIdFormat
                    .Replace("{orgId}", org?.Id ?? "")
                    .Replace("{id}", idValue);
            }
        }

        private sealed class EncryptedFieldPlan<TDomain, TDto> : IEncryptedFieldPlan<TDomain, TDto>
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

            public EncryptedFieldPlan(
                string ciphertextPropertyName,
                string saltPropertyName,
                bool skipIfEmpty,
                Func<TDto, string> getCiphertext,
                Action<TDto, string> setCiphertext,
                Func<TDto, string> getSalt,
                Func<TDomain, string> getPlaintext,
                Action<TDomain, string> setPlaintext)
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