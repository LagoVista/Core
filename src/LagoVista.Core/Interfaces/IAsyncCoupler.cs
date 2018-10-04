using System;
using System.Threading.Tasks;
using LagoVista.Core.Validation;

namespace LagoVista.Core.Interfaces
{
    public interface IAsyncCoupler
    {
        Task<InvokeResult> CompleteAsync<TAsyncResult>(string correlationId, TAsyncResult item);

        Task<InvokeResult<TAsyncResult>> WaitOnAsync<TAsyncResult>(string correlationId, TimeSpan timeout);
        Task<InvokeResult<TAsyncResult>> WaitOnAsync<TAsyncResult>(Action action, string correlationId, TimeSpan timeout);
        Task<InvokeResult<TAsyncResult>> WaitOnAsync<TAsyncResult>(Func<Task> function, string correlationId, TimeSpan timeout);

        IUsageMetrics GetAndResetReadMetrics(DateTime dateStamp, string hostVersion);
    }

    public interface IAsyncCoupler<TAsyncResult>
    {
        Task<InvokeResult> CompleteAsync(string correlationId, TAsyncResult item);

        Task<InvokeResult<TAsyncResult>> WaitOnAsync(string correlationId, TimeSpan timeout);
        Task<InvokeResult<TAsyncResult>> WaitOnAsync(Action action, string correlationId, TimeSpan timeout);
        Task<InvokeResult<TAsyncResult>> WaitOnAsync(Func<Task> function, string correlationId, TimeSpan timeout);

        IUsageMetrics GetAndResetReadMetrics(DateTime dateStamp, string hostVersion);
    }
}