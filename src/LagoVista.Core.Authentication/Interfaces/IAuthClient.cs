using System.Threading;
using System.Threading.Tasks;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Networking.Interfaces;
using LagoVista.Core.Authentication.Models;
using LagoVista.Core.Networking.Models;
using LagoVista.Core.Validation;

namespace LagoVista.Core.Authentication.Interfaces
{
    public interface IAuthClient
    {
        Task<InvokeResult<AuthResponse>> LoginAsync(AuthRequest loginInfo, CancellationTokenSource cancellationTokenSource = null);
        Task<InvokeResult> ResetPasswordAsync(string emailAddress, CancellationTokenSource cancellationTokenSource = null);
    }
}