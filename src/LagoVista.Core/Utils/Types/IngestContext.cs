using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Utils.Types
{
    public sealed class IngestContext
    {
        public string OrgId { get; set; }          // from AgentContext.OwnerOrganization.Id
        public string ProjectId { get; set; }      // e.g., agent context key or project
        public string EmbeddingModel { get; set; } = "text-embedding-3-large";
        public int IndexVersion { get; set; } = 1;
    }
}
