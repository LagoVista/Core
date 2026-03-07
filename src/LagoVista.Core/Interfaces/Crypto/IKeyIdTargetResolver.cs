using LagoVista.Core.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Core.Interfaces.Crypto
{
    /// <summary>
    /// Resolves a Guid value for a target path using a foreign key.
    ///
    /// Intended for "missing navigation on write" scenarios where a DTO needs a parent-owned id
    /// (e.g., Invoice.CustomerId) but only has the foreign key (e.g., InvoiceId).
    ///
    /// Implementations may be domain-explicit and SHOULD throw when resolution is not supported.
    /// </summary>
    public interface IKeyIdTargetResolver
    {
        /// <summary>
        /// Resolve a Guid for the given targetPath using the provided fkValue.
        /// Example:
        /// - targetPath: "Invoice.CustomerId", fkValue: <InvoiceId>
        /// - targetPath: "Agreement.CustomerId", fkValue: <AgreementId>
        ///
        /// MUST throw if the targetPath is unsupported, fkValue is empty, or the target cannot be resolved.
        /// </summary>
        Task<Guid> ResolveIdAsync(string targetPath, Guid fkValue, EntityHeader org, EntityHeader user, CancellationToken ct = default);
    }
}
