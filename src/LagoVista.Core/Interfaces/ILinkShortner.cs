using LagoVista.Core.Validation;
using System.Threading.Tasks;

namespace LagoVista.Core.Interfaces
{
    public interface ILinkShortner
    {
        Task<InvokeResult<string>> ShortenLinkAsync(string url);
    }
}
