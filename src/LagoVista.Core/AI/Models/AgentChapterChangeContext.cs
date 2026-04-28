using LagoVista.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.AI.Models
{
    public enum AgentChapterEntityRole
    {
        Primary,
        OutputArtifactSpecification,
        InputArtifactSpecification,
        ParentEssentialJobActivity,
        Survey,
        Tool,
        RuntimeArtifact,
        Reference
    }

    public enum AgentEntityChangeProposalStatus
    {
        Proposed,
        Accepted,
        Rejected,
        Applied,
        Failed
    }

    public enum AgentEntityPatchOperationKind
    {
        Replace
    }

    public sealed class AgentChapterChangeContext
    {
        public List<AgentChapterEntityContext> Entities { get; set; } = new List<AgentChapterEntityContext>();

        public List<AgentEntityChangeProposal> Proposals { get; set; } = new List<AgentEntityChangeProposal>();
    }

    public sealed class AgentChapterEntityContext
    {
        public string Id { get; set; } = Guid.NewGuid().ToId();

        public string EntityType { get; set; }

        public EntityHeader Entity { get; set; }

        public EntityHeader<AgentChapterEntityRole> Role { get; set; }

        public bool IsPrimary { get; set; }

        public string RelationshipToPrimary { get; set; }

        public string EntityJson { get; set; }

        public string EntityJsonSha256 { get; set; }

        public string AIFormDescriptorJson { get; set; }

        public string AIFormDescriptorSha256 { get; set; }

        public string AttachedUtc { get; set; }

        public EntityHeader AttachedBy { get; set; }

        public string LastRefreshedUtc { get; set; }

        public EntityHeader LastRefreshedBy { get; set; }
    }

    public sealed class AgentEntityChangeProposal
    {
        public string Id { get; set; } = Guid.NewGuid().ToId();

        public string ProposalKey { get; set; }

        public string Title { get; set; }

        public string Summary { get; set; }

        public string Rationale { get; set; }

        public string Impact { get; set; }

        public EntityHeader<AgentEntityChangeProposalStatus> Status { get; set; } =
            EntityHeader<AgentEntityChangeProposalStatus>.Create(AgentEntityChangeProposalStatus.Proposed);

        public List<EntityHeader> TouchedEntities { get; set; } = new List<EntityHeader>();

        public List<AgentEntityPatchOperation> Operations { get; set; } = new List<AgentEntityPatchOperation>();

        public string CreatedByTurnId { get; set; }

        public string CreatedUtc { get; set; }

        public EntityHeader CreatedBy { get; set; }

        public string LastUpdatedUtc { get; set; }

        public EntityHeader LastUpdatedBy { get; set; }
    }

    public sealed class AgentEntityPatchOperation
    {
        public string Id { get; set; } = Guid.NewGuid().ToId();

        public EntityHeader TargetEntity { get; set; }

        public string EntityType { get; set; }

        public EntityHeader<AgentEntityPatchOperationKind> Operation { get; set; } =
            EntityHeader<AgentEntityPatchOperationKind>.Create(AgentEntityPatchOperationKind.Replace);

        public string Path { get; set; }

        public string Label { get; set; }

        public string CurrentValueJson { get; set; }

        public string ProposedValueJson { get; set; }

        public string Reason { get; set; }
    }
}
