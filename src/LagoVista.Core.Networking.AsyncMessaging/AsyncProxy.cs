using LagoVista.Core.Interfaces;
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
        //todo: ML - add logger
        private IAsyncCoupler<IAsyncResponse> _asyncCoupler { get; set; }
        private IAsyncRequestHandler _requestSender { get; set; }

        internal static TProxy CreateProxy<TProxy>(IAsyncCoupler<IAsyncResponse> asyncCoupler, IAsyncRequestHandler requestSender)
        {
            var result = Create<TProxy, AsyncProxy>();
            (result as AsyncProxy)._asyncCoupler = asyncCoupler;
            (result as AsyncProxy)._requestSender = requestSender;
            return result;
        }

        private static MethodInfo FromResultMethodInfo { get; } = typeof(Task).GetMethod("FromResult", BindingFlags.Static | BindingFlags.Public);

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            var asyncRequest = new AsyncRequest(targetMethod, args);

            // note: we prep that async coupler before sending the request - the coupler won't be awaited until after the call to RequestSender.HandleRequest
            var waitOnAsync = _asyncCoupler.WaitOnAsync(asyncRequest.CorrelationId, TimeSpan.FromSeconds(30)).ContinueWith(waitOnAsyncTask =>
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

            _requestSender.HandleRequest(asyncRequest).ContinueWith(sendAsyncTask =>
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

            waitOnAsync.Wait();

            MethodInfo genericFromResult = null;
            if (targetMethod.ReturnType.BaseType == typeof(Task))
            {
                var genericArguments = targetMethod.ReturnType.GetGenericArguments();
                if (genericArguments.Length > 0)
                {
                    genericFromResult = FromResultMethodInfo.MakeGenericMethod(genericArguments);
                }
                else
                {
                    genericFromResult = FromResultMethodInfo.MakeGenericMethod();
                }
            }

            var invokeResult = waitOnAsync.Result;
            if (invokeResult.Successful)
            {
                var response = invokeResult.Result;
                if (response.Success)
                {
                    if (genericFromResult != null)
                    {
                        return genericFromResult.Invoke(null, new object[] { response.ReturnValue });
                    }
                    else
                    {
                        return response.ReturnValue;
                    }
                }
                else
                {
                    //todo: ML - response.Exception
                    if (genericFromResult != null)
                    {
                        return genericFromResult.Invoke(null, new object[] { null });
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
                if (genericFromResult != null)
                {
                    return genericFromResult.Invoke(null, new object[] { null });
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
