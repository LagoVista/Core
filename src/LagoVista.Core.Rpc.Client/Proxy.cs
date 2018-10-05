using LagoVista.Core.Interfaces;
using LagoVista.Core.PlatformSupport;
using LagoVista.Core.Rpc.Attributes;
using LagoVista.Core.Rpc.Messages;
using LagoVista.Core.Rpc.Middleware;
using LagoVista.Core.Rpc.Settings;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace LagoVista.Core.Rpc.Client
{
    /// <remarks>
    /// Decendents of DispatchProxy cannot be sealed.
    /// Decendents of DispatchProxy cannot be internal nor private.
    /// </remarks>
    public class Proxy : DispatchProxy
    {
        private ILogger _logger;
        private ITransceiver _client;
        private ITransceiverConnectionSettings _connectionSettings;
        private IProxySettings _proxySettings;
        private IAsyncCoupler<IMessage> _asyncCoupler;
        private string _replyPath;
        private TimeSpan _requestTimeout;
        private readonly static MethodInfo _fromResultMethodInfo = typeof(Task).GetMethod(nameof(Task.FromResult), BindingFlags.Static | BindingFlags.Public);

        internal static TProxyInterface Create<TProxyInterface>(
            ITransceiverConnectionSettings connectionSettings,
            ITransceiver client,
            IAsyncCoupler<IMessage> asyncCoupler,
            ILogger logger,
            IProxySettings proxySettings) where TProxyInterface : class
        {
            var result = Create<TProxyInterface, Proxy>();

            var proxy = (result as Proxy);
            proxy._connectionSettings = connectionSettings ?? throw new ArgumentNullException(nameof(connectionSettings));
            proxy._client = client ?? throw new ArgumentNullException(nameof(client));
            proxy._asyncCoupler = asyncCoupler ?? throw new ArgumentNullException(nameof(asyncCoupler));
            proxy._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            proxy._proxySettings = proxySettings ?? throw new ArgumentNullException(nameof(proxySettings));
            if (string.IsNullOrEmpty(proxySettings.OrganizationId))
            {
                throw new ArgumentNullException(nameof(proxySettings.OrganizationId));
            }

            if (string.IsNullOrEmpty(proxySettings.InstanceId))
            {
                throw new ArgumentNullException(nameof(proxySettings.InstanceId));
            }

            if (string.IsNullOrEmpty(connectionSettings.RpcReceiver.ResourceName))
            {
                throw new ArgumentNullException(nameof(connectionSettings.RpcReceiver.ResourceName));
            }

            proxy._replyPath = connectionSettings.RpcReceiver.ResourceName;
            if (connectionSettings.RpcTransmitter.TimeoutInSeconds == 0)
            {
                throw new ArgumentException("timeout must be  greater than zero", nameof(connectionSettings.RpcReceiver.Uri));
            }

            proxy._requestTimeout = TimeSpan.FromSeconds(connectionSettings.RpcTransmitter.TimeoutInSeconds);

            return result;
        }

        private async Task<IResponse> InvokeRemoteMethodAsync(IRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var invokeResult = await _asyncCoupler.WaitOnAsync(
                async () => await _client.TransmitAsync(request), 
                request.CorrelationId, 
                _requestTimeout);

            if (invokeResult == null)
            {
                throw new NullReferenceException(nameof(invokeResult));
            }

            // timeout is the only likely failure case
            if (!invokeResult.Successful)
            {
                if (invokeResult.Errors == null)
                {
                    throw new NullReferenceException($"{nameof(InvokeRemoteMethodAsync)} failed: {nameof(invokeResult)}.{nameof(invokeResult.Errors)} is null.");
                }

                var error = invokeResult.Errors.FirstOrDefault();
                if (error != null)
                {
                    throw new RpcException(RpcException.FormatErrorMessage(error, "{nameof(InvokeRemoteMethodAsync)} failed: AsyncCoupler failed to complete message with error:"));
                }
            }

            if (invokeResult.Result == null)
            {
                throw new NullReferenceException($"{nameof(InvokeRemoteMethodAsync)} failed: {nameof(invokeResult)}.{nameof(invokeResult.Result)} is null.");
            }

            return (IResponse)invokeResult.Result;
        }

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            if (targetMethod.GetCustomAttribute<RpcIgnoreMethodAttribute>() != null)
            {
                throw new NotSupportedException($"{nameof(Proxy)}.{nameof(Invoke)}: method not suported: {targetMethod.DeclaringType.FullName}.{targetMethod.Name}");
            }

            // set ignored parmeters to null
            var parameters = targetMethod.GetParameters();
            for (var i = 0; i < parameters.Length; ++i)
            {
                var parameter = parameters[i];
                if (parameter.GetCustomAttribute<RpcIgnoreParameterAttribute>() != null)
                {
                    args[parameter.Position] = null;
                }
            }

            // setup and transmit the request
            var request = new Request(targetMethod, args, _proxySettings.OrganizationId, _proxySettings.InstanceId, _replyPath);
            var responseTask = InvokeRemoteMethodAsync(request);

            // wait for response and handle exceptions
            responseTask.Wait(_requestTimeout);
            if (responseTask.Status == TaskStatus.Faulted && responseTask.Exception != null)
            {
                throw new RpcException($"Proxy for {targetMethod.DeclaringType.FullName}.{targetMethod.Name} failed with message '{responseTask.Exception.Message}'. See inner exception for details.", responseTask.Exception);
            }
            else if (responseTask.Status != TaskStatus.RanToCompletion)
            {
                throw new RpcException($"Proxy for {targetMethod.DeclaringType.FullName}.{targetMethod.Name} terminated with unexpected status: '{responseTask.Status}'.");
            }

            // test response for server side exceptions
            var response = responseTask.Result;
            if (response == null)
            {
                throw new NullReferenceException($"{nameof(Proxy)}.{nameof(Invoke)} failed: {nameof(response)} is null.");
            }

            if (!response.Success)
            {
                if (response.Exception != null)
                {
                    throw new RpcException($"ProxyServer for {targetMethod.DeclaringType.FullName}.{targetMethod.Name} failed with message '{response.Exception.Message}'. See inner exception for details.", response.Exception);
                }
                else
                {
                    throw new RpcException($"ProxyServer for {targetMethod.DeclaringType.FullName}.{targetMethod.Name} failed. No reason given.");
                }
            }

            // return response result to the proxy caller
            var taskFromResult = GetTaskFromResultMethod(targetMethod);

            // if task from result method is null then this method didn't return an awaitable Task
            return taskFromResult != null
                ? taskFromResult.Invoke(null, new object[] { response.ReturnValue })
                : response.ReturnValue;
        }

        private MethodInfo GetTaskFromResultMethod(MethodInfo targetMethod)
        {
            MethodInfo taskFromResultMethod = null;
            if (targetMethod.ReturnType.IsGenericType && targetMethod.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
            {
                var genericArguments = targetMethod.ReturnType.GetGenericArguments();
                taskFromResultMethod = genericArguments.Length > 0
                    ? _fromResultMethodInfo.MakeGenericMethod(genericArguments)
                    : _fromResultMethodInfo.MakeGenericMethod();
            }
            else if(targetMethod.ReturnType == typeof(Task))
            {
                taskFromResultMethod = _fromResultMethodInfo.MakeGenericMethod(new Type[]{ typeof(object)});
            }
            return taskFromResultMethod;
        }
    }
}
