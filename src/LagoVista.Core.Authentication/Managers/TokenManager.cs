// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: e5b31b8fc7837f186e67ca58b2d1604f6f3be5d0329243df13d6f0c03133e175
// IndexVersion: 1
// --- END CODE INDEX META ---
using LagoVista.Core.Interfaces;
using LagoVista.Core.Networking.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Core.Authentication.Managers
{
    public class TokenManager 
    {
        public Task<bool> ValidateTokenAsync(IAuthManager authManager, CancellationTokenSource cancellationTokenSource)
        {
            throw new NotImplementedException();
        }
    }
}
