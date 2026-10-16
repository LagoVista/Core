using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Core.Interfaces
{
    /// <summary>
    /// Provider-neutral storage contract for compact, mutable operational records.
    /// Implementations may use Cassandra or another backend suitable for high-cardinality
    /// keyed state without exposing provider topology to callers.
    /// </summary>
    public interface IOperationalRecordStore<TRecord>
        where TRecord : class, IOperationalRecord
    {
        Task<TRecord> GetAsync(OperationalRecordKey key, CancellationToken cancellationToken = default);
        Task UpsertAsync(TRecord record, CancellationToken cancellationToken = default);
        Task UpsertBatchAsync(IEnumerable<TRecord> records, CancellationToken cancellationToken = default);
        Task DeleteAsync(OperationalRecordKey key, CancellationToken cancellationToken = default);
    }
}
