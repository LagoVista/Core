using System;

namespace LagoVista.Core.Security
{
    public class SignedRequestAuthorization
    {
        public string Scheme { get; set; }
        public string RequestId { get; set; }
        public string Signature { get; set; }

        public SignedRequestAuthorization()
        {
            Scheme = String.Empty;
            RequestId = String.Empty;
            Signature = String.Empty;
        }

        public static SignedRequestAuthorization Parse(string authorizationHeader)
        {
            if (String.IsNullOrWhiteSpace(authorizationHeader))
            {
                throw new InvalidOperationException("Authorization header is required.");
            }

            var trimmed = authorizationHeader.Trim();
            var firstSpaceIndex = trimmed.IndexOf(' ');
            if (firstSpaceIndex <= 0 || firstSpaceIndex >= trimmed.Length - 1)
            {
                throw new InvalidOperationException("Authorization header must be in the format 'SAS {requestId}:{signature}'.");
            }

            var scheme = trimmed.Substring(0, firstSpaceIndex);
            var credentials = trimmed.Substring(firstSpaceIndex + 1);
            var separatorIndex = credentials.IndexOf(':');
            if (separatorIndex <= 0 || separatorIndex >= credentials.Length - 1)
            {
                throw new InvalidOperationException("Authorization credentials must be in the format '{requestId}:{signature}'.");
            }

            return new SignedRequestAuthorization
            {
                Scheme = scheme,
                RequestId = credentials.Substring(0, separatorIndex),
                Signature = credentials.Substring(separatorIndex + 1)
            };
        }

        public static string CreateHeader(string requestId, string signature)
        {
            if (String.IsNullOrWhiteSpace(requestId)) throw new ArgumentNullException(nameof(requestId));
            if (String.IsNullOrWhiteSpace(signature)) throw new ArgumentNullException(nameof(signature));

            return $"SAS {requestId}:{signature}";
        }
    }
}
