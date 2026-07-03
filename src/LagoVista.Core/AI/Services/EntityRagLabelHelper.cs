using LagoVista.Core.Models;
using LagoVista.Core.Utils.Types.Nuviot.RagIndexing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LagoVista.Core.AI.Services
{
    public static class EntityRagLabelHelper
    {
        public static void AddLabel(RagVectorPayload payload, string label)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            var normalized = NormalizeLabel(label);

            if (String.IsNullOrWhiteSpace(normalized))
            {
                return;
            }

            payload.Meta.LabelSlugs ??= new List<string>();

            if (!payload.Meta.LabelSlugs.Contains(normalized, StringComparer.OrdinalIgnoreCase))
            {
                payload.Meta.LabelSlugs.Add(normalized);
            }
        }

        public static void AddLabels(RagVectorPayload payload, IEnumerable<string> labels)
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

        public static void AddEntityLabels(RagVectorPayload payload, IEnumerable<Label> labels)
        {
            if (labels == null)
            {
                return;
            }

            foreach (var label in labels.Where(item => item != null && !String.IsNullOrWhiteSpace(item.Text)))
            {
                AddLabel(payload, label.Text);
            }
        }

        public static void AddRelationshipLabel(RagVectorPayload payload, string relationshipType, EntityHeader entity)
        {
            if (entity == null || String.IsNullOrWhiteSpace(entity.Id) || String.IsNullOrWhiteSpace(relationshipType))
            {
                return;
            }

            AddLabel(payload, $"{NormalizeLabel(relationshipType)}:{entity.Id}");
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