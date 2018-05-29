using System.Threading.Tasks;

namespace LagoVista.Core.Networking.AsyncMessaging.Tests.Models
{
    public interface IProxySubject
    {
        Task<string> EchoAsync(string value);
        string Echo(string value);
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

        public string PassParams(params string[] value)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(value);
        }

        public string PassParams(params object[] value)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(value);
        }
    }
}
