using LagoVista.Core.Interfaces;
using LagoVista.Core.PlatformSupport;
using LagoVista.Core.Validation;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LagoVista.Core.Utils
{
    public sealed class WaitOnRequest<TResult>
    {
        public WaitOnRequest(string correlationId)
        {
            CompletionSource = new TaskCompletionSource<TResult>();
            CorrelationId = correlationId;
            Details = new List<string>();
        }

        public List<string> Details { get; private set; }

        public string CorrelationId { get; private set; }

        public DateTime Enqueued { get; private set; }

        public TaskCompletionSource<TResult> CompletionSource { get; private set; }
    }

    public class AsyncCoupler : IAsyncCoupler
    {
        protected ILogger Logger { get; }
        protected IUsageMetrics UsageMetrics { get; private set; }
        protected ConcurrentDictionary<string, WaitOnRequest<object>> Sessions { get; } = new ConcurrentDictionary<string, WaitOnRequest<object>>();

        public AsyncCoupler(ILogger logger, IUsageMetrics usageMetrics)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            UsageMetrics = usageMetrics ?? throw new ArgumentNullException(nameof(usageMetrics));
        }

        public IUsageMetrics GetAndResetReadMetrics(DateTime dateStamp, string hostVersion)
        {
            UsageMetrics.Version = hostVersion;
            lock (UsageMetrics)
            {
                var clonedMetrics = UsageMetrics.Clone();
                clonedMetrics.SetDatestamp(dateStamp);

                clonedMetrics.EndTimeStamp = dateStamp.ToJSONString();
                clonedMetrics.StartTimeStamp = UsageMetrics.StartTimeStamp;
                clonedMetrics.Status = "Running";

                clonedMetrics.Calculate();

                UsageMetrics.Reset(clonedMetrics.EndTimeStamp);
                return clonedMetrics;
            }
        }

        protected Task<InvokeResult> InternalCompleteAsync(string correlationId, object item)
        {
            if (Sessions.TryRemove(correlationId, out var requestAwaiter))
            {
                requestAwaiter.CompletionSource.SetResult(item);
                return Task.FromResult(InvokeResult.Success);
            }
            return Task.FromResult(InvokeResult.FromErrors(new ErrorMessage("Could not find anyone waiting for supplied correlation id.") { Details = $"CorrelationId={correlationId}" }));
        }

        public int ActiveSessions => Sessions.Count;

        protected Task<InvokeResult<TAsyncResult>> WaitOnAsyncInternal<TAsyncResult>(string correlationId, TimeSpan timeout)
        {
            try
            {
                UsageMetrics.ActiveCount++;
                var wor = new WaitOnRequest<object>(correlationId);
                if (!Sessions.TryAdd(correlationId, wor))
                {
                    return Task.FromResult(InvokeResult<TAsyncResult>.FromError($"Could not add correlation id {correlationId}."));
                }
                wor.CompletionSource.Task.Wait(timeout);
                UsageMetrics.MessagesProcessed++;
                UsageMetrics.ActiveCount--;

                UsageMetrics.ElapsedMS = (DateTime.Now - wor.Enqueued).TotalMilliseconds;

                if (!wor.CompletionSource.Task.IsCompleted)
                {
                    UsageMetrics.ErrorCount++;
                    return Task.FromResult(InvokeResult<TAsyncResult>.FromError("Timeout waiting for response."));
                }
                else if (wor.CompletionSource.Task.Result == null)
                {
                    UsageMetrics.ErrorCount++;
                    return Task.FromResult(InvokeResult<TAsyncResult>.FromError("Null Response From Completion Routine."));
                }
                else
                {
                    var result = wor.CompletionSource.Task.Result;
                    if (result is TAsyncResult typedResult)
                    {
                        return Task.FromResult(InvokeResult<TAsyncResult>.Create(typedResult));
                    }
                    else
                    {
                        UsageMetrics.ErrorCount++;
                        return Task.FromResult(InvokeResult<TAsyncResult>.FromError($"Type Mismatch - Expected: {typeof(TAsyncResult).Name} - Actual: {result.GetType().Name}."));
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.AddException("AsyncCoupler_WaitOnAsync", ex);
                UsageMetrics.ErrorCount++;

                return Task.FromResult(InvokeResult<TAsyncResult>.FromException("AsyncCoupler_WaitOnAsync", ex));
            }
        }

        public Task<InvokeResult> CompleteAsync<TResponseItem>(string correlationId, TResponseItem item)
        {
            return InternalCompleteAsync(correlationId, item);
        }

        public Task<InvokeResult<TResponseItem>> WaitOnAsync<TResponseItem>(string correlationId, TimeSpan timeout)
        {
            return WaitOnAsyncInternal<TResponseItem>(correlationId, timeout);
        }
    }

    public class AsyncCoupler<TResponseItem> : AsyncCoupler, IAsyncCoupler<TResponseItem>
    {
        public AsyncCoupler(ILogger logger, IUsageMetrics usageMetrics) : base(logger, usageMetrics)
        {
        }

        public Task<InvokeResult> CompleteAsync(string correlationId, TResponseItem item)
        {
            return InternalCompleteAsync(correlationId, item);
        }

        public Task<InvokeResult<TResponseItem>> WaitOnAsync(string correlationId, TimeSpan timeout)
        {
            return WaitOnAsyncInternal<TResponseItem>(correlationId, timeout);
        }
    }
}

