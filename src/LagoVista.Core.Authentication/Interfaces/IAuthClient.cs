using System.Threading;
using System.Threading.Tasks;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Networking.Interfaces;
using LagoVista.Core.Authentication.Models;
using LagoVista.Core.Networking.Models;

namespace LagoVista.Core.Authentication.Interfaces
{
    public interface IAuthClient
    {
        Task<APIResponse<AuthResponse>> LoginAsync(AuthRequest loginInfo, CancellationTokenSource cancellationTokenSource = null);
        Task<APIResponse> ResetPasswordAsync(string emailAddress, CancellationTokenSource cancellationTokenSource = null);
    }
}