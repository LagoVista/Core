using System;
using System.Collections.Generic;

namespace LagoVista.Core.Security
{
    public class SignedRequestSigner
    {
        public string Sign(SignedRequestSigningContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (context.Headers == null) throw new ArgumentNullException(nameof(context.Headers));

            var headers = new Dictionary<string, string>(context.Headers, StringComparer.OrdinalIgnoreCase);
            var requestId = SignedRequestHeaders.GetRequestId(headers, context.Profile);
            var algorithm = ResolveSigningAlgorithm(context);
            var keyMaterialFormat = ResolveKeyMaterialFormat(context);

            if (context.Profile == SignedRequestCanonicalProfile.ServiceHttpV1)
            {
                if (algorithm != SignedRequestSignatureAlgorithms.RsaPssSha256) throw new InvalidOperationException("ServiceHttpV1 signed requests must use rsa-pss-sha256.");
                if (keyMaterialFormat != SignedRequestKeyMaterialFormats.RsaXml) throw new InvalidOperationException("ServiceHttpV1 signed requests must use rsa-xml key material.");
                if (String.IsNullOrWhiteSpace(context.SigningKeyId)) throw new InvalidOperationException("SigningKeyId is required for ServiceHttpV1 signed requests.");
                if (String.IsNullOrWhiteSpace(context.PrivateKeyMaterial)) throw new InvalidOperationException("PrivateKeyMaterial is required for ServiceHttpV1 signed requests.");

                context.Headers[SignedRequestHeaders.SigningKeyId] = context.SigningKeyId;
                context.Headers[SignedRequestHeaders.SignatureAlgorithm] = algorithm;
                context.Headers[SignedRequestHeaders.KeyMaterialFormat] = keyMaterialFormat;
                headers[SignedRequestHeaders.SigningKeyId] = context.SigningKeyId;
                headers[SignedRequestHeaders.SignatureAlgorithm] = algorithm;
                headers[SignedRequestHeaders.KeyMaterialFormat] = keyMaterialFormat;
            }
            else
            {
                if (algorithm != SignedRequestSignatureAlgorithms.HmacSha256) throw new InvalidOperationException("Runtime signed requests must use hmac-sha256.");
                if (keyMaterialFormat != SignedRequestKeyMaterialFormats.Raw) throw new InvalidOperationException("Runtime signed requests must use raw key material.");
                if (String.IsNullOrWhiteSpace(context.Key)) throw new ArgumentNullException(nameof(context.Key));
            }

            var canonicalContext = new SignedRequestCanonicalContext
            {
                Profile = context.Profile,
                Headers = headers,
                Method = context.Method,
                PathAndQuery = context.PathAndQuery,
                BodySha256 = context.BodySha256
            };

            var canonicalSource = SignedRequestCanonicalizer.Build(canonicalContext);
            var keyMaterial = context.Profile == SignedRequestCanonicalProfile.ServiceHttpV1 ? context.PrivateKeyMaterial : context.Key;
            var signature = SignedRequestCrypto.Sign(algorithm, keyMaterialFormat, keyMaterial, canonicalSource);
            var authorizationHeader = SignedRequestAuthorization.CreateHeader(requestId, signature);
            context.Headers[SignedRequestHeaders.Authorization] = authorizationHeader;
            return authorizationHeader;
        }

        public static string ComputeSignature(string key, string canonicalSource)
        {
            return SignedRequestCrypto.ComputeHmacSha256(key, canonicalSource);
        }

        private static string ResolveSigningAlgorithm(SignedRequestSigningContext context)
        {
            var algorithm = SignedRequestSignatureAlgorithms.Normalize(context.SignatureAlgorithm);
            if (!String.IsNullOrWhiteSpace(algorithm))
            {
                return algorithm;
            }

            if (context.Profile == SignedRequestCanonicalProfile.ServiceHttpV1)
            {
                return SignedRequestSignatureAlgorithms.RsaPssSha256;
            }

            return SignedRequestSignatureAlgorithms.HmacSha256;
        }

        private static string ResolveKeyMaterialFormat(SignedRequestSigningContext context)
        {
            var format = SignedRequestKeyMaterialFormats.Normalize(context.KeyMaterialFormat);
            if (!String.IsNullOrWhiteSpace(format))
            {
                return format;
            }

            if (context.Profile == SignedRequestCanonicalProfile.ServiceHttpV1)
            {
                return SignedRequestKeyMaterialFormats.RsaXml;
            }

            return SignedRequestKeyMaterialFormats.Raw;
        }
    }
}
