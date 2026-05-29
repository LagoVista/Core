using System;
using System.Collections.Generic;
using System.Globalization;

namespace LagoVista.Core.Security
{
    public class SignedRequestValidator
    {
        public SignedRequestValidationResult Validate(SignedRequestValidationContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (context.Headers == null) throw new ArgumentNullException(nameof(context.Headers));

            try
            {
                var headers = SignedRequestHeaders.Normalize(context.Headers);
                var requestId = SignedRequestHeaders.GetRequestId(headers, context.Profile);
                var authorizationHeader = SignedRequestHeaders.GetRequired(headers, SignedRequestHeaders.Authorization);
                var authorization = SignedRequestAuthorization.Parse(authorizationHeader);

                if (!String.Equals(authorization.Scheme, "SAS", StringComparison.OrdinalIgnoreCase))
                {
                    return SignedRequestValidationResult.FromError("invalid_authorization_scheme", "Authorization scheme must be SAS.");
                }

                if (!String.Equals(authorization.RequestId, requestId, StringComparison.Ordinal))
                {
                    return SignedRequestValidationResult.FromError("request_id_mismatch", "Authorization request id does not match signed request header.");
                }

                if (context.ValidateTimestamp)
                {
                    var timestampResult = ValidateTimestamp(headers, context.MaxClockSkew);
                    if (!timestampResult.Successful)
                    {
                        return timestampResult;
                    }
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

                if (context.Profile == SignedRequestCanonicalProfile.ServiceHttpV1)
                {
                    return ValidateServiceRequest(context, headers, requestId, canonicalSource, authorization.Signature);
                }

                return ValidateRuntimeRequest(context, requestId, canonicalSource, authorization.Signature);
            }
            catch (Exception ex)
            {
                return SignedRequestValidationResult.FromError("signed_request_validation_failed", ex.Message);
            }
        }

        private static SignedRequestValidationResult ValidateServiceRequest(SignedRequestValidationContext context, IReadOnlyDictionary<string, string> headers, string requestId, string canonicalSource, string signature)
        {
            if (context.ValidationKeyResolver == null)
            {
                return SignedRequestValidationResult.FromError("missing_validation_key_resolver", "ValidationKeyResolver is required for ServiceHttpV1 signed requests.");
            }

            var callerId = SignedRequestHeaders.GetRequired(headers, SignedRequestHeaders.AppKey);
            var keyId = SignedRequestHeaders.GetRequired(headers, SignedRequestHeaders.SigningKeyId);
            var algorithm = SignedRequestSignatureAlgorithms.Normalize(SignedRequestHeaders.GetRequired(headers, SignedRequestHeaders.SignatureAlgorithm));
            var keyMaterialFormat = SignedRequestKeyMaterialFormats.Normalize(SignedRequestHeaders.GetRequired(headers, SignedRequestHeaders.KeyMaterialFormat));

            if (algorithm != SignedRequestSignatureAlgorithms.RsaPssSha256)
            {
                return SignedRequestValidationResult.FromError("invalid_signature_algorithm", "ServiceHttpV1 signed requests must use rsa-pss-sha256.");
            }

            if (keyMaterialFormat != SignedRequestKeyMaterialFormats.RsaXml)
            {
                return SignedRequestValidationResult.FromError("invalid_key_material_format", "ServiceHttpV1 signed requests must use rsa-xml key material.");
            }

            var validationKey = context.ValidationKeyResolver.Resolve(callerId, keyId, algorithm);
            if (validationKey == null)
            {
                return SignedRequestValidationResult.FromError("validation_key_not_found", "Signed request validation key could not be resolved.");
            }

            if (!String.Equals(validationKey.KeyId, keyId, StringComparison.OrdinalIgnoreCase))
            {
                return SignedRequestValidationResult.FromError("validation_key_id_mismatch", "Signed request validation key id does not match the request key id.");
            }

            if (!String.Equals(SignedRequestSignatureAlgorithms.Normalize(validationKey.Algorithm), algorithm, StringComparison.OrdinalIgnoreCase))
            {
                return SignedRequestValidationResult.FromError("validation_key_algorithm_mismatch", "Signed request validation key algorithm does not match the request algorithm.");
            }

            if (!String.Equals(SignedRequestKeyMaterialFormats.Normalize(validationKey.KeyMaterialFormat), keyMaterialFormat, StringComparison.OrdinalIgnoreCase))
            {
                return SignedRequestValidationResult.FromError("validation_key_material_format_mismatch", "Signed request validation key material format does not match the request key material format.");
            }

            if (!SignedRequestValidationKeyStatuses.CanValidate(validationKey.Status))
            {
                return SignedRequestValidationResult.FromError("validation_key_not_active", $"Signed request validation key status '{validationKey.Status}' cannot validate requests.");
            }

            if (SignedRequestCrypto.Verify(algorithm, keyMaterialFormat, validationKey.PublicKeyMaterial, canonicalSource, signature))
            {
                return SignedRequestValidationResult.Success(requestId, "PublicKey", keyId, algorithm);
            }

            return SignedRequestValidationResult.FromError("invalid_signature", "Signed request signature could not be validated.");
        }

        private static SignedRequestValidationResult ValidateRuntimeRequest(SignedRequestValidationContext context, string requestId, string canonicalSource, string signature)
        {
            if (ValidateWithKey(context.Key1, canonicalSource, signature))
            {
                return SignedRequestValidationResult.Success(requestId, "Key1", String.Empty, SignedRequestSignatureAlgorithms.HmacSha256);
            }

            if (ValidateWithKey(context.Key2, canonicalSource, signature))
            {
                return SignedRequestValidationResult.Success(requestId, "Key2", String.Empty, SignedRequestSignatureAlgorithms.HmacSha256);
            }

            return SignedRequestValidationResult.FromError("invalid_signature", "Signed request signature could not be validated.");
        }

        private static SignedRequestValidationResult ValidateTimestamp(IReadOnlyDictionary<string, string> headers, TimeSpan maxClockSkew)
        {
            var dateValue = SignedRequestHeaders.GetRequired(headers, SignedRequestHeaders.Date);

            if (!DateTimeOffset.TryParse(dateValue, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var signedUtc))
            {
                return SignedRequestValidationResult.FromError("invalid_timestamp", "Signed request timestamp could not be parsed.");
            }

            var now = DateTimeOffset.UtcNow;
            var delta = now - signedUtc;
            if (delta.Duration() > maxClockSkew)
            {
                return SignedRequestValidationResult.FromError("timestamp_out_of_range", $"Signed request timestamp is outside the allowed clock skew of {maxClockSkew.TotalSeconds} seconds.");
            }

            return SignedRequestValidationResult.Success(String.Empty, String.Empty);
        }

        private static bool ValidateWithKey(string key, string canonicalSource, string expectedSignature)
        {
            if (String.IsNullOrWhiteSpace(key))
            {
                return false;
            }

            if (String.IsNullOrWhiteSpace(expectedSignature))
            {
                return false;
            }

            return SignedRequestCrypto.Verify(SignedRequestSignatureAlgorithms.HmacSha256, SignedRequestKeyMaterialFormats.Raw, key, canonicalSource, expectedSignature);
        }
    }
}
