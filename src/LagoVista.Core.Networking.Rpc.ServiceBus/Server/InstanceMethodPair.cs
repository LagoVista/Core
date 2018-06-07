using System;
using System.Reflection;
using System.Threading.Tasks;

namespace LagoVista.Core.Networking.Rpc
{
    internal sealed class InstanceMethodPair
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

        internal static object[] GetArguments(IAsyncRequest request, ParameterInfo[] parameters)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (parameters == null) throw new ArgumentNullException(nameof(parameters));

            // 1. check request value count == param count
            if (request.ArgumentCount != parameters.Length)
                throw new ArgumentException($"parameter count mismatch. params {parameters.Length}, args {request.ArgumentCount}.");

            var arguments = new object[parameters.Length];

            // 2. validate the types
            for (var i = 0; i < parameters.Length; ++i)
            {
                var parameter = parameters[i];

                if (parameter.GetCustomAttribute(typeof(ParamArrayAttribute)) != null)
                    throw new NotSupportedException($"unsupported type - params keyword not allowed. type: '{parameter.Name}'.");

                var argValue = request.GetValue(parameters[i].Name);
                if(argValue != null)
                {
                    var argType = argValue.GetType();
                    if (parameter.ParameterType != argType)
                        throw new ArgumentException($"parameter type mismatch. param type: '{parameter.ParameterType.FullName}', arg type: '{argType.FullName}'.");

                }
                arguments[i] = argValue;
            }
            return arguments;
        }

        public async Task<IAsyncResponse> Invoke(IAsyncRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            IAsyncResponse response = null;
            try
            {
                var arguments = GetArguments(request, _parameters);
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
