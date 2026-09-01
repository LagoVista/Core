using Microsoft.Extensions.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Core.Configuration
{
    public interface IRemoteConfigurationClient
    {
        Task<IConfigurationRoot> LoadAsync(RemoteConfigurationSettings settings, string appKey, string environmentKey, CancellationToken cancellationToken = default);
    }
}
