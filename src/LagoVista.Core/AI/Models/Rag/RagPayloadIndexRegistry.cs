using System;
using System.Collections.Generic;
using System.Linq;

namespace LagoVista.Core.AI.Models.Rag
{
    public static class RagPayloadIndexRegistry
    {
        private static readonly object _syncLock = new object();
        private static readonly Dictionary<string, QdrantPayloadIndexSpec> _indexes =
            new Dictionary<string, QdrantPayloadIndexSpec>(StringComparer.Ordinal);

        public static IReadOnlyList<QdrantPayloadIndexSpec> GetIndexes()
        {
            lock (_syncLock)
            {
                return _indexes.Values
                    .OrderBy(index => index.Path)
                    .ToList()
                    .AsReadOnly();
            }
        }

        public static void Register<TPayload>()
            where TPayload : IRagVectorPayload, new()
        {
            var payload = new TPayload();

            Register(payload.GetIndexes());
        }

        private static void Register(IEnumerable<QdrantPayloadIndexSpec> indexes)
        {
            if (indexes == null)
            {
                return;
            }

            lock (_syncLock)
            {
                foreach (var index in indexes)
                {
                    if (index == null)
                    {
                        continue;
                    }

                    if (!_indexes.TryGetValue(index.Path, out var existing))
                    {
                        _indexes.Add(index.Path, index);
                        continue;
                    }

                    if (existing.Kind != index.Kind)
                    {
                        throw new InvalidOperationException(
                            $"Conflicting Qdrant payload index definitions were registered for '{index.Path}'. " +
                            $"Existing kind: '{existing.Kind}', requested kind: '{index.Kind}'.");
                    }
                }
            }
        }
    }
}