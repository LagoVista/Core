using System.Collections.Generic;
using System.Linq;

namespace LagoVista.Core.AI.Models
{
    public sealed class EntityRagSection
    {
        public EntityRagSection(string heading, string content, int priority = 50)
        {
            Heading = heading;
            Content = content;
            Priority = priority;
        }

        public string Heading { get; }

        public string Content { get; }

        public int Priority { get; }
    }

    public sealed class EntityRagListSection
    {
        public EntityRagListSection(string heading, IEnumerable<string> items, int priority = 50, int? maxItems = null)
        {
            Heading = heading;
            Items = items?.ToList() ?? new List<string>();
            Priority = priority;
            MaxItems = maxItems;
        }

        public string Heading { get; }

        public IReadOnlyList<string> Items { get; }

        public int Priority { get; }

        public int? MaxItems { get; }
    }
}