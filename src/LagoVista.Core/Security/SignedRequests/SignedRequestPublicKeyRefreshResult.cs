using System;

namespace LagoVista.Core.Security
{
    public class SignedRequestPublicKeyRefreshResult
    {
        public bool Successful { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string Version { get; set; }
        public DateTimeOffset RefreshedUtc { get; set; }

        public SignedRequestPublicKeyRefreshResult()
        {
            ErrorCode = String.Empty;
            ErrorMessage = String.Empty;
            Version = String.Empty;
            RefreshedUtc = DateTimeOffset.MinValue;
        }

        public static SignedRequestPublicKeyRefreshResult Success(string version)
        {
            return new SignedRequestPublicKeyRefreshResult
            {
                Successful = true,
                Version = version ?? String.Empty,
                RefreshedUtc = DateTimeOffset.UtcNow
            };
        }

        public static SignedRequestPublicKeyRefreshResult FromError(string errorCode, string errorMessage)
        {
            return new SignedRequestPublicKeyRefreshResult
            {
                Successful = false,
                ErrorCode = errorCode ?? String.Empty,
                ErrorMessage = errorMessage ?? String.Empty,
                RefreshedUtc = DateTimeOffset.UtcNow
            };
        }
    }
}
