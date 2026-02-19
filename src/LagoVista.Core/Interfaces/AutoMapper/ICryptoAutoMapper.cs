using LagoVista.Core.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Core.Interfaces.AutoMapper
{
    public interface IMapValueConverter
    {
        bool CanConvert(Type sourceType, Type targetType);
        object Convert(object sourceValue, Type targetType);
    }

    public interface IMapValueConverterRegistry
    {
        bool TryConvert(object sourceValue, Type targetType, out object convertedValue);
    }


    public interface ILagoVistaAutoMapper
    {
        Task<TTarget> CreateAsync<TSource, TTarget>(TSource source, EntityHeader org, EntityHeader user, Action<TSource, TTarget> afterMap = null, CancellationToken ct = default)
            where TTarget : class, new()
            where TSource : class;
        Task MapAsync<TSource, TTarget>(TSource source, TTarget target, EntityHeader org, EntityHeader user, Action<TSource, TTarget> afterMap = null, CancellationToken ct = default)
            where TTarget : class
            where TSource : class;
    }

    public interface IEncryptionKeyProvider
    {
        Task<string> GetKeyAsync(string secretId, EntityHeader org, EntityHeader user, bool createIfMissing, CancellationToken ct = default);
    }

    public interface IEncryptedMapper
    {
        Task MapDecryptAsync<TDomain, TDto>(TDomain domain, TDto dto, EntityHeader org, EntityHeader user, CancellationToken ct = default)
            where TDomain : class
            where TDto : class;

        Task MapEncryptAsync<TDomain, TDto>(TDomain domain, TDto dto, EntityHeader org, EntityHeader user, CancellationToken ct = default)
            where TDomain : class
            where TDto : class;
    }

    public interface IEncryptor
    {
        string Decrypt(string salt, string ciphertext, string key);
        string Encrypt(string salt, string plaintext, string key);
    }
}
