using System;
using System.Collections.Generic;

namespace LagoVista.Core.Utils
{
    // ChunkSplitter.cs

    public static class ChunkSplitter
    {
        public static List<string> SplitWithOverlap(string text, int targetTokens, int overlapTokens)
        {
            var result = new List<string>();
            if (string.IsNullOrEmpty(text)) { result.Add(string.Empty); return result; }

            // Convert tokens → chars (conservative)
            int max = Math.Max(200, targetTokens * 4);
            int ovl = Math.Max(32, overlapTokens * 4); // ensure some overlap even on hard splits

            int start = 0;
            while (start < text.Length)
            {
                int end = Math.Min(text.Length, start + max);

                // If we’re not at the last chunk, try to back up to a nice boundary
                if (end < text.Length)
                {
                    int cut = FindCut(text, start, end, preferWithin: 120); // try to cut near the end
                    if (cut > start) end = cut;
                }

                // Emit chunk
                result.Add(text.Substring(start, end - start));

                if (end >= text.Length) break;

                // Next start with overlap
                int nextStart = end - ovl;
                if (nextStart <= start) nextStart = end; // safety: avoid infinite loop
                start = nextStart;
            }

            return result;
        }

        // Try to find a friendly cut before 'end' but close to it.
        // preferWithin = how many chars back from 'end' we search for punctuation/space/newline.
        private static int FindCut(string s, int start, int end, int preferWithin)
        {
            int lo = Math.Max(start + 1, end - preferWithin); // don’t cut at 'start'

            // 1) newline
            for (int i = end - 1; i >= lo; i--) if (s[i] == '\n' || s[i] == '\r') return i;

            // 2) sentence-ish punctuation
            for (int i = end - 1; i >= lo; i--) { char c = s[i]; if (c == '.' || c == '?' || c == '!' || c == ';' || c == ':') return i + 1; }

            // 3) whitespace
            for (int i = end - 1; i >= lo; i--) if (char.IsWhiteSpace(s[i])) return i + 1;

            // 4) give up → hard cut
            return end;
        }
    }
}
