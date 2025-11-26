using System.Collections.Generic;
using System.Text;
using System;
using System.Collections;
using LagoVista.Core.Validation;

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

    /// <summary>
    /// Strongly typed metadata payload stored with each vector in Qdrant.
    /// Includes a SemanticId that acts as the natural key for the chunk
    /// (typically DocId + SectionKey + PartIndex).
    /// </summary>
    public sealed class RagVectorPayload
    {
        // ---------- Identity / Tenant Isolation ----------
        public string PointId { get; set; }
        public string OrgNamespace { get; set; }
        public string ProjectId { get; set; }
        public string DocId { get; set; }

        // ---------- Domain Classification ----------
        public string BusinessDomainKey { get; set; }     // e.g., "billing", "customers", "iot", "hr"
        public string BusinessDomainArea { get; set; }    // optional: e.g., "invoicing", "payments", "onboarding"


        // ---------- System CLassification ---------- IDX-007
        public string SysDomain { get; set; }   // e.g. Backend, UI, Integration
        public string SysLayer { get; set; }    // Primitive, Composite, Orchestration
        public string SysRole { get; set; }     // Similar to SubType

        // ---------- Semantic Identity ----------
        /// <summary>
        /// Deterministic, human-readable identity for this chunk.
        /// Typically built from DocId + SectionKey + PartIndex using BuildSemanticId.
        /// Used as a natural key for idempotent indexing, logging, and debugging.
        /// </summary>
        public string SemanticId { get; set; }

        // ---------- Content Classification ----------
        public RagContentType ContentTypeId { get; set; }
        /// <summary>
        /// Optional text label for the content type; if null/empty we fall back to ContentTypeId.ToString().
        /// </summary>
        public string ContentType { get => ContentTypeId.ToString(); }
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

        // ---------- Raw Source Pointers ----------
        public string BlobUri { get; set; }
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

        // ---------- Index / Embedding Metadata ----------
        public int IndexVersion { get; set; } = 1;
        public string EmbeddingModel { get; set; } = "text-embedding-3-large";
        public string ContentHash { get; set; }
        public int? ChunkSizeTokens { get; set; }
        public int? OverlapTokens { get; set; }
        public int? ContentLenChars { get; set; }
        public DateTime IndexedUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedUtc { get; set; }

        public string SourceSystem { get; set; }
        public string SourceObjectId { get; set; }

        // ---------- SourceCode Specific ----------
        public string Repo { get; set; }
        public string RepoBranch { get; set; }
        public string CommitSha { get; set; }
        public string Path { get; set; }
        public int? StartLine { get; set; }
        public int? EndLine { get; set; }

        public float[] Vectors { get; set; }

        // ---------- Utility ----------
        public override string ToString()
        {
            return $"{OrgNamespace}/{ProjectId}/{DocId} ({ContentType ?? ContentTypeId.ToString()}) sec={SectionKey} p={PartIndex}/{PartTotal}";
        }

        /// <summary>
        /// Validate and normalize the payload before indexing.
        /// Returns an InvokeResult that aggregates all errors and warnings.
        /// </summary>
        public InvokeResult ValidateForIndex()
        {
            var result = new InvokeResult();

            // --- Required identity ---
            if (string.IsNullOrWhiteSpace(OrgNamespace))
            {
                result.AddUserError("OrgId is required.");
            }

            if (string.IsNullOrWhiteSpace(ProjectId))
            {
                result.AddUserError("ProjectId is required.");
            }

            if (string.IsNullOrWhiteSpace(DocId))
            {
                result.AddUserError("DocId is required.");
            }

            // --- Content classification ---
            if (ContentTypeId == RagContentType.Unknown)
            {
                result.AddUserError("ContentTypeId must be specified and cannot be Unknown for indexed content.");
            }

            // --- Section / chunking ---
            if (string.IsNullOrWhiteSpace(SectionKey))
            {
                SectionKey = "body";
                result.AddWarning("SectionKey was empty; defaulted to 'body'.");
            }

            if (PartIndex < 1)
            {
                PartIndex = 1;
                result.AddWarning("PartIndex was less than 1; normalized to 1.");
            }

            if (PartTotal < PartIndex)
            {
                PartTotal = PartIndex;
                result.AddWarning("PartTotal was less than PartIndex; normalized to match PartIndex.");
            }

            if (string.IsNullOrWhiteSpace(BusinessDomainKey))
            {
                result.AddWarning("BusinessDomainKey is not set. Domain classification is strongly recommended for all indexed content.");
            }

            // --- Index metadata ---
            if (IndexVersion <= 0)
            {
                IndexVersion = 1;
                result.AddWarning("IndexVersion was not set or invalid; defaulted to 1.");
            }

            if (string.IsNullOrWhiteSpace(EmbeddingModel))
            {
                EmbeddingModel = "text-embedding-3-large";
                result.AddWarning("EmbeddingModel was empty; defaulted to 'text-embedding-3-large'.");
            }

            if (IndexedUtc == default)
            {
                IndexedUtc = DateTime.UtcNow;
                result.AddWarning("IndexedUtc was default; set to current UTC time.");
            }

            // --- Semantic identity ---
            if (string.IsNullOrWhiteSpace(SemanticId))
            {
                // Only generate SemanticId if we have enough information.
                if (!string.IsNullOrWhiteSpace(DocId))
                {
                    SemanticId = BuildSemanticId(DocId, SectionKey, PartIndex);
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
        /// Converts this payload to a dictionary containing only non-null / non-empty values.
        /// Ideal for uploading as Qdrant payload metadata.
        /// Uses PascalCase keys per DDR.
        /// </summary>
        public Dictionary<string, object> ToDictionary()
        {
            var dict = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

            Action<string, object> Add = (key, value) =>
            {
                if (value == null) return;

                // Handle strings
                var s = value as string;
                if (s != null)
                {
                    if (!string.IsNullOrWhiteSpace(s)) dict[key] = s;
                    return;
                }

                // Handle enumerable (lists, arrays) but skip string
                var enumerable = value as IEnumerable;
                if (enumerable != null)
                {
                    var list = new List<object>();
                    foreach (var v in enumerable)
                    {
                        if (v != null) list.Add(v);
                    }
                    if (list.Count > 0) dict[key] = list;
                    return;
                }

                // Handle value types or complex types
                dict[key] = value;
            };

            // --- Identity ---
            Add("OrgNsamepace", OrgNamespace);
            Add("ProjectId", ProjectId);
            Add("DocId", DocId);
            Add("SemanticId", SemanticId);

            // --- Domain Classification --
            Add("BusinessDomainKey", BusinessDomainKey);
            Add("BusinessDomainArea", BusinessDomainArea);

            // --- Classification ---
            Add("ContentTypeId", (int)ContentTypeId);
            var contentTypeName = !string.IsNullOrWhiteSpace(ContentType) ? ContentType : ContentTypeId.ToString();
            Add("ContentType", contentTypeName);
            Add("Subtype", Subtype);
            Add("SubtypeFlavor", SubtypeFlavor);

            // --- Section ---
            Add("SectionKey", SectionKey);
            Add("PartIndex", PartIndex);
            Add("PartTotal", PartTotal);

            // --- Core Meta ---
            Add("Title", Title);
            Add("Language", Language);
            Add("Priority", Priority);
            Add("Audience", Audience);
            Add("Persona", Persona);
            Add("Stage", Stage);
            Add("LabelSlugs", LabelSlugs);
            Add("LabelIds", LabelIds);

            // --- Raw pointers ---
            Add("BlobUri", BlobUri);
            Add("BlobVersionId", BlobVersionId);
            Add("SourceSha256", SourceSha256);
            Add("LineStart", LineStart);
            Add("LineEnd", LineEnd);
            Add("CharStart", CharStart);
            Add("CharEnd", CharEnd);
            Add("HtmlAnchor", HtmlAnchor);
            Add("PdfPages", PdfPages);

            // --- Index meta ---
            Add("IndexVersion", IndexVersion);
            Add("EmbeddingModel", EmbeddingModel);
            Add("ContentHash", ContentHash);
            Add("ChunkSizeTokens", ChunkSizeTokens);
            Add("OverlapTokens", OverlapTokens);
            Add("ContentLenChars", ContentLenChars);
            Add("IndexedUtc", IndexedUtc.ToString("o"));
            Add("UpdatedUtc", UpdatedUtc?.ToString("o"));
            Add("SourceSystem", SourceSystem);
            Add("SourceObjectId", SourceObjectId);

            // --- SourceCode meta ---
            Add("Repo", Repo);
            Add("RepoBranch", RepoBranch);
            Add("CommitSha", CommitSha);
            Add("Path", Path);
            Add("Symbol", Symbol);
            Add("SymbolType", SymbolType);
            Add("StartLine", StartLine);
            Add("EndLine", EndLine);

            return dict;
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
        /// This is a natural key and is separate from the Qdrant primary key,
        /// which may be a GUID or numeric identifier.
        /// </summary>
        public static string BuildSemanticId(string docId, string sectionKey, int partIndex)
        {
            if (string.IsNullOrWhiteSpace(docId)) throw new ArgumentException("docId required", nameof(docId));
            if (string.IsNullOrWhiteSpace(sectionKey)) sectionKey = "body";
            if (partIndex < 1) partIndex = 1;
            return docId + ":sec:" + Slug(sectionKey) + "#p" + partIndex;
        }

        /// <summary>
        /// Build deterministic point IDs like "DOCID:sec:{section_key}#p{n}".
        /// Kept for compatibility; in many cases Qdrant Ids will be GUIDs,
        /// while this value is also available via SemanticId.
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
    }
}
