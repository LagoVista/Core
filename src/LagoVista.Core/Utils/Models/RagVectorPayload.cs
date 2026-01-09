using LagoVista.Core.Validation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace LagoVista.Core.Utils.Types.Nuviot.RagIndexing
{
    public enum RagContentType
    {
        Unknown = 0,

        // Documents / Knowledge
        DomainDocument = 1,
        SourceCode = 2,

        Policy = 3,
        Procedure = 4,
        Reference = 5,

        // Source
        Configuration = 6,
        Infrastructure = 7,

        // System assets
        Schema = 8,
        ApiContract = 9,
        Spec = 10
    }

    public enum QdrantPayloadIndexKind
    {
        Keyword = 0,
        Integer = 1,
        Boolean = 2
    }

    public sealed class QdrantPayloadIndexSpec
    {
        public QdrantPayloadIndexSpec(string path, QdrantPayloadIndexKind kind)
        {
            Path = path ?? throw new ArgumentNullException(nameof(path));
            Kind = kind;
        }

        public string Path { get; }
        public QdrantPayloadIndexKind Kind { get; }
    }


    public sealed class RagVectorPayloadMeta
    {
        // ---------- Identity / Tenant Isolation ----------
        public string PointId { get; set; }
        public string OrgNamespace { get; set; }
        public string ProjectId { get; set; }
        public string DocId { get; set; }

        // ---------- Domain Classification ----------
        public string BusinessDomainKey { get; set; }     // e.g., "billing", "customers", "iot", "hr"
        public string BusinessDomainArea { get; set; }    // optional: e.g., "invoicing", "payments", "onboarding"

        // ---------- System Classification ----------
        public string SysDomain { get; set; }   // e.g. Backend, UI, Integration
        public string SysLayer { get; set; }    // Primitive, Composite, Orchestration
        public string SysRole { get; set; }     // Similar to SubType

        // ---------- Semantic Identity ----------
        public string SemanticId { get; set; }

        // ---------- Content Classification ----------
        public RagContentType ContentTypeId { get; set; }

        /// <summary>
        /// Text label for ContentTypeId. Stored explicitly alongside numeric ContentTypeId.
        /// </summary>
        public string ContentType { get; set; }

        public string Subtype { get; set; } // e.g., "UserGuide", "CSharp", etc.
        public string SubtypeFlavor { get; set; }

        // ---------- Section & Chunking ----------
        public string SectionKey { get; set; }
        public int PartIndex { get; set; }
        public int PartTotal { get; set; }

        // ---------- Core Metadata ----------
        public string Title { get; set; }
        public string Language { get; set; }
        public int Priority { get; set; } = 3;
        public string Audience { get; set; }
        public string Persona { get; set; }
        public string Stage { get; set; }
        public List<string> LabelSlugs { get; set; } = new List<string>();
        public List<string> LabelIds { get; set; } = new List<string>();

        // ---------- Index / Embedding Metadata ----------
        public int IndexVersion { get; set; } = 1;
        public string EmbeddingModel { get; set; } = "text-embedding-3-large";
        public string ContentHash { get; set; }
        public int? ChunkSizeTokens { get; set; }
        public int? OverlapTokens { get; set; }
        public int? ContentLenChars { get; set; }
        public DateTime IndexedUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedUtc { get; set; }

        public long IndexedUnix { get; set; }
        public long? UpdatedUnix { get; set; }

        public string SourceSystem { get; set; }
        public string SourceObjectId { get; set; }
    }

    public sealed class RagVectorPayloadExtra
    {
        // ---------- Raw pointers ----------
        public string FullDocumentBlobUri { get; set; }
        public string SourceSliceBlobUri { get; set; }
        public string DescriptionBlobUri { get; set; }

        public string BlobVersionId { get; set; }
        public string SourceSha256 { get; set; }

        public int? LineStart { get; set; }
        public int? LineEnd { get; set; }
        public int? CharStart { get; set; }
        public int? CharEnd { get; set; }

        public string Symbol { get; set; }
        public string SymbolType { get; set; }

        // Optional alternate locators
        public string HtmlAnchor { get; set; }
        public List<int> PdfPages { get; set; }

        // ---------- SourceCode Specific ----------
        public string Repo { get; set; }
        public string RepoBranch { get; set; }
        public string CommitSha { get; set; }
        public string Path { get; set; }
        public int? StartLine { get; set; }
        public int? EndLine { get; set; }

        public string EditorUrl { get; set; }
    }

    public sealed class RagVectorPayloadLenses
    {
        public string Embed { get; set; }
        public string Model { get; set; }
        public string User { get; set; }
        public string Cleanup { get; set; }
    }

    /// <summary>
    /// Strongly typed payload stored with each vector in Qdrant, with nested buckets:
    /// {
    ///   "meta": { ...filterable / canonical... },
    ///   "lenses": { ...generated texts... },
    ///   "extra": { ...non-filter helpers... }
    /// }
    ///
    /// Notes:
    /// - Vectors are NOT part of payload; they are uploaded separately as the point vector.
    /// - Validation rules intentionally mirror your original class.
    /// </summary>
    public sealed class RagVectorPayload
    {
        public const string BucketMeta = "meta";
        public const string BucketExtra = "extra";
        public const string BucketLenses = "lenses";

        public RagVectorPayloadMeta Meta { get; set; } = new RagVectorPayloadMeta();
        public RagVectorPayloadExtra Extra { get; set; } = new RagVectorPayloadExtra();
        public RagVectorPayloadLenses Lenses { get; set; } = new RagVectorPayloadLenses();

        /// <summary>
        /// Optional: hold vectors in-memory for pipeline convenience. Never emitted in payload.
        /// </summary>
        public float[] Vectors { get; set; }

        public override string ToString()
        {
            var ct = !string.IsNullOrWhiteSpace(Meta.ContentType)
                ? Meta.ContentType
                : (Meta.ContentTypeId != RagContentType.Unknown ? Meta.ContentTypeId.ToString() : "Unknown");

            return $"{Meta.OrgNamespace}/{Meta.ProjectId}/{Meta.DocId} ({ct}) sec={Meta.SectionKey} p={Meta.PartIndex}/{Meta.PartTotal}";
        }

        /// <summary>
        /// Validate and normalize the payload before indexing.
        /// Returns an InvokeResult that aggregates all errors and warnings.
        /// Business rules intentionally match the original version.
        /// </summary>
        public InvokeResult ValidateForIndex()
        {
            var result = new InvokeResult();

            // --- Required identity ---
            if (string.IsNullOrWhiteSpace(Meta.OrgNamespace))
            {
                result.AddUserError("OrgId is required.");
            }

            if (string.IsNullOrWhiteSpace(Meta.ProjectId))
            {
                result.AddUserError("ProjectId is required.");
            }

            if (string.IsNullOrWhiteSpace(Meta.DocId))
            {
                result.AddUserError("DocId is required.");
            }

            // --- Content classification ---
            if (Meta.ContentTypeId == RagContentType.Unknown)
            {
                result.AddUserError("ContentTypeId must be specified and cannot be Unknown for indexed content.");
            }

            // Keep the numeric id + string label strategy.
            if (string.IsNullOrWhiteSpace(Meta.ContentType))
            {
                Meta.ContentType = Meta.ContentTypeId.ToString();
            }

            // --- Section / chunking ---
            if (string.IsNullOrWhiteSpace(Meta.SectionKey))
            {
                Meta.SectionKey = "body";
                result.AddWarning("SectionKey was empty; defaulted to 'body'.");
            }

            if (Meta.PartIndex < 1)
            {
                Meta.PartIndex = 1;
                result.AddWarning("PartIndex was less than 1; normalized to 1.");
            }

            if (Meta.PartTotal < Meta.PartIndex)
            {
                Meta.PartTotal = Meta.PartIndex;
                result.AddWarning("PartTotal was less than PartIndex; normalized to match PartIndex.");
            }

            if (string.IsNullOrWhiteSpace(Meta.BusinessDomainKey))
            {
                result.AddWarning("BusinessDomainKey is not set. Domain classification is strongly recommended for all indexed content.");
            }

            // --- Index metadata ---
            if (Meta.IndexVersion <= 0)
            {
                Meta.IndexVersion = 1;
                result.AddWarning("IndexVersion was not set or invalid; defaulted to 1.");
            }

            if (string.IsNullOrWhiteSpace(Meta.EmbeddingModel))
            {
                Meta.EmbeddingModel = "text-embedding-3-large";
                result.AddWarning("EmbeddingModel was empty; defaulted to 'text-embedding-3-large'.");
            }

            if (Meta.IndexedUtc == default)
            {
                Meta.IndexedUtc = DateTime.UtcNow;
                result.AddWarning("IndexedUtc was default; set to current UTC time.");
            }

            if (Meta.IndexedUtc == default)
            {
                Meta.IndexedUtc = DateTime.UtcNow;
                result.AddWarning("IndexedUtc was default; set to current UTC time.");
            }

            // Always keep IndexedUnix in sync
            Meta.IndexedUnix = new DateTimeOffset(Meta.IndexedUtc).ToUnixTimeSeconds();

            if (Meta.UpdatedUtc.HasValue)
            {
                Meta.UpdatedUnix = new DateTimeOffset(Meta.UpdatedUtc.Value).ToUnixTimeSeconds();
            }
            else
            {
                Meta.UpdatedUnix = null;
            }

            // --- Semantic identity ---
            if (string.IsNullOrWhiteSpace(Meta.SemanticId))
            {
                // Only generate SemanticId if we have enough information.
                if (!string.IsNullOrWhiteSpace(Meta.DocId))
                {
                    Meta.SemanticId = BuildSemanticId(Meta.DocId, Meta.SectionKey, Meta.PartIndex);
                    result.AddWarning("SemanticId was not supplied; generated from DocId, SectionKey, and PartIndex.");
                }
                else
                {
                    result.AddUserError("SemanticId is missing and DocId is not available to generate one.");
                }
            }

            return result;
        }

        /// <summary>
        /// Converts this payload to a nested dictionary containing only non-null / non-empty values.
        /// Payload keys inside buckets use nameof(...) to avoid typos.
        /// Bucket names ("meta", "extra", "lenses") are literal.
        /// </summary>
        public Dictionary<string, object> ToDictionary()
        {
            Dictionary<string, object> BuildBucket(Action<Action<string, object>> fill)
            {
                var dict = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

                Action<string, object> Add = (key, value) =>
                {
                    if (value == null) return;

                    if (value is string s)
                    {
                        if (!string.IsNullOrWhiteSpace(s)) dict[key] = s;
                        return;
                    }

                    if (value is IEnumerable enumerable && !(value is string))
                    {
                        var list = new List<object>();
                        foreach (var v in enumerable)
                        {
                            if (v != null) list.Add(v);
                        }
                        if (list.Count > 0) dict[key] = list;
                        return;
                    }

                    dict[key] = value;
                };

                fill(Add);
                return dict.Count == 0 ? null : dict;
            }

            var meta = BuildBucket(Add =>
            {
                // Identity / tenant
                Add(nameof(RagVectorPayloadMeta.PointId), Meta.PointId);
                Add(nameof(RagVectorPayloadMeta.OrgNamespace), Meta.OrgNamespace);
                Add(nameof(RagVectorPayloadMeta.ProjectId), Meta.ProjectId);
                Add(nameof(RagVectorPayloadMeta.DocId), Meta.DocId);

                // Domain classification
                Add(nameof(RagVectorPayloadMeta.BusinessDomainKey), Meta.BusinessDomainKey);
                Add(nameof(RagVectorPayloadMeta.BusinessDomainArea), Meta.BusinessDomainArea);

                // System classification
                Add(nameof(RagVectorPayloadMeta.SysDomain), Meta.SysDomain);
                Add(nameof(RagVectorPayloadMeta.SysLayer), Meta.SysLayer);
                Add(nameof(RagVectorPayloadMeta.SysRole), Meta.SysRole);

                // Semantic identity
                Add(nameof(RagVectorPayloadMeta.SemanticId), Meta.SemanticId);

                // Content classification (keep both numeric id + string label)
                Add(nameof(RagVectorPayloadMeta.ContentTypeId), (int)Meta.ContentTypeId);
                Add(nameof(RagVectorPayloadMeta.ContentType), Meta.ContentType);
                Add(nameof(RagVectorPayloadMeta.Subtype), Meta.Subtype);
                Add(nameof(RagVectorPayloadMeta.SubtypeFlavor), Meta.SubtypeFlavor);

                // Section / chunking
                Add(nameof(RagVectorPayloadMeta.SectionKey), Meta.SectionKey);
                Add(nameof(RagVectorPayloadMeta.PartIndex), Meta.PartIndex);
                Add(nameof(RagVectorPayloadMeta.PartTotal), Meta.PartTotal);

                // Core metadata
                Add(nameof(RagVectorPayloadMeta.Title), Meta.Title);
                Add(nameof(RagVectorPayloadMeta.Language), Meta.Language);
                Add(nameof(RagVectorPayloadMeta.Priority), Meta.Priority);
                Add(nameof(RagVectorPayloadMeta.Audience), Meta.Audience);
                Add(nameof(RagVectorPayloadMeta.Persona), Meta.Persona);
                Add(nameof(RagVectorPayloadMeta.Stage), Meta.Stage);
                Add(nameof(RagVectorPayloadMeta.LabelSlugs), Meta.LabelSlugs);
                Add(nameof(RagVectorPayloadMeta.LabelIds), Meta.LabelIds);

                // Index / embedding metadata
                Add(nameof(RagVectorPayloadMeta.IndexVersion), Meta.IndexVersion);
                Add(nameof(RagVectorPayloadMeta.EmbeddingModel), Meta.EmbeddingModel);
                Add(nameof(RagVectorPayloadMeta.ContentHash), Meta.ContentHash);
                Add(nameof(RagVectorPayloadMeta.ChunkSizeTokens), Meta.ChunkSizeTokens);
                Add(nameof(RagVectorPayloadMeta.OverlapTokens), Meta.OverlapTokens);
                Add(nameof(RagVectorPayloadMeta.ContentLenChars), Meta.ContentLenChars);
                Add(nameof(RagVectorPayloadMeta.IndexedUtc), Meta.IndexedUtc.ToString("o"));
                Add(nameof(RagVectorPayloadMeta.UpdatedUtc), Meta.UpdatedUtc?.ToString("o"));
                Add(nameof(RagVectorPayloadMeta.SourceSystem), Meta.SourceSystem);
                Add(nameof(RagVectorPayloadMeta.SourceObjectId), Meta.SourceObjectId);
            });

            var extra = BuildBucket(Add =>
            {
                Add(nameof(RagVectorPayloadExtra.FullDocumentBlobUri), Extra.FullDocumentBlobUri);
                Add(nameof(RagVectorPayloadExtra.SourceSliceBlobUri), Extra.SourceSliceBlobUri);
                Add(nameof(RagVectorPayloadExtra.DescriptionBlobUri), Extra.DescriptionBlobUri);

                Add(nameof(RagVectorPayloadExtra.BlobVersionId), Extra.BlobVersionId);
                Add(nameof(RagVectorPayloadExtra.SourceSha256), Extra.SourceSha256);

                Add(nameof(RagVectorPayloadExtra.LineStart), Extra.LineStart);
                Add(nameof(RagVectorPayloadExtra.LineEnd), Extra.LineEnd);
                Add(nameof(RagVectorPayloadExtra.CharStart), Extra.CharStart);
                Add(nameof(RagVectorPayloadExtra.CharEnd), Extra.CharEnd);

                Add(nameof(RagVectorPayloadExtra.Symbol), Extra.Symbol);
                Add(nameof(RagVectorPayloadExtra.SymbolType), Extra.SymbolType);

                Add(nameof(RagVectorPayloadExtra.HtmlAnchor), Extra.HtmlAnchor);
                Add(nameof(RagVectorPayloadExtra.PdfPages), Extra.PdfPages);

                Add(nameof(RagVectorPayloadExtra.Repo), Extra.Repo);
                Add(nameof(RagVectorPayloadExtra.RepoBranch), Extra.RepoBranch);
                Add(nameof(RagVectorPayloadExtra.CommitSha), Extra.CommitSha);
                Add(nameof(RagVectorPayloadExtra.Path), Extra.Path);
                Add(nameof(RagVectorPayloadExtra.StartLine), Extra.StartLine);
                Add(nameof(RagVectorPayloadExtra.EndLine), Extra.EndLine);

                Add(nameof(RagVectorPayloadExtra.EditorUrl), Extra.EditorUrl);
            });

            var lenses = BuildBucket(Add =>
            {
                Add(nameof(RagVectorPayloadLenses.Embed), Lenses.Embed);
                Add(nameof(RagVectorPayloadLenses.Model), Lenses.Model);
                Add(nameof(RagVectorPayloadLenses.User), Lenses.User);
                Add(nameof(RagVectorPayloadLenses.Cleanup), Lenses.Cleanup);
            });

            var root = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            if (meta != null) root[BucketMeta] = meta;
            if (lenses != null) root[BucketLenses] = lenses;
            if (extra != null) root[BucketExtra] = extra;

            return root;
        }

        public QdrantPoint ToQdrantPoint(string pointId, float[] embedding)
        {
            if (string.IsNullOrWhiteSpace(pointId)) throw new ArgumentException("pointId required", nameof(pointId));
            if (embedding == null || embedding.Length == 0) throw new ArgumentException("embedding required", nameof(embedding));

            return new QdrantPoint
            {
                Id = pointId,
                Vector = embedding,
                Payload = ToDictionary()
            };
        }

        /// <summary>
        /// Build deterministic semantic IDs like "DOCID:sec:{section_key}#p{n}".
        /// </summary>
        public static string BuildSemanticId(string docId, string sectionKey, int partIndex)
        {
            if (string.IsNullOrWhiteSpace(docId)) throw new ArgumentException("docId required", nameof(docId));
            if (string.IsNullOrWhiteSpace(sectionKey)) sectionKey = "body";
            if (partIndex < 1) partIndex = 1;
            return docId + ":sec:" + Slug(sectionKey) + "#p" + partIndex;
        }

        /// <summary>
        /// Kept for compatibility with existing conventions.
        /// </summary>
        public static string BuildPointId(string docId, string sectionKey, int partIndex)
        {
            if (string.IsNullOrWhiteSpace(docId)) throw new ArgumentException("docId required", nameof(docId));
            if (string.IsNullOrWhiteSpace(sectionKey)) sectionKey = "body";
            if (partIndex < 1) partIndex = 1;
            return docId + ":sec:" + Slug(sectionKey) + "#p" + partIndex;
        }

        // Simple slug helper (C# 8–friendly)
        private static string Slug(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return "body";
            var sb = new StringBuilder(s.Length);
            var lower = s.ToLowerInvariant();
            for (int i = 0; i < lower.Length; i++)
            {
                char ch = lower[i];
                if ((ch >= 'a' && ch <= 'z') || (ch >= '0' && ch <= '9')) sb.Append(ch);
                else if (char.IsWhiteSpace(ch) || ch == '-' || ch == '_' || ch == '.' || ch == '/')
                {
                    if (sb.Length == 0 || sb[sb.Length - 1] != '-') sb.Append('-');
                }
            }
            var slug = sb.ToString().Trim('-');
            return string.IsNullOrEmpty(slug) ? "body" : slug;
        }

        /// <summary>
        /// Materialize from a Qdrant payload dictionary.
        /// Expects nested buckets: meta/extra/lenses. If missing, attempts flat reads as fallback.
        /// </summary>
        public static RagVectorPayload FromDictionary(IDictionary<string, object> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            static IDictionary<string, object> AsDict(object o)
                => o as IDictionary<string, object>;

            object GetRaw(IDictionary<string, object> dict, string key)
            {
                if (dict == null) return null;
                foreach (var kvp in dict)
                {
                    if (string.Equals(kvp.Key, key, StringComparison.OrdinalIgnoreCase))
                        return kvp.Value;
                }
                return null;
            }

            string GetString(IDictionary<string, object> dict, string key)
            {
                var raw = GetRaw(dict, key);
                if (raw == null) return null;
                if (raw is string s) return s;
                return raw.ToString();
            }

            int GetInt(IDictionary<string, object> dict, string key, int defaultValue = 0)
            {
                var raw = GetRaw(dict, key);
                if (raw == null) return defaultValue;

                if (raw is int i) return i;
                if (raw is long l) return (int)l;

                if (int.TryParse(raw.ToString(), out var v)) return v;
                return defaultValue;
            }

            int? GetNullableInt(IDictionary<string, object> dict, string key)
            {
                var raw = GetRaw(dict, key);
                if (raw == null) return null;

                if (raw is int i) return i;
                if (raw is long l) return (int)l;

                if (int.TryParse(raw.ToString(), out var v)) return v;
                return null;
            }

            DateTime GetDateTimeFromIso(IDictionary<string, object> dict, string key, DateTime defaultValue)
            {
                var raw = GetRaw(dict, key);
                if (raw == null) return defaultValue;

                if (raw is DateTime dt) return dt;

                if (DateTime.TryParse(
                        raw.ToString(),
                        null,
                        DateTimeStyles.RoundtripKind,
                        out var parsed))
                {
                    return parsed;
                }

                return defaultValue;
            }

            DateTime? GetNullableDateTimeFromIso(IDictionary<string, object> dict, string key)
            {
                var raw = GetRaw(dict, key);
                if (raw == null) return null;

                if (raw is DateTime dt) return dt;

                if (DateTime.TryParse(
                        raw.ToString(),
                        null,
                        DateTimeStyles.RoundtripKind,
                        out var parsed))
                {
                    return parsed;
                }

                return null;
            }

            List<string> GetStringList(IDictionary<string, object> dict, string key)
            {
                var raw = GetRaw(dict, key);
                if (raw == null) return new List<string>();

                if (raw is IEnumerable enumerable && !(raw is string))
                {
                    var list = new List<string>();
                    foreach (var item in enumerable)
                    {
                        if (item == null) continue;
                        list.Add(item.ToString());
                    }
                    return list;
                }

                return new List<string> { raw.ToString() };
            }

            List<int> GetIntList(IDictionary<string, object> dict, string key)
            {
                var raw = GetRaw(dict, key);
                if (raw == null) return null;

                if (raw is IEnumerable enumerable && !(raw is string))
                {
                    var list = new List<int>();
                    foreach (var item in enumerable)
                    {
                        if (item == null) continue;

                        if (item is int i) list.Add(i);
                        else if (item is long l) list.Add((int)l);
                        else if (int.TryParse(item.ToString(), out var v)) list.Add(v);
                    }
                    return list;
                }

                return null;
            }

            RagContentType GetContentTypeId(IDictionary<string, object> dict)
            {
                var raw = GetRaw(dict, nameof(RagVectorPayloadMeta.ContentTypeId));
                if (raw == null) return RagContentType.Unknown;

                if (raw is int i) return (RagContentType)i;
                if (raw is long l) return (RagContentType)(int)l;

                if (int.TryParse(raw.ToString(), out var v))
                    return (RagContentType)v;

                if (Enum.TryParse<RagContentType>(raw.ToString(), true, out var parsedEnum))
                    return parsedEnum;

                return RagContentType.Unknown;
            }

            // Buckets (preferred)
            var metaDict = AsDict(GetRaw(source, BucketMeta));
            var extraDict = AsDict(GetRaw(source, BucketExtra));
            var lensesDict = AsDict(GetRaw(source, BucketLenses));

            // Flat fallback: if no buckets exist, treat root as meta-ish
            var flatFallback = (metaDict == null && extraDict == null && lensesDict == null) ? source : null;

            IDictionary<string, object> M = metaDict ?? flatFallback;
            IDictionary<string, object> E = extraDict ?? flatFallback;
            IDictionary<string, object> L = lensesDict ?? flatFallback;

            var payload = new RagVectorPayload();

            // --- Meta
            payload.Meta.PointId = GetString(M, nameof(RagVectorPayloadMeta.PointId));
            payload.Meta.OrgNamespace = GetString(M, nameof(RagVectorPayloadMeta.OrgNamespace));
            payload.Meta.ProjectId = GetString(M, nameof(RagVectorPayloadMeta.ProjectId));
            payload.Meta.DocId = GetString(M, nameof(RagVectorPayloadMeta.DocId));

            payload.Meta.BusinessDomainKey = GetString(M, nameof(RagVectorPayloadMeta.BusinessDomainKey));
            payload.Meta.BusinessDomainArea = GetString(M, nameof(RagVectorPayloadMeta.BusinessDomainArea));

            payload.Meta.SysDomain = GetString(M, nameof(RagVectorPayloadMeta.SysDomain));
            payload.Meta.SysLayer = GetString(M, nameof(RagVectorPayloadMeta.SysLayer));
            payload.Meta.SysRole = GetString(M, nameof(RagVectorPayloadMeta.SysRole));

            payload.Meta.SemanticId = GetString(M, nameof(RagVectorPayloadMeta.SemanticId));

            payload.Meta.ContentTypeId = GetContentTypeId(M);
            payload.Meta.ContentType = GetString(M, nameof(RagVectorPayloadMeta.ContentType)) ?? payload.Meta.ContentTypeId.ToString();
            payload.Meta.Subtype = GetString(M, nameof(RagVectorPayloadMeta.Subtype));
            payload.Meta.SubtypeFlavor = GetString(M, nameof(RagVectorPayloadMeta.SubtypeFlavor));

            payload.Meta.SectionKey = GetString(M, nameof(RagVectorPayloadMeta.SectionKey));
            payload.Meta.PartIndex = GetInt(M, nameof(RagVectorPayloadMeta.PartIndex), 1);
            payload.Meta.PartTotal = GetInt(M, nameof(RagVectorPayloadMeta.PartTotal), 1);

            payload.Meta.Title = GetString(M, nameof(RagVectorPayloadMeta.Title));
            payload.Meta.Language = GetString(M, nameof(RagVectorPayloadMeta.Language));
            payload.Meta.Priority = GetInt(M, nameof(RagVectorPayloadMeta.Priority), 3);
            payload.Meta.Audience = GetString(M, nameof(RagVectorPayloadMeta.Audience));
            payload.Meta.Persona = GetString(M, nameof(RagVectorPayloadMeta.Persona));
            payload.Meta.Stage = GetString(M, nameof(RagVectorPayloadMeta.Stage));
            payload.Meta.LabelSlugs = GetStringList(M, nameof(RagVectorPayloadMeta.LabelSlugs));
            payload.Meta.LabelIds = GetStringList(M, nameof(RagVectorPayloadMeta.LabelIds));

            payload.Meta.IndexVersion = GetInt(M, nameof(RagVectorPayloadMeta.IndexVersion), 1);
            payload.Meta.EmbeddingModel = GetString(M, nameof(RagVectorPayloadMeta.EmbeddingModel)) ?? "text-embedding-3-large";
            payload.Meta.ContentHash = GetString(M, nameof(RagVectorPayloadMeta.ContentHash));
            payload.Meta.ChunkSizeTokens = GetNullableInt(M, nameof(RagVectorPayloadMeta.ChunkSizeTokens));
            payload.Meta.OverlapTokens = GetNullableInt(M, nameof(RagVectorPayloadMeta.OverlapTokens));
            payload.Meta.ContentLenChars = GetNullableInt(M, nameof(RagVectorPayloadMeta.ContentLenChars));
            payload.Meta.IndexedUtc = GetDateTimeFromIso(M, nameof(RagVectorPayloadMeta.IndexedUtc), DateTime.UtcNow);
            payload.Meta.UpdatedUtc = GetNullableDateTimeFromIso(M, nameof(RagVectorPayloadMeta.UpdatedUtc));
            payload.Meta.SourceSystem = GetString(M, nameof(RagVectorPayloadMeta.SourceSystem));
            payload.Meta.SourceObjectId = GetString(M, nameof(RagVectorPayloadMeta.SourceObjectId));

            // --- Extra
            payload.Extra.FullDocumentBlobUri = GetString(E, nameof(RagVectorPayloadExtra.FullDocumentBlobUri));
            payload.Extra.SourceSliceBlobUri = GetString(E, nameof(RagVectorPayloadExtra.SourceSliceBlobUri));
            payload.Extra.DescriptionBlobUri = GetString(E, nameof(RagVectorPayloadExtra.DescriptionBlobUri));

            payload.Extra.BlobVersionId = GetString(E, nameof(RagVectorPayloadExtra.BlobVersionId));
            payload.Extra.SourceSha256 = GetString(E, nameof(RagVectorPayloadExtra.SourceSha256));

            payload.Extra.LineStart = GetNullableInt(E, nameof(RagVectorPayloadExtra.LineStart));
            payload.Extra.LineEnd = GetNullableInt(E, nameof(RagVectorPayloadExtra.LineEnd));
            payload.Extra.CharStart = GetNullableInt(E, nameof(RagVectorPayloadExtra.CharStart));
            payload.Extra.CharEnd = GetNullableInt(E, nameof(RagVectorPayloadExtra.CharEnd));

            payload.Extra.Symbol = GetString(E, nameof(RagVectorPayloadExtra.Symbol));
            payload.Extra.SymbolType = GetString(E, nameof(RagVectorPayloadExtra.SymbolType));

            payload.Extra.HtmlAnchor = GetString(E, nameof(RagVectorPayloadExtra.HtmlAnchor));
            payload.Extra.PdfPages = GetIntList(E, nameof(RagVectorPayloadExtra.PdfPages));

            payload.Extra.Repo = GetString(E, nameof(RagVectorPayloadExtra.Repo));
            payload.Extra.RepoBranch = GetString(E, nameof(RagVectorPayloadExtra.RepoBranch));
            payload.Extra.CommitSha = GetString(E, nameof(RagVectorPayloadExtra.CommitSha));
            payload.Extra.Path = GetString(E, nameof(RagVectorPayloadExtra.Path));
            payload.Extra.StartLine = GetNullableInt(E, nameof(RagVectorPayloadExtra.StartLine));
            payload.Extra.EndLine = GetNullableInt(E, nameof(RagVectorPayloadExtra.EndLine));

            payload.Extra.EditorUrl = GetString(E, nameof(RagVectorPayloadExtra.EditorUrl));

            // --- Lenses
            payload.Lenses.Embed = GetString(L, nameof(RagVectorPayloadLenses.Embed));
            payload.Lenses.Model = GetString(L, nameof(RagVectorPayloadLenses.Model));
            payload.Lenses.User = GetString(L, nameof(RagVectorPayloadLenses.User));
            payload.Lenses.Cleanup = GetString(L, nameof(RagVectorPayloadLenses.Cleanup));

            return payload;
        }

        private static string MetaPath(string propertyName) => $"{RagVectorPayload.BucketMeta}.{propertyName}";

        public static readonly IReadOnlyList<QdrantPayloadIndexSpec> Indexes =
            new List<QdrantPayloadIndexSpec>
            {
            // Tenant / isolation
            new QdrantPayloadIndexSpec(MetaPath(nameof(RagVectorPayloadMeta.OrgNamespace)), QdrantPayloadIndexKind.Keyword),
            new QdrantPayloadIndexSpec(MetaPath(nameof(RagVectorPayloadMeta.ProjectId)), QdrantPayloadIndexKind.Keyword),

            // Document identity / chunk addressing
            new QdrantPayloadIndexSpec(MetaPath(nameof(RagVectorPayloadMeta.DocId)), QdrantPayloadIndexKind.Keyword),
            new QdrantPayloadIndexSpec(MetaPath(nameof(RagVectorPayloadMeta.SemanticId)), QdrantPayloadIndexKind.Keyword),

            // Classification
            new QdrantPayloadIndexSpec(MetaPath(nameof(RagVectorPayloadMeta.ContentTypeId)), QdrantPayloadIndexKind.Integer),
            new QdrantPayloadIndexSpec(MetaPath(nameof(RagVectorPayloadMeta.Subtype)), QdrantPayloadIndexKind.Keyword),
            new QdrantPayloadIndexSpec(MetaPath(nameof(RagVectorPayloadMeta.BusinessDomainKey)), QdrantPayloadIndexKind.Keyword),
            new QdrantPayloadIndexSpec(MetaPath(nameof(RagVectorPayloadMeta.SysDomain)), QdrantPayloadIndexKind.Keyword),
            new QdrantPayloadIndexSpec(MetaPath(nameof(RagVectorPayloadMeta.SysLayer)), QdrantPayloadIndexKind.Keyword),
            new QdrantPayloadIndexSpec(MetaPath(nameof(RagVectorPayloadMeta.SysRole)), QdrantPayloadIndexKind.Keyword),

            // Ranking helper
            new QdrantPayloadIndexSpec(MetaPath(nameof(RagVectorPayloadMeta.Priority)), QdrantPayloadIndexKind.Integer),

            // Labels/tags
            new QdrantPayloadIndexSpec(MetaPath(nameof(RagVectorPayloadMeta.LabelSlugs)), QdrantPayloadIndexKind.Keyword),
            new QdrantPayloadIndexSpec(MetaPath(nameof(RagVectorPayloadMeta.LabelIds)), QdrantPayloadIndexKind.Keyword),

            // Optional numeric date fields (if you add them)
            new QdrantPayloadIndexSpec(MetaPath(nameof(RagVectorPayloadMeta.IndexedUnix)), QdrantPayloadIndexKind.Integer),
            new QdrantPayloadIndexSpec(MetaPath(nameof(RagVectorPayloadMeta.UpdatedUnix)), QdrantPayloadIndexKind.Integer),
            };
    }

    // You already have these types in your project; included here just to show the shape.
    // Remove if duplicates exist elsewhere.
    public sealed class QdrantPoint
    {
        public string Id { get; set; }
        public float[] Vector { get; set; }
        public Dictionary<string, object> Payload { get; set; }
    }
}
