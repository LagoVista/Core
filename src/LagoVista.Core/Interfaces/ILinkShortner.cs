// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 0fba31730b83aa9f49cf22f99f95937353ab08ad3eca394df599c58de788a279
// IndexVersion: 1
// --- END CODE INDEX META ---
using LagoVista.Core.Validation;
using System.Threading.Tasks;

namespace LagoVista.Core.Interfaces
{
    public interface ILinkShortener
    {
        Task<InvokeResult<string>> ShortenLinkAsync(string url, string expires = null);
        Task<InvokeResult<string>> RestoreLinkAsync(string partition, string link);
    }
}
