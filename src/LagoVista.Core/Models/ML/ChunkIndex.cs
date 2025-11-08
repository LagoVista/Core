// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: f1fe64786d7ba7d22de6ace2801ea65bbf003cb341f0f88dface32aa050b1925
// IndexVersion: 2
// --- END CODE INDEX META ---
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace LagoVista.Core.Models.ML
{
    public static class ContentTypes
    {
        public const string DomainDoc = "domain_doc";
        public const string Code = "code";
        public const string Label = "label";
        public const string Glossary = "glossary";
        public const string Html = "html";
    }

    /// <summary>
    /// Minimal “chunk” carrier for domain documents (guides, emails, personas, FAQs, etc.).
    /// </summary>
    public sealed class DomainDocChunk
    {
        // Required (tenant/identity)
        public string OrgId { get; set; }                  // tenant
        public string ProjectId { get; set; }              // can be null/empty
        public string DocId { get; set; }                  // your DOCID (GUID PK)
        public string Subtype { get; set; }                // e.g., "UserGuide","EmailTemplate","Persona","FAQ","ProductPage"
        public string Title { get; set; }                  // optional but useful for UI/search

        // Sectioning
        public string SectionKey { get; set; }             // slug like "configure-sensors" or "front"
        public int? SectionIndex { get; set; }             // optional ordering
        public int? PartIndex { get; set; }                // sub-chunk index within section (1-based)
        public int? PartTotal { get; set; }                // total parts for the section

        // Content
        public string TextNormalized { get; set; }         // normalized text used for embedding + hashing
        public string ContentForPayload { get; set; }      // only for small items (front matter), else null
        public string HtmlUri { get; set; }                // optional pointer to full HTML
        public string BlobUri { get; set; }                // optional pointer to large blob

        // Routing / boosts
        public string Language { get; set; } = "en-US";
        public int Priority { get; set; } = 1;
        public string Audience { get; set; }               // e.g., "RetailOwner"
        public string Persona { get; set; }                // e.g., "SmallRetailerOwner"
        public string Stage { get; set; }                  // e.g., "Onboarding"

        public IReadOnlyList<string> LabelSlugs { get; set; } = Array.Empty<string>();
        public IReadOnlyList<string> LabelIds { get; set; } = Array.Empty<string>(); // GUIDs as strings

        // Governance / diagnostics
        public int IndexVersion { get; set; } = 1;
        public string EmbeddingModel { get; set; } = "text-embedding-3-large";
        public int? ChunkSizeTokens { get; set; }
        public int? OverlapTokens { get; set; }
        public int? ContentLengthChars { get; set; }
        public string SourceSystem { get; set; } = "nuviot-app";
        public string SourceObjectId { get; set; }         // duplicate of DocId if useful
        public DateTimeOffset IndexedUtc { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? UpdatedUtc { get; set; }     // document's business updated time, optional
    }

    /// <summary>
    /// Minimal chunk carrier for code (C#, TypeScript, etc.).
    /// </summary>
    public sealed class CodeChunk
    {
        // Required (tenant/identity)
        public string OrgId { get; set; }                  // tenant
        public string ProjectId { get; set; }              // can be null/empty
        public string DocId { get; set; }                  // file GUID or your source object id
        public string Subtype { get; set; } = "CSharp";    // "CSharp","TypeScript", etc.
        public string Title { get; set; }                  // e.g., file name "OrderService.cs"

        // Repo context
        public string Repo { get; set; }                   // e.g., "github.com/acme/nuviot"
        public string RepoBranch { get; set; }             // e.g., "main"
        public string CommitSha { get; set; }              // pinned commit
        public string Path { get; set; }                   // "src/Services/OrderService.cs"
        public string Symbol { get; set; }                 // "Namespace.Type.Member" if available
        public int StartLine { get; set; }
        public int EndLine { get; set; }

        // Content
        public string TextNormalized { get; set; }         // normalized code chunk text (CRLF, header stripped, etc.)

        // Governance / diagnostics
        public int IndexVersion { get; set; } = 1;
        public string EmbeddingModel { get; set; } = "text-embedding-3-large";
        public int? ChunkSizeTokens { get; set; }
        public int? OverlapTokens { get; set; }
        public int? ContentLengthChars { get; set; }
        public string SourceSystem { get; set; } = "repo-indexer";
        public string SourceObjectId { get; set; }         // often the path
        public DateTimeOffset IndexedUtc { get; set; } = DateTimeOffset.UtcNow;
    }

    public static class PayloadBuilder
    {
        // -----------------------------
        // Public: payload builders
        // -----------------------------

        public static Dictionary<string, object> BuildDomainDocPayload(DomainDocChunk c)
        {
            var contentHash = Hashing.Sha256Hex(c.TextNormalized ?? string.Empty);

            var payload = new Dictionary<string, object>
            {
                ["org_id"] = c.OrgId,
                ["project_id"] = NullOr(c.ProjectId),

                ["content_type"] = ContentTypes.DomainDoc,
                ["subtype"] = c.Subtype,

                ["doc_id"] = c.DocId,
                ["title"] = NullOr(c.Title),

                ["label_ids"] = c.LabelIds?.ToArray() ?? Array.Empty<string>(),
                ["label_slugs"] = c.LabelSlugs?.ToArray() ?? Array.Empty<string>(),

                ["language"] = c.Language ?? "en-US",
                ["priority"] = c.Priority,
                ["audience"] = NullOr(c.Audience),
                ["persona"] = NullOr(c.Persona),
                ["stage"] = NullOr(c.Stage),

                ["section_key"] = c.SectionKey,               // e.g., "configure-sensors", "front"
                ["section_index"] = c.SectionIndex,
                ["part_index"] = c.PartIndex,
                ["part_total"] = c.PartTotal,

                ["html_uri"] = NullOr(c.HtmlUri),
                ["blob_uri"] = NullOr(c.BlobUri),

                // Small content only; omit for large bodies
                ["content"] = NullOr(c.ContentForPayload),

                // Governance / indexing
                ["index_version"] = c.IndexVersion,
                ["content_hash"] = contentHash,
                ["embedding_model"] = c.EmbeddingModel,
                ["chunk_size_tokens"] = c.ChunkSizeTokens,
                ["overlap_tokens"] = c.OverlapTokens,
                ["content_len_chars"] = c.ContentLengthChars,

                ["source_system"] = c.SourceSystem,
                ["source_object_id"] = NullOr(c.SourceObjectId ?? c.DocId),
                ["indexed_utc"] = c.IndexedUtc.UtcDateTime.ToString("o"),
                ["updated_utc"] = c.UpdatedUtc?.UtcDateTime.ToString("o")
            };

            return payload;
        }

        public static Dictionary<string, object> BuildCodePayload(CodeChunk c)
        {
            var contentHash = Hashing.Sha256Hex(c.TextNormalized ?? string.Empty);

            var payload = new Dictionary<string, object>
            {
                ["org_id"] = c.OrgId,
                ["project_id"] = NullOr(c.ProjectId),

                ["content_type"] = ContentTypes.Code,
                ["subtype"] = c.Subtype,

                ["doc_id"] = c.DocId,
                ["title"] = NullOr(c.Title),

                ["language"] = "en-US",
                ["priority"] = 1,

                // Repo context
                ["repo"] = c.Repo,
                ["repo_branch"] = c.RepoBranch,
                ["commit_sha"] = c.CommitSha,

                // SourceCode location
                ["path"] = c.Path,
                ["symbol"] = NullOr(c.Symbol),
                ["start_line"] = c.StartLine,
                ["end_line"] = c.EndLine,

                // Governance / indexing
                ["index_version"] = c.IndexVersion,
                ["content_hash"] = contentHash,
                ["embedding_model"] = c.EmbeddingModel,
                ["chunk_size_tokens"] = c.ChunkSizeTokens,
                ["overlap_tokens"] = c.OverlapTokens,
                ["content_len_chars"] = c.ContentLengthChars,

                ["source_system"] = c.SourceSystem,
                ["source_object_id"] = NullOr(c.SourceObjectId ?? c.Path),
                ["indexed_utc"] = c.IndexedUtc.UtcDateTime.ToString("o")
            };

            return payload;
        }

        /// <summary>
        /// Label definition payload (when seeding labels as first-class points).
        /// </summary>
        public static Dictionary<string, object> BuildLabelPayload(
            string orgId,
            string projectId,
            string name,
            string slug,
            string description,
            IEnumerable<string> synonyms,
            int weight = 0,
            bool isActive = true,
            string path = null,
            int indexVersion = 1)
        {
            return new Dictionary<string, object>
            {
                ["org_id"] = orgId,
                ["project_id"] = NullOr(projectId),

                ["content_type"] = ContentTypes.Label,
                ["name"] = name,
                ["slug"] = slug,
                ["description"] = description,
                ["synonyms"] = (synonyms ?? Array.Empty<string>()).ToArray(),
                ["weight"] = weight,
                ["is_active"] = isActive,
                ["path"] = NullOr(path),

                ["index_version"] = indexVersion,
                ["indexed_utc"] = DateTimeOffset.UtcNow.UtcDateTime.ToString("o")
            };
        }

        /// <summary>
        /// Glossary term payload (optional, if you store terms as vectors).
        /// </summary>
        public static Dictionary<string, object> BuildGlossaryPayload(
            string orgId,
            string projectId,
            string term,
            string definition,
            IEnumerable<string> aliases = null,
            int indexVersion = 1)
        {
            return new Dictionary<string, object>
            {
                ["org_id"] = orgId,
                ["project_id"] = NullOr(projectId),

                ["content_type"] = ContentTypes.Glossary,
                ["term"] = term,
                ["definition"] = definition,
                ["aliases"] = (aliases ?? Array.Empty<string>()).ToArray(),

                ["index_version"] = indexVersion,
                ["indexed_utc"] = DateTimeOffset.UtcNow.UtcDateTime.ToString("o")
            };
        }

        // -----------------------------
        // Utilities
        // -----------------------------

        public static string NormalizeText(string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;

            // Normalize line endings to CRLF, trim trailing whitespace lines.
            var s = text.Replace("\r\n", "\n").Replace("\r", "\n")
                        .TrimEnd();
            s = s.Replace("\n", "\r\n");
            return s;
        }

        /// <summary>
        /// Build a stable point Id for a domain doc “front” or section sub-part.
        /// Example: DOCID:front  or  DOCID:sec:configure-sensors#p2
        /// </summary>
        public static string BuildDomainDocPointId(string docId, string sectionKey, int? partIndex = null)
        {
            if (string.IsNullOrWhiteSpace(sectionKey)) sectionKey = "front";
            var id = $"{docId}:sec:{Slugify(sectionKey)}";
            if (partIndex.HasValue && partIndex.Value > 0) id += $"#p{partIndex.Value}";
            return id;
        }

        /// <summary>
        /// Build a stable point Id for code (you can also use the same sec/part scheme if you prefer).
        /// Example: DOCID:sec:order-service#p1
        /// </summary>
        public static string BuildCodePointId(string docId, string sectionKey, int partIndex)
            => $"{docId}:sec:{Slugify(sectionKey)}#p{partIndex}";

        public static string Slugify(string s)
        {
            if (string.IsNullOrEmpty(s)) return "body";
            var sb = new StringBuilder(s.Length);
            foreach (var ch in s.ToLowerInvariant())
            {
                if ((ch >= 'a' && ch <= 'z') || (ch >= '0' && ch <= '9')) sb.Append(ch);
                else if (char.IsWhiteSpace(ch) || ch == '-' || ch == '_' || ch == '.' || ch == '/')
                {
                    if (sb.Length > 0 && sb[sb.Length - 1] != '-') sb.Append('-');
                }
                // else skip punctuation
            }
            var slug = sb.ToString().Trim('-');
            return string.IsNullOrEmpty(slug) ? "body" : slug;
        }

        private static object NullOr(string s) => string.IsNullOrWhiteSpace(s) ? null : (object)s;
    }

    public static class Hashing
    {
        public static string Sha256Hex(string text)
        {
            using (var sha = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(text ?? string.Empty);
                var hash = sha.ComputeHash(bytes);
                var sb = new StringBuilder(hash.Length * 2);
                foreach (var b in hash) sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }
    }
}
