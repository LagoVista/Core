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
    }
}

