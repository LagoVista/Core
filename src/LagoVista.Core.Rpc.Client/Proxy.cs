using LagoVista.Core.Attributes;
using LagoVista.Core.PlatformSupport;
using LagoVista.Core.Rpc.Messages;
using LagoVista.Core.Rpc.Middleware;
using LagoVista.Core.Rpc.Settings;
using System;
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
        private string _replyPath;

        private readonly static MethodInfo _fromResultMethodInfo = typeof(Task).GetMethod(nameof(Task.FromResult), BindingFlags.Static | BindingFlags.Public);

        internal static TProxyInterface Create<TProxyInterface>(ITransceiverConnectionSettings connectionSettings, ITransceiver client, ILogger logger, ProxySettings proxySettings) where TProxyInterface : class
        {
            var result = Create<TProxyInterface, Proxy>();

            (result as Proxy)._connectionSettings = connectionSettings ?? throw new ArgumentNullException(nameof(connectionSettings));
            (result as Proxy)._replyPath = connectionSettings.RpcReceiver.Uri;
            (result as Proxy)._client = client ?? throw new ArgumentNullException(nameof(client));
            (result as Proxy)._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            (result as Proxy)._proxySettings = proxySettings ?? throw new ArgumentNullException(nameof(proxySettings));

            return result;
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
            var responseTask = _client.TransmitAsync(request);

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
            var response = (IResponse)responseTask.Result;
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
