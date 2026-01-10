// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 301068645372eb5bb755b7175c3de45c2694d541d012b26b53e49122503602dc
// IndexVersion: 2
// --- END CODE INDEX META ---
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Utils.Types
{
    public sealed class IngestContext
    {
        public string OrgNamspace { get; set; }
        public string OrgId { get; set; }          // from AgentContext.OwnerOrganization.Id
        public string ProjectId { get; set; }      // e.g., agent context key or project
        public string EmbeddingModel { get; set; } = "text-embedding-3-large";
        public int IndexVersion { get; set; } = 1;
    }
}
