using LagoVista.Core.Models.Crypto;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Core.Interfaces
{
    public interface IModernEncryption
    {
        /// <summary>
        /// Encrypts plaintext (UTF-8) into a wrapped/envelope string suitable for storage in a single string column.
        /// </summary>
        Task<string> EncryptAsync(EncryptStringRequest request, CancellationToken ct = default);

        /// <summary>
        /// Decrypts a wrapped/envelope string into plaintext (UTF-8).
        /// </summary>
        Task<string> DecryptAsync(DecryptStringRequest request, CancellationToken ct = default);
    }
}
