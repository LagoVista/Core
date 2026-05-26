using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Core.Security
{
    public interface ISignedRequestPublicKeySetClient
    {
        Task<SignedRequestPublicKeySet> FetchAsync(CancellationToken cancellationToken);
    }
}
