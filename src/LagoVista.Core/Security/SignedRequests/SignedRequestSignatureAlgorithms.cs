using System;

namespace LagoVista.Core.Security
{
    public static class SignedRequestSignatureAlgorithms
    {
        public const string HmacSha256 = "hmac-sha256";
        public const string RsaPssSha256 = "rsa-pss-sha256";

        public static string Normalize(string algorithm)
        {
            if (String.IsNullOrWhiteSpace(algorithm))
            {
                return String.Empty;
            }

            return algorithm.Trim().ToLowerInvariant();
        }

        public static bool IsSupported(string algorithm)
        {
            var normalized = Normalize(algorithm);
            return normalized == HmacSha256 || normalized == RsaPssSha256;
        }
    }
}
