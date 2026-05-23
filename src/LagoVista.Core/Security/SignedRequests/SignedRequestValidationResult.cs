using System;

namespace LagoVista.Core.Security
{
    public class SignedRequestValidationResult
    {
        public bool Successful { get; set; }
        public string RequestId { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string MatchedKeyName { get; set; }

        public SignedRequestValidationResult()
        {
            RequestId = String.Empty;
            ErrorCode = String.Empty;
            ErrorMessage = String.Empty;
            MatchedKeyName = String.Empty;
        }

        public static SignedRequestValidationResult Success(string requestId, string matchedKeyName)
        {
            return new SignedRequestValidationResult
            {
                Successful = true,
                RequestId = requestId,
                MatchedKeyName = matchedKeyName
            };
        }

        public static SignedRequestValidationResult FromError(string errorCode, string errorMessage)
        {
            return new SignedRequestValidationResult
            {
                Successful = false,
                ErrorCode = errorCode,
                ErrorMessage = errorMessage
            };
        }
    }
}
