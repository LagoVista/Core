using System.Threading.Tasks;

namespace LagoVista.Core.Networking.AsyncMessaging.Tests.Models
{
    public interface IEchoTest
    {
        Task<string> EchoAsync(string value);
        string Echo(string value);
    }

    public sealed class EchoTest : IEchoTest
    {
        public static readonly string EchoValueConst = "ping";

        public string Echo(string value)
        {
            return value;
        }

        public Task<string> EchoAsync(string value)
        {
            return Task.FromResult(value);
        }
    }
}
