// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 841545c40df34bd9ca3ea4ab6b26881927e02a655ddff19c7a1ff507b921e4e5
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.Utils.Types.Nuviot.RagIndexing;
using System;
using System.Collections.Generic;

namespace LagoVista.Core.Utils.Types
{
    namespace Nuviot.RagIndexing
    {
        public static class RagVectorPayloadValidator
        {
            public sealed class ValidateOptions
            {
                // Require raw-source pointers (blob + offsets). Set false if you allow payload-only content.
                public bool RequireBlobPointer { get; set; } = true;

                // Enforce a bounded priority (inclusive range). Set nulls to skip.
                public int? MinPriority { get; set; } = 1;
                public int? MaxPriority { get; set; } = 5;

                // Require label IDs to be present when label slugs exist.
                public bool RequireLabelIdsWhenSlugsPresent { get; set; } = false;

                // Enforce that PartIndex/PartTotal are sane (>=1 and PartIndex <= PartTotal).
                public bool EnforcePartBounds { get; set; } = true;

                // ContentType-specific strictness
                public bool RequireCodeRepoFields { get; set; } = true; // for ContentType=SourceCode
            }

            public static List<string> Validate(RagVectorPayload p, ValidateOptions opt = null)
            {
                var errs = new List<string>();
                if (p == null) { errs.Add("Payload is null."); return errs; }
                opt = opt ?? new ValidateOptions();

                var m = p.Meta;
                var x = p.Extra;

                if (m == null) { errs.Add("meta is required."); return errs; }
                if (x == null) x = new RagVectorPayloadExtra(); // allow null extra but avoid null refs

                // ---------- Identity / tenancy ----------
                RequireNonEmpty(m.OrgNamespace, "org_namespace", errs);
                // project_id may be optional in some installs
                RequireNonEmpty(m.DocId, "doc_id", errs);

                // ---------- Classification ----------
                if (m.ContentTypeId == RagContentType.Unknown)
                    errs.Add("content_type must be DomainDocument or Code.");
                RequireNonEmpty(m.Subtype, "subtype", errs);

                // ---------- Sectioning ----------
                RequireNonEmpty(m.SectionKey, "section_key", errs);
                if (opt.EnforcePartBounds)
                {
                    if (m.PartIndex < 1) errs.Add("part_index must be >= 1.");
                    if (m.PartTotal < 1) errs.Add("part_total must be >= 1.");
                    if (m.PartIndex > 0 && m.PartTotal > 0 && m.PartIndex > m.PartTotal)
                        errs.Add("part_index cannot be greater than part_total.");
                }

                // ---------- Core metadata ----------
                // Language is optional, but if present, sanity check length
                if (!string.IsNullOrWhiteSpace(m.Language) && m.Language.Length > 20)
                    errs.Add("language looks too long (expected like 'en-US').");

                // Priority range
                if (opt.MinPriority.HasValue && m.Priority < opt.MinPriority.Value)
                    errs.Add($"priority must be >= {opt.MinPriority.Value}.");
                if (opt.MaxPriority.HasValue && m.Priority > opt.MaxPriority.Value)
                    errs.Add($"priority must be <= {opt.MaxPriority.Value}.");

                if (opt.RequireLabelIdsWhenSlugsPresent &&
                    m.LabelSlugs != null && m.LabelSlugs.Count > 0 &&
                    (m.LabelIds == null || m.LabelIds.Count != m.LabelSlugs.Count))
                {
                    errs.Add("label_ids must be present and match label_slugs count when slugs are provided.");
                }

                // ---------- Raw pointers / provenance ----------
                if (opt.RequireBlobPointer)
                {
                    RequireNonEmpty(x.FullDocumentBlobUri, "blob_uri", errs);

                    // If any offsets are present, check ordering
                    ValidateRange(x.LineStart, x.LineEnd, "line_start", "line_end", errs, min: 1);
                    ValidateRange(x.CharStart, x.CharEnd, "char_start", "char_end", errs, min: 0);
                }

                // SHA checks (64 hex chars typical for sha256)
                if (!string.IsNullOrWhiteSpace(x.SourceSha256) && !LooksLikeHexOfLen(x.SourceSha256, 64))
                    errs.Add("source_sha256 must be 64 hex chars (sha256).");

                // ---------- Index / embedding ----------
                if (m.IndexVersion < 1) errs.Add("index_version must be >= 1.");
                RequireNonEmpty(m.EmbeddingModel, "embedding_model", errs);

                // content_hash should exist for dedup/incremental
                RequireNonEmpty(m.ContentHash, "content_hash", errs);
                if (!string.IsNullOrWhiteSpace(m.ContentHash) && !LooksLikeHexOfLen(m.ContentHash, 64))
                    errs.Add("content_hash must be 64 hex chars (sha256 of normalized chunk text).");

                if (m.ChunkSizeTokens.HasValue && m.ChunkSizeTokens.Value <= 0)
                    errs.Add("chunk_size_tokens must be > 0 when provided.");
                if (m.OverlapTokens.HasValue && m.OverlapTokens.Value < 0)
                    errs.Add("overlap_tokens cannot be negative.");
                if (m.ContentLenChars.HasValue && m.ContentLenChars.Value < 0)
                    errs.Add("content_len_chars cannot be negative.");

                // IndexedUtc must not be default
                if (m.IndexedUtc == default(DateTime))
                    errs.Add("indexed_utc must be set (UTC).");

                // ---------- SourceCode-specific ----------
                if (m.ContentTypeId == RagContentType.SourceCode && opt.RequireCodeRepoFields)
                {
                    RequireNonEmpty(x.Repo, "repo", errs);
                    RequireNonEmpty(x.Path, "path", errs);
                    RequireNonEmpty(x.CommitSha, "commit_sha", errs);

                    // basic commit sha sanity (7+ hex chars typical)
                    if (!string.IsNullOrWhiteSpace(x.CommitSha) && !LooksLikeHexOfMinLen(x.CommitSha, 7))
                        errs.Add("commit_sha should be hex (>=7 chars).");

                    // start/end line ordering if provided
                    ValidateRange(x.StartLine, x.EndLine, "start_line", "end_line", errs, min: 1);
                }

                return errs;
            }

