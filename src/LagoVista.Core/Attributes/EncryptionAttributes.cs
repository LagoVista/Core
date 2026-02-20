using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Attributes
{
    /// <summary>
    /// Applied to the DTO type. Defines how to derive the secret id in KeyVault and where to get the scope id.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class EncryptionKeyAttribute : Attribute
    {
        /// <summary>
        /// Secret id template, e.g. "RateKey-{id}" or "ExpenseRecordKey-{orgId}-{id}".
        /// Supported tokens: {id}, {orgId}
        /// </summary>
        public string SecretIdFormat { get; }

        /// <summary>
        /// DTO property name used for {id}. Defaults to "Id".
        /// Example for transactions: "AccountId"
        /// </summary>
        public string IdProperty { get; set; } = "Id";

        /// <summary>
        /// If secret is missing, create it (your existing behavior). Default true.
        /// </summary>
        public bool CreateIfMissing { get; set; } = true;

        public EncryptionKeyAttribute(string secretIdFormat)
        {
            SecretIdFormat = secretIdFormat ?? throw new ArgumentNullException(nameof(secretIdFormat));
        }
    }

    /// <summary>
    /// Applied to domain plaintext properties. Points to ciphertext property on the DTO.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class EncryptedFieldAttribute : Attribute
    {
        public string CiphertextProperty { get; }
        public string SaltProperty { get; set; } = "Id"; // defaults to dto.Id
        public bool SkipIfEmpty { get; set; } = true;

        public EncryptedFieldAttribute(string ciphertextProperty)
        {
            CiphertextProperty = ciphertextProperty ?? throw new ArgumentNullException(nameof(ciphertextProperty));
        }
    }

    /// <summary>
    /// Applied to domain plaintext properties. Points to ciphertext property on the DTO.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class DecryptedFieldAttribute : Attribute
    {
        public string DecryptedValueProperty { get; }
        public string SaltProperty { get; set; } = "Id"; // defaults to dto.Id
        public bool SkipIfEmpty { get; set; } = true;

        public DecryptedFieldAttribute(string decryptedValueProperty)
        {
            DecryptedValueProperty = decryptedValueProperty ?? throw new ArgumentNullException(nameof(decryptedValueProperty));
        }
    }
}
