using System;
using System.Security.Cryptography;
using System.Text;

namespace LagoVista.Core.Security
{
    public static class SignedRequestCrypto
    {
        public static string Sign(string algorithm, string keyMaterialFormat, string keyMaterial, string canonicalSource)
        {
            if (String.IsNullOrWhiteSpace(algorithm)) throw new ArgumentNullException(nameof(algorithm));
            if (String.IsNullOrWhiteSpace(keyMaterialFormat)) throw new ArgumentNullException(nameof(keyMaterialFormat));
            if (String.IsNullOrWhiteSpace(keyMaterial)) throw new ArgumentNullException(nameof(keyMaterial));
            if (canonicalSource == null) throw new ArgumentNullException(nameof(canonicalSource));

            var normalizedAlgorithm = SignedRequestSignatureAlgorithms.Normalize(algorithm);
            var normalizedFormat = SignedRequestKeyMaterialFormats.Normalize(keyMaterialFormat);

            if (normalizedAlgorithm == SignedRequestSignatureAlgorithms.HmacSha256)
            {
                if (normalizedFormat != SignedRequestKeyMaterialFormats.Raw) throw new InvalidOperationException("hmac-sha256 signed requests must use raw key material.");
                return ComputeHmacSha256(keyMaterial, canonicalSource);
            }

            if (normalizedAlgorithm == SignedRequestSignatureAlgorithms.RsaPssSha256)
            {
                if (normalizedFormat != SignedRequestKeyMaterialFormats.RsaXml) throw new InvalidOperationException("rsa-pss-sha256 signed requests must use rsa-xml key material.");
                return SignRsaPssSha256(keyMaterial, canonicalSource);
            }

            throw new InvalidOperationException($"Unsupported signed request signature algorithm '{algorithm}'.");
        }

        public static bool Verify(string algorithm, string keyMaterialFormat, string keyMaterial, string canonicalSource, string expectedSignature)
        {
            if (String.IsNullOrWhiteSpace(algorithm)) throw new ArgumentNullException(nameof(algorithm));
            if (String.IsNullOrWhiteSpace(keyMaterialFormat)) throw new ArgumentNullException(nameof(keyMaterialFormat));
            if (String.IsNullOrWhiteSpace(keyMaterial)) return false;
            if (canonicalSource == null) throw new ArgumentNullException(nameof(canonicalSource));
            if (String.IsNullOrWhiteSpace(expectedSignature)) return false;

            var normalizedAlgorithm = SignedRequestSignatureAlgorithms.Normalize(algorithm);
            var normalizedFormat = SignedRequestKeyMaterialFormats.Normalize(keyMaterialFormat);

            if (normalizedAlgorithm == SignedRequestSignatureAlgorithms.HmacSha256)
            {
                if (normalizedFormat != SignedRequestKeyMaterialFormats.Raw) throw new InvalidOperationException("hmac-sha256 signed requests must use raw key material.");

                var actualSignature = ComputeHmacSha256(keyMaterial, canonicalSource);
                var actualBytes = Encoding.UTF8.GetBytes(actualSignature);
                var expectedBytes = Encoding.UTF8.GetBytes(expectedSignature);

                return FixedTimeEquals(actualBytes, expectedBytes);
            }

            if (normalizedAlgorithm == SignedRequestSignatureAlgorithms.RsaPssSha256)
            {
                if (normalizedFormat != SignedRequestKeyMaterialFormats.RsaXml) throw new InvalidOperationException("rsa-pss-sha256 signed requests must use rsa-xml key material.");
                return VerifyRsaPssSha256(keyMaterial, canonicalSource, expectedSignature);
            }

            throw new InvalidOperationException($"Unsupported signed request signature algorithm '{algorithm}'.");
        }

        public static string ComputeHmacSha256(string key, string canonicalSource)
        {
            if (String.IsNullOrWhiteSpace(key)) throw new ArgumentNullException(nameof(key));
            if (canonicalSource == null) throw new ArgumentNullException(nameof(canonicalSource));

            var sourceBytes = Encoding.UTF8.GetBytes(canonicalSource);
            var keyBytes = Encoding.UTF8.GetBytes(key);

            using (var hmac = new HMACSHA256(keyBytes))
            {
                var resultBytes = hmac.ComputeHash(sourceBytes);
                return Convert.ToBase64String(resultBytes);
            }
        }

        public static string SignRsaPssSha256(string privateKeyXml, string canonicalSource)
        {
            if (String.IsNullOrWhiteSpace(privateKeyXml)) throw new ArgumentNullException(nameof(privateKeyXml));
            if (canonicalSource == null) throw new ArgumentNullException(nameof(canonicalSource));

            var sourceBytes = Encoding.UTF8.GetBytes(canonicalSource);
            var parameters = SignedRequestRsaXmlSerializer.FromXmlString(privateKeyXml);

            using (var rsa = RSA.Create())
            {
                rsa.ImportParameters(parameters);
                var signatureBytes = rsa.SignData(sourceBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pss);
                return Convert.ToBase64String(signatureBytes);
            }
        }

        public static bool VerifyRsaPssSha256(string publicKeyXml, string canonicalSource, string expectedSignature)
        {
            if (String.IsNullOrWhiteSpace(publicKeyXml)) return false;
            if (canonicalSource == null) throw new ArgumentNullException(nameof(canonicalSource));
            if (String.IsNullOrWhiteSpace(expectedSignature)) return false;

            byte[] signatureBytes;
            try
            {
                signatureBytes = Convert.FromBase64String(expectedSignature);
            }
            catch
            {
                return false;
            }

            var sourceBytes = Encoding.UTF8.GetBytes(canonicalSource);
            var parameters = SignedRequestRsaXmlSerializer.FromXmlString(publicKeyXml);

            using (var rsa = RSA.Create())
            {
                rsa.ImportParameters(parameters);
                return rsa.VerifyData(sourceBytes, signatureBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pss);
            }
        }

        private static bool FixedTimeEquals(byte[] actualBytes, byte[] expectedBytes)
        {
            if (actualBytes == null) throw new ArgumentNullException(nameof(actualBytes));
            if (expectedBytes == null) throw new ArgumentNullException(nameof(expectedBytes));

            var difference = actualBytes.Length ^ expectedBytes.Length;
            var length = Math.Min(actualBytes.Length, expectedBytes.Length);

            for (var idx = 0; idx < length; ++idx)
            {
                difference |= actualBytes[idx] ^ expectedBytes[idx];
            }

            return difference == 0;
        }
    }
}
