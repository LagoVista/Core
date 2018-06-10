using Newtonsoft.Json;
using System;
using System.Reflection;

namespace LagoVista.Core.Diagnostics.ConsoleProxy
{
    public sealed class ConsoleProxyFactory
    {
        private readonly IConsoleWriter _console;

        public ConsoleProxyFactory(IConsoleWriter console)
        {
            _console = console != null ? console : new ConsoleWriter();
        }

        public TInterface Create<TInterface>(TInterface instance) where TInterface : class
        {
//#if DEBUG
            return ConsoleProxy.Create<TInterface>(instance, _console);
//#else
//            return instance;
//#endif
        }
    }

    public class ConsoleProxy : DispatchProxy 
    {
        private IConsoleWriter _console;
        private object _instance;

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            object result = null;
            _console.WriteLine($"-> {targetMethod.DeclaringType.FullName}.{targetMethod.Name}");
            if (args != null && args.Length > 0)
            {
                try
                {
                    _console.WriteLine($"{targetMethod.DeclaringType.FullName}.{targetMethod.Name} args: {JsonConvert.SerializeObject(args)}");
                }
                catch(Exception ex)
                {
                    _console.WriteError($"{targetMethod.DeclaringType.FullName}.{targetMethod.Name} failed to convert args to json. Exception: {ex.GetType().Name}, {ex.Message}, {ex.Source}");
                }
            }

            try
            {
                result = targetMethod.Invoke(_instance, args);
                _console.WriteLine($"<- {targetMethod.DeclaringType.FullName}.{targetMethod.Name}");
            }
            catch (Exception ex)
            {
                _console.WriteError($"{targetMethod.DeclaringType.FullName}.{targetMethod.Name} Exception: {ex.GetType().Name}, {ex.Message}, {ex.Source}");
                _console.WriteError($"{targetMethod.DeclaringType.FullName}.{targetMethod.Name} Stack Trace: {ex.StackTrace}");
                throw;
            }
            return result;
        }

        public static TInterface Create<TInterface>(TInterface instance, IConsoleWriter console) where TInterface : class
        {
            var result = Create<TInterface, ConsoleProxy>();
            var proxy = (result as ConsoleProxy);
            proxy._console = console ?? throw new ArgumentNullException(nameof(console));
            proxy._instance = instance ?? throw new ArgumentNullException(nameof(instance));
            return result;
        }
    }
}
