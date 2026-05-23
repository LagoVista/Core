using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace LagoVista.Core.Security
{
    public class SignedRequestSigner
    {
        public string Sign(SignedRequestSigningContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (context.Headers == null) throw new ArgumentNullException(nameof(context.Headers));
            if (String.IsNullOrWhiteSpace(context.Key)) throw new ArgumentNullException(nameof(context.Key));

            var headers = new Dictionary<string, string>(context.Headers, StringComparer.OrdinalIgnoreCase);
            var requestId = SignedRequestHeaders.GetRequestId(headers, context.Profile);
            var canonicalContext = new SignedRequestCanonicalContext
            {
                Profile = context.Profile,
                Headers = headers,
                Method = context.Method,
                PathAndQuery = context.PathAndQuery,
                BodySha256 = context.BodySha256
            };

            var canonicalSource = SignedRequestCanonicalizer.Build(canonicalContext);
            var signature = ComputeSignature(context.Key, canonicalSource);
            var authorizationHeader = SignedRequestAuthorization.CreateHeader(requestId, signature);
            context.Headers[SignedRequestHeaders.Authorization] = authorizationHeader;
            return authorizationHeader;
        }

        public static string ComputeSignature(string key, string canonicalSource)
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
    }
}
