using LagoVista.Core.Authentication.Interfaces;
using LagoVista.Core.Authentication.Models;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Networking.Interfaces;
using LagoVista.Core.Networking.Models;
using LagoVista.Core.PlatformSupport;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Core.Authentication.Rest
{
    public class AuthClient : IAuthClient
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

        public async Task<IAPIResponse<ILoginResponse>> LoginAsync(IRemoteLoginModel loginInfo, CancellationTokenSource cancellationTokenSource = null)
        {
            if(cancellationTokenSource == null)
            {
                cancellationTokenSource = new CancellationTokenSource();
            }

            var formContent = new Dictionary<string, string>
                {
                    {"grant_type", loginInfo.GrantType},
                    {"refresh_token", loginInfo.GrantType},
                    {"username", loginInfo.Email},
                    {"password", loginInfo.Password},
                };


            var postContent = new FormUrlEncodedContent(formContent);
            postContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

            _httpClient.DefaultRequestHeaders.Clear();

            try
            {
                var response = await _httpClient.PostAsync(PATH, postContent, cancellationTokenSource.Token);
                if (response.IsSuccessStatusCode)
                {
                    var responseContents = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<AuthResponse>(responseContents);
                    return new APIResponse<ILoginResponse>(result);
                }
                else
                {
                    return APIResponse<ILoginResponse>.FromFailedStatusCode(response.StatusCode);
                }
            }
            catch(Exception ex)
            {
                var json = JsonConvert.SerializeObject(formContent);
                _logger.LogException("AuthClient_LoginAsync", ex, new System.Collections.Generic.KeyValuePair<string, string>("json", json));
                return APIResponse<ILoginResponse>.FromException(ex);
            }
        }

        public Task<IAPIResponse> ResetPasswordAsync(String emailAddress, CancellationTokenSource cancellationTokenSource = null)
        {
            if (cancellationTokenSource == null)
            {
                cancellationTokenSource = new CancellationTokenSource();
            }

            return Task.FromResult<IAPIResponse>(null);
        }

    }
}
