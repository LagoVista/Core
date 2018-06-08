using LagoVista.Core.Rpc.Messages;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace LagoVista.Core.Networking.Rpc.ServiceBus.Server
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

        internal static object[] GetArguments(IRequest request, ParameterInfo[] parameters)
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
                if (argValue != null)
                {
                    var argType = argValue.GetType();
                    if (parameter.ParameterType != argType)
                        throw new ArgumentException($"parameter type mismatch. param type: '{parameter.ParameterType.FullName}', arg type: '{argType.FullName}'.");

                }
                arguments[i] = argValue;
            }
            return arguments;
        }

        public async Task<IResponse> Invoke(IRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var arguments = GetArguments(request, _parameters);

            object result = null;
            if (_isAwaitable)
            {
                result = await (dynamic)_methodInfo.Invoke(_instance, arguments);
            }
            else
            {
                result = _methodInfo.Invoke(_instance, arguments);
            }

            return (IResponse)Activator.CreateInstance(typeof(Response), new object[] { request, result });
        }
    }
}
