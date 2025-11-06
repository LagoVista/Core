using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace LagoVista.Core.Utils
{
    // RagChunkBuilder.cs
    // RagChunkBuilder.cs (concise)

    public interface IRagIndexable
    {
        string OrgId { get; }
        string ProjectId { get; }
        string UniqueId { get; }          // DOCID
        string ContentSubtype { get; }

        string GetFrontMatter();
        IEnumerable<IndexSection> GetBodySections();

        IEnumerable<string> GetLabelSlugs();
        string Language { get; }
        int Priority { get; }
        string Audience { get; }
        string Persona { get; }
        string Stage { get; }
    }

    public sealed class IndexSection
    {
        public string Key { get; set; }
        public string Heading { get; set; }
        public string Text { get; set; }
    }

    public sealed class ChunkingOptions
    {
        public int TargetTokensPerChunk { get; set; } = 700;
        public int OverlapTokens { get; set; } = 80;
        public bool IncludeFrontMatter { get; set; } = true;
        public string VersionOrDate { get; set; } = null; // e.g. "2025-11-03" or commit SHA
        public string RawFileExtension { get; set; } = "txt"; // default if not inferred
    }

    public sealed class Chunk
    {
        public string PointId { get; set; }
        public string DocId { get; set; }
        public string SectionKey { get; set; }
        public int PartIndex { get; set; }     // 1-based
        public int PartTotal { get; set; }
        public string Title { get; set; }
        public string TextNormalized { get; set; }
        public int? LineStart { get; set; }    // optional pointer into raw text (1-based)
        public int? LineEnd { get; set; }      // optional pointer into raw text (inclusive)
        public int? CharStart { get; set; }
        public int? CharEnd { get; set; }
    }

    public sealed class RawArtifact
    {
        public string MimeType { get; set; }           // e.g., "text/markdown", "text/html"
        public string SuggestedBlobPath { get; set; }  // /{org}/{project}/{subtype}/{docId}/{ver}/source.ext
        public string SourceSha256 { get; set; }
        public bool IsText { get; set; }             // if true, Text is set; else Bytes/Stream provided elsewhere
        public string Text { get; set; }               // small docs only
    }

    public sealed class ChunkPlan
    {
        public string DocId { get; set; }
        public IReadOnlyList<Chunk> Chunks { get; set; }
        public RawArtifact Raw { get; set; }
    }

    public sealed class RawInput
    {
        public string MimeType { get; set; }      // if null, defaults to "text/plain"
        public string RawText { get; set; }       // preferred when source is text (markdown/html/plain)
                                                  // If you later add streaming/bytes, extend this class (kept minimal per your request)
    }


    public static class RagChunkBuilder
    {
        private static string ExtractTitle(string frontText)
        {
            if (string.IsNullOrWhiteSpace(frontText))
                return null;

            // Look for "TITLE:" prefix convention first
            foreach (var line in frontText.Split(new[] { "\r\n" }, StringSplitOptions.None))
            {
                var trimmed = line.Trim();
                if (trimmed.StartsWith("TITLE:", StringComparison.OrdinalIgnoreCase))
                    return trimmed.Substring(6).Trim();
            }

            // Otherwise first non-empty line is the title candidate
            var firstLine = frontText
                .Split(new[] { "\r\n" }, StringSplitOptions.None)
                .FirstOrDefault(l => !string.IsNullOrWhiteSpace(l));

            return string.IsNullOrWhiteSpace(firstLine) ? null : firstLine.Trim();
        }

        public static ChunkPlan Build(IRagIndexable doc, RawInput raw, ChunkingOptions opts = null)
        {
            opts = opts ?? new ChunkingOptions();
            var chunks = new List<Chunk>();
            var docId = doc.UniqueId;
            var allLines = new List<string>(); // only used for line pointers when raw text is present

            // Raw artifact (text case)
            var mime = string.IsNullOrWhiteSpace(raw?.MimeType) ? "text/plain" : raw.MimeType;
            var ext = GuessExtension(mime, opts.RawFileExtension);
            var ver = string.IsNullOrWhiteSpace(opts.VersionOrDate) ? DateStampUtc() : opts.VersionOrDate;
            var suggested = "/" + Safe(doc.OrgId) + "/" + Safe(doc.ProjectId)
                            + "/" + Safe(doc.ContentSubtype) + "/" + Safe(docId)
                            + "/" + Safe(ver) + "/source." + ext;

            string rawTextNorm = null;
            string sha = null;
            if (!string.IsNullOrEmpty(raw?.RawText))
            {
                rawTextNorm = Normalize(raw.RawText);
                sha = Sha256(rawTextNorm);
                // build line index for pointers (1-based)
                allLines = rawTextNorm.Replace("\r\n", "\n").Split('\n').ToList();
            }
            else
            {
                sha = Sha256(string.Empty);
            }

            int scanCharPos = 0;

            var rawArtifact = new RawArtifact
            {
                MimeType = mime,
                SuggestedBlobPath = suggested,
                SourceSha256 = sha,
                IsText = !string.IsNullOrEmpty(rawTextNorm),
                Text = rawTextNorm // small docs ok; for large sources you can null this before persisting the plan
            };


            // Front matter
            if (opts.IncludeFrontMatter)
            {
                var fm = Normalize(doc.GetFrontMatter());
                if (!string.IsNullOrWhiteSpace(fm))
                {
                    var c = new Chunk
                    {
                        DocId = docId,
                        SectionKey = "front",
                        PartIndex = 1,
                        PartTotal = 1,
                        Title = ExtractTitle(fm),
                        TextNormalized = fm,
                        PointId = BuildPointId(docId, "front", 1),
                        LineStart = null,
                        LineEnd = null
                    };

                    chunks.Add(c);
                }
            }

            // Body sections
            foreach (var s in doc.GetBodySections() ?? Enumerable.Empty<IndexSection>())
            {
                var key = Slug(string.IsNullOrWhiteSpace(s.Key) ? s.Heading : s.Key);
                var header = string.IsNullOrWhiteSpace(s.Heading) ? "" : "SECTION: " + s.Heading + "\r\n";
                var sectionText = Normalize(header + (s.Text ?? string.Empty));

                var sub = ChunkSplitter.SplitWithOverlap(sectionText, opts.TargetTokensPerChunk * 4, opts.OverlapTokens * 4);
                var total = Math.Max(1, sub.Count);

                // Optional line pointers (only meaningful if raw text exists and roughly matches this normalized text)
                // Here we compute pointers within the concatenated raw text using a simple scan.
                int scanPos = 0; // line index in allLines (0-based)
                for (int i = 0; i < total; i++)
                {
                    var part = sub[i];
                    var c = new Chunk
                    {
                        DocId = docId,
                        SectionKey = key,
                        PartIndex = i + 1,
                        PartTotal = total,
                        Title = s.Heading,
                        TextNormalized = part
                    };

                    c.PointId = BuildPointId(docId, key, c.PartIndex);

                    if (rawArtifact.IsText && allLines.Count > 0)
                    {
                        var partLines = part.Replace("\r\n", "\n").Split('\n');
                        // naive: try to match part’s first non-empty line forward from scanPos
                        var firstNonEmpty = partLines.FirstOrDefault(l => l.Length > 0);
                        int start = FindLineForward(allLines, firstNonEmpty, scanPos);
                        if (start >= 0)
                        {
                            c.LineStart = start + 1;
                            c.LineEnd = Math.Min(allLines.Count, start + Math.Max(1, partLines.Length));
                            scanPos = c.LineEnd.Value - 1;
                        }
                    }

                    // --- Character offsets against the normalized raw text (if available) ---
                    if (rawArtifact.IsText && rawTextNorm != null)
                    {
                        // If we injected a header ("SECTION: ...\r\n"), drop it for matching against raw
                        var toFind = c.TextNormalized;
                        int sep = toFind.IndexOf("\r\n", StringComparison.Ordinal);
                        if (sep >= 0 && toFind.StartsWith("SECTION:", StringComparison.OrdinalIgnoreCase))
                            toFind = toFind.Substring(sep + 2); // skip header line

                        // Try to find this chunk’s content in the normalized raw text, starting from our scan cursor
                        int idx = rawTextNorm.IndexOf(toFind, scanCharPos, StringComparison.Ordinal);
                        if (idx < 0 && toFind.Length > 64)
                        {
                            // Fallback: match on a middle slice to survive small drift
                            int mid = toFind.Length / 2;
                            int span = Math.Min(256, toFind.Length - mid);
                            string probe = toFind.Substring(mid, span);
                            int probeIdx = rawTextNorm.IndexOf(probe, scanCharPos, StringComparison.Ordinal);
                            if (probeIdx >= 0) idx = Math.Max(0, probeIdx - mid);
                        }

                        if (idx >= 0)
                        {
                            c.CharStart = idx;
                            c.CharEnd = idx + toFind.Length;

                            // advance cursor, leaving some overlap window so the next search tends forward
                            scanCharPos = Math.Max(scanCharPos, idx + Math.Max(1, toFind.Length - 512));
                        }
                        // else: leave CharStart/End null if we couldn’t confidently place it
                    }

                    chunks.Add(c);
                }
            }

            return new ChunkPlan { DocId = docId, Chunks = chunks, Raw = rawArtifact };
        }

        // helpers

        public static string BuildPointId(string docId, string sectionKey, int partIndex)
        {
            var s = Slug(sectionKey);
            var id = docId + ":sec:" + s;
            if (partIndex > 0) id += "#p" + partIndex;
            return id;
        }

        public static string Normalize(string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            var s = text.Replace("\r\n", "\n").Replace("\r", "\n").TrimEnd();
            return s.Replace("\n", "\r\n");
        }

        public static string Slug(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return "body";
            var sb = new StringBuilder(s.Length);
            foreach (var ch in s.ToLowerInvariant())
            {
                if ((ch >= 'a' && ch <= 'z') || (ch >= '0' && ch <= '9')) sb.Append(ch);
                else if (char.IsWhiteSpace(ch) || ch == '-' || ch == '_' || ch == '.' || ch == '/')
                { if (sb.Length == 0 || sb[sb.Length - 1] != '-') sb.Append('-'); }
            }
            var slug = sb.ToString().Trim('-');
            return string.IsNullOrEmpty(slug) ? "body" : slug;
        }

        // Simpler: split by lines with a fixed line overlap.
        // We convert approxChars/overlapChars into line counts using an estimated average line length.
        // Guarantees at least 3 overlapping lines between consecutive chunks.

        private static List<string> SplitWithOverlap(string text, int approxChars, int overlapChars)
        {
            if (string.IsNullOrEmpty(text)) return new List<string> { string.Empty };

            // Work with \n, restore CRLF at the end.
            var lines = text.Replace("\r\n", "\n").Split('\n');
            if (lines.Length <= 1) return new List<string> { text };

            // Estimate average line length (bounded) to convert char budgets → line counts.
            int totalLen = 0, nonEmpty = 0;
            foreach (var l in lines) { var ln = l.Length; totalLen += ln; if (ln > 0) nonEmpty++; }
            var avg = Math.Max(20, Math.Min(200, (nonEmpty > 0 ? totalLen / nonEmpty : 80))); // clamp

            int chunkLines = Math.Max(60, Math.Min(400, approxChars / avg));       // typical ~60–200 lines
            int overlapLines = Math.Max(3, Math.Min(chunkLines / 4, overlapChars / Math.Max(1, avg)));

            // If content is short, single chunk.
            if (lines.Length <= chunkLines) return new List<string> { text };

            var chunks = new List<string>();
            int step = Math.Max(1, chunkLines - overlapLines);

            for (int start = 0; start < lines.Length; start += step)
            {
                int end = Math.Min(lines.Length, start + chunkLines); // exclusive
                var slice = string.Join("\n", lines, start, end - start);
                chunks.Add(slice.Replace("\n", "\r\n"));
                if (end == lines.Length) break;
            }

            return chunks;
        }


        private static int FindLineForward(List<string> haystack, string needleLine, int startAt)
        {
            if (string.IsNullOrEmpty(needleLine)) return -1;
            for (int i = Math.Max(0, startAt); i < haystack.Count; i++)
                if (haystack[i] == needleLine) return i;
            return -1;
        }

        private static string GuessExtension(string mime, string fallback)
        {
            switch ((mime ?? "").ToLowerInvariant())
            {
                case "text/markdown": return "md";
                case "text/html": return "html";
                case "text/plain": return "txt";
                case "application/pdf": return "pdf";
                default: return string.IsNullOrWhiteSpace(fallback) ? "bin" : fallback;
            }
        }

        private static string DateStampUtc()
            => DateTime.UtcNow.ToString("yyyy-MM-dd");

        private static string Safe(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return "_";
            var sb = new StringBuilder(s.Length);
            foreach (var ch in s.ToLowerInvariant())
            {
                if ((ch >= 'a' && ch <= 'z') || (ch >= '0' && ch <= '9')) sb.Append(ch);
                else if (ch == '-' || ch == '_' || ch == '/') sb.Append(ch);
                else if (char.IsWhiteSpace(ch)) sb.Append('-');
                // else skip
            }
            var v = sb.ToString().Trim('-');
            return string.IsNullOrEmpty(v) ? "_" : v;
        }

        private static string Sha256(string text)
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