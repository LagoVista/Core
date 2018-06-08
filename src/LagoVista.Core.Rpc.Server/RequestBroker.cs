using LagoVista.Core.Attributes;
using LagoVista.Core.Rpc.Attributes;
using LagoVista.Core.Rpc.Messages;
using LagoVista.Core.Rpc.Middleware;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace LagoVista.Core.Rpc.Server
{
    public sealed class RequestBroker : IRequestBroker
    {
        private readonly ConcurrentDictionary<string, InstanceMethodPair> _subjectRegistry = new ConcurrentDictionary<string, InstanceMethodPair>();
        private readonly static MethodInfo[] _objectMethods = typeof(object).GetMethods(BindingFlags.Instance | BindingFlags.Public);

        private void RegisterSubjectMethod<TImplementation>(TImplementation subject, MethodInfo methodInfo) where TImplementation : class
        {
            if (subject == null)
            {
                throw new ArgumentNullException(nameof(subject));
            }

            if (methodInfo == null)
            {
                throw new ArgumentNullException(nameof(methodInfo));
            }

            // This should be impossible, but if TImplementation is explicitly provided as an interface like this: RegisterSubjectMethod<IMyThing>(thingInstance, IOtherThing.methodInfo)
            // the type of the instance might be restricted in such a way as to hide the method represented by methodInfo. 
            if (typeof(TImplementation) != methodInfo.DeclaringType)
            {
                throw new ArgumentException($"{typeof(TImplementation).FullName} does not contain method '{methodInfo.Name}'.");
            }

            var subjectKey = PathBuilder.BuildPath(methodInfo);

            if (_subjectRegistry.ContainsKey(subjectKey))
            {
                throw new ArgumentException($"Subject key '{subjectKey}' already registed with RequestBroker.");
            }

            if (!_subjectRegistry.TryAdd(subjectKey, new InstanceMethodPair(subject, methodInfo)))
            {
                throw new RpcException($"Could not register subject key '{subjectKey}' with RequestBroker.");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <param name="subject"></param>
        public int AddService<TInterface>(TInterface subject) where TInterface : class
        {
            if (subject == null)
            {
                throw new ArgumentNullException(nameof(subject));
            }

            var interfaceType = typeof(TInterface);
            if (!interfaceType.IsInterface)
            {
                throw new ArgumentException($"TInterface type must be an interface: '{interfaceType.FullName}'.");
            }

            var methods = interfaceType.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Except(_objectMethods)
                .Where(m => m.GetCustomAttribute<RpcIgnoreAttribute>() == null);
            var methodCount = 0;
            foreach (var method in methods)
            {
                RegisterSubjectMethod(subject, method);
                ++methodCount;
            }
            return methodCount;
        }

        public async Task<IResponse> InvokeAsync(IRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            // 1. get handler
            if (!_subjectRegistry.TryGetValue(request.DestinationPath, out InstanceMethodPair messageHandler))
            {
                throw new KeyNotFoundException($"Handler not found for {request.DestinationPath}");
            }

            // 2. call handler and get response
            IResponse response = null;
            try
            {
                response = await messageHandler.Invoke(request);
            }
            catch(Exception ex)
            {
                //todo: ML - log exception
                response = new Response(request, ex);
            }

            return response;
        }
    }
}
