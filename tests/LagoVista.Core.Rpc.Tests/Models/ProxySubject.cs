using LagoVista.Core.Attributes;
using System.Threading.Tasks;

namespace LagoVista.Core.Networking.Rpc.Tests.Models
{
    public interface IProxySubject
    {
        Task<string> EchoAsync(string value);
        string Echo(string value);
        string PassStringParams(params string[] value);
        string PassObjectParams(params object[] value);

        [RpcIgnore]
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
    }
}
