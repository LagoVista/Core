using LagoVista.Core.AI.Models;
using LagoVista.Core.AI.Models.Rag;
using LagoVista.Core.Models;
using LagoVista.Core.Utils.Types.Nuviot.RagIndexing;
using System.Collections.Generic;

namespace LagoVista.Core.AI.Interfaces
{
    public interface IEntityRagContentBuilder
    {
        IEntityBase Entity { get; }

        RagEntityVectorPayload Payload { get; }

        IEntityRagContentBuilder WithSubtypeFlavor(string subtypeFlavor);


        IEntityRagContentBuilder WithShortSummary(string shortSummary);

        IEntityRagContentBuilder AddShortSummaryText(string content, int priority = 50);

        IEntityRagContentBuilder AddShortSummaryList(string heading, IEnumerable<string> items, int priority = 50, int maxItems = 8);

        IEntityRagContentBuilder AddShortSummaryList(string heading, IEnumerable<EntityHeader> items, int priority = 50, int maxItems = 8);

        IEntityRagContentBuilder AddLabel(string label);

        IEntityRagContentBuilder SetVersion(int version);

        IEntityRagContentBuilder SetStatusKey(string statusKey);

        IEntityRagContentBuilder AddLabels(params string[] labels);

        IEntityRagContentBuilder AddEmbeddingSection(string heading, string content, int priority = 50);

        IEntityRagContentBuilder AddModelSection(string heading, string content, int priority = 50);

        IEntityRagContentBuilder AddModelListSection(string heading, IEnumerable<string> items, int priority = 50);

        IEntityRagContentBuilder AddModelListSection(string heading, IEnumerable<EntityHeader> items, int priority = 50, bool includeIds = true);

        IEntityRagContentBuilder AddHumanSection(string heading, string content, int priority = 50);

        IEntityRagContentBuilder AddHumanListSection(string heading, IEnumerable<string> items, int priority = 50);

        IEntityRagContentBuilder AddHumanListSection(string heading, IEnumerable<EntityHeader> items, int priority = 50, bool includeIds = false);

        IEntityRagContentBuilder AddIssue(string issue);

        IEntityRagContentBuilder AddIssueIf(bool condition, string issue);

        EntityRagContent<RagEntityVectorPayload> Build();
    }
}