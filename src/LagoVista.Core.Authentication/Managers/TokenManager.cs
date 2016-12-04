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
