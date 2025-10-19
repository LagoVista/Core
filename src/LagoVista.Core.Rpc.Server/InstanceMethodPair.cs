// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: a5a1a00a44ece7e227c813a7a379a333c330e262f8107fd88a0eec41095555ee
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core.Rpc.Messages;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace LagoVista.Core.Rpc.Server
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
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            // 1. check request value count == param count
            if (request.ArgumentCount != parameters.Length)
            {
                throw new ArgumentException($"parameter count mismatch. params {parameters.Length}, args {request.ArgumentCount}.");
            }

            var arguments = new object[parameters.Length];

            // 2. validate the types
            for (var i = 0; i < parameters.Length; ++i)
            {
                var parameter = parameters[i];

                if (parameter.GetCustomAttribute(typeof(ParamArrayAttribute)) != null)
                {
                    throw new NotSupportedException($"unsupported type - params keyword not allowed. type: '{parameter.Name}'.");
                }

                var argValue = request.GetValue(parameters[i].Name);
                if (argValue != null)
                {
                    var argType = argValue.GetType();
                    if(argType == typeof(Int64))
                    {
                        if(parameter.ParameterType == typeof(Int32))
                            argValue = Convert.ToInt32(argValue);

                        if (parameter.ParameterType == typeof(Int16))
                            argValue = Convert.ToInt16(argValue);

                        if (parameter.ParameterType == typeof(byte))
                            argValue = Convert.ToByte(argValue);
                    }
                    else if (parameter.ParameterType != argType)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("[InstanceMethodPair__GetArguments] Parameter Type Mismatch");
                        Console.WriteLine($"\tMethod Type: {parameter.Name}");
                        Console.WriteLine($"\tMethod Type: {parameter.ParameterType.FullName}");
                        Console.WriteLine($"\tActual Type: {argType.FullName}");
                        Console.ResetColor(); 

                        throw new ArgumentException($"parameter type mismatch: param type: '{parameter.ParameterType.FullName}', arg type: '{argType.FullName}'.");
                    }
                }
                arguments[i] = argValue;
            }
            return arguments;
        }

        public async Task<IResponse> InvokeAsync(IRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var arguments = GetArguments(request, _parameters);

            Console.WriteLine($"[InstanceMethodPair__InvokeAsync] - {request.DestinationPath} - argument count {arguments.Length}");

            object invokeResult = null;
            if (_isAwaitable)
            {
                if (_methodInfo.ReturnType.IsGenericType)
                {
                    invokeResult = (object)await (dynamic)_methodInfo.Invoke(_instance, arguments);
                }
                else
                {
                    await (Task)_methodInfo.Invoke(_instance, arguments);
                }
            }
            else
            {
                if (_methodInfo.ReturnType == typeof(void))
                {
                    _methodInfo.Invoke(_instance, arguments);
                }
                else
                {
                    invokeResult = _methodInfo.Invoke(_instance, arguments);
                }
            }

            Console.WriteLine($"[InstanceMethodPair__InvokeAsync] - completed");

            return (IResponse)Activator.CreateInstance(typeof(Response), new object[] { request, invokeResult });
        }
    }
}
