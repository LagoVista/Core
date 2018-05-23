﻿using LagoVista.Core.Interfaces;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace LagoVista.Core.Networking.AsyncMessaging
{
    internal sealed class RemoteProxy : DispatchProxy
    {
        internal IAsyncCoupler<IAsyncResponse> AsyncCoupler { get; set; }
        internal IAsyncRequestHandler RequestSender { get; set; }
        private static MethodInfo FromResultMethodInfo { get; } = typeof(Task).GetMethod("FromResult", BindingFlags.Static | BindingFlags.Public);

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            var request = new AsyncRequest()
            {
                Id = Guid.NewGuid().ToString(),
                CorrelationId = Guid.NewGuid().ToString(),
                //todo: ML - write test to guarantee this name is the same as the name being generated by the AsyncRequestBroker
                Path = $"{targetMethod.DeclaringType.FullName}.{targetMethod.Name}",
                TimeStamp = DateTime.UtcNow
            };

            //todo: ML - scan params to validate names
            var parameters = targetMethod.GetParameters();
            for (var i = 0; i < parameters.Length; ++i)
            {
                request.AddValue(parameters[i].Name, args[i]);
            }

            RequestSender.HandleRequest(request).ContinueWith(sendAsyncTask =>
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

            var waitOnAsync = AsyncCoupler.WaitOnAsync(request.CorrelationId, TimeSpan.FromSeconds(30)).ContinueWith(waitOnAsyncTask =>
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
            waitOnAsync.Wait();

            var genericFromResult = FromResultMethodInfo.MakeGenericMethod(new Type[] { targetMethod.ReturnType.GetGenericArguments()[0] });
            var invokeResult = waitOnAsync.Result;
            if (invokeResult.Successful)
            {
                var response = invokeResult.Result;
                if (response.Success)
                {
                    return genericFromResult.Invoke(null, new object[] { response.Response });
                }
                else
                {
                    //todo: ML - response.Exception
                    return genericFromResult.Invoke(null, new object[] { null });
                }
            }
            else
            {
                //todo: ML - handle failed invoke result
                return genericFromResult.Invoke(null, new object[] { null });
            }
        }
    }

    public sealed class AsyncProxyFactory : IAsyncProxyFactory
    {
        public TProxy Create<TProxy>(IAsyncCoupler<IAsyncResponse> asyncCoupler, IAsyncRequestHandler sender)
        {
            var result = DispatchProxy.Create<TProxy, RemoteProxy>();
            (result as RemoteProxy).AsyncCoupler = asyncCoupler;
            (result as RemoteProxy).RequestSender = sender;
            return result;
        }
    }
}
