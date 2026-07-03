using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LagoVista.Core.AI.Models.Rag
{
    internal static class RagPayloadIndexResolver
    {
        private static readonly ConcurrentDictionary<string, IReadOnlyList<QdrantPayloadIndexSpec>> _cache =
            new ConcurrentDictionary<string, IReadOnlyList<QdrantPayloadIndexSpec>>(StringComparer.Ordinal);

        public static IReadOnlyList<QdrantPayloadIndexSpec> GetIndexes<TMeta>(string bucketName)
        {
            if (String.IsNullOrWhiteSpace(bucketName))
            {
                throw new ArgumentException("Bucket name is required.", nameof(bucketName));
            }

            var cacheKey = $"{typeof(TMeta).AssemblyQualifiedName}:{bucketName}";

            return _cache.GetOrAdd(cacheKey, _ => BuildIndexes(typeof(TMeta), bucketName));
        }

        private static IReadOnlyList<QdrantPayloadIndexSpec> BuildIndexes(Type metaType, string bucketName)
        {
            return metaType
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Select(property => new
                {
                    Property = property,
                    Attribute = property.GetCustomAttribute<QdrantPayloadIndexAttribute>(true)
                })
                .Where(item => item.Attribute != null)
                .Select(item => new QdrantPayloadIndexSpec($"{bucketName}.{item.Property.Name}", item.Attribute.Kind))
                .ToList()
                .AsReadOnly();
        }
    }
}