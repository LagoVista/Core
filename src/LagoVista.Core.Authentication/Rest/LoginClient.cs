using LagoVista.Core.Authentication.Interfaces;
using LagoVista.Core.Authentication.Models;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Networking.Interfaces;
using LagoVista.Core.Networking.Models;
using LagoVista.Core.PlatformSupport;
using LagoVista.Core.Validation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Core.Authentication.Rest
{
    /*public class AuthClient 
    {
        private const string PATH = "/api/v1/token";

        HttpClient _httpClient;
        ILogger _logger;
        INetworkService _networkService;

        public AuthClient(HttpClient httpClient, ILogger logger, INetworkService networkService)
        {
            _httpClient = httpClient;
            _logger = logger;
            _networkService = networkService;
        }

        public  Task<InvokeResult<AuthResponse>> LoginAsync(AuthRequest loginInfo, CancellationTokenSource cancellationTokenSource = null)
        {
            throw new NotImplementedException();
        }

        public Task<APIResponse> ResetPasswordAsync(String emailAddress, CancellationTokenSource cancellationTokenSource = null)
        {
            if (cancellationTokenSource == null)
            {
                cancellationTokenSource = new CancellationTokenSource();
            }

            return Task.FromResult<APIResponse>(null);
        }

    }*/
}
