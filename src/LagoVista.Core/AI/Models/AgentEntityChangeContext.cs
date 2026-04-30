using LagoVista.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.AI.Models
{

    public enum AgentAuthoringEntityRole
    {
        Subject,
        Input,
        Output,
        Reference,
        Supporting,
        Tool,
        RuntimeArtifact,
        Related
    }


    public enum AgentAuthoringEntityAddedBy
    {
        User,
        Model,
        Pipeline,
        Tool
    }

    public sealed class AgentAuthoringTurnResult
    {
        public AgentAuthoringContext Context { get; set; }
    }


    public sealed class AgentAuthoringContext
    {
        public List<AgentAuthoringEntityParticipant> Entities { get; set; } = new List<AgentAuthoringEntityParticipant>();

        public string LastUpdatedUtc { get; set; }

        public EntityHeader LastUpdatedBy { get; set; }
    }

    public sealed class AgentAuthoringEntityParticipant
    {
        public string Id { get; set; } = Guid.NewGuid().ToId();

        public string EntityType { get; set; }

        public EntityHeader Entity { get; set; }

        public EntityHeader<AgentAuthoringEntityRole> Role { get; set; }

        public bool IsAuthorable { get; set; }

        public string RelationshipSummary { get; set; }

        public string AddedReason { get; set; }

        public EntityHeader<AgentAuthoringEntityAddedBy> AddedByKind { get; set; }

        public string AddedByTurnId { get; set; }

        public string AddedByChapterId { get; set; }

        public string AiDetailResponseJson { get; set; }

        public string AiDetailResponseSha256 { get; set; }

        public string LastRefreshedUtc { get; set; }

        public EntityHeader LastRefreshedBy { get; set; }
    }
}
