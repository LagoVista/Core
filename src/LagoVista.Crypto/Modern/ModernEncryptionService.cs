using LagoVista.Core.Interfaces;
using LagoVista.Core.Models.Crypto;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Crypto.Modern
{
    /// <summary>
    /// Orchestrates: validate -> AAD -> key material -> AES-GCM -> envelope.
    /// Modern-only: will throw if envelope is not the expected modern format.
    /// </summary>
    public sealed class ModernEncryptionService : IModernEncryption
    {
        private readonly IAadBuilder _aadBuilder;
        private readonly IEnvelopeCodec _envelopeCodec;
        private readonly IKeyMaterialStore _keyMaterialStore;
        private readonly IAeadEncryptor _aead;

        public ModernEncryptionService(
            IAadBuilder aadBuilder,
            IEnvelopeCodec envelopeCodec,
            IKeyMaterialStore keyMaterialStore,
            IAeadEncryptor aead)
        {
            _aadBuilder = aadBuilder ?? throw new ArgumentNullException(nameof(aadBuilder));
            _envelopeCodec = envelopeCodec ?? throw new ArgumentNullException(nameof(envelopeCodec));
            _keyMaterialStore = keyMaterialStore ?? throw new ArgumentNullException(nameof(keyMaterialStore));
            _aead = aead ?? throw new ArgumentNullException(nameof(aead));
        }

        public async Task<string> EncryptAsync(EncryptStringRequest request, CancellationToken ct = default)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            ValidateBadString(request.FieldName, nameof(request.FieldName));
            ValidateBadString(request.KeyId, nameof(request.KeyId));
            ValidateBadString(request.Plaintext, nameof(request.Plaintext));
            if (request.Kv <= 0) throw new ArgumentOutOfRangeException(nameof(request.Kv));

            var fieldLower = request.FieldName.ToLowerInvariant();

            var aad = _aadBuilder.BuildAadV1(request.OrgId, request.RecId, fieldLower, request.KeyId);
            var key32 = await _keyMaterialStore.GetOrCreateKey32Async(request.Org, request.User, request.KeyId, request.Kv, ct);

            var nonce = new byte[12];
            RandomNumberGenerator.Fill(nonce);

            var plaintextBytes = Encoding.UTF8.GetBytes(request.Plaintext);
            var ciphertext = new byte[plaintextBytes.Length];
            var tag = new byte[16];

            _aead.Encrypt(key32, nonce, plaintextBytes, aad, ciphertext, tag);

            return _envelopeCodec.Build(request.Kv, nonce, ciphertext, tag);
        }

        public async Task<string> DecryptAsync(DecryptStringRequest request, CancellationToken ct = default)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            ValidateBadString(request.FieldName, nameof(request.FieldName));
            ValidateBadString(request.KeyId, nameof(request.KeyId));
            ValidateBadString(request.Envelope, nameof(request.Envelope));

            var fieldLower = request.FieldName.ToLowerInvariant();

            var parts = _envelopeCodec.Parse(request.Envelope);

            var aad = _aadBuilder.BuildAadV1(request.OrgId, request.RecId, fieldLower, request.KeyId);
            var key32 = await _keyMaterialStore.GetOrCreateKey32Async(request.Org, request.User, request.KeyId, parts.Kv, ct);

            var plaintextBytes = _aead.Decrypt(key32, parts.Nonce, parts.Ciphertext, parts.Tag, aad);
            return Encoding.UTF8.GetString(plaintextBytes);
        }

        private static void ValidateBadString(string value, string name)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentNullException(name);
        }
    }
}
