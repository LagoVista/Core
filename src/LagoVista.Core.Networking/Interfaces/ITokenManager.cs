// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 564aab538b74e4baa306b2f498afb9c0206974053af1e5da477c31a8ac6db05f
// IndexVersion: 2
// --- END CODE INDEX META ---
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
