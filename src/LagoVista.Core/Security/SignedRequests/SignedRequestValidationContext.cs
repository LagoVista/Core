using System;
using System.Collections.Generic;

namespace LagoVista.Core.Security
{
    public class SignedRequestValidationContext
    {
        public SignedRequestCanonicalProfile Profile { get; set; }
        public IReadOnlyDictionary<string, string> Headers { get; set; }
        public string Key1 { get; set; }
        public string Key2 { get; set; }
        public string Method { get; set; }
        public string PathAndQuery { get; set; }
        public string BodySha256 { get; set; }
        public TimeSpan MaxClockSkew { get; set; }
        public bool ValidateTimestamp { get; set; }

        public SignedRequestValidationContext()
        {
            Headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            Key1 = String.Empty;
            Key2 = String.Empty;
            Method = String.Empty;
            PathAndQuery = String.Empty;
            BodySha256 = String.Empty;
            MaxClockSkew = TimeSpan.FromMinutes(5);
            ValidateTimestamp = true;
        }
    }
}
