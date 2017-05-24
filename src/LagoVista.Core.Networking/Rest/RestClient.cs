using System;
using System.Threading.Tasks;
using LagoVista.Core.Models;
using System.Net.Http;
using System.Threading;
using System.Net.Http.Headers;
using LagoVista.Core.Networking.Models;
using LagoVista.Core.Networking.Interfaces;
using LagoVista.Core.PlatformSupport;
using LagoVista.Core.Interfaces;

namespace LagoVista.Core.Networking.Rest
{
    public class RestClient : IRestClient
    {
        HttpClient _httpClient;
        IAuthManager _authManager;
        ITokenManager _tokenManager;
        ILogger _logger;
        INetworkService _networkserice;

        public RestClient(HttpClient httpClient, IAuthManager authManager, ITokenManager tokenManager, ILogger logger, INetworkService networkService)
        {
            _networkserice = networkService;
            _logger = logger;
            _httpClient = httpClient;
            _authManager = authManager;
            _tokenManager = tokenManager;
        }

        public async Task<APIResponse> DeleteAsync<TModel>(string path, TModel model, CancellationTokenSource cancellationTokenSource = null) where TModel : ModelBase
        {
            if(!await _tokenManager.ValidateTokenAsync(_authManager, cancellationTokenSource))
            {
                return APIResponse.CreateFailed(System.Net.HttpStatusCode.Unauthorized);
            }

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authManager.AuthToken);

            try
            {
                var response = await _httpClient.DeleteAsync(path, cancellationTokenSource.Token);
                return (response.IsSuccessStatusCode) ? APIResponse.CreateOK() : APIResponse.CreateFailed(response.StatusCode);
            }
            catch(Exception ex)
            {
                return APIResponse.CreateForException(ex);
            }
        }

        public Task<APIResponse> GetAsync<TModel>(string path, TModel model, CancellationTokenSource cancellationTokenSource = null) where TModel : ModelBase
        {
            throw new NotImplementedException();
        }

        public Task<APIResponse<TResponse>> GetAsync<TModel, TResponse>(string path, TModel model, CancellationTokenSource cancellationTokenSource = null) where TModel : ModelBase where TResponse : ModelBase
        {
            throw new NotImplementedException();
        }

        public Task<APIResponse> PostAsync<TModel>(string path, TModel model, CancellationTokenSource cancellationTokenSource = null) where TModel : ModelBase
        {
            throw new NotImplementedException();
        }

        public Task<APIResponse<TResponse>> PostAsync<TModel, TResponse>(string path, TModel model, CancellationTokenSource cancellationTokenSource = null) where TModel : ModelBase where TResponse : ModelBase
        {
            throw new NotImplementedException();
        }

        public Task<APIResponse> PutAsync<TModel>(string path, TModel model, CancellationTokenSource cancellationTokenSource = null) where TModel : ModelBase
        {
            throw new NotImplementedException();
        }

        public Task<APIResponse<TResponse>> PutAsync<TModel, TResponse>(string path, TModel model, CancellationTokenSource cancellationTokenSource = null) where TModel : ModelBase where TResponse : ModelBase
        {
            throw new NotImplementedException();
        }
    }
}
