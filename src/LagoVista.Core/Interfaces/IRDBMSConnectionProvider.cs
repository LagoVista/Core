// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: cb3c9c79f8cf1057e0a7a8e9737f95b61fad6fb4aa4a0fc19abf07d527846f7f
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core.Models;
using System.Threading.Tasks;

namespace LagoVista
{
    public interface IRDBMSConnectionProvider
    {
        Task<RDBMSConnection> GetConnectionAsync(string orgId);
        Task MigrateSharedRDBMSAsync(string orgId, RDBMSConnectionType srcConnectionType, RDBMSConnectionType destConnection);
    }
}
