using System;

namespace LagoVista.Core.Security
{
    public class SignedRequestPublicKeySetClientSettings
    {
        public string PublicKeySetUrl { get; set; }
        public string AppKey { get; set; }
        public string Environment { get; set; }
        public string AuthorizationToken { get; set; }

        public SignedRequestPublicKeySetClientSettings()
        {
            PublicKeySetUrl = String.Empty;
            AppKey = String.Empty;
            Environment = String.Empty;
            AuthorizationToken = String.Empty;
        }
    }
}
