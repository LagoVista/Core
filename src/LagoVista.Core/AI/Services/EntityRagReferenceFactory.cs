using LagoVista.Core.AI.Models;
using LagoVista.Core.AI.Models.Rag;
using LagoVista.Core.Attributes;
using LagoVista.Core.Models;
using LagoVista.Core.Utils.Types.Nuviot.RagIndexing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static LagoVista.Core.Models.AdaptiveCard.MSTeams;

namespace LagoVista.Core.AI.Services
{
    public static class EntityRagReferenceFactory
    {
        public static IReadOnlyList<EntityRagReferenceContent> Create(IEntityBase entity, RagEntityVectorPayload primaryPayload, string primaryPointId)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (primaryPayload == null)
            {
                throw new ArgumentNullException(nameof(primaryPayload));
            }

            if (String.IsNullOrWhiteSpace(primaryPointId))
            {
                throw new ArgumentException("Primary point ID is required.", nameof(primaryPointId));
            }

            var results = new List<EntityRagReferenceContent>();
            var seenContent = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            var properties = entity
                .GetType()
                .GetRuntimeProperties()
                .Select(property => new
                {
                    Property = property,
                    Attribute = property.GetCustomAttribute<RagEmbeddingContentAttribute>(true)
                })
                .Where(item => item.Attribute != null)
                .ToList();

            foreach (var item in properties)
            {
                AddPropertyReferences(results, seenContent, entity, primaryPayload, primaryPointId, item.Property, item.Attribute);
            }

            return results;
        }

        private static void AddPropertyReferences(List<EntityRagReferenceContent> results, HashSet<string> seenContent, IEntityBase entity, RagEntityVectorPayload primaryPayload, string primaryPointId, PropertyInfo property, RagEmbeddingContentAttribute attribute)
        {
            var values = property.GetValue(entity) as IEnumerable<string>;

            if (values == null)
            {
                return;
            }

            var referenceType = !String.IsNullOrWhiteSpace(attribute.ReferenceType)
                ? attribute.ReferenceType.Trim()
                : RagReferenceNameHelper.ToReferenceType(property.Name);

            var sourceIndex = 0;

            foreach (var value in values)
            {
                var normalized = EntityRagText.Normalize(value);

                if (String.IsNullOrWhiteSpace(normalized))
                {
                    sourceIndex++;
                    continue;
                }

                var deduplicationKey = $"{referenceType}:{normalized}";

                if (!seenContent.Add(deduplicationKey))
                {
                    sourceIndex++;
                    continue;
                }

                var payload = RagReferenceVectorPayload.FromPrimary(primaryPayload);

                payload.Meta.ContentHash = EntityRagText.ComputeSha256(normalized);
                EntityRagLabelHelper.AddLabel(payload.Meta, $"entity-type:{RagReferenceNameHelper.ToKebabCase(property.Name)}-{sourceIndex+1}");


                results.Add(new EntityRagReferenceContent
                {
                    PointId = RagReferenceVectorPayload.BuildPointId(entity.EntityType ?? entity.GetType().Name, entity.Id, property.Name, sourceIndex),
                    EmbeddingContent = normalized,
                    Payload = payload
                });

                sourceIndex++;
            }
        }
    }
}