using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

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

                if (ValidateWithKey(context.Key1, canonicalSource, authorization.Signature))
                {
                    return SignedRequestValidationResult.Success(requestId, "Key1");
                }

                if (ValidateWithKey(context.Key2, canonicalSource, authorization.Signature))
                {
                    return SignedRequestValidationResult.Success(requestId, "Key2");
                }

                return SignedRequestValidationResult.FromError("invalid_signature", "Signed request signature could not be validated.");
            }
            catch (Exception ex)
            {
                return SignedRequestValidationResult.FromError("signed_request_validation_failed", ex.Message);
            }
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

            var actualSignature = SignedRequestSigner.ComputeSignature(key, canonicalSource);
            var actualBytes = Encoding.UTF8.GetBytes(actualSignature);
            var expectedBytes = Encoding.UTF8.GetBytes(expectedSignature);

            return FixedTimeEquals(actualBytes, expectedBytes);
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
