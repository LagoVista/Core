using System;
using System.Collections.Generic;

namespace LagoVista.Core.Security
{
    public class SignedRequestCanonicalContext
    {
        public SignedRequestCanonicalProfile Profile { get; set; }
        public IReadOnlyDictionary<string, string> Headers { get; set; }
        public string Method { get; set; }
        public string PathAndQuery { get; set; }
        public string BodySha256 { get; set; }

        public SignedRequestCanonicalContext()
        {
            Headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            Method = String.Empty;
            PathAndQuery = String.Empty;
            BodySha256 = String.Empty;
        }
    }
}
