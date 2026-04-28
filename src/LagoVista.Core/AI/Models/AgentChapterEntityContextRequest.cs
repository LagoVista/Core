using LagoVista.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.AI.Models
{
    public sealed class AgentChapterEntityContextRequest
    {
        public string EntityType { get; set; }

        public string EntityId { get; set; }

        public EntityHeader Entity { get; set; }

        public EntityHeader<AgentChapterEntityRole> Role { get; set; }

        public bool IsPrimary { get; set; }

        public string RelationshipToPrimary { get; set; }

        public bool IncludeEntityJson { get; set; } = true;

        public bool IncludeAIFormDescriptor { get; set; } = true;

        public bool RefreshExistingContext { get; set; } = true;
    }
}
