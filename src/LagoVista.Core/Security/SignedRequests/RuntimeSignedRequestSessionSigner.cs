using System;
using System.Security.Cryptography;
using System.Text;

namespace LagoVista.Core.Security
{
    /// <summary>
    /// Signs the existing RuntimeInstanceHttpV1 canonical request with a short-lived
    /// runtime-session secret. The durable runtime HMAC remains present as fallback.
    /// </summary>
    public static class RuntimeSignedRequestSessionSigner
    {
        public static string Sign(
            string sessionSecret,
            string sessionId,
            SignedRequestCanonicalContext context)
        {
            if (String.IsNullOrWhiteSpace(sessionSecret)) throw new ArgumentNullException(nameof(sessionSecret));
            if (String.IsNullOrWhiteSpace(sessionId)) throw new ArgumentNullException(nameof(sessionId));
            if (context == null) throw new ArgumentNullException(nameof(context));

            context.Profile = SignedRequestCanonicalProfile.RuntimeInstanceHttpV1;
            var canonical = SignedRequestCanonicalizer.Build(context) + sessionId + "\r\n";
            var key = Convert.FromBase64String(sessionSecret);

            using (var hmac = new HMACSHA256(key))
            {
                return Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(canonical)));
            }
        }

        public static bool Validate(
            string sessionSecret,
            string sessionId,
            string signature,
            SignedRequestCanonicalContext context)
        {
            if (String.IsNullOrWhiteSpace(signature)) return false;

            try
            {
                var expected = Convert.FromBase64String(Sign(sessionSecret, sessionId, context));
                var actual = Convert.FromBase64String(signature);
                return expected.Length == actual.Length && CryptographicOperations.FixedTimeEquals(expected, actual);
            }
            catch (FormatException)
            {
                return false;
            }
        }

        public static string CreateSecret()
        {
            var bytes = new byte[32];
            RandomNumberGenerator.Fill(bytes);
            return Convert.ToBase64String(bytes);
        }

        public static string CreateSessionId()
        {
            var bytes = new byte[16];
            RandomNumberGenerator.Fill(bytes);
            return Convert.ToBase64String(bytes)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }
    }
}
