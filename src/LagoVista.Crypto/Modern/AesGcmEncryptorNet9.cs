using System;
using System.Security.Cryptography;

namespace LagoVista.Crypto.Modern
{
    public sealed class AesGcmEncryptorNet9 : IAeadEncryptor
    {
        public void Encrypt(byte[] key32, byte[] nonce12, byte[] plaintext, byte[] aad, byte[] ciphertextOut, byte[] tag16Out)
        {
            if (key32 == null) throw new ArgumentNullException(nameof(key32));
            if (nonce12 == null) throw new ArgumentNullException(nameof(nonce12));
            if (plaintext == null) throw new ArgumentNullException(nameof(plaintext));
            if (aad == null) throw new ArgumentNullException(nameof(aad));
            if (ciphertextOut == null) throw new ArgumentNullException(nameof(ciphertextOut));
            if (tag16Out == null) throw new ArgumentNullException(nameof(tag16Out));

            if (key32.Length != 32) throw new ArgumentOutOfRangeException(nameof(key32), "Key must be 32 bytes.");
            if (nonce12.Length != 12) throw new ArgumentOutOfRangeException(nameof(nonce12), "Nonce must be 12 bytes.");
            if (tag16Out.Length != 16) throw new ArgumentOutOfRangeException(nameof(tag16Out), "Tag must be 16 bytes.");
            if (ciphertextOut.Length != plaintext.Length) throw new ArgumentOutOfRangeException(nameof(ciphertextOut), "Ciphertext buffer must match plaintext length.");

            using (var gcm = new AesGcm(key32, 16))
            {
                gcm.Encrypt(nonce12, plaintext, ciphertextOut, tag16Out, aad);
            }
        }

        public byte[] Decrypt(byte[] key32, byte[] nonce12, byte[] ciphertext, byte[] tag16, byte[] aad)
        {
            if (key32 == null) throw new ArgumentNullException(nameof(key32));
            if (nonce12 == null) throw new ArgumentNullException(nameof(nonce12));
            if (ciphertext == null) throw new ArgumentNullException(nameof(ciphertext));
            if (tag16 == null) throw new ArgumentNullException(nameof(tag16));
            if (aad == null) throw new ArgumentNullException(nameof(aad));

            if (key32.Length != 32) throw new ArgumentOutOfRangeException(nameof(key32), "Key must be 32 bytes.");
            if (nonce12.Length != 12) throw new ArgumentOutOfRangeException(nameof(nonce12), "Nonce must be 12 bytes.");
            if (tag16.Length != 16) throw new ArgumentOutOfRangeException(nameof(tag16), "Tag must be 16 bytes.");

            var plaintext = new byte[ciphertext.Length];
            using (var gcm = new AesGcm(key32, 16))
            {
                gcm.Decrypt(nonce12, ciphertext, tag16, plaintext, aad);
            }

            return plaintext;
        }
    }
}
