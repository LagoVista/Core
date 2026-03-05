using LagoVista.Core.Models;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Crypto.Modern
{
    public interface IKeyMaterialStore
    {
        Task<byte[]> GetOrCreateKey32Async(EntityHeader org, EntityHeader user, string keyId, int kv, CancellationToken ct = default);
    }
}
