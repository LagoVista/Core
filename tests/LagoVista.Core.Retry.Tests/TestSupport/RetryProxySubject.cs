// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: c4d920bc99b7fe00b88f7f7491ecd3505e284249abfc0a59e1f7ba43631ea744
// IndexVersion: 1
// --- END CODE INDEX META ---
using System.Threading.Tasks;

namespace LagoVista.Core.Retry
{
    internal interface IRetryProxySubject
    {
        string Succeed();
        Task<string> SucceedAsync();
        string Fail();
        Task<string> FailAsync();
    }

    internal class RetryProxySubject : IRetryProxySubject
    {
        public string Succeed()
        {
            return "succeed";
        }

        public async Task<string> SucceedAsync()
        {
            return await Task.Run(() => { return "succeed"; });
        }

        public string Fail()
        {
            throw new ProxySubjectTestException("fail");
        }

        public async Task<string> FailAsync()
        {
            return await Task.Run(() =>
            {
                throw new ProxySubjectTestException("fail");
#pragma warning disable CS0162 // Unreachable code detected
                return "fail";
#pragma warning restore CS0162 // Unreachable code detected
            });

        }
    }
}
