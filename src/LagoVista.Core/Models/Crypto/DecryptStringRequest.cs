using LagoVista.Core.Models;

namespace LagoVista.Core.Models.Crypto
{
    /// <summary>
    /// Mapper -> encryption boundary packet for decrypting a wrapped/envelope string.
    /// All string fields are required unless otherwise noted.
    /// </summary>
    public sealed class DecryptStringRequest
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
        /// Wrapped/envelope string stored in the DB column.
        /// Must be the modern format (enc;v=2;...).
        /// </summary>
        public string Envelope { get; set; }

        public EntityHeader Org { get; set; }
        public EntityHeader User { get; set; }  
    }
}
