using LagoVista.Core.AI.Models.Rag;
using LagoVista.Core.Models;
using LagoVista.Core.Utils.Types.Nuviot.RagIndexing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LagoVista.Core.AI.Services
{
    public static class EntityRagLabelHelper
    {
        public static void AddLabel(RagCoreVectorPayloadMeta meta, string label)
        {
            if (meta == null)
            {
                throw new ArgumentNullException(nameof(meta));
            }

            var normalized = NormalizeLabel(label);

            if (String.IsNullOrWhiteSpace(normalized))
            {
                return;
            }

            meta.LabelSlugs ??= new List<string>();

            if (!meta.LabelSlugs.Contains(normalized, StringComparer.OrdinalIgnoreCase))
            {
                meta.LabelSlugs.Add(normalized);
            }
        }

        public static void AddLabels(RagCoreVectorPayloadMeta payload, IEnumerable<string> labels)
        {
            if (labels == null)
            {
                return;
            }

            foreach (var label in labels)
            {
                AddLabel(payload, label);
            }
        }

        public static void AddEntityLabels(RagEntityVectorPayload payload, IEnumerable<Label> labels)
        {
            if (labels == null)
            {
                return;
            }

            foreach (var label in labels.Where(item => item != null && !String.IsNullOrWhiteSpace(item.Text)))
            {
                AddLabel(payload.Meta, label.Text);
            }
        }

        public static void AddRelationshipLabel(RagVectorPayload payload, string relationshipType, EntityHeader entity)
        {
            if (entity == null || String.IsNullOrWhiteSpace(entity.Id) || String.IsNullOrWhiteSpace(relationshipType))
            {
                return;
            }

            AddLabel(payload.Meta, $"{NormalizeLabel(relationshipType)}:{entity.Id}");
        }

        private static string NormalizeLabel(string value)
        {
            if (String.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            return value.Trim().ToLowerInvariant().Replace(" ", "-");
        }
    }
}