using LagoVista.Core.Attributes;
using LagoVista.Core.Interfaces;
using LagoVista.Core.PlatformSupport;
using LagoVista.Core.Rpc.Messages;
using LagoVista.Core.Rpc.Middleware;
using LagoVista.Core.Rpc.Settings;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace LagoVista.Core.Rpc.Client
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Decendents of DispatchProxy cannot be sealed.
    /// Decendents of DispatchProxy cannot be internal nor private.
    /// </remarks>
    public class Proxy : DispatchProxy
    {
        private ILogger _logger;
        private ITransceiver _client;
        private ITransceiverConnectionSettings _connectionSettings;
        private ProxySettings _proxySettings;
        private IAsyncCoupler<IMessage> _asyncCoupler;
        private string _replyPath;
        private TimeSpan _requestTimeout;


        private readonly static MethodInfo _fromResultMethodInfo = typeof(Task).GetMethod(nameof(Task.FromResult), BindingFlags.Static | BindingFlags.Public);

        internal static TProxyInterface Create<TProxyInterface>(
            ITransceiverConnectionSettings connectionSettings,
            ITransceiver client,
            IAsyncCoupler<IMessage> asyncCoupler,
            ILogger logger,
            ProxySettings proxySettings) where TProxyInterface : class
        {
            var result = Create<TProxyInterface, Proxy>();

            (result as Proxy)._connectionSettings = connectionSettings ?? throw new ArgumentNullException(nameof(connectionSettings));
            (result as Proxy)._client = client ?? throw new ArgumentNullException(nameof(client));
            (result as Proxy)._asyncCoupler = asyncCoupler ?? throw new ArgumentNullException(nameof(asyncCoupler));
            (result as Proxy)._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            (result as Proxy)._proxySettings = proxySettings ?? throw new ArgumentNullException(nameof(proxySettings));
            if (string.IsNullOrEmpty(proxySettings.OrganizationId)) throw new ArgumentNullException(nameof(proxySettings.OrganizationId));
            if (string.IsNullOrEmpty(proxySettings.InstanceId)) throw new ArgumentNullException(nameof(proxySettings.InstanceId));

            (result as Proxy)._replyPath = connectionSettings.RpcReceiver.Uri;
            (result as Proxy)._requestTimeout = TimeSpan.FromSeconds(connectionSettings.RpcTransmitter.TimeoutInSeconds);

            return result;
        }

        private async Task<IResponse> InvokeRemoteMethod(IRequest request)
        {
            await _client.TransmitAsync(request);

            var invokeResult = await _asyncCoupler.WaitOnAsync(request.CorrelationId, _requestTimeout);

            // timeout is the only likely failure case
            if (!invokeResult.Successful)
            {
                var error = invokeResult.Errors.FirstOrDefault();
                if (error != null)
                    throw new RpcException(RpcException.FormatErrorMessage(error, "AsyncCoupler failed to complete message with error:"));
            }

            return (IResponse)invokeResult.Result;
        }

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            if (targetMethod.GetCustomAttribute<AsyncIgnoreAttribute>() != null)
            {
                throw new NotSupportedException($"{targetMethod.DeclaringType.FullName}.{targetMethod.Name}");
            }

            //todo: ML - add logging

            // setup and transmit the request
            var request = new Request(targetMethod, args, _proxySettings.OrganizationId, _proxySettings.InstanceId, _replyPath);
            var responseTask = InvokeRemoteMethod(request);

            // wait for response and handle exceptions
            responseTask.Wait();
            if (responseTask.Status == TaskStatus.Faulted && responseTask.Exception != null)
            {
                //todo: ML - log exception
                throw new RpcException($"Proxy for {targetMethod.DeclaringType.FullName}.{targetMethod.Name} failed with message '{responseTask.Exception.Message}'. See inner exception for details.", responseTask.Exception);
            }
            else if (responseTask.Status != TaskStatus.RanToCompletion)
            {
                //todo: ML - log unexpected status
                throw new RpcException($"ProxyClient for {targetMethod.DeclaringType.FullName}.{targetMethod.Name} terminated with unexpected status: '{responseTask.Status}'");
            }

            // test response for server side exceptions
            var response = responseTask.Result;
            if (!response.Success)
            {
                //todo: ML - log exception
                throw new RpcException($"ProxyServer for {targetMethod.DeclaringType.FullName}.{targetMethod.Name} failed with message '{response.Exception.Message}'. See inner exception for details.", response.Exception);
            }

            // return response result to the proxy caller
            var taskFromResult = GetGenericTaskFromResult(targetMethod);
            // if task from result method is null then this method didn't return an awaitable Task
            if (taskFromResult != null)
            {
                return taskFromResult.Invoke(null, new object[] { response.ReturnValue });
            }
            else
            {
                return response.ReturnValue;
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
