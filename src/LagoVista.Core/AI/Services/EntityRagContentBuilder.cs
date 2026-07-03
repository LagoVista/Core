using LagoVista.Core.AI.Interfaces;
using LagoVista.Core.AI.Models;
using LagoVista.Core.Models;
using LagoVista.Core.Models.EntityReadiness;
using LagoVista.Core.Utils.Types.Nuviot.RagIndexing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LagoVista.Core.AI.Services
{
    public sealed class EntityRagContentBuilder : IEntityRagContentBuilder
    {
        private const int ShortSummaryMaximumLength = 1024;

        private readonly List<EntityRagSection> _embeddingSections = new List<EntityRagSection>();
        private readonly List<EntityRagSection> _modelSections = new List<EntityRagSection>();
        private readonly List<EntityRagListSection> _modelListSections = new List<EntityRagListSection>();
        private readonly List<EntityRagSection> _humanSections = new List<EntityRagSection>();
        private readonly List<EntityRagListSection> _humanListSections = new List<EntityRagListSection>();
        private readonly List<EntityRagSection> _shortSummarySections = new List<EntityRagSection>();
        private readonly List<EntityRagListSection> _shortSummaryListSections = new List<EntityRagListSection>();
        private readonly List<string> _issues = new List<string>();

        private string _explicitShortSummary;

        private EntityRagContentBuilder(IEntityBase entity)
        {
            Entity = entity ?? throw new ArgumentNullException(nameof(entity));
            Payload = RagVectorPayload.FromEntity(entity);

            ApplyStandardPayload();
        }

        public IEntityBase Entity { get; }

        public RagVectorPayload Payload { get; }

        public static EntityRagContentBuilder For(IEntityBase entity)
        {
            return new EntityRagContentBuilder(entity);
        }

        public IEntityRagContentBuilder WithSubtypeFlavor(string subtypeFlavor)
        {
            Payload.Meta.SubtypeFlavor = EntityRagText.NormalizeInline(subtypeFlavor);
            return this;
        }

        public IEntityRagContentBuilder WithAudience(string audience)
        {
            Payload.Meta.Audience = EntityRagText.NormalizeInline(audience);
            return this;
        }

        public IEntityRagContentBuilder WithPersona(string persona)
        {
            Payload.Meta.Persona = EntityRagText.NormalizeInline(persona);
            return this;
        }

        public IEntityRagContentBuilder WithShortSummary(string shortSummary)
        {
            _explicitShortSummary = shortSummary;
            return this;
        }

        public IEntityRagContentBuilder AddShortSummaryText(string content, int priority = 50)
        {
            if (!String.IsNullOrWhiteSpace(content))
            {
                _shortSummarySections.Add(new EntityRagSection(null, content, priority));
            }

            return this;
        }

        public IEntityRagContentBuilder AddShortSummaryList(string heading, IEnumerable<string> items, int priority = 50, int maxItems = 8)
        {
            var normalizedItems = EntityRagText.NormalizeList(items, maxItems);

            if (normalizedItems.Any())
            {
                _shortSummaryListSections.Add(new EntityRagListSection(heading, normalizedItems, priority, maxItems));
            }

            return this;
        }

        public IEntityRagContentBuilder AddShortSummaryList(string heading, IEnumerable<EntityHeader> items, int priority = 50, int maxItems = 8)
        {
            return AddShortSummaryList(heading, FormatEntityHeaders(items, includeIds: true), priority, maxItems);
        }

        public IEntityRagContentBuilder AddLabel(string label)
        {
            EntityRagLabelHelper.AddLabel(Payload, label);
            return this;
        }

        public IEntityRagContentBuilder AddLabels(params string[] labels)
        {
            EntityRagLabelHelper.AddLabels(Payload, labels);
            return this;
        }

        public IEntityRagContentBuilder AddEmbeddingSection(string heading, string content, int priority = 50)
        {
            if (!String.IsNullOrWhiteSpace(content))
            {
                _embeddingSections.Add(new EntityRagSection(heading, content, priority));
            }

            return this;
        }

        public IEntityRagContentBuilder AddModelSection(string heading, string content, int priority = 50)
        {
            if (!String.IsNullOrWhiteSpace(content))
            {
                _modelSections.Add(new EntityRagSection(heading, content, priority));
            }

            return this;
        }

        public IEntityRagContentBuilder AddModelListSection(string heading, IEnumerable<string> items, int priority = 50)
        {
            var normalizedItems = EntityRagText.NormalizeList(items);

            if (normalizedItems.Any())
            {
                _modelListSections.Add(new EntityRagListSection(heading, normalizedItems, priority));
            }

            return this;
        }

        public IEntityRagContentBuilder AddModelListSection(string heading, IEnumerable<EntityHeader> items, int priority = 50, bool includeIds = true)
        {
            return AddModelListSection(heading, FormatEntityHeaders(items, includeIds), priority);
        }

        public IEntityRagContentBuilder AddHumanSection(string heading, string content, int priority = 50)
        {
            if (!String.IsNullOrWhiteSpace(content))
            {
                _humanSections.Add(new EntityRagSection(heading, content, priority));
            }

            return this;
        }

        public IEntityRagContentBuilder AddHumanListSection(string heading, IEnumerable<string> items, int priority = 50)
        {
            var normalizedItems = EntityRagText.NormalizeList(items);

            if (normalizedItems.Any())
            {
                _humanListSections.Add(new EntityRagListSection(heading, normalizedItems, priority));
            }

            return this;
        }

        public IEntityRagContentBuilder AddHumanListSection(string heading, IEnumerable<EntityHeader> items, int priority = 50, bool includeIds = false)
        {
            return AddHumanListSection(heading, FormatEntityHeaders(items, includeIds), priority);
        }

        public IEntityRagContentBuilder AddIssue(string issue)
        {
            var normalized = EntityRagText.NormalizeInline(issue);

            if (!String.IsNullOrWhiteSpace(normalized) && !_issues.Contains(normalized, StringComparer.OrdinalIgnoreCase))
            {
                _issues.Add(normalized);
            }

            return this;
        }

        public IEntityRagContentBuilder AddIssueIf(bool condition, string issue)
        {
            if (condition)
            {
                AddIssue(issue);
            }

            return this;
        }

        public EntityRagContent Build()
        {
            ApplyStandardIssues();

            var issues = _issues.Any() ? String.Join(" ", _issues) : null;

            Payload.Meta.HasIssues = !String.IsNullOrWhiteSpace(issues);
            Payload.Extra.ShortSummary = BuildShortSummary();

            var content = new EntityRagContent
            {
                Payload = Payload,
                SummarizeModelForEmbeddings = false,
                EmbeddingContent = BuildEmbeddingContent(),
                ModelContent = BuildModelContent(),
                HumanContent = BuildHumanContent(),
                Issues = issues
            };

            var primaryPointId = RagVectorPayload.BuildPointId(Payload.Meta.DocId, Payload.Meta.SectionKey, Payload.Meta.PartIndex);

            content.ReferenceContents = EntityRagReferenceFactory.Create(Entity, Payload, primaryPointId);

            return content;
        }

        private void ApplyStandardPayload()
        {
            Payload.Meta.SourceObjectId = Entity.Id;
            Payload.Meta.Stage = Entity.GetRagReadinessStage();

            EntityRagLabelHelper.AddEntityLabels(Payload, Entity.Labels);
            EntityRagLabelHelper.AddLabel(Payload, $"entity-type:{RagReferenceNameHelper.ToKebabCase(Entity.EntityType ?? Entity.GetType().Name)}");

            if (Entity.IsReadyFor(EntityReadinessCheckType.CoreDefinition))
            {
                EntityRagLabelHelper.AddLabel(Payload, "readiness:core-definition-ready");
            }

            if (Entity.IsReadyFor(EntityReadinessCheckType.EntityDefinition))
            {
                EntityRagLabelHelper.AddLabel(Payload, "readiness:entity-definition-ready");
            }

            if (Entity.IsReadyFor(EntityReadinessCheckType.RelationshipReadiness))
            {
                EntityRagLabelHelper.AddLabel(Payload, "readiness:relationship-ready");
            }

            if (Entity.IsDraft)
            {
                EntityRagLabelHelper.AddLabel(Payload, "lifecycle:draft");
            }

            if (Entity.IsDeprecated)
            {
                EntityRagLabelHelper.AddLabel(Payload, "lifecycle:deprecated");
            }

            if (Entity.IsDeleted == true)
            {
                EntityRagLabelHelper.AddLabel(Payload, "lifecycle:deleted");
            }
        }

        private void ApplyStandardIssues()
        {
            AddIssueIf(String.IsNullOrWhiteSpace(Entity.Name), "Name is missing.");
            AddIssueIf(String.IsNullOrWhiteSpace(Entity.Description), "Description is missing.");
            AddIssueIf(Entity.OwnerOrganization == null, "Owner organization is missing.");
            AddIssueIf(!Entity.IsReadyFor(EntityReadinessCheckType.CoreDefinition), "Core definition readiness is incomplete.");
        }

        private string BuildShortSummary()
        {
            if (!String.IsNullOrWhiteSpace(_explicitShortSummary))
            {
                return EntityRagText.NormalizeAndLimitMarkdown(_explicitShortSummary, ShortSummaryMaximumLength);
            }

            var entries = new List<ShortSummaryEntry>();

            entries.AddRange(_shortSummarySections.Select(item => new ShortSummaryEntry(item.Priority, builder =>
            {
                var content = EntityRagText.NormalizeInline(item.Content);

                if (!String.IsNullOrWhiteSpace(content))
                {
                    builder.AppendLine(content);
                    builder.AppendLine();
                }
            })));

            entries.AddRange(_shortSummaryListSections.Select(item => new ShortSummaryEntry(item.Priority, builder =>
            {
                AppendCompactList(builder, item.Heading, item.Items, item.MaxItems);
            })));

            var summaryBuilder = new StringBuilder();

            foreach (var entry in entries.OrderByDescending(item => item.Priority))
            {
                entry.Append(summaryBuilder);
            }

            var result = EntityRagText.NormalizeAndLimitMarkdown(summaryBuilder.ToString(), ShortSummaryMaximumLength);

            if (!String.IsNullOrWhiteSpace(result))
            {
                return result;
            }

            return EntityRagText.NormalizeAndLimitMarkdown(Entity.Description ?? Entity.Name, ShortSummaryMaximumLength);
        }

        private string BuildEmbeddingContent()
        {
            var builder = new StringBuilder();

            foreach (var section in _embeddingSections.OrderByDescending(item => item.Priority))
            {
                EntityRagText.AppendSection(builder, section.Heading, section.Content);
            }

            return EntityRagText.NormalizeMarkdown(builder.ToString());
        }

        private string BuildModelContent()
        {
            var builder = new StringBuilder();

            AppendTitle(builder);
            AppendModelIdentity(builder);

            foreach (var entry in BuildOrderedEntries(_modelSections, _modelListSections))
            {
                entry.Append(builder);
            }

            return EntityRagText.NormalizeMarkdown(builder.ToString());
        }

        private string BuildHumanContent()
        {
            var builder = new StringBuilder();

            AppendTitle(builder);

            if (!String.IsNullOrWhiteSpace(Payload.Extra.ShortSummary))
            {
                builder.AppendLine(Payload.Extra.ShortSummary);
                builder.AppendLine();
            }

            foreach (var entry in BuildOrderedEntries(_humanSections, _humanListSections))
            {
                entry.Append(builder);
            }

            return EntityRagText.NormalizeMarkdown(builder.ToString());
        }

        private void AppendTitle(StringBuilder builder)
        {
            var title = EntityRagText.NormalizeInline(Entity.Name);

            if (String.IsNullOrWhiteSpace(title))
            {
                return;
            }

            builder.AppendLine($"# {title}");
            builder.AppendLine();
        }

        private void AppendModelIdentity(StringBuilder builder)
        {
            builder.AppendLine("## Identity");
            builder.AppendLine();
            builder.AppendLine($"- Entity type: {Entity.EntityType ?? Entity.GetType().Name}");
            builder.AppendLine($"- Entity ID: {Entity.Id}");

            if (!String.IsNullOrWhiteSpace(Entity.Key))
            {
                builder.AppendLine($"- Key: {Entity.Key}");
            }

            if (!String.IsNullOrWhiteSpace(Payload.Meta.Stage))
            {
                builder.AppendLine($"- Readiness stage: {Payload.Meta.Stage}");
            }

            if (Entity.OwnerOrganization != null)
            {
                builder.AppendLine($"- Owner organization: {FormatEntityHeader(Entity.OwnerOrganization, includeId: true)}");
            }

            if (Entity.Category != null)
            {
                builder.AppendLine($"- Category: {FormatEntityHeader(Entity.Category, includeId: true)}");
            }

            builder.AppendLine();

            EntityRagText.AppendSection(builder, "Description", Entity.Description);
        }

        private static IReadOnlyList<ContentEntry> BuildOrderedEntries(IEnumerable<EntityRagSection> sections, IEnumerable<EntityRagListSection> listSections)
        {
            var entries = new List<ContentEntry>();

            entries.AddRange(sections.Select(item => new ContentEntry(item.Priority, builder =>
            {
                EntityRagText.AppendSection(builder, item.Heading, item.Content);
            })));

            entries.AddRange(listSections.Select(item => new ContentEntry(item.Priority, builder =>
            {
                EntityRagText.AppendListSection(builder, item.Heading, item.Items, item.MaxItems);
            })));

            return entries
                .OrderByDescending(item => item.Priority)
                .ToList();
        }

        private static IEnumerable<string> FormatEntityHeaders(IEnumerable<EntityHeader> items, bool includeIds)
        {
            if (items == null)
            {
                return Array.Empty<string>();
            }

            return items
                .Where(item => item != null)
                .Select(item => FormatEntityHeader(item, includeIds))
                .Where(item => !String.IsNullOrWhiteSpace(item))
                .ToList();
        }

        private static string FormatEntityHeader(EntityHeader item, bool includeId)
        {
            if (item == null)
            {
                return null;
            }

            var text = EntityRagText.NormalizeInline(item.Text);

            if (String.IsNullOrWhiteSpace(text))
            {
                text = item.Id;
            }

            if (String.IsNullOrWhiteSpace(text))
            {
                return null;
            }

            if (!includeId || String.IsNullOrWhiteSpace(item.Id))
            {
                return text;
            }

            return $"{text} [{item.Id}]";
        }

        private static void AppendCompactList(StringBuilder builder, string heading, IEnumerable<string> items, int? maxItems)
        {
            var normalizedItems = EntityRagText.NormalizeList(items, maxItems);

            if (!normalizedItems.Any())
            {
                return;
            }

            if (!String.IsNullOrWhiteSpace(heading))
            {
                builder.AppendLine($"{EntityRagText.NormalizeInline(heading)}:");
            }

            foreach (var item in normalizedItems)
            {
                builder.AppendLine($"- {item}");
            }

            builder.AppendLine();
        }

        private sealed class ContentEntry
        {
            public ContentEntry(int priority, Action<StringBuilder> append)
            {
                Priority = priority;
                Append = append;
            }

            public int Priority { get; }

            public Action<StringBuilder> Append { get; }
        }

        private sealed class ShortSummaryEntry
        {
            public ShortSummaryEntry(int priority, Action<StringBuilder> append)
            {
                Priority = priority;
                Append = append;
            }

            public int Priority { get; }

            public Action<StringBuilder> Append { get; }
        }
    }
}