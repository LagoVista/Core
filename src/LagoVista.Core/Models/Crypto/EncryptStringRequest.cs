using LagoVista.Core.Models;

namespace LagoVista.Core.Models.Crypto
{
    /// <summary>
    /// Mapper -> encryption boundary packet for encrypting a string.
    /// All string fields are required unless otherwise noted.
    /// </summary>
    public sealed class EncryptStringRequest
    {
        public GuidString36 OrgId { get; set; }
        public GuidString36 RecId { get; set; }

        /// <summary>
        /// DTO property name lowercased (e.g. "encryptedbalance").
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// Key identifier (e.g. "account-<32hex>:v2").
        /// </summary>
        public string KeyId { get; set; }

        /// <summary>
        /// Key material version (global kv). For now, typically 1.
        /// </summary>
        public int Kv { get; set; }

        /// <summary>
        /// Plaintext string to encrypt.
        /// </summary>
        public string Plaintext { get; set; }
    
        public EntityHeader Org { get; set; }
        public EntityHeader User { get; set; }
    }
}
