using System;
using System.Collections.Generic;

namespace LagoVista.Core.Interfaces
{
    /// <summary>
    /// Provider-neutral identity for an operational record.
    /// Scope values describe the logical record boundary (for example repository or instance)
    /// without exposing provider-specific partition, shard, table, or collection concepts.
    /// </summary>
    public sealed class OperationalRecordKey
    {
        public OperationalRecordKey(string organizationId, string id, IReadOnlyDictionary<string, string> scope = null)
        {
            if (String.IsNullOrWhiteSpace(organizationId)) throw new ArgumentNullException(nameof(organizationId));
            if (String.IsNullOrWhiteSpace(id)) throw new ArgumentNullException(nameof(id));

            OrganizationId = organizationId;
            Id = id;
            Scope = scope ?? new Dictionary<string, string>();
        }

        public string OrganizationId { get; }
        public string Id { get; }
        public IReadOnlyDictionary<string, string> Scope { get; }
    }
}
