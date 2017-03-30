using LagoVista.Core.Models;
using LagoVista.Core.Networking.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Core.Networking.Interfaces
{
    public interface IRestClient
    {
        Task<APIResponse> PostAsync<TModel>(String path, TModel model, CancellationTokenSource cancellationTokenSource = null) where TModel : ModelBase;
        Task<APIResponse<TResponse>> PostAsync<TModel, TResponse>(String path, TModel model, CancellationTokenSource cancellationTokenSource = null) where TModel : ModelBase where TResponse : ModelBase;
        Task<APIResponse> GetAsync<TModel>(String path, TModel model, CancellationTokenSource cancellationTokenSource = null) where TModel : ModelBase;
        Task<APIResponse<TResponse>> GetAsync<TModel, TResponse>(String path, TModel model, CancellationTokenSource cancellationTokenSource = null) where TModel : ModelBase where TResponse : ModelBase;
        Task<APIResponse> PutAsync<TModel>(String path, TModel model, CancellationTokenSource cancellationTokenSource = null) where TModel : ModelBase;
        Task<APIResponse<TResponse>> PutAsync<TModel, TResponse>(String path, TModel model, CancellationTokenSource cancellationTokenSource = null) where TModel : ModelBase where TResponse : ModelBase;
        Task<APIResponse> DeleteAsync<TModel>(String path, TModel model, CancellationTokenSource cancellationTokenSource = null) where TModel : ModelBase;
    }
}
