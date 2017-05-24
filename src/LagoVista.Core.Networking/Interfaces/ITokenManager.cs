using LagoVista.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Core.Networking.Interfaces
{
    public interface ITokenManager
    {
        Task<bool> ValidateTokenAsync(IAuthManager authManager, CancellationTokenSource cancellationTokenSource);
    }
}
