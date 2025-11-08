// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 37fa677a9715437cb09abcdcf2e7a28e4c0656a6576f2be07c741778ce231168
// IndexVersion: 2
// --- END CODE INDEX META ---
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Utils.Types
{
    /// <summary>
    /// Deterministic PointId builders and input validators.
    /// </summary>
    public static class RagPointId
    {
        // --------------------
        // Public API
        // --------------------

        public static string BuildForDocument(string docId, string sectionKey, int partIndex)
        {
            var errs = ValidateForDocument(docId, sectionKey, partIndex);
            if (errs.Count > 0) throw new ArgumentException("Invalid document PointId: " + string.Join("; ", errs));

            return docId + ":sec:" + Slug(sectionKey) + "#p" + partIndex;
        }

        // Validate inputs that feed the ID (returns empty list if valid)
        public static System.Collections.Generic.List<string> ValidateForDocument(string docId, string sectionKey, int partIndex)
        {
            var errs = new System.Collections.Generic.List<string>();
            if (string.IsNullOrWhiteSpace(docId)) errs.Add("doc_id is required.");
            if (!LooksLikeGuid(docId)) errs.Add("doc_id should be a GUID-like string.");
            if (string.IsNullOrWhiteSpace(sectionKey)) errs.Add("section_key is required.");
            if (partIndex < 1) errs.Add("part_index must be >= 1.");
            return errs;
        }

        public static System.Collections.Generic.List<string> ValidateForCode(string repo, string commitSha, string path, string docId, string sectionKey, int partIndex)
        {
            var errs = ValidateForDocument(docId, sectionKey, partIndex);
            if (string.IsNullOrWhiteSpace(repo)) errs.Add("repo is required.");
            if (string.IsNullOrWhiteSpace(path)) errs.Add("path is required.");
            if (string.IsNullOrWhiteSpace(commitSha)) errs.Add("commit_sha is required.");
            else if (!LooksLikeHexMin(commitSha, 7)) errs.Add("commit_sha should be hex (>=7 chars).");
            return errs;
        }

        // --------------------
        // Helpers
        // --------------------

        private static bool LooksLikeGuid(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return false;
            Guid g;
            return Guid.TryParse(s, out g);
        }

        private static bool LooksLikeHexMin(string s, int minLen)
        {
            if (string.IsNullOrWhiteSpace(s) || s.Length < minLen) return false;
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

        // Lowercase, keep [a-z0-9], collapse separators to '-'
        private static string Slug(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return "body";
            var lower = s.ToLowerInvariant();
            var sb = new StringBuilder(lower.Length);
            char prev = '\0';
            for (int i = 0; i < lower.Length; i++)
            {
                char ch = lower[i];
                bool isAlnum = (ch >= 'a' && ch <= 'z') || (ch >= '0' && ch <= '9');
                bool isSep = char.IsWhiteSpace(ch) || ch == '-' || ch == '_' || ch == '.' || ch == '/' || ch == '\\' || ch == ':';

                if (isAlnum) { sb.Append(ch); prev = ch; }
                else if (isSep)
                {
                    if (prev != '-') { sb.Append('-'); prev = '-'; }
                }
                // else drop
            }
            var slug = sb.ToString().Trim('-');
            return string.IsNullOrEmpty(slug) ? "body" : slug;
        }
    }
}

