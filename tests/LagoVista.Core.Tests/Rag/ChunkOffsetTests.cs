// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: d2ed24cce56c3c398ebad187e5168d0377bb4e17bb79fa6fca94a5ab9409f884
// IndexVersion: 2
// --- END CODE INDEX META ---
// ChunkCharOffsetsTests.cs (NUnit 4.4)
using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using NUnit.Framework;
using LagoVista.Core.Utils;
using LagoVista.Core.Utils.Types;
using LagoVista.Core.Models;


namespace LagoVista.Core.Tests.Rag
{

    [TestFixture]
    public class ChunkCharOffsetsTests
    {
        // ---- helpers ----
        private sealed class TestDoc : IRagIndexable
        {
            public EntityHeader OwnerOrganization => EntityHeader.Create("803F1C881DB54495831415D53F48FA85", "Some Interesting org");
            public string Id { get; set; } = Guid.NewGuid().ToString();
            public string ContentSubtype { get; set; } = "UserGuide";
            public List<IndexSection> Sections { get; } = new();
            public string GetFrontMatter() => "TITLE: Offsets\r\nSUMMARY: test";
            public IEnumerable<IndexSection> GetBodySections() => Sections;
            public IEnumerable<string> GetLabelSlugs() => Array.Empty<string>();
            public string Language => "en-US";
            public int Priority => 3;
            public string Audience => null;
            public string Persona => null;
            public string Stage => null;
        }

        private static string MakeLines(int count, string prefix)
            => string.Join("\r\n", Enumerable.Range(1, count).Select(i => $"{prefix} line {i:D5} some words here."));

        // Strip the artificial header the builder may prepend (SECTION: Heading\r\n)
        private static string StripHeader(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;
            if (text.StartsWith("SECTION:", StringComparison.OrdinalIgnoreCase))
            {
                var idx = text.IndexOf("\r\n", StringComparison.Ordinal);
                if (idx >= 0) return text.Substring(idx + 2);
            }
            return text;
        }

        // ---- tests ----

        [Test]
        public void SingleLongLine_Offsets_ArePresent_AndExact()
        {
            // one huge line, no breaks
            var raw = new string('x', 12000); // > token target
            var doc = new TestDoc();
            // No heading to avoid injected header in chunk text
            doc.Sections.Add(new IndexSection { Key = "big", Heading = null, Text = raw });

            var plan = RagChunkBuilder.Build(
                doc,
                new RawInput { MimeType = "text/plain", RawText = raw },
                new ChunkingOptions { TargetTokensPerChunk = 600, OverlapTokens = 80, IncludeFrontMatter = false });

            var chunks = plan.Chunks.Where(c => c.SectionKey == "big").ToList();
            Assert.That(chunks.Count, Is.GreaterThan(1), "Should split very long single line");

            var rawNorm = RagChunkBuilder.Normalize(raw);
            foreach (var c in chunks)
            {
                Assert.That(c.CharStart, Is.Not.Null);
                Assert.That(c.CharEnd, Is.Not.Null);
                Assert.That(c.CharStart!.Value, Is.GreaterThanOrEqualTo(0));
                Assert.That(c.CharEnd!.Value, Is.GreaterThan(c.CharStart!.Value));
                Assert.That(c.CharEnd!.Value, Is.LessThanOrEqualTo(rawNorm.Length));

                var slice = rawNorm.Substring(c.CharStart.Value, c.CharEnd.Value - c.CharStart.Value);
                Assert.That(slice, Is.EqualTo(c.TextNormalized), "Chunk text must match raw slice exactly");
            }
        }

        [Test]
        public void MultiLine_WithOverlap_Offsets_Monotonic_AndWithinBounds()
        {
            var raw = MakeLines(1200, "alpha"); // many lines ⇒ multiple chunks
            var doc = new TestDoc();
            doc.Sections.Add(new IndexSection { Key = "ml", Heading = "Alpha Section", Text = raw });

            var plan = RagChunkBuilder.Build(
                doc,
                new RawInput { MimeType = "text/markdown", RawText = raw },
                new ChunkingOptions { TargetTokensPerChunk = 500, OverlapTokens = 80, IncludeFrontMatter = false });

            var chunks = plan.Chunks.Where(c => c.SectionKey == "ml").ToList();
            Assert.That(chunks.Count, Is.GreaterThanOrEqualTo(2));

            var rawNorm = RagChunkBuilder.Normalize(raw);
            int? prevStart = null, prevEnd = null;

            foreach (var c in chunks)
            {
                // Offsets exist and are in range
                Assert.That(c.CharStart, Is.Not.Null);
                Assert.That(c.CharEnd, Is.Not.Null);
                Assert.That(c.CharStart!.Value, Is.GreaterThanOrEqualTo(0));
                Assert.That(c.CharEnd!.Value, Is.GreaterThan(c.CharStart!.Value));
                Assert.That(c.CharEnd!.Value, Is.LessThanOrEqualTo(rawNorm.Length));

                // Offsets non-decreasing (allow overlap)
                if (prevStart.HasValue)
                    Assert.That(c.CharStart!.Value, Is.GreaterThanOrEqualTo(prevStart.Value));

                // Sanity: slice equals chunk (with header stripped if present)
                var expected = StripHeader(c.TextNormalized);
                var slice = rawNorm.Substring(c.CharStart.Value, c.CharEnd.Value - c.CharStart.Value);
                Assert.That(slice, Is.EqualTo(expected));

                prevStart = c.CharStart;
                prevEnd = c.CharEnd;
            }
        }

        [Test]
        public void HeaderInjected_Offsets_AreComputed_AfterHeader()
        {
            // raw has no header; builder will prepend "SECTION: Heading\r\n" into chunk text
            var body = MakeLines(200, "beta");
            var doc = new TestDoc();
            doc.Sections.Add(new IndexSection { Key = "steps", Heading = "Steps Heading", Text = body });

            var plan = RagChunkBuilder.Build(
                doc,
                new RawInput { MimeType = "text/plain", RawText = body },
                new ChunkingOptions { TargetTokensPerChunk = 300, OverlapTokens = 50, IncludeFrontMatter = false });

            var first = plan.Chunks.First(c => c.SectionKey == "steps");
            Assert.That(first.TextNormalized.StartsWith("SECTION:"), Is.True, "Header should be injected");

            var rawNorm = RagChunkBuilder.Normalize(body);
            var expected = StripHeader(first.TextNormalized);

            Assert.That(first.CharStart, Is.Not.Null);
            Assert.That(first.CharEnd, Is.Not.Null);
            var slice = rawNorm.Substring(first.CharStart!.Value, first.CharEnd!.Value - first.CharStart!.Value);
            Assert.That(slice, Is.EqualTo(expected), "Offsets should refer to body content, not the injected header");
        }
    }

}