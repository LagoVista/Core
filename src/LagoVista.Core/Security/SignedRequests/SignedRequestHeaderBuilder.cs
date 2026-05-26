using System;
using System.Collections.Generic;

namespace LagoVista.Core.Security
{
    public class SignedRequestHeaderBuilder
    {
        private readonly SignedRequestSigner _signer = new SignedRequestSigner();

        public Dictionary<string, string> BuildHeaders(SignedRequestHeaderBuildContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (String.IsNullOrWhiteSpace(context.RequestId)) throw new ArgumentNullException(nameof(context.RequestId));

            var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            SignedRequestHeaders.SetRequestId(headers, context.Profile, context.RequestId);
            headers[SignedRequestHeaders.Date] = context.DateUtc.UtcDateTime.ToString("o");
            headers[SignedRequestHeaders.Version] = context.Version;

            if (context.Profile == SignedRequestCanonicalProfile.ServiceHttpV1)
            {
                if (ResolveSigningAlgorithm(context) != SignedRequestSignatureAlgorithms.RsaPssSha256) throw new InvalidOperationException("ServiceHttpV1 signed requests must use rsa-pss-sha256.");
                if (ResolveKeyMaterialFormat(context) != SignedRequestKeyMaterialFormats.RsaXml) throw new InvalidOperationException("ServiceHttpV1 signed requests must use rsa-xml key material.");
                if (String.IsNullOrWhiteSpace(context.CallerId)) throw new InvalidOperationException("CallerId is required for ServiceHttpV1 signed requests.");
                if (String.IsNullOrWhiteSpace(context.SigningKeyId)) throw new InvalidOperationException("SigningKeyId is required for ServiceHttpV1 signed requests.");
                if (String.IsNullOrWhiteSpace(context.PrivateKeyMaterial)) throw new InvalidOperationException("PrivateKeyMaterial is required for ServiceHttpV1 signed requests.");

                headers[SignedRequestHeaders.CallerId] = context.CallerId;
                headers[SignedRequestHeaders.SigningKeyId] = context.SigningKeyId;
                headers[SignedRequestHeaders.SignatureAlgorithm] = ResolveSigningAlgorithm(context);
                headers[SignedRequestHeaders.KeyMaterialFormat] = ResolveKeyMaterialFormat(context);
            }
            else
            {
                if (ResolveSigningAlgorithm(context) != SignedRequestSignatureAlgorithms.HmacSha256) throw new InvalidOperationException("Runtime signed requests must use hmac-sha256.");
                if (ResolveKeyMaterialFormat(context) != SignedRequestKeyMaterialFormats.Raw) throw new InvalidOperationException("Runtime signed requests must use raw key material.");
                if (String.IsNullOrWhiteSpace(context.Key)) throw new ArgumentNullException(nameof(context.Key));
                if (String.IsNullOrWhiteSpace(context.OrganizationId)) throw new InvalidOperationException("OrganizationId is required for runtime signed requests.");
                if (String.IsNullOrWhiteSpace(context.UserId)) throw new InvalidOperationException("UserId is required for runtime signed requests.");
                if (String.IsNullOrWhiteSpace(context.InstanceId)) throw new InvalidOperationException("InstanceId is required for runtime signed requests.");
                if (String.IsNullOrWhiteSpace(context.Organization)) throw new InvalidOperationException("Organization is required for runtime signed requests.");
                if (String.IsNullOrWhiteSpace(context.User)) throw new InvalidOperationException("User is required for runtime signed requests.");
                if (String.IsNullOrWhiteSpace(context.Instance)) throw new InvalidOperationException("Instance is required for runtime signed requests.");

                headers[SignedRequestHeaders.OrganizationId] = context.OrganizationId;
                headers[SignedRequestHeaders.UserId] = context.UserId;
                headers[SignedRequestHeaders.InstanceId] = context.InstanceId;
                headers[SignedRequestHeaders.Organization] = context.Organization;
                headers[SignedRequestHeaders.User] = context.User;
                headers[SignedRequestHeaders.Instance] = context.Instance;
            }

            var bodySha256 = context.BodySha256;
            if (String.IsNullOrWhiteSpace(bodySha256) && context.Body != null)
            {
                bodySha256 = SignedRequestBodyHasher.ComputeSha256Base64(context.Body);
            }

            if (context.Profile == SignedRequestCanonicalProfile.ServiceHttpV1 || context.Profile == SignedRequestCanonicalProfile.RuntimeInstanceHttpV1)
            {
                if (String.IsNullOrWhiteSpace(context.Method)) throw new InvalidOperationException("Method is required for HTTP signed request profiles.");
                if (String.IsNullOrWhiteSpace(context.PathAndQuery)) throw new InvalidOperationException("PathAndQuery is required for HTTP signed request profiles.");
                if (String.IsNullOrWhiteSpace(bodySha256)) throw new InvalidOperationException("BodySha256 or Body is required for HTTP signed request profiles.");

                headers[SignedRequestHeaders.BodySha256] = bodySha256;
            }

            _signer.Sign(new SignedRequestSigningContext
            {
                Profile = context.Profile,
                Headers = headers,
                Key = context.Key,
                PrivateKeyMaterial = context.PrivateKeyMaterial,
                SigningKeyId = context.SigningKeyId,
                SignatureAlgorithm = ResolveSigningAlgorithm(context),
                KeyMaterialFormat = ResolveKeyMaterialFormat(context),
                Method = context.Method,
                PathAndQuery = context.PathAndQuery,
                BodySha256 = bodySha256
            });

            return headers;
        }

        private static string ResolveSigningAlgorithm(SignedRequestHeaderBuildContext context)
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

        private static string ResolveKeyMaterialFormat(SignedRequestHeaderBuildContext context)
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
