using System.Threading;
using System.Threading.Tasks;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Networking.Interfaces;

namespace LagoVista.Core.Authentication.Interfaces
{
    public interface IAuthClient
    {
        Task<IAPIResponse<ILoginResponse>> LoginAsync(IRemoteLoginModel loginInfo, CancellationTokenSource cancellationTokenSource = null);
        Task<IAPIResponse> ResetPasswordAsync(string emailAddress, CancellationTokenSource cancellationTokenSource = null);
    }
}