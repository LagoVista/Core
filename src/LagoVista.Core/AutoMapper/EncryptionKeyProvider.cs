using LagoVista.Core.Interfaces;
using LagoVista.Core.Interfaces.AutoMapper;
using LagoVista.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Core.AutoMapper
{
    public sealed class EncryptionKeyProvider : IEncryptionKeyProvider
    {
        private readonly ISecureStorage _secureStorage;

        public EncryptionKeyProvider(ISecureStorage secureStorage)
        {
            _secureStorage = secureStorage ?? throw new ArgumentNullException(nameof(secureStorage));
        }

        public async Task<string> GetKeyAsync(string secretId, EntityHeader org, EntityHeader user, bool createIfMissing, CancellationToken ct = default)
        {
            var keyResult = await _secureStorage.GetSecretAsync(org, secretId, user);

            if (keyResult.Successful)
                return keyResult.Result;

            if (!createIfMissing)
                throw new ArgumentNullException($"Could not read encryption key for: {secretId} - {keyResult.ErrorMessage}");

            // Create
            var key = Guid.NewGuid().ToString("N"); // swap to your ToId() if you want
            await _secureStorage.AddSecretAsync(org, secretId, key);

            // Re-read
            keyResult = await _secureStorage.GetSecretAsync(org, secretId, user);
            if (!keyResult.Successful)
                throw new ArgumentNullException($"Could not create encryption key for: {secretId} - {keyResult.ErrorMessage}");

            return keyResult.Result;
        }
    }
}