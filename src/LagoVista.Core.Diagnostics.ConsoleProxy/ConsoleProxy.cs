using System;
using System.Reflection;

namespace LagoVista.Core.Diagnostics.ConsoleProxy
{
    public sealed class ConsoleProxyFactory
    {
        private readonly IConsoleWriter _console;
        private readonly bool _debug;

        public ConsoleProxyFactory(IConsoleWriter console, bool debug = false)
        {
            if (debug)
            {
                _console = console ?? new ConsoleWriter();
            }
            _debug = debug;
        }

        public TInterface Create<TInterface>(TInterface instance) where TInterface : class
        {
            return _debug ? ConsoleProxy.Create<TInterface>(instance, _console) : instance;
        }
    }

    public class ConsoleProxy : DispatchProxy
    {
        private IConsoleWriter _writer;
        private object _instance;

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            object result = null;
            _writer.WriteLine($">>> {targetMethod.DeclaringType.FullName}.{targetMethod.Name}");
            if (args != null && args.Length > 0)
            {
                var parameters = targetMethod.GetParameters();
                for (var i = 0; i < parameters.Length; ++i)
                {
                    try
                    {
                        _writer.WriteLine($"{i}. {parameters[i].Name}.");
                        //_writer.WriteLine(JsonConvert.SerializeObject(args[i], Formatting.Indented));
                    }
                    catch (Exception ex)
                    {
                        _writer.WriteWarning($"{typeof(ConsoleProxyFactory).Name}::{targetMethod.DeclaringType.FullName}.{targetMethod.Name} failed to convert args for param '{parameters[i].Name}' to json. Exception: {ex.GetType().Name}, {ex.Message}, {ex.Source}");
                    }
                }
            }

            try
            {
                result = targetMethod.Invoke(_instance, args);
                _writer.WriteLine($"<<< {targetMethod.DeclaringType.FullName}.{targetMethod.Name}");
            }
            catch (Exception ex)
            {
                _writer.WriteError($"{targetMethod.DeclaringType.FullName}.{targetMethod.Name} Exception: {ex.GetType().Name}, {ex.Message}, {ex.Source}");
                _writer.WriteError($"{targetMethod.DeclaringType.FullName}.{targetMethod.Name} Stack Trace: {ex.StackTrace}");
                throw;
            }
            return result;
        }

        public static TInterface Create<TInterface>(TInterface instance, IConsoleWriter console) where TInterface : class
        {
            var result = Create<TInterface, ConsoleProxy>();
            var proxy = (result as ConsoleProxy);
            proxy._writer = console ?? throw new ArgumentNullException(nameof(console));
            proxy._instance = instance ?? throw new ArgumentNullException(nameof(instance));
            return result;
        }
    }
}
