using System;

namespace LagoVista.Core.Security
{
    public static class SignedRequestKeyMaterialFormats
    {
        public const string Raw = "raw";
        public const string RsaXml = "rsa-xml";

        public static string Normalize(string format)
        {
            if (String.IsNullOrWhiteSpace(format))
            {
                return String.Empty;
            }

            return format.Trim().ToLowerInvariant();
        }
    }
}
