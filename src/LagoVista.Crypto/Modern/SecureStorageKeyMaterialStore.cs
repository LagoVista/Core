using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using System;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Crypto.Modern
{
    public sealed class SecureStorageKeyMaterialStore : IKeyMaterialStore
    {
        private readonly ISecureStorage _secureStorage;

        public SecureStorageKeyMaterialStore(ISecureStorage secureStorage)
        {
            _secureStorage = secureStorage ?? throw new ArgumentNullException(nameof(secureStorage));
        }

        public async Task<byte[]> GetOrCreateKey32Async(EntityHeader org, EntityHeader user, string keyId, int kv, CancellationToken ct = default)
        {
            if (org == null) throw new ArgumentNullException(nameof(org));
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrWhiteSpace(keyId)) throw new ArgumentNullException(nameof(keyId));
            if (kv <= 0) throw new ArgumentOutOfRangeException(nameof(kv));

            var secretId = BuildSecretId(keyId, kv);

            var get = await _secureStorage.GetSecretAsync(org, secretId, user);
            if (!get.Successful)
            {
                // auto-create
                var key = new byte[32];
                RandomNumberGenerator.Fill(key);
                var keyB64 = Convert.ToBase64String(key);

                await _secureStorage.AddSecretAsync(org, secretId, keyB64);

                get = await _secureStorage.GetSecretAsync(org, secretId, user);
                if (!get.Successful)
                    throw new InvalidOperationException($"Could not create key material for: {secretId} - {get.ErrorMessage}");
            }

            var bytes = Convert.FromBase64String(get.Result);
            if (bytes.Length != 32) throw new InvalidOperationException($"Key material for '{secretId}' must be 32 bytes.");
            return bytes;
        }

        private static string BuildSecretId(string keyId, int kv)
        {
            // Locked naming: <KeyId>:kv:<int>
            return $"{keyId}:kv:{kv}";
        }
    }
}
