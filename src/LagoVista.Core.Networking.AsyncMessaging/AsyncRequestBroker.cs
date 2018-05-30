using LagoVista.Core.Attributes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace LagoVista.Core.Networking.AsyncMessaging
{
    public sealed class AsyncRequestBroker : IAsyncRequestBroker
    {
        private readonly ConcurrentDictionary<string, InstanceMethodPair> _subjectRegistry = new ConcurrentDictionary<string, InstanceMethodPair>();
        private readonly static MethodInfo[] _objectMethods = typeof(object).GetMethods(BindingFlags.Instance | BindingFlags.Public);

        private void RegisterSubjectMethod<T>(T instance, MethodInfo methodInfo) where T : class
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            if (methodInfo == null)
            {
                throw new ArgumentNullException(nameof(methodInfo));
            }

            // This should be impossible, but if T is explicitly provided as an interface like this: RegisterSubjectMethod<IMyThing>(thingInstance, IOtherThing.methodInfo)
            // the type of the instance might be restricted in such a way as to hide the method 
            // represented by methodInfo. 
            if (typeof(T) != methodInfo.DeclaringType)
            {
                throw new ArgumentException($"{typeof(T).FullName} does not contain method '{methodInfo.Name}'.");
            }

            var subjectKey = PathBuilder.BuildPath(methodInfo);

            if (_subjectRegistry.ContainsKey(subjectKey))
            {
                throw new ArgumentException($"{subjectKey} handler already registed.");
            }

            if (!_subjectRegistry.TryAdd(subjectKey, new InstanceMethodPair(instance, methodInfo)))
            {
                throw new Exception($"Could not register {subjectKey} with RequestBroker.");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="subject"></param>
        public void RegisterSubject<T>(T subject) where T : class
        {
            if (subject == null)
            {
                throw new ArgumentNullException(nameof(subject));
            }

            var type = typeof(T);
            if (!type.IsInterface)
            {
                throw new ArgumentException($"Subject type must be an interface: '{typeof(T).FullName}'");
            }

            var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Except(_objectMethods)
                .Where(m => m.GetCustomAttribute<AsyncIgnoreAttribute>() == null);
            foreach (var method in methods)
            {
                RegisterSubjectMethod(subject, method);
            }
        }

        public async Task<IAsyncResponse> HandleRequestAsync(IAsyncRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            // 1. get handler
            if (!_subjectRegistry.TryGetValue(request.Path, out InstanceMethodPair messageHandler))
            {
                throw new KeyNotFoundException($"Handler not found for {request.Path}");
            }

            // 2. call handler and get response
            var response = await messageHandler.Invoke(request);

            return response;
        }
    }
}
