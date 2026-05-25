// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 133b5c6037474b666177b477872da6b02a4697fefe43e3752e284261de35313e
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core;
using LagoVista.Core.PlatformSupport;
using LagoVista.Core.Rpc.Attributes;
using LagoVista.Core.Rpc.Client.Interfaces;
using LagoVista.Core.Rpc.Messages;
using LagoVista.Core.Rpc.Middleware;
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
        private const string RemoteControlReplyPath = "rcg";

        private ILogger _logger;
        private IRpcInvocationTransport _invocationTransport;
        private IProxySettings _proxySettings;
        private TimeSpan _requestTimeout;
        private readonly static MethodInfo _fromResultMethodInfo = typeof(Task).GetMethod(nameof(Task.FromResult), BindingFlags.Static | BindingFlags.Public);

        internal static TProxyInterface Create<TProxyInterface>(IRpcInvocationTransport invocationTransport, ILogger logger, IProxySettings proxySettings, TimeSpan requestTimeout) where TProxyInterface : class
        {
            var result = Create<TProxyInterface, Proxy>();

            var proxy = result as Proxy;
            proxy._invocationTransport = invocationTransport ?? throw new ArgumentNullException(nameof(invocationTransport));
            proxy._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            proxy._proxySettings = proxySettings ?? throw new ArgumentNullException(nameof(proxySettings));

            if (String.IsNullOrEmpty(proxySettings.OrganizationId))
            {
                throw new ArgumentNullException(nameof(proxySettings.OrganizationId));
            }

            if (String.IsNullOrEmpty(proxySettings.InstanceId) && String.IsNullOrEmpty(proxySettings.HostId))
            {
                throw new ArgumentNullException($"{nameof(proxySettings.InstanceId)} and {nameof(proxySettings.HostId)}");
            }

            if (!String.IsNullOrEmpty(proxySettings.InstanceId) && !String.IsNullOrEmpty(proxySettings.HostId))
            {
                throw new InvalidOperationException("Must not provide both InstanceId and HostId on proxy settings.");
            }

            if (requestTimeout <= TimeSpan.Zero)
            {
                throw new ArgumentException("timeout must be greater than zero", nameof(requestTimeout));
            }

            proxy._requestTimeout = requestTimeout;

            return result;
        }

        private async Task<IResponse> InvokeRemoteMethodAsync(IRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            _logger.AddCustomEvent(LogLevel.Message, this.Tag(), "Invoke remote RPC method.", request.CorrelationId.ToKVP("CorrelationId"), request.DestinationPath.ToKVP("Method"), request.InstanceId.ToKVP("TargetInstanceId"));

            var invokeResult = await _invocationTransport.InvokeAsync(request, _requestTimeout);

            if (invokeResult == null)
            {
                throw new NullReferenceException(nameof(invokeResult));
            }

            if (!invokeResult.Successful)
            {
                if (invokeResult.Errors == null)
                {
                    throw new NullReferenceException($"RPC for {request.DestinationPath} failed, {nameof(invokeResult)}.{nameof(invokeResult.Errors)} is null.");
                }

                var error = invokeResult.Errors.FirstOrDefault();
                if (error != null)
                {
                    throw new RpcException($"RPC for {request.DestinationPath} failed, {error.Message}");
                }

                throw new RpcException($"RPC for {request.DestinationPath} failed. No error message was provided.");
            }

            if (invokeResult.Result == null)
            {
                throw new NullReferenceException($"RPC for {request.DestinationPath} failed, {nameof(invokeResult)}.{nameof(invokeResult.Result)} is null.");
            }

            _logger.AddCustomEvent(LogLevel.Message, this.Tag(), "Remote RPC method completed.", request.CorrelationId.ToKVP("CorrelationId"), request.DestinationPath.ToKVP("Method"), request.InstanceId.ToKVP("TargetInstanceId"));

            return invokeResult.Result;
        }

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            if (targetMethod.GetCustomAttribute<RpcIgnoreMethodAttribute>() != null)
            {
                throw new NotSupportedException($"{nameof(Proxy)}.{nameof(Invoke)}: method not supported: {targetMethod.DeclaringType.FullName}.{targetMethod.Name}");
            }

            var parameters = targetMethod.GetParameters();
            for (var i = 0; i < parameters.Length; ++i)
            {
                var parameter = parameters[i];
                if (parameter.GetCustomAttribute<RpcIgnoreParameterAttribute>() != null)
                {
                    args[parameter.Position] = null;
                }
            }

            var resourceId = String.IsNullOrEmpty(_proxySettings.InstanceId) ? _proxySettings.HostId : _proxySettings.InstanceId;
            var request = new Request(targetMethod, args, _proxySettings.OrganizationId, resourceId, RemoteControlReplyPath);
            var responseTask = InvokeRemoteMethodAsync(request);

            responseTask.Wait(_requestTimeout.Add(TimeSpan.FromSeconds(30)));
            if (responseTask.Status == TaskStatus.Faulted && responseTask.Exception != null)
            {
                throw new RpcException($"Proxy for {targetMethod.DeclaringType.FullName}.{targetMethod.Name} failed with message '{responseTask.Exception.Message}'. See inner exception for details.", responseTask.Exception);
            }
            else if (responseTask.Status != TaskStatus.RanToCompletion)
            {
                throw new RpcException($"Proxy for {targetMethod.DeclaringType.FullName}.{targetMethod.Name} terminated with unexpected status: '{responseTask.Status}'.");
            }

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

            var taskFromResult = GetTaskFromResultMethod(targetMethod);

            var returnValue = response.ReturnValue;

            if (returnValue != null && returnValue.GetType() == typeof(Int64))
            {
                if (targetMethod.ReturnType == typeof(Int32))
                {
                    returnValue = Convert.ToInt32(returnValue);
                }
                else if (targetMethod.ReturnType == typeof(Int16))
                {
                    returnValue = Convert.ToInt16(returnValue);
                }
                else if (targetMethod.ReturnType == typeof(byte))
                {
                    returnValue = Convert.ToByte(returnValue);
                }
            }

            return taskFromResult != null ? taskFromResult.Invoke(null, new object[] { returnValue }) : returnValue;
        }

        private MethodInfo GetTaskFromResultMethod(MethodInfo targetMethod)
        {
            MethodInfo taskFromResultMethod = null;
            if (targetMethod.ReturnType.IsGenericType && targetMethod.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
            {
                var genericArguments = targetMethod.ReturnType.GetGenericArguments();
                taskFromResultMethod = genericArguments.Length > 0 ? _fromResultMethodInfo.MakeGenericMethod(genericArguments) : _fromResultMethodInfo.MakeGenericMethod();
            }
            else if (targetMethod.ReturnType == typeof(Task))
            {
                taskFromResultMethod = _fromResultMethodInfo.MakeGenericMethod(new Type[] { typeof(object) });
            }
            return taskFromResultMethod;
        }
    }
}
