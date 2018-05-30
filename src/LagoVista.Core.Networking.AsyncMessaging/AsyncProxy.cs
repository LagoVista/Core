using LagoVista.Core.Interfaces;
using LagoVista.Core.PlatformSupport;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace LagoVista.Core.Networking.AsyncMessaging
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Decendents of DispatchProxy cannot be sealed.
    /// Decendents of DispatchProxy cannot be internal nor private.
    /// </remarks>
    public class AsyncProxy : DispatchProxy
    {
        private IAsyncCoupler<IAsyncResponse> _asyncCoupler;
        private IAsyncRequestHandler _requestSender;
        private ILogger _logger;
        private string _destination;
        private TimeSpan _timeout;
        //private IUsageMetrics _usageMetrics;
        private static MethodInfo _fromResultMethodInfo =
            typeof(Task).GetMethod(nameof(Task.FromResult), BindingFlags.Static | BindingFlags.Public);

        internal static TProxy CreateProxy<TProxy>(
            IAsyncCoupler<IAsyncResponse> asyncCoupler,
            IAsyncRequestHandler requestSender,
            ILogger logger,
            //IUsageMetrics usageMetrics,
            string destination,
            TimeSpan timeout)
        {
            var result = Create<TProxy, AsyncProxy>();

            (result as AsyncProxy)._asyncCoupler = asyncCoupler ?? throw new ArgumentNullException(nameof(asyncCoupler));
            (result as AsyncProxy)._requestSender = requestSender ?? throw new ArgumentNullException(nameof(requestSender));
            (result as AsyncProxy)._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            //(result as AsyncProxy)._usageMetrics = usageMetrics ?? throw new ArgumentNullException(nameof(usageMetrics));
            (result as AsyncProxy)._destination = destination ?? throw new ArgumentNullException(nameof(destination));
            (result as AsyncProxy)._timeout = timeout;

            return result;
        }

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            //todo: ML - add logging
            //todo: ML - add usage metrics

            var request = new AsyncRequest(targetMethod, args);

            // note: no reason to wait on this - the result will be returned by the async coupler
            _requestSender.HandleRequest(request, _destination).ContinueWith(sendAsyncTask =>
            {
                if (sendAsyncTask.Status == TaskStatus.Faulted && sendAsyncTask.Exception != null)
                {
                    //todo: ML - handle exception
                }
                else if (sendAsyncTask.Status != TaskStatus.RanToCompletion)
                {
                    //todo: ML - handle unexpected status
                }
            });

            var waitOnAsync = _asyncCoupler.WaitOnAsync(request.CorrelationId, _timeout).ContinueWith(waitOnAsyncTask =>
            {
                if (waitOnAsyncTask.Status == TaskStatus.Faulted && waitOnAsyncTask.Exception != null)
                {
                    //todo: ML - handle exception
                }
                else if (waitOnAsyncTask.Status != TaskStatus.RanToCompletion)
                {
                    //todo: ML - handle unexpected status
                }
                return waitOnAsyncTask.Result;
            });
            // note: wait is absolutely required
            waitOnAsync.Wait();

            var taskFromResult = GetGenericTaskFromResult(targetMethod);

            var invokeResult = waitOnAsync.Result;
            if (invokeResult.Successful)
            {
                var response = invokeResult.Result;
                if (string.Compare(request.CorrelationId, response.CorrelationId) != 0 ||
                    string.Compare(request.Id, response.RequestId) != 0)
                {
                    //todo: ML - handle id mismatch
                    if (taskFromResult != null)
                    {
                        return taskFromResult.Invoke(null, new object[] { null });
                    }
                    else
                    {
                        return null;
                    }
                }
                if (response.Success)
                {
                    if (taskFromResult != null)
                    {
                        return taskFromResult.Invoke(null, new object[] { response.ReturnValue });
                    }
                    else
                    {
                        return response.ReturnValue;
                    }
                }
                else
                {
                    //todo: ML - handle response.Exception
                    if (taskFromResult != null)
                    {
                        return taskFromResult.Invoke(null, new object[] { null });
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            else
            {
                //todo: ML - handle failed invoke result
                if (taskFromResult != null)
                {
                    return taskFromResult.Invoke(null, new object[] { null });
                }
                else
                {
                    return null;
                }
            }
        }

        private MethodInfo GetGenericTaskFromResult(MethodInfo targetMethod)
        {
            MethodInfo generic_Task_FromResult = null;
            if (targetMethod.ReturnType.BaseType == typeof(Task))
            {
                var genericArguments = targetMethod.ReturnType.GetGenericArguments();
                if (genericArguments.Length > 0)
                {
                    generic_Task_FromResult = _fromResultMethodInfo.MakeGenericMethod(genericArguments);
                }
                else
                {
                    generic_Task_FromResult = _fromResultMethodInfo.MakeGenericMethod();
                }
            }
            return generic_Task_FromResult;
        }
    }
}
