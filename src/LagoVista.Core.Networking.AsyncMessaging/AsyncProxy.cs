using LagoVista.Core.Attributes;
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
        private string _organizationId;
        private string _instanceId;
        private string _replyPath;
        private TimeSpan _timeout;
        private readonly static MethodInfo _fromResultMethodInfo =
            typeof(Task).GetMethod(nameof(Task.FromResult), BindingFlags.Static | BindingFlags.Public);

        internal static TProxy CreateProxy<TProxy>(
            IAsyncCoupler<IAsyncResponse> asyncCoupler,
            IAsyncRequestHandler requestSender,
            ILogger logger,
            string organizationId,
            string instanceId,
            string replyPath,
            TimeSpan timeout)
        {
            Console.WriteLine("AsyncProxy::ctor >>");
            var result = Create<TProxy, AsyncProxy>();

            (result as AsyncProxy)._asyncCoupler = asyncCoupler ?? throw new ArgumentNullException(nameof(asyncCoupler));
            (result as AsyncProxy)._requestSender = requestSender ?? throw new ArgumentNullException(nameof(requestSender));
            (result as AsyncProxy)._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            (result as AsyncProxy)._organizationId = organizationId ?? throw new ArgumentNullException(nameof(organizationId));
            (result as AsyncProxy)._instanceId = instanceId ?? throw new ArgumentNullException(nameof(instanceId));
            (result as AsyncProxy)._replyPath = replyPath ?? throw new ArgumentNullException(nameof(replyPath));
            (result as AsyncProxy)._timeout = timeout;

            Console.WriteLine("AsyncProxy::ctor <<");

            return result;
        }

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            Console.WriteLine($"AsyncProxy.Invoke: {targetMethod.DeclaringType.Name}.{targetMethod.Name}");
            if (targetMethod.GetCustomAttribute<AsyncIgnoreAttribute>() != null)
            {
                throw new NotSupportedException($"{targetMethod.DeclaringType.FullName}.{targetMethod.Name}");
            }

            //todo: ML - add logging

            var request = new AsyncRequest(targetMethod, args, _organizationId, _instanceId, _replyPath);
            Console.WriteLine("reqeust created:");
            Console.WriteLine(request.Json);

            var senderHandleRequestTask = _requestSender.HandleRequest(request);
            senderHandleRequestTask.Wait();
            if (senderHandleRequestTask.Status == TaskStatus.Faulted && senderHandleRequestTask.Exception != null)
            {
                //todo: ML - handle exception
            }
            else if (senderHandleRequestTask.Status != TaskStatus.RanToCompletion)
            {
                //todo: ML - handle unexpected status
            }
            //todo: ML - if _requestSender.HandleRequest failed we can't continue

            var couplerWaitOnAsyncTask = _asyncCoupler.WaitOnAsync(request.CorrelationId, _timeout);
            couplerWaitOnAsyncTask.Wait();
            if (couplerWaitOnAsyncTask.Status == TaskStatus.Faulted && couplerWaitOnAsyncTask.Exception != null)
            {
                //todo: ML - handle exception
            }
            else if (couplerWaitOnAsyncTask.Status != TaskStatus.RanToCompletion)
            {
                //todo: ML - handle unexpected status
            }
            //todo: ML - if _asyncCoupler.WaitOnAsync failed we can't continue

            var taskFromResult = GetGenericTaskFromResult(targetMethod);

            //todo: ML - there has to be a simpler wait to handle all the cases
            var invokeResult = couplerWaitOnAsyncTask.Result;
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
