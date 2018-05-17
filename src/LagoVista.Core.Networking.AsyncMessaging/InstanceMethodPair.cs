using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Core.Networking.AsyncMessaging
{
    internal class InstanceMethodPair
    {
        private bool _isAwaitable = false;
        private Type _responseType = null;
        private object _instance = null;
        private MethodInfo _methodInfo;
        private ParameterInfo[] _parameters;

        public InstanceMethodPair(object instance, MethodInfo methodInfo) : base()
        {
            _instance = instance;
            _methodInfo = methodInfo;

            _isAwaitable = _methodInfo.ReturnType.GetMethod("GetAwaiter") != null;
            _parameters = _methodInfo.GetParameters();
        }

        public async Task<IAsyncResponse> Invoke(IAsyncRequest request)
        {
            IResponse response = null;
            try
            {
                //todo: ML - validate the request to parameter mapping
                // 1. check request value count == param count
                // 2. loop each list indepently to validate param and argument names and types
                var arguments = new object[_parameters.Length];
                for (var i = 0; i < _parameters.Length; ++i)
                {
                    //todo: ML - check value for IValidatable and if exits then call Validator.Valiate(value);
                    arguments[i] = request.GetValue(_parameters[i].Name);
                }

                object result = null;
                if (_isAwaitable)
                {
                    result = await (dynamic)_methodInfo.Invoke(_instance, arguments);
                }
                else
                {
                    result = _methodInfo.Invoke(_instance, arguments);
                }
                if (_responseType == null)
                {
                    _responseType = typeof(RemoteResponseJson<>).MakeGenericType(result.GetType());
                }
                response = (IResponse)Activator.CreateInstance(_responseType, new[] { result });
            }
            catch (Exception ex)
            {
                response = (IResponse)Activator.CreateInstance(_responseType, new[] { ex });
            }

            return response;
        }
    }
}
