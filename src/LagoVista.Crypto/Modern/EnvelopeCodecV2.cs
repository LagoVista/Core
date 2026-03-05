using System;
using System.Collections.Generic;

namespace LagoVista.Crypto.Modern
{
    /// <summary>
    /// Envelope format:
    /// enc;v=2;alg=a256gcm;kv=<int>;aad=v1;data=<base64url(nonce|ciphertext|tag)>;
    /// - base64url has no padding.
    /// - must start with 'enc;' and end with ';'
    /// </summary>
    public sealed class EnvelopeCodecV2 : IEnvelopeCodec
    {
        private const string Prefix = "enc;";

        public string Build(int kv, byte[] nonce12, byte[] ciphertext, byte[] tag16)
        {
            if (kv <= 0) throw new ArgumentOutOfRangeException(nameof(kv));
            if (nonce12 == null) throw new ArgumentNullException(nameof(nonce12));
            if (ciphertext == null) throw new ArgumentNullException(nameof(ciphertext));
            if (tag16 == null) throw new ArgumentNullException(nameof(tag16));
            if (nonce12.Length != 12) throw new ArgumentOutOfRangeException(nameof(nonce12), "Nonce must be 12 bytes.");
            if (tag16.Length != 16) throw new ArgumentOutOfRangeException(nameof(tag16), "Tag must be 16 bytes.");

            var data = new byte[nonce12.Length + ciphertext.Length + tag16.Length];
            Buffer.BlockCopy(nonce12, 0, data, 0, nonce12.Length);
            Buffer.BlockCopy(ciphertext, 0, data, nonce12.Length, ciphertext.Length);
            Buffer.BlockCopy(tag16, 0, data, nonce12.Length + ciphertext.Length, tag16.Length);

            var dataB64Url = Base64Url.Encode(data);

            return $"enc;v=2;alg=a256gcm;kv={kv};aad=v1;data={dataB64Url};";
        }

        public EnvelopeParts Parse(string envelope)
        {
            if (string.IsNullOrWhiteSpace(envelope)) throw new ArgumentNullException(nameof(envelope));
            if (!envelope.StartsWith(Prefix, StringComparison.Ordinal)) throw new FormatException("Envelope must start with 'enc;'.");
            if (!envelope.EndsWith(";", StringComparison.Ordinal)) throw new FormatException("Envelope must end with ';'.");

            // Split tokens by ';' (final is empty due to trailing ';')
            var tokens = envelope.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            // tokens[0] == "enc"
            var dict = new Dictionary<string, string>(StringComparer.Ordinal);
            for (var i = 1; i < tokens.Length; i++)
            {
                var t = tokens[i];
                var idx = t.IndexOf('=');
                if (idx <= 0 || idx == t.Length - 1) throw new FormatException($"Invalid token '{t}'.");
                var k = t.Substring(0, idx);
                var v = t.Substring(idx + 1);
                dict[k] = v;
            }

            Require(dict, "v", "2");
            Require(dict, "alg", "a256gcm");
            Require(dict, "aad", "v1");

            if (!dict.TryGetValue("kv", out var kvStr) || string.IsNullOrWhiteSpace(kvStr))
                throw new FormatException("Missing kv.");
            if (!int.TryParse(kvStr, out var kv) || kv <= 0)
                throw new FormatException("Invalid kv.");

            if (!dict.TryGetValue("data", out var dataStr) || string.IsNullOrWhiteSpace(dataStr))
                throw new FormatException("Missing data.");

            var data = Base64Url.Decode(dataStr);
            if (data.Length < 12 + 16) throw new FormatException("Data too short.");

            var nonce = new byte[12];
            Buffer.BlockCopy(data, 0, nonce, 0, 12);

            var tag = new byte[16];
            Buffer.BlockCopy(data, data.Length - 16, tag, 0, 16);

            var ciphertextLen = data.Length - 12 - 16;
            var ciphertext = new byte[ciphertextLen];
            Buffer.BlockCopy(data, 12, ciphertext, 0, ciphertextLen);

            return new EnvelopeParts
            {
                Kv = kv,
                Nonce = nonce,
                Ciphertext = ciphertext,
                Tag = tag,
            };
        }

        private static void Require(Dictionary<string, string> dict, string key, string expected)
        {
            if (!dict.TryGetValue(key, out var val)) throw new FormatException($"Missing {key}.");
            if (!string.Equals(val, expected, StringComparison.Ordinal)) throw new FormatException($"Invalid {key}.");
        }
    }
}
