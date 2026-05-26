using System;
using System.Collections.Generic;

namespace LagoVista.Core.Security
{
    public class SignedRequestSigningContext
    {
        public SignedRequestCanonicalProfile Profile { get; set; }
        public IDictionary<string, string> Headers { get; set; }
        public string Key { get; set; }
        public string PrivateKeyMaterial { get; set; }
        public string SigningKeyId { get; set; }
        public string SignatureAlgorithm { get; set; }
        public string KeyMaterialFormat { get; set; }
        public string Method { get; set; }
        public string PathAndQuery { get; set; }
        public string BodySha256 { get; set; }

        public SignedRequestSigningContext()
        {
            Headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            Key = String.Empty;
            PrivateKeyMaterial = String.Empty;
            SigningKeyId = String.Empty;
            SignatureAlgorithm = String.Empty;
            KeyMaterialFormat = String.Empty;
            Method = String.Empty;
            PathAndQuery = String.Empty;
            BodySha256 = String.Empty;
        }
    }
}
