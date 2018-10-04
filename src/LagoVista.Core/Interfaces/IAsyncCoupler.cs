using System;
using System.Threading.Tasks;
using LagoVista.Core.Validation;

namespace LagoVista.Core.Interfaces
{
    public interface IAsyncCoupler
    {
        Task<InvokeResult> CompleteAsync<TResponseItem>(string correlationId, TResponseItem item);
        Task<InvokeResult<TResponseItem>> WaitOnAsync<TResponseItem>(string correlationId, TimeSpan timeout);
        IUsageMetrics GetAndResetReadMetrics(DateTime dateStamp, string hostVersion);
    }

    public interface IAsyncCoupler<TResponseItem>
    {
        Task<InvokeResult> CompleteAsync(string correlationId, TResponseItem item);
        Task<InvokeResult<TResponseItem>> WaitOnAsync(string correlationId, TimeSpan timeout);
        IUsageMetrics GetAndResetReadMetrics(DateTime dateStamp, string hostVersion);
    }
}