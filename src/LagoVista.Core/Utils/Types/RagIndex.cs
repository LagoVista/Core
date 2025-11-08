using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Utils.Types
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    namespace Nuviot.RagIndexing
    {
        public enum RagContentType
        {
            Unknown = 0,
            DomainDocument = 1,
            SourceCode = 2
        }

        /// <summary>
        /// Strongly typed metadata payload stored with each vector in Qdrant.
        /// </summary>
        public sealed class RagVectorPayload
        {
            // ---------- Identity / Tenant Isolation ----------
            public string OrgId { get; set; }
            public string ProjectId { get; set; }
            public string DocId { get; set; }

            // ---------- Content Classification ----------
            public RagContentType ContentTypeId { get; set; }
            public string ContentType { get; set; }
            public string Subtype { get; set; } // e.g., "UserGuide", "CSharp", etc.

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

            // ---------- Utility ----------
            public override string ToString()
            {
                return $"{OrgId}/{ProjectId}/{DocId} ({ContentType}) sec={SectionKey} p={PartIndex}/{PartTotal}";
            }

            /// <summary>
            /// Converts this payload to a dictionary containing only non-null / non-empty values.
            /// Ideal for uploading as Qdrant payload metadata.
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
                        if (!string.IsNullOrWhiteSpace(s))
                            dict[key] = s;
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
                Add("org_id", OrgId);
                Add("project_id", ProjectId);
                Add("doc_id", DocId);

                // --- Classification ---
                Add("content_type", ContentType.ToString());
                Add("subtype", Subtype);

                // --- Section ---
                Add("section_key", SectionKey);
                Add("part_index", PartIndex);
                Add("part_total", PartTotal);

                // --- Core Meta ---
                Add("title", Title);
                Add("language", Language);
                Add("priority", Priority);
                Add("audience", Audience);
                Add("persona", Persona);
                Add("stage", Stage);
                Add("label_slugs", LabelSlugs);
                Add("label_ids", LabelIds);

                // --- Raw pointers ---
                Add("blob_uri", BlobUri);
                Add("blob_version_id", BlobVersionId);
                Add("source_sha256", SourceSha256);
                Add("line_start", LineStart);
                Add("line_end", LineEnd);
                Add("char_start", CharStart);
                Add("char_end", CharEnd);
                Add("html_anchor", HtmlAnchor);
                Add("pdf_pages", PdfPages);


                // --- Index meta ---
                Add("index_version", IndexVersion);
                Add("embedding_model", EmbeddingModel);
                Add("content_hash", ContentHash);
                Add("chunk_size_tokens", ChunkSizeTokens);
                Add("overlap_tokens", OverlapTokens);
                Add("content_len_chars", ContentLenChars);
                Add("indexed_utc", IndexedUtc.ToString("o"));
                Add("updated_utc", UpdatedUtc?.ToString("o"));
                Add("source_system", SourceSystem);
                Add("source_object_id", SourceObjectId);

                // --- SourceCode meta ---
                Add("repo", Repo);
                Add("repo_branch", RepoBranch);
                Add("commit_sha", CommitSha);
                Add("path", Path);
                Add("symbol", Symbol);
                Add("symbolType", SymbolType);
                Add("start_line", StartLine);
                Add("end_line", EndLine);

                return dict;
            }


            public QdrantPoint ToQdrantPoint(string pointId, float[] embedding)
            {
                if (string.IsNullOrWhiteSpace(pointId)) throw new System.ArgumentException("pointId required", nameof(pointId));
                if (embedding == null || embedding.Length == 0) throw new System.ArgumentException("embedding required", nameof(embedding));

                return new QdrantPoint
                {
                    Id = pointId,
                    Vector = embedding,
                    Payload = this.ToDictionary()
                };
            }

            /// <summary>
            /// Build deterministic point IDs like "DOCID:sec:{section_key}#p{n}".
            /// </summary>
            public static string BuildPointId(string docId, string sectionKey, int partIndex)
            {
                if (string.IsNullOrWhiteSpace(docId)) throw new System.ArgumentException("docId required", nameof(docId));
                if (string.IsNullOrWhiteSpace(sectionKey)) sectionKey = "body";
                if (partIndex < 1) partIndex = 1;
                return docId + ":sec:" + Slug(sectionKey) + "#p" + partIndex;
            }

            // Simple slug helper (C# 8–friendly)
            private static string Slug(string s)
            {
                if (string.IsNullOrWhiteSpace(s)) return "body";
                var sb = new System.Text.StringBuilder(s.Length);
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
}