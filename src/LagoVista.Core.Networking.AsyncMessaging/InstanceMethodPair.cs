using System;
using System.Reflection;
using System.Threading.Tasks;

namespace LagoVista.Core.Networking.AsyncMessaging
{
    internal sealed class InstanceMethodPair
    {
        private readonly bool _isAwaitable = false;
        private readonly object _instance = null;
        private readonly MethodInfo _methodInfo;
        private readonly ParameterInfo[] _parameters;
        private Type _responseType = null;

        public InstanceMethodPair(object instance, MethodInfo methodInfo) : base()
        {
            _instance = instance;
            _methodInfo = methodInfo;

            _isAwaitable = _methodInfo.ReturnType.GetMethod("GetAwaiter") != null;
            _parameters = _methodInfo.GetParameters();
        }

        public async Task<IAsyncResponse> Invoke(IAsyncRequest request)
        {
            IAsyncResponse response = null;
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

                object invokeResult = null;
                if (_isAwaitable)
                {
                    invokeResult = await (dynamic)_methodInfo.Invoke(_instance, arguments);
                }
                else
                {
                    invokeResult = _methodInfo.Invoke(_instance, arguments);
                }

                //todo: ML - this could probably be done in the constructor based on the return type of MethodInfo, but a test must be done to extract the sub type if the return type is Task<>. It's just easier to do it here one time.
                if (_responseType == null)
                {
                    _responseType = typeof(AsyncResponse<>).MakeGenericType(invokeResult.GetType());
                }
                response = (IAsyncResponse)Activator.CreateInstance(_responseType, new[] { invokeResult });
            }
            catch (Exception ex)
            {
                response = (IAsyncResponse)Activator.CreateInstance(typeof(AsyncResponse), new[] { ex });
            }

            return response;
        }
    }
}
