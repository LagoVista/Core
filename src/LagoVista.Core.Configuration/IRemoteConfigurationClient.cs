using LagoVista.Core.Models.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Core.Configuration
{
    public interface IRemoteConfigurationClient
    {
        Task<ResolvedConfiguration> LoadAsync(RemoteConfigurationSettings settings, string appKey, string deploymentKey, CancellationToken cancellationToken = default);
    }
}
