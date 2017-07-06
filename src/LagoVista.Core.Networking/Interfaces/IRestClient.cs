using LagoVista.Core.Models;
using LagoVista.Core.Networking.Models;
using LagoVista.Core.Validation;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Core.Networking.Interfaces
{
    public interface IRestClient
    {
        Task<InvokeResult> PostAsync<TModel>(String path, TModel model, CancellationTokenSource cancellationTokenSource = null) where TModel : ModelBase;
        Task<InvokeResult<TResponse>> PostAsync<TModel, TResponse>(String path, TModel model, CancellationTokenSource cancellationTokenSource = null) where TModel : ModelBase where TResponse : ModelBase;
        Task<InvokeResult> GetAsync<TModel>(String path, TModel model, CancellationTokenSource cancellationTokenSource = null) where TModel : ModelBase;
        Task<InvokeResult<TResponse>> GetAsync<TModel, TResponse>(String path, TModel model, CancellationTokenSource cancellationTokenSource = null) where TModel : ModelBase where TResponse : ModelBase;
        Task<InvokeResult> PutAsync<TModel>(String path, TModel model, CancellationTokenSource cancellationTokenSource = null) where TModel : ModelBase;
        Task<InvokeResult<TResponse>> PutAsync<TModel, TResponse>(String path, TModel model, CancellationTokenSource cancellationTokenSource = null) where TModel : ModelBase where TResponse : ModelBase;
        Task<InvokeResult> DeleteAsync<TModel>(String path, TModel model, CancellationTokenSource cancellationTokenSource = null) where TModel : ModelBase;
    }
}
