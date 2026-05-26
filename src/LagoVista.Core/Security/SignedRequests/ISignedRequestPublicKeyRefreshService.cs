using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Core.Security
{
    public interface ISignedRequestPublicKeyRefreshService
    {
        Task<SignedRequestPublicKeyRefreshResult> RefreshAsync(CancellationToken cancellationToken);
    }
}
