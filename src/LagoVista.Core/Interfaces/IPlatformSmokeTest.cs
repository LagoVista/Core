using LagoVista.Core.Models.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Core.Interfaces
{
    public interface IPlatformSmokeTest
    {
        string Key { get; }
        string Name { get; }
        string Category { get; }

        Task<PlatformSmokeTestResult> ExecuteAsync(CancellationToken cancellationToken);
    }
}
