using System;
using System.Collections.Generic;

namespace LagoVista.Core.Security
{
    public static class SignedRequestHeaders
    {
        public const string Authorization = "Authorization";
        public const string RequestId = "X-Nuviot-Request-Id";
        public const string RuntimeRequestId = "X-Nuviot-Runtime-Request-Id";
        public const string CallerId = "X-Nuviot-Caller-Id";
        public const string SigningKeyId = "X-Nuviot-Signing-Key-Id";
        public const string SignatureAlgorithm = "X-Nuviot-Signature-Algorithm";
        public const string KeyMaterialFormat = "X-Nuviot-Key-Material-Format";
        public const string Date = "X-Nuviot-Date";
        public const string Version = "X-Nuviot-Version";
        public const string BodySha256 = "X-Nuviot-Body-Sha256";
        public const string OrganizationId = "X-Nuviot-Orgid";
        public const string Organization = "X-Nuviot-Org";
        public const string UserId = "X-Nuviot-Userid";
        public const string User = "X-Nuviot-User";
        public const string InstanceId = "X-Nuviot-Instanceid";
        public const string Instance = "X-Nuviot-Instance";

        public static Dictionary<string, string> Normalize(IReadOnlyDictionary<string, string> headers)
        {
            if (headers == null) throw new ArgumentNullException(nameof(headers));

            var normalized = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var header in headers)
            {
                normalized[header.Key] = header.Value;
            }

            return normalized;
        }

        public static string GetRequired(IReadOnlyDictionary<string, string> headers, string headerName)
        {
            if (headers == null) throw new ArgumentNullException(nameof(headers));
            if (String.IsNullOrWhiteSpace(headerName)) throw new ArgumentNullException(nameof(headerName));

            var normalized = Normalize(headers);
            if (!normalized.TryGetValue(headerName, out var value) || String.IsNullOrWhiteSpace(value))
            {
                throw new InvalidOperationException($"Missing required signed request header: {headerName}");
            }

            return value;
        }

        public static string GetOptional(IReadOnlyDictionary<string, string> headers, string headerName)
        {
            if (headers == null) throw new ArgumentNullException(nameof(headers));
            if (String.IsNullOrWhiteSpace(headerName)) throw new ArgumentNullException(nameof(headerName));

            var normalized = Normalize(headers);
            if (!normalized.TryGetValue(headerName, out var value))
            {
                return String.Empty;
            }

            return value ?? String.Empty;
        }

        public static string GetRequestId(IReadOnlyDictionary<string, string> headers, SignedRequestCanonicalProfile profile)
        {
            if (profile == SignedRequestCanonicalProfile.RuntimeInstanceV1 || profile == SignedRequestCanonicalProfile.RuntimeInstanceHttpV1)
            {
                var runtimeRequestId = GetOptional(headers, RuntimeRequestId);
                if (!String.IsNullOrWhiteSpace(runtimeRequestId))
                {
                    return runtimeRequestId;
                }
            }

            return GetRequired(headers, RequestId);
        }

        public static void SetRequestId(IDictionary<string, string> headers, SignedRequestCanonicalProfile profile, string requestId)
        {
            if (headers == null) throw new ArgumentNullException(nameof(headers));
            if (String.IsNullOrWhiteSpace(requestId)) throw new ArgumentNullException(nameof(requestId));

            if (profile == SignedRequestCanonicalProfile.RuntimeInstanceV1 || profile == SignedRequestCanonicalProfile.RuntimeInstanceHttpV1)
            {
                headers[RuntimeRequestId] = requestId;
            }
            else
            {
                headers[RequestId] = requestId;
            }
        }
    }
}
