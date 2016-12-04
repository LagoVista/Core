using LagoVista.Core.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Core.Networking.Interfaces
{
    public interface IRestClient
    {
        Task<IAPIResponse> PostAsync<TModel>(String path, TModel model, CancellationTokenSource cancellationTokenSource = null) where TModel : ModelBase;
        Task<IAPIResponse<TResponse>> PostAsync<TModel, TResponse>(String path, TModel model, CancellationTokenSource cancellationTokenSource = null) where TModel : ModelBase where TResponse : ModelBase;
        Task<IAPIResponse> GetAsync<TModel>(String path, TModel model, CancellationTokenSource cancellationTokenSource = null) where TModel : ModelBase;
        Task<IAPIResponse<TResponse>> GetAsync<TModel, TResponse>(String path, TModel model, CancellationTokenSource cancellationTokenSource = null) where TModel : ModelBase where TResponse : ModelBase;
        Task<IAPIResponse> PutAsync<TModel>(String path, TModel model, CancellationTokenSource cancellationTokenSource = null) where TModel : ModelBase;
        Task<IAPIResponse<TResponse>> PutAsync<TModel, TResponse>(String path, TModel model, CancellationTokenSource cancellationTokenSource = null) where TModel : ModelBase where TResponse : ModelBase;
        Task<IAPIResponse> DeleteAsync<TModel>(String path, TModel model, CancellationTokenSource cancellationTokenSource = null) where TModel : ModelBase;
    }
}
