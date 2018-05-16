using LagoVista.Core.PlatformSupport;
using LagoVista.Core.Validation;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LagoVista.Core.PlatformSupport
{
    public class WaitOnRequest<TModel>
    {
        public WaitOnRequest(string correlationId)
        {
            CompletionSource = new TaskCompletionSource<TModel>();
            CorrelationId = correlationId;
            Details = new List<string>();
        }

        public List<String> Details { get; private set; }

        public string CorrelationId { get; private set; }

        public DateTime Enqueued { get; private set; }

        public TaskCompletionSource<TModel> CompletionSource { get; private set; }
    }

    public class AsyncCouplerBase
    {
        protected ILogger Logger { get; }
        protected IUsageMetrics UsageMetrics { get; private set; }
        protected ConcurrentDictionary<string, WaitOnRequest<object>> Sessions { get; } = new ConcurrentDictionary<string, WaitOnRequest<object>>();

        public AsyncCouplerBase(ILogger logger, IUsageMetrics  usageMetrics)
        {
            Logger = logger ?? throw new ArgumentNullException("logger");
            UsageMetrics = usageMetrics ?? throw new ArgumentNullException("usageMetrics");
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
            if (Sessions.TryGetValue(correlationId, out var tcs))
            {
                tcs.CompletionSource.SetResult(item);
                return Task.FromResult(InvokeResult.Success);
            }
            return Task.FromResult(InvokeResult.FromErrors(new ErrorMessage("Could not find anyone waiting for supplied correlation id.") { Details = $"CorrelationId={correlationId}" }));
        }

        public int ActiveSessions { get { return Sessions.Count; } }

        protected Task<InvokeResult<TResponseItem>> WaitOnAsyncInternal<TResponseItem>(string correlationId, TimeSpan timeout)
        {
            try
            {
                UsageMetrics.ActiveCount++;
                var wor = new WaitOnRequest<object>(correlationId);
                Sessions[correlationId] = wor;
                wor.CompletionSource.Task.Wait(timeout);
                UsageMetrics.MessagesProcessed++;
                UsageMetrics.ActiveCount--;
                
                UsageMetrics.ElapsedMS = (DateTime.Now - wor.Enqueued).TotalMilliseconds;

                if (!wor.CompletionSource.Task.IsCompleted)
                {
                    UsageMetrics.ErrorCount++;
                    return Task.FromResult(InvokeResult<TResponseItem>.FromError("Timeout waiting for response."));
                }
                else if (wor.CompletionSource.Task.Result == null)
                {
                    UsageMetrics.ErrorCount++;
                    return Task.FromResult(InvokeResult<TResponseItem>.FromError("Null Response From Completion Routine."));
                }
                else
                {
                    var result = wor.CompletionSource.Task.Result;
                    if (result is TResponseItem typedResult)
                    {
                        return Task.FromResult(InvokeResult<TResponseItem>.Create(typedResult));
                    }
                    else
                    {
                        UsageMetrics.ErrorCount++;
                        return Task.FromResult(InvokeResult<TResponseItem>.FromError($"Type Mismatch - Expected: {typeof(TResponseItem).Name} - Actual: {result.GetType().Name}."));
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.AddException("AsyncCoupler_WaitOnAsync", ex);
                UsageMetrics.ErrorCount++;

                return Task.FromResult(InvokeResult<TResponseItem>.FromException("AsyncCoupler_WaitOnAsync", ex));
            }
            finally
            {
                Sessions.TryRemove(correlationId, out WaitOnRequest<Object> obj);
            }
        }
    }

    public class AsyncCoupler : AsyncCouplerBase
    {
        public AsyncCoupler(ILogger logger, IUsageMetrics usageMetrics) : base(logger, usageMetrics)
        {
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

    public class AsyncCoupler<TResponseItem> : AsyncCouplerBase
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

