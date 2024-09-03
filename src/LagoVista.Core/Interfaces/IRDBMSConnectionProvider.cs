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
