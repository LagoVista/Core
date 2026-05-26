using System;

namespace LagoVista.Core.Security
{
    public static class SignedRequestValidationKeyStatuses
    {
        public const string Future = "future";
        public const string Active = "active";
        public const string Retiring = "retiring";
        public const string Revoked = "revoked";
        public const string Expired = "expired";

        public static bool CanValidate(string status)
        {
            return String.Equals(status, Active, StringComparison.OrdinalIgnoreCase) ||
                   String.Equals(status, Retiring, StringComparison.OrdinalIgnoreCase);
        }
    }
}
