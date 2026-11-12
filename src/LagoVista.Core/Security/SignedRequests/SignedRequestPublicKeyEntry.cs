using System;

namespace LagoVista.Core.Security
{
    public class SignedRequestPublicKeyEntry
    {
        public string AppKey { get; set; }
        // Set by the receiving client from the authenticated key-set source.
        // The signer never gets to select this value in its request.
        public string SourceEnvironment { get; set; }
        public string KeyId { get; set; }
        public string Algorithm { get; set; }
        public string KeyMaterialFormat { get; set; }
        public string PublicKeyMaterial { get; set; }
        public string Status { get; set; }

        public SignedRequestPublicKeyEntry()
        {
            AppKey = String.Empty;
            KeyId = String.Empty;
            Algorithm = SignedRequestSignatureAlgorithms.RsaPssSha256;
            KeyMaterialFormat = SignedRequestKeyMaterialFormats.RsaXml;
            PublicKeyMaterial = String.Empty;
            Status = SignedRequestValidationKeyStatuses.Active;
        }

        public SignedRequestValidationKey ToValidationKey()
        {
            return new SignedRequestValidationKey
            {
                AppKey = AppKey,
                KeyId = KeyId,
                Algorithm = Algorithm,
                KeyMaterialFormat = KeyMaterialFormat,
                PublicKeyMaterial = PublicKeyMaterial,
                Status = Status
            };
        }
    }
}
