using LagoVista.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.AI.Models
{
    public sealed class AgentAuthoringEntityRequest
    {
        public string EntityType { get; set; }

        public string EntityId { get; set; }

        public EntityHeader Entity { get; set; }

        public EntityHeader<AgentAuthoringEntityRole> Role { get; set; }

        public string RelationshipSummary { get; set; }

        public string AddedReason { get; set; }

        public bool? IsAuthorable { get; set; }

        public bool RefreshExistingContext { get; set; } = true;
    }

    public sealed class AgentAuthoringTurnContext
    {
        public AgentAuthoringEntityRequest Entity { get; set; }

        public bool IncludeAuthoringContext { get; set; } = true;
    }
}