            // ---------- helpers ----------

            private static void RequireNonEmpty(string value, string field, List<string> errs)
            {
                if (string.IsNullOrWhiteSpace(value))
                    errs.Add(field + " is required.");
            }

            private static void ValidateRange(int? start, int? end, string startName, string endName, List<string> errs, int min)
            {
                if (start.HasValue && start.Value < min)
                    errs.Add(startName + " must be >= " + min + ".");
                if (end.HasValue && end.Value < min)
                    errs.Add(endName + " must be >= " + min + ".");
                if (start.HasValue && end.HasValue && end.Value < start.Value)
                    errs.Add(endName + " must be >= " + startName + ".");
            }

            private static bool LooksLikeHexOfLen(string s, int exactLen)
            {
                if (string.IsNullOrEmpty(s) || s.Length != exactLen) return false;
                for (int i = 0; i < s.Length; i++)
                {
                    char c = s[i];
                    bool hex = (c >= '0' && c <= '9') ||
                               (c >= 'a' && c <= 'f') ||
                               (c >= 'A' && c <= 'F');
                    if (!hex) return false;
                }
                return true;
            }

            private static bool LooksLikeHexOfMinLen(string s, int minLen)
            {
                if (string.IsNullOrEmpty(s) || s.Length < minLen) return false;
                for (int i = 0; i < s.Length; i++)
                {
                    char c = s[i];
                    bool hex = (c >= '0' && c <= '9') ||
                               (c >= 'a' && c <= 'f') ||
                               (c >= 'A' && c <= 'F');
                    if (!hex) return false;
                }
                return true;
            }
        }
    }
}
