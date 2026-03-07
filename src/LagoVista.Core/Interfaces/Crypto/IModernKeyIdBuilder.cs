using LagoVista.Core.Attributes;
using LagoVista.Core.Models;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Core.Interfaces.Crypto
{
    /// <summary>
    /// Builds modern v2 KeyIds (e.g. "customer-&lt;id32&gt;:v2") for encryption/decryption.
    ///
    /// Resolution rules:
    /// - Try IdPath (supports dotted navigation paths)
    /// - If IdPath cannot be resolved and fallback is configured, use the configured FK property and resolver
    /// - Canonicalize Guids to 32-hex lowercase
    /// - Substitute tokens in KeyIdFormat
    /// </summary>
    public interface IModernKeyIdBuilder
    {
        Task<string> BuildKeyGuiIdAsync<TDto>(TDto dto, ModernKeyIdAttribute attr, EntityHeader org, EntityHeader user, CancellationToken ct = default) where TDto : class;
    }
}
