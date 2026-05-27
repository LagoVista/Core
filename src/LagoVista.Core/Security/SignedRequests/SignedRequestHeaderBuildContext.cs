using System;

namespace LagoVista.Core.Security
{
    public class SignedRequestHeaderBuildContext
    {
        public SignedRequestCanonicalProfile Profile { get; set; }
        public string Key { get; set; }
        public string PrivateKeyMaterial { get; set; }
        public string SigningKeyId { get; set; }
        public string SignatureAlgorithm { get; set; }
        public string KeyMaterialFormat { get; set; }
        public string RequestId { get; set; }
        public DateTimeOffset DateUtc { get; set; }
        public string Version { get; set; }
        public string AppKey { get; set; }
        public string OrganizationId { get; set; }
        public string Organization { get; set; }
        public string UserId { get; set; }
        public string User { get; set; }
        public string InstanceId { get; set; }
        public string Instance { get; set; }
        public string Method { get; set; }
        public string PathAndQuery { get; set; }
        public string BodySha256 { get; set; }
        public byte[] Body { get; set; }

        public SignedRequestHeaderBuildContext()
        {
            Key = String.Empty;
            PrivateKeyMaterial = String.Empty;
            SigningKeyId = String.Empty;
            SignatureAlgorithm = String.Empty;
            KeyMaterialFormat = String.Empty;
            RequestId = String.Empty;
            DateUtc = DateTimeOffset.UtcNow;
            Version = "1";
            AppKey = String.Empty;
            OrganizationId = String.Empty;
            Organization = String.Empty;
            UserId = String.Empty;
            User = String.Empty;
            InstanceId = String.Empty;
            Instance = String.Empty;
            Method = String.Empty;
            PathAndQuery = String.Empty;
            BodySha256 = String.Empty;
        }
    }
}
