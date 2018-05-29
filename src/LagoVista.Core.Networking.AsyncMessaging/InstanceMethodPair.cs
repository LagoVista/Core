using System;
using System.Reflection;
using System.Threading.Tasks;

namespace LagoVista.Core.Networking.AsyncMessaging
{
    internal sealed class InstanceMethodPair : IAsyncRequestValidator
    {
        private readonly bool _isAwaitable = false;
        private readonly object _instance = null;
        private readonly MethodInfo _methodInfo;
        private readonly ParameterInfo[] _parameters;

        public InstanceMethodPair(object instance, MethodInfo methodInfo) : base()
        {
            _instance = instance;
            _methodInfo = methodInfo;
            _isAwaitable = _methodInfo.ReturnType.GetMethod(nameof(Task.GetAwaiter)) != null;
            _parameters = _methodInfo.GetParameters();
        }

        public void ValidateArguments(IAsyncRequest request, ParameterInfo[] parameters)
        {
            // 1. check request value count == param count
            if (request.ArgumentCount != parameters.Length)
                throw new ArgumentException($"parameter count mismatch. params {parameters.Length}, args {request.ArgumentCount}.");

            //todo: ML - validate the request to parameter mapping
            // 2. loop each list indepently to validate param and argument names and types
            for (var i = 0; i < parameters.Length; ++i)
            {

            }
            //throw new ArgumentException("argument validation failure");
        }

        public async Task<IAsyncResponse> Invoke(IAsyncRequest request)
        {
            IAsyncResponse response = null;
            try
            {
                ValidateArguments(request, _parameters);

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

                response = (IAsyncResponse)Activator.CreateInstance(typeof(AsyncResponse), new object[] { request, invokeResult });
            }
            catch (Exception ex)
            {
                response = (IAsyncResponse)Activator.CreateInstance(typeof(AsyncResponse), new object[] { request, ex });
            }

            return response;
        }

    }
}
