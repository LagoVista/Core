using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.AI.Models
{
    public sealed class AgentChapterAuthoringRequest
    {
        public bool IncludeChapterChangeContext { get; set; } = true;

        public bool IncludeAttachedEntities { get; set; } = true;

        public bool IncludeEntityJson { get; set; } = true;

        public bool IncludeAIFormDescriptors { get; set; } = true;

        public bool IncludeProposalQueue { get; set; } = true;
    }
}
