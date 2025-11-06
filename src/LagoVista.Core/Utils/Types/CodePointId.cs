using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Utils.Types
{
        public static class CodePointId
        {
            public static string Build(Guid fileDocId, string sectionKey, int partIndex)
            {
                var errs = RagPointId.ValidateForDocument(fileDocId.ToString(), sectionKey, partIndex);
                if (errs.Count > 0) throw new System.ArgumentException("Invalid code PointId: " + string.Join("; ", errs));
                return fileDocId.ToString() + ":sec:" + SafeSlug(sectionKey) + "#p" + partIndex;
            }

            private static string SafeSlug(string s)
            {
                // reuse RagPointId.Slug if available; otherwise a local, simple slug:
                return string.IsNullOrWhiteSpace(s) ? "body" : RagPointIdSlug(s);
            }

            private static string RagPointIdSlug(string s)
            {
                var lower = s.ToLowerInvariant();
                var sb = new System.Text.StringBuilder(lower.Length);
                char prev = '\0';
                for (int i = 0; i < lower.Length; i++)
                {
                    char ch = lower[i];
                    bool isAlnum = (ch >= 'a' && ch <= 'z') || (ch >= '0' && ch <= '9');
                    bool isSep = char.IsWhiteSpace(ch) || ch == '-' || ch == '_' || ch == '.' || ch == '/' || ch == '\\' || ch == ':';
                    if (isAlnum) { sb.Append(ch); prev = ch; }
                    else if (isSep) { if (prev != '-') { sb.Append('-'); prev = '-'; } }
                }
                var slug = sb.ToString().Trim('-');
                return string.IsNullOrEmpty(slug) ? "body" : slug;
            }
        }
    }
