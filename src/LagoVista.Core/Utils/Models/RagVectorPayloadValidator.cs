// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 841545c40df34bd9ca3ea4ab6b26881927e02a655ddff19c7a1ff507b921e4e5
// IndexVersion: 2
// --- END CODE INDEX META ---
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Utils.Types
{
    using System;
    using System.Collections.Generic;

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

                // ---------- Identity / tenancy ----------
                RequireNonEmpty(p.OrgNamespace, "org_namespace", errs);
                // project_id may be optional in some installs
                RequireNonEmpty(p.DocId, "doc_id", errs);

                // ---------- Classification ----------
                if (p.ContentTypeId == RagContentType.Unknown)
                    errs.Add("content_type must be DomainDocument or Code.");
                RequireNonEmpty(p.Subtype, "subtype", errs);

                // ---------- Sectioning ----------
                RequireNonEmpty(p.SectionKey, "section_key", errs);
                if (opt.EnforcePartBounds)
                {
                    if (p.PartIndex < 1) errs.Add("part_index must be >= 1.");
                    if (p.PartTotal < 1) errs.Add("part_total must be >= 1.");
                    if (p.PartIndex > 0 && p.PartTotal > 0 && p.PartIndex > p.PartTotal)
                        errs.Add("part_index cannot be greater than part_total.");
                }

                // ---------- Core metadata ----------
                // Language is optional, but if present, sanity check length
                if (!string.IsNullOrWhiteSpace(p.Language) && p.Language.Length > 20)
                    errs.Add("language looks too long (expected like 'en-US').");
                // Priority range
                if (opt.MinPriority.HasValue && p.Priority < opt.MinPriority.Value)
                    errs.Add($"priority must be >= {opt.MinPriority.Value}.");
                if (opt.MaxPriority.HasValue && p.Priority > opt.MaxPriority.Value)
                    errs.Add($"priority must be <= {opt.MaxPriority.Value}.");

                if (opt.RequireLabelIdsWhenSlugsPresent &&
                    p.LabelSlugs != null && p.LabelSlugs.Count > 0 &&
                    (p.LabelIds == null || p.LabelIds.Count != p.LabelSlugs.Count))
                {
                    errs.Add("label_ids must be present and match label_slugs count when slugs are provided.");
                }

                // ---------- Raw pointers / provenance ----------
                if (opt.RequireBlobPointer)
                {
                    RequireNonEmpty(p.BlobUri, "blob_uri", errs);
                    // If any offsets are present, check ordering
                    ValidateRange(p.LineStart, p.LineEnd, "line_start", "line_end", errs, min: 1);
                    ValidateRange(p.CharStart, p.CharEnd, "char_start", "char_end", errs, min: 0);
                }
                // SHA checks (64 hex chars typical for sha256)
                if (!string.IsNullOrWhiteSpace(p.SourceSha256) && !LooksLikeHexOfLen(p.SourceSha256, 64))
                    errs.Add("source_sha256 must be 64 hex chars (sha256).");

                // ---------- Index / embedding ----------
                if (p.IndexVersion < 1) errs.Add("index_version must be >= 1.");
                RequireNonEmpty(p.EmbeddingModel, "embedding_model", errs);

                // content_hash should exist for dedup/incremental
                RequireNonEmpty(p.ContentHash, "content_hash", errs);
                if (!string.IsNullOrWhiteSpace(p.ContentHash) && !LooksLikeHexOfLen(p.ContentHash, 64))
                    errs.Add("content_hash must be 64 hex chars (sha256 of normalized chunk text).");

                if (p.ChunkSizeTokens.HasValue && p.ChunkSizeTokens.Value <= 0)
                    errs.Add("chunk_size_tokens must be > 0 when provided.");
                if (p.OverlapTokens.HasValue && p.OverlapTokens.Value < 0)
                    errs.Add("overlap_tokens cannot be negative.");
                if (p.ContentLenChars.HasValue && p.ContentLenChars.Value < 0)
                    errs.Add("content_len_chars cannot be negative.");

                // IndexedUtc must not be default
                if (p.IndexedUtc == default(DateTime))
                    errs.Add("indexed_utc must be set (UTC).");

                // ---------- SourceCode-specific ----------
                if (p.ContentTypeId == RagContentType.SourceCode && opt.RequireCodeRepoFields)
                {
                    RequireNonEmpty(p.Repo, "repo", errs);
                    RequireNonEmpty(p.Path, "path", errs);
                    RequireNonEmpty(p.CommitSha, "commit_sha", errs);

                    // basic commit sha sanity (7+ hex chars typical)
                    if (!string.IsNullOrWhiteSpace(p.CommitSha) && !LooksLikeHexOfMinLen(p.CommitSha, 7))
                        errs.Add("commit_sha should be hex (>=7 chars).");

                    // start/end line ordering if provided
                    ValidateRange(p.StartLine, p.EndLine, "start_line", "end_line", errs, min: 1);
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
