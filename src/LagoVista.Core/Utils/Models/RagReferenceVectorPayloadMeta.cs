using LagoVista.Core.Validation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LagoVista.Core.Utils.Types.Nuviot.RagIndexing
{
    public sealed class RagReferenceVectorPayloadMeta
    {
        // ---------- Identity / Tenant Isolation ----------

        public string OrgNamespace { get; set; }

        public string OrgId { get; set; }


        /// <summary>
        /// Canonical entity/document identifier. This should match the DocId
        /// placed on the primary RAG payload.
        /// </summary>
        public string DocId { get; set; }


        public bool IsReference { get; set; } = true;

        // ---------- Domain / Content Classification ----------

        public string BusinessDomainKey { get; set; }

        public RagContentType ContentTypeId { get; set; }

        public string ContentType { get; set; }

        public string Subtype { get; set; }

        public string SubtypeFlavor { get; set; }

        // ---------- Core Search Metadata ----------

        public string Title { get; set; }

        public string Stage { get; set; }

        public List<string> LabelSlugs { get; set; } = new List<string>();

        // ---------- Index / Embedding Metadata ----------

        public int IndexVersion { get; set; } = 1;

        public string EmbeddingModel { get; set; } = "text-embedding-3-large";

        public string ContentHash { get; set; }

        public DateTime IndexedUtc { get; set; } = DateTime.UtcNow;

        public long IndexedUnix { get; set; }

        public bool HasIssues { get; set; }

        public bool Deleted { get; set; }

        // ---------- Entity Relationship Filters ----------\
        public string ScopeType { get; set; }

        public string ScopeId { get; set; }
    }

    public sealed class RagReferenceVectorPayloadExtra
    {
        /// <summary>
        /// Stable entity-level summary copied from the primary payload.
        /// This gives retrieval results enough context to be grouped and
        /// evaluated before loading the full canonical record.
        /// </summary>
        public string ShortSummary { get; set; }

        public string ModelContentUrl { get; set; }
        public string HumanContentUrl { get; set; }
        public string IssuesContentUrl { get; set; }

    }

    /// <summary>
    /// Lightweight payload stored with a specialized retrieval vector.
    ///
    /// The reference vector is used to discover a canonical RAG record.
    /// It contains only the metadata required for filtering, authorization,
    /// grouping, diagnostics, and loading the primary point.
    /// </summary>
    public sealed class RagReferenceVectorPayload
    {
        public const string BucketMeta = RagVectorPayload.BucketMeta;
        public const string BucketExtra = RagVectorPayload.BucketExtra;

        public RagReferenceVectorPayloadMeta Meta { get; set; } = new RagReferenceVectorPayloadMeta();

        public RagReferenceVectorPayloadExtra Extra { get; set; } = new RagReferenceVectorPayloadExtra();

        public override string ToString()
        {
            return $"{Meta.OrgNamespace}/{Meta.ProjectId}/{Meta.DocId} ref={Meta.ReferenceType} field={Meta.SourceField}[{Meta.SourceIndex}]";
        }

        public InvokeResult ValidateForIndex()
        {
            var result = new InvokeResult();

            if (String.IsNullOrWhiteSpace(Meta.OrgId))
            {
                result.AddUserError("OrgId is required.");
            }

            if (String.IsNullOrWhiteSpace(Meta.ProjectId))
            {
                result.AddUserError("ProjectId is required.");
            }

            if (String.IsNullOrWhiteSpace(Meta.DocId))
            {
                result.AddUserError("DocId is required.");
            }

            if (String.IsNullOrWhiteSpace(Meta.SourceObjectId))
            {
                Meta.SourceObjectId = Meta.DocId;
                result.AddWarning("SourceObjectId was empty and was defaulted to DocId.");
            }

            if (String.IsNullOrWhiteSpace(Meta.PrimaryPointId))
            {
                result.AddUserError("PrimaryPointId is required.");
            }

            if (!Meta.IsReference)
            {
                Meta.IsReference = true;
                result.AddWarning("IsReference was false and was normalized to true.");
            }

            if (String.IsNullOrWhiteSpace(Meta.ReferenceType))
            {
                result.AddUserError("ReferenceType is required.");
            }

            if (String.IsNullOrWhiteSpace(Meta.SourceField))
            {
                result.AddUserError("SourceField is required.");
            }

            if (Meta.SourceIndex < 0)
            {
                result.AddUserError("SourceIndex cannot be less than zero.");
            }

            if (Meta.ContentTypeId == RagContentType.Unknown)
            {
                result.AddUserError("ContentTypeId must be specified and cannot be Unknown.");
            }

            if (String.IsNullOrWhiteSpace(Meta.ContentType))
            {
                Meta.ContentType = Meta.ContentTypeId.ToString();
            }

            if (Meta.IndexVersion <= 0)
            {
                Meta.IndexVersion = 1;
                result.AddWarning("IndexVersion was invalid and was defaulted to 1.");
            }

            if (String.IsNullOrWhiteSpace(Meta.EmbeddingModel))
            {
                Meta.EmbeddingModel = "text-embedding-3-large";
                result.AddWarning("EmbeddingModel was empty and was defaulted to text-embedding-3-large.");
            }

            if (Meta.IndexedUtc == default)
            {
                Meta.IndexedUtc = DateTime.UtcNow;
                result.AddWarning("IndexedUtc was empty and was set to the current UTC time.");
            }

            Meta.IndexedUnix = new DateTimeOffset(Meta.IndexedUtc).ToUnixTimeSeconds();

            if (String.IsNullOrWhiteSpace(Extra.ShortSummary))
            {
                result.AddWarning("ShortSummary is not set. The reference will require canonical lookup before it can be meaningfully evaluated.");
            }

            return result;
        }

        public Dictionary<string, object> ToDictionary()
        {
            Dictionary<string, object> BuildBucket(Action<Action<string, object>> fill)
            {
                var dictionary = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

                void Add(string key, object value)
                {
                    if (value == null)
                    {
                        return;
                    }

                    if (value is string stringValue)
                    {
                        if (!String.IsNullOrWhiteSpace(stringValue))
                        {
                            dictionary[key] = stringValue;
                        }

                        return;
                    }

                    if (value is IEnumerable enumerable && !(value is string))
                    {
                        var values = new List<object>();

                        foreach (var item in enumerable)
                        {
                            if (item != null)
                            {
                                values.Add(item);
                            }
                        }

                        if (values.Any())
                        {
                            dictionary[key] = values;
                        }

                        return;
                    }

                    dictionary[key] = value;
                }

                fill(Add);

                return dictionary.Any() ? dictionary : null;
            }

            var meta = BuildBucket(Add =>
            {
                // Identity / tenant
                Add(nameof(RagReferenceVectorPayloadMeta.OrgNamespace), Meta.OrgNamespace);
                Add(nameof(RagReferenceVectorPayloadMeta.OrgId), Meta.OrgId);
                Add(nameof(RagReferenceVectorPayloadMeta.ProjectId), Meta.ProjectId);
                Add(nameof(RagReferenceVectorPayloadMeta.DocId), Meta.DocId);
                Add(nameof(RagReferenceVectorPayloadMeta.SourceSystem), Meta.SourceSystem);
                Add(nameof(RagReferenceVectorPayloadMeta.SourceObjectId), Meta.SourceObjectId);

                // Reference identity
                Add(nameof(RagReferenceVectorPayloadMeta.PrimaryPointId), Meta.PrimaryPointId);
                Add(nameof(RagReferenceVectorPayloadMeta.IsReference), Meta.IsReference);
                Add(nameof(RagReferenceVectorPayloadMeta.ReferenceType), Meta.ReferenceType);
                Add(nameof(RagReferenceVectorPayloadMeta.SourceField), Meta.SourceField);
                Add(nameof(RagReferenceVectorPayloadMeta.SourceIndex), Meta.SourceIndex);

                // Classification
                Add(nameof(RagReferenceVectorPayloadMeta.BusinessDomainKey), Meta.BusinessDomainKey);
                Add(nameof(RagReferenceVectorPayloadMeta.ContentTypeId), (int)Meta.ContentTypeId);
                Add(nameof(RagReferenceVectorPayloadMeta.ContentType), Meta.ContentType);
                Add(nameof(RagReferenceVectorPayloadMeta.Subtype), Meta.Subtype);
                Add(nameof(RagReferenceVectorPayloadMeta.SubtypeFlavor), Meta.SubtypeFlavor);

                // Search metadata
                Add(nameof(RagReferenceVectorPayloadMeta.Title), Meta.Title);
                Add(nameof(RagReferenceVectorPayloadMeta.Stage), Meta.Stage);
                Add(nameof(RagReferenceVectorPayloadMeta.LabelSlugs), Meta.LabelSlugs);

                // Index metadata
                Add(nameof(RagReferenceVectorPayloadMeta.IndexVersion), Meta.IndexVersion);
                Add(nameof(RagReferenceVectorPayloadMeta.EmbeddingModel), Meta.EmbeddingModel);
                Add(nameof(RagReferenceVectorPayloadMeta.ContentHash), Meta.ContentHash);
                Add(nameof(RagReferenceVectorPayloadMeta.IndexedUtc), Meta.IndexedUtc.ToString("o"));
                Add(nameof(RagReferenceVectorPayloadMeta.IndexedUnix), Meta.IndexedUnix);
                Add(nameof(RagReferenceVectorPayloadMeta.HasIssues), Meta.HasIssues);
                Add(nameof(RagReferenceVectorPayloadMeta.Deleted), Meta.Deleted);

                // Entity relationship filters
                Add(nameof(RagReferenceVectorPayloadMeta.VirtualTeamMemberId), Meta.VirtualTeamMemberId);
                Add(nameof(RagReferenceVectorPayloadMeta.EssentialJobActivityId), Meta.EssentialJobActivityId);
                Add(nameof(RagReferenceVectorPayloadMeta.ArtifactTypeId), Meta.ArtifactTypeId);
                Add(nameof(RagReferenceVectorPayloadMeta.ArtifactId), Meta.ArtifactId);
                Add(nameof(RagReferenceVectorPayloadMeta.SopWorkItemId), Meta.SopWorkItemId);
                Add(nameof(RagReferenceVectorPayloadMeta.VtmMeetingId), Meta.VtmMeetingId);
                Add(nameof(RagReferenceVectorPayloadMeta.SopExecutionId), Meta.SopExecutionId);
                Add(nameof(RagReferenceVectorPayloadMeta.ScopeType), Meta.ScopeType);
                Add(nameof(RagReferenceVectorPayloadMeta.ScopeId), Meta.ScopeId);
                Add(nameof(RagReferenceVectorPayloadMeta.IsSample), Meta.IsSample);
                Add(nameof(RagReferenceVectorPayloadMeta.SampleKindId), Meta.SampleKindId);
            });

            var extra = BuildBucket(Add =>
            {
                Add(nameof(RagReferenceVectorPayloadExtra.ShortSummary), Extra.ShortSummary);
            });

            var root = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

            if (meta != null)
            {
                root[BucketMeta] = meta;
            }

            if (extra != null)
            {
                root[BucketExtra] = extra;
            }

            return root;
        }

        public QdrantPoint ToQdrantPoint(string pointId, float[] embedding)
        {
            if (String.IsNullOrWhiteSpace(pointId))
            {
                throw new ArgumentException("Point ID is required.", nameof(pointId));
            }

            if (embedding == null || embedding.Length == 0)
            {
                throw new ArgumentException("Embedding is required.", nameof(embedding));
            }

            return new QdrantPoint
            {
                Id = pointId,
                Vector = embedding,
                Payload = ToDictionary()
            };
        }

        public static RagReferenceVectorPayload FromPrimary(RagVectorPayload primary, string primaryPointId, string referenceType, string sourceField, int sourceIndex)
        {
            if (primary == null)
            {
                throw new ArgumentNullException(nameof(primary));
            }

            if (String.IsNullOrWhiteSpace(primaryPointId))
            {
                throw new ArgumentException("Primary point ID is required.", nameof(primaryPointId));
            }

            if (String.IsNullOrWhiteSpace(referenceType))
            {
                throw new ArgumentException("Reference type is required.", nameof(referenceType));
            }

            if (String.IsNullOrWhiteSpace(sourceField))
            {
                throw new ArgumentException("Source field is required.", nameof(sourceField));
            }

            if (sourceIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(sourceIndex), "Source index cannot be less than zero.");
            }

            var payload = new RagReferenceVectorPayload
            {
                Meta =
                {
                    OrgNamespace = primary.Meta.OrgNamespace,
                    OrgId = primary.Meta.OrgId,
                    ProjectId = primary.Meta.ProjectId,
                    DocId = primary.Meta.DocId,
                    SourceSystem = primary.Meta.SourceSystem,
                    SourceObjectId = primary.Meta.SourceObjectId,

                    PrimaryPointId = primaryPointId,
                    IsReference = true,
                    ReferenceType = referenceType,
                    SourceField = sourceField,
                    SourceIndex = sourceIndex,

                    BusinessDomainKey = primary.Meta.BusinessDomainKey,
                    ContentTypeId = primary.Meta.ContentTypeId,
                    ContentType = primary.Meta.ContentType,
                    Subtype = primary.Meta.Subtype,
                    SubtypeFlavor = primary.Meta.SubtypeFlavor,

                    Title = primary.Meta.Title,
                    Stage = primary.Meta.Stage,
                    LabelSlugs = CopyDistinctStrings(primary.Meta.LabelSlugs),

                    IndexVersion = primary.Meta.IndexVersion,
                    EmbeddingModel = primary.Meta.EmbeddingModel,
                    HasIssues = primary.Meta.HasIssues,
                    Deleted = primary.Meta.Deleted,

                    VirtualTeamMemberId = primary.Meta.VirtualTeamMemberId,
                    EssentialJobActivityId = primary.Meta.EssentialJobActivityId,
                    ArtifactTypeId = primary.Meta.ArtifactTypeId,
                    ArtifactId = primary.Meta.ArtifactId,
                    SopWorkItemId = primary.Meta.SopWorkItemId,
                    VtmMeetingId = primary.Meta.VtmMeetingId,
                    SopExecutionId = primary.Meta.SopExecutionId,
                    ScopeType = primary.Meta.ScopeType,
                    ScopeId = primary.Meta.ScopeId,
                    IsSample = primary.Meta.IsSample,
                    SampleKindId = primary.Meta.SampleKindId
                },
                Extra =
                {
                    ShortSummary = primary.Extra.ShortSummary
                }
            };

            return payload;
        }

        public static string BuildPointId(string entityType, string entityId, string sourceField, int sourceIndex)
        {
            if (String.IsNullOrWhiteSpace(entityType))
            {
                throw new ArgumentException("Entity type is required.", nameof(entityType));
            }

            if (String.IsNullOrWhiteSpace(entityId))
            {
                throw new ArgumentException("Entity ID is required.", nameof(entityId));
            }

            if (String.IsNullOrWhiteSpace(sourceField))
            {
                throw new ArgumentException("Source field is required.", nameof(sourceField));
            }

            if (sourceIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(sourceIndex), "Source index cannot be less than zero.");
            }

            return $"{Slug(entityType)}:{Slug(entityId)}:{Slug(sourceField)}:{sourceIndex}";
        }

        private static List<string> CopyDistinctStrings(IEnumerable<string> values)
        {
            if (values == null)
            {
                return new List<string>();
            }

            return values
                .Where(value => !String.IsNullOrWhiteSpace(value))
                .Select(value => value.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private static string Slug(string value)
        {
            if (String.IsNullOrWhiteSpace(value))
            {
                return "unknown";
            }

            var builder = new StringBuilder(value.Length);
            var lower = value.Trim().ToLowerInvariant();

            foreach (var character in lower)
            {
                if ((character >= 'a' && character <= 'z') || (character >= '0' && character <= '9'))
                {
                    builder.Append(character);
                    continue;
                }

                if (Char.IsWhiteSpace(character) || character == '-' || character == '_' || character == '.' || character == '/')
                {
                    if (builder.Length > 0 && builder[builder.Length - 1] != '-')
                    {
                        builder.Append('-');
                    }
                }
            }

            var slug = builder.ToString().Trim('-');

            return String.IsNullOrWhiteSpace(slug) ? "unknown" : slug;
        }

        private static string MetaPath(string propertyName)
        {
            return $"{BucketMeta}.{propertyName}";
        }

        public static readonly IReadOnlyList<QdrantPayloadIndexSpec> Indexes = new List<QdrantPayloadIndexSpec>
        {
            // Tenant isolation
            new QdrantPayloadIndexSpec(MetaPath(nameof(RagReferenceVectorPayloadMeta.OrgNamespace)), QdrantPayloadIndexKind.Keyword),
            new QdrantPayloadIndexSpec(MetaPath(nameof(RagReferenceVectorPayloadMeta.OrgId)), QdrantPayloadIndexKind.Keyword),
            new QdrantPayloadIndexSpec(MetaPath(nameof(RagReferenceVectorPayloadMeta.ProjectId)), QdrantPayloadIndexKind.Keyword),

            // Canonical identity and reference addressing
            new QdrantPayloadIndexSpec(MetaPath(nameof(RagReferenceVectorPayloadMeta.DocId)), QdrantPayloadIndexKind.Keyword),
            new QdrantPayloadIndexSpec(MetaPath(nameof(RagReferenceVectorPayloadMeta.SourceObjectId)), QdrantPayloadIndexKind.Keyword),
            new QdrantPayloadIndexSpec(MetaPath(nameof(RagReferenceVectorPayloadMeta.PrimaryPointId)), QdrantPayloadIndexKind.Keyword),
            new QdrantPayloadIndexSpec(MetaPath(nameof(RagReferenceVectorPayloadMeta.IsReference)), QdrantPayloadIndexKind.Boolean),
            new QdrantPayloadIndexSpec(MetaPath(nameof(RagReferenceVectorPayloadMeta.ReferenceType)), QdrantPayloadIndexKind.Keyword),
            new QdrantPayloadIndexSpec(MetaPath(nameof(RagReferenceVectorPayloadMeta.SourceField)), QdrantPayloadIndexKind.Keyword),

            // Classification and state
            new QdrantPayloadIndexSpec(MetaPath(nameof(RagReferenceVectorPayloadMeta.ContentTypeId)), QdrantPayloadIndexKind.Integer),
            new QdrantPayloadIndexSpec(MetaPath(nameof(RagReferenceVectorPayloadMeta.Subtype)), QdrantPayloadIndexKind.Keyword),
            new QdrantPayloadIndexSpec(MetaPath(nameof(RagReferenceVectorPayloadMeta.SubtypeFlavor)), QdrantPayloadIndexKind.Keyword),
            new QdrantPayloadIndexSpec(MetaPath(nameof(RagReferenceVectorPayloadMeta.BusinessDomainKey)), QdrantPayloadIndexKind.Keyword),
            new QdrantPayloadIndexSpec(MetaPath(nameof(RagReferenceVectorPayloadMeta.Stage)), QdrantPayloadIndexKind.Keyword),
            new QdrantPayloadIndexSpec(MetaPath(nameof(RagReferenceVectorPayloadMeta.LabelSlugs)), QdrantPayloadIndexKind.Keyword),
            new QdrantPayloadIndexSpec(MetaPath(nameof(RagReferenceVectorPayloadMeta.HasIssues)), QdrantPayloadIndexKind.Boolean),
            new QdrantPayloadIndexSpec(MetaPath(nameof(RagReferenceVectorPayloadMeta.Deleted)), QdrantPayloadIndexKind.Boolean),

            // Index versioning
            new QdrantPayloadIndexSpec(MetaPath(nameof(RagReferenceVectorPayloadMeta.IndexVersion)), QdrantPayloadIndexKind.Integer),
            new QdrantPayloadIndexSpec(MetaPath(nameof(RagReferenceVectorPayloadMeta.IndexedUnix)), QdrantPayloadIndexKind.Integer),

            // Relationship filters
            new QdrantPayloadIndexSpec(MetaPath(nameof(RagReferenceVectorPayloadMeta.VirtualTeamMemberId)), QdrantPayloadIndexKind.Keyword),
            new QdrantPayloadIndexSpec(MetaPath(nameof(RagReferenceVectorPayloadMeta.EssentialJobActivityId)), QdrantPayloadIndexKind.Keyword),
            new QdrantPayloadIndexSpec(MetaPath(nameof(RagReferenceVectorPayloadMeta.ArtifactTypeId)), QdrantPayloadIndexKind.Keyword),
            new QdrantPayloadIndexSpec(MetaPath(nameof(RagReferenceVectorPayloadMeta.ArtifactId)), QdrantPayloadIndexKind.Keyword),
            new QdrantPayloadIndexSpec(MetaPath(nameof(RagReferenceVectorPayloadMeta.SopWorkItemId)), QdrantPayloadIndexKind.Keyword),
            new QdrantPayloadIndexSpec(MetaPath(nameof(RagReferenceVectorPayloadMeta.VtmMeetingId)), QdrantPayloadIndexKind.Keyword),
            new QdrantPayloadIndexSpec(MetaPath(nameof(RagReferenceVectorPayloadMeta.SopExecutionId)), QdrantPayloadIndexKind.Keyword),
            new QdrantPayloadIndexSpec(MetaPath(nameof(RagReferenceVectorPayloadMeta.ScopeType)), QdrantPayloadIndexKind.Keyword),
            new QdrantPayloadIndexSpec(MetaPath(nameof(RagReferenceVectorPayloadMeta.ScopeId)), QdrantPayloadIndexKind.Keyword),
            new QdrantPayloadIndexSpec(MetaPath(nameof(RagReferenceVectorPayloadMeta.IsSample)), QdrantPayloadIndexKind.Boolean),
            new QdrantPayloadIndexSpec(MetaPath(nameof(RagReferenceVectorPayloadMeta.SampleKindId)), QdrantPayloadIndexKind.Keyword)
        };
    }
}