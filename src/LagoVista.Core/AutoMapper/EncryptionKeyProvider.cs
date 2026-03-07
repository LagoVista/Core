using LagoVista.Core.Interfaces;
using LagoVista.Core.Interfaces.AutoMapper;
using LagoVista.Core.Models;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Core.Services
{
    /// <summary>
    /// Encryption key provider with per-instance caching.
    ///
    /// Register this as Scoped to ensure:
    /// - Keys are fetched once per operation (list/graph mapping)
    /// - Cache is discarded after the operation
    ///
    /// Cache key is the provided secretId. In your system secretId already includes ":kv:<int>",
    /// so kv is naturally part of the cache key.
    /// </summary>
    [CriticalCoverage]
    public sealed class EncryptionKeyProvider : IEncryptionKeyProvider
    {
        private readonly ISecureStorage _secureStorage;

        // Cache successful resolutions only; store the inflight Task so concurrent callers await the same fetch.
        private readonly ConcurrentDictionary<string, Lazy<Task<string>>> _cache =
            new ConcurrentDictionary<string, Lazy<Task<string>>>(StringComparer.Ordinal);

        public EncryptionKeyProvider(ISecureStorage secureStorage)
        {
            _secureStorage = secureStorage ?? throw new ArgumentNullException(nameof(secureStorage));
        }

        public Task<string> GetKeyAsync(string secretId, EntityHeader org, EntityHeader user, bool createIfMissing, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(secretId)) throw new ArgumentNullException(nameof(secretId));
            if (org == null) throw new ArgumentNullException(nameof(org));
            if (user == null) throw new ArgumentNullException(nameof(user));

            // If createIfMissing is false, we still want to cache successes,
            // but we do NOT want to cache failures. We enforce this by evicting on failure.
            var lazy = _cache.GetOrAdd(
                secretId,
                sid => new Lazy<Task<string>>(
                    () => GetOrCreateKeyInternalAsync(sid, org, user, createIfMissing, ct),
                    LazyThreadSafetyMode.ExecutionAndPublication));

            return AwaitAndEvictOnFailureAsync(secretId, lazy);
        }

        private async Task<string> AwaitAndEvictOnFailureAsync(string secretId, Lazy<Task<string>> lazy)
        {
            try
            {
                return await lazy.Value.ConfigureAwait(false);
            }
            catch
            {
                // Do not cache failures. If a later call has different createIfMissing semantics,
                // it should get a fresh attempt (though in your rule-set, a failure usually means bail).
                _cache.TryRemove(secretId, out _);
                throw;
            }
        }

        private async Task<string> GetOrCreateKeyInternalAsync(string secretId, EntityHeader org, EntityHeader user, bool createIfMissing, CancellationToken ct)
        {
            // Note: ISecureStorage does not accept CancellationToken in current signature.
            // We still accept ct to preserve interface symmetry.
            ct.ThrowIfCancellationRequested();

            var keyResult = await _secureStorage.GetSecretAsync(org, secretId, user).ConfigureAwait(false);

            if (keyResult.Successful)
                return keyResult.Result;

            if (!createIfMissing)
                throw new InvalidOperationException($"Could not read encryption key for: {secretId} - {keyResult.ErrorMessage}");

            // Create
            var key = Guid.NewGuid().ToString("N");
            await _secureStorage.AddSecretAsync(org, secretId, key).ConfigureAwait(false);

            // Re-read
            keyResult = await _secureStorage.GetSecretAsync(org, secretId, user).ConfigureAwait(false);
            if (!keyResult.Successful)
                throw new InvalidOperationException($"Could not create encryption key for: {secretId} - {keyResult.ErrorMessage}");

            return keyResult.Result;
        }
    }
}