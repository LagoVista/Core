using System;

namespace LagoVista.Core.Security
{
    public class SignedRequestPublicKeyEntry
    {
        public string CallerId { get; set; }
        public string KeyId { get; set; }
        public string Algorithm { get; set; }
        public string KeyMaterialFormat { get; set; }
        public string PublicKeyMaterial { get; set; }
        public string Status { get; set; }

        public SignedRequestPublicKeyEntry()
        {
            CallerId = String.Empty;
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
                CallerId = CallerId,
                KeyId = KeyId,
                Algorithm = Algorithm,
                KeyMaterialFormat = KeyMaterialFormat,
                PublicKeyMaterial = PublicKeyMaterial,
                Status = Status
            };
        }
    }
}
