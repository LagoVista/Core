// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: aac61d466eb2e0281eeb0c29fa6c6982630d0236c6126f5ad410b7e6df5f19b2
// IndexVersion: 1
// --- END CODE INDEX META ---
using LagoVista.Core.Rpc.Attributes;
using System.Threading.Tasks;

namespace LagoVista.Core.Rpc.Tests.Models
{
    public interface IProxySubject
    {
        Task<string> EchoAsync(string value);
        string Echo(string value);
        string PassStringParams(params string[] value);
        string PassObjectParams(params object[] value);

        string IgnoreParametersAtInterface([RpcIgnoreParameter]string param1, string param2);
        string IgnoreParametersAtImplementation(string param1, string param2);
        Task VoidTaskMethod();
        void VoidMethod();

        [RpcIgnoreMethod]
        string SkipMe();
    }

    public sealed class ProxySubject : IProxySubject
    {
        public static readonly string EchoValueConst = "ping";

        public string Echo(string value)
        {
            return value;
        }

        public async Task<string> EchoAsync(string value)
        {
            return await Task.FromResult(value);
        }

        public string IgnoreParametersAtInterface(string param1, string param2)
        {
            return param1 + param2;
        }

        public string PassStringParams(params string[] value)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(value);
        }

        public string PassObjectParams(params object[] value)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(value);
        }

        public string SkipMe()
        {
            return EchoValueConst;
        }

        // proxy does not see the ignore attribute
        public string IgnoreParametersAtImplementation([RpcIgnoreParameter]string param1, string param2)
        {
            return param1 + param2;
        }

        public async Task VoidTaskMethod()
        {
            await Task.FromResult<object>(null);
        }

        public void VoidMethod()
        {
            var s = string.Empty;
        }
    }
}

