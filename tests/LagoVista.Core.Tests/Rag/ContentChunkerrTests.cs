// RagChunkBuilderTests.cs (NUnit 4.4)
using System;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using LagoVista.Core.Utils;

namespace LagoVista.Core.Tests.Rag
{
    public sealed class TestDoc : IRagIndexable
    {
        public string OrgId { get; set; } = "org-123";
        public string ProjectId { get; set; } = "proj-abc";
        public string UniqueId { get; set; } = Guid.NewGuid().ToString();
        public string ContentSubtype { get; set; } = "UserGuide";

        public string Title { get; set; } = "TITLE: Demo Guide";
        public List<IndexSection> Sections { get; } = new();

        public string GetFrontMatter() => Title + "\r\nSUMMARY: lorem ipsum";
        public IEnumerable<IndexSection> GetBodySections() => Sections;

        public IEnumerable<string> GetLabelSlugs() => Array.Empty<string>();
        public string Language => "en-US";
        public int Priority => 3;
        public string Audience => "RetailOwner";
        public string Persona => "SmallRetailerOwner";
        public string Stage => "Onboarding";
    }

    [TestFixture]
    public class RagChunkBuilderTests
    {
        private static string MakeLines(int count, string prefix)
            => string.Join("\r\n", Enumerable.Range(1, count).Select(i => $"{prefix} line {i:D4}"));

        [Test]
        public void FrontMatter_ProducesSingleChunk_WithTitle()
        {
            var doc = new TestDoc();
            doc.Sections.Add(new IndexSection { Key = "intro", Heading = "Intro", Text = "hello" });

            var plan = RagChunkBuilder.Build(
                doc,
                new RawInput { MimeType = "text/markdown", RawText = "# Heading\r\nSome text" },
                new ChunkingOptions { IncludeFrontMatter = true });

            var front = plan.Chunks.First(c => c.SectionKey == "front");
            Assert.That(front.PointId, Is.EqualTo(doc.UniqueId + ":sec:front#p1"));
            Assert.That(front.Title, Is.EqualTo("Demo Guide"));
            Assert.That(front.TextNormalized, Does.Contain("SUMMARY:"));
        }

        [Test]
        public void LargeSection_SplitsIntoMultipleChunks_WithOverlap()
        {
            var doc = new TestDoc();
            var big = MakeLines(500, "alpha");
            doc.Sections.Add(new IndexSection { Key = "configure-sensors", Heading = "Configure Sensors", Text = big });

            var plan = RagChunkBuilder.Build(
                doc,
                new RawInput { MimeType = "text/markdown", RawText = big },
                new ChunkingOptions { TargetTokensPerChunk = 300, OverlapTokens = 50 });

            var secChunks = plan.Chunks.Where(c => c.SectionKey == "configure-sensors").ToList();
            Assert.That(secChunks.Count, Is.GreaterThanOrEqualTo(2));

            for (int i = 0; i < secChunks.Count; i++)
                Assert.That(secChunks[i].PointId, Is.EqualTo($"{doc.UniqueId}:sec:configure-sensors#p{i + 1}"));

            for (int i = 0; i < secChunks.Count - 1; i++)
            {
                var tail = LastNonEmptyLines(secChunks[i].TextNormalized, 5);
                var head = FirstNonEmptyLines(secChunks[i + 1].TextNormalized, 5);
                Assert.That(tail.Intersect(head), Is.Not.Empty, $"Expected overlap between chunk {i + 1} and {i + 2}");
            }
        }

        [Test]
        public void Normalization_EnforcesCRLF_AndTrimsTrailing()
        {
            var doc = new TestDoc();
            var raw = "a\nb\r\nc\r\r\n";
            doc.Sections.Add(new IndexSection { Key = "body", Heading = "Body", Text = raw });

            var plan = RagChunkBuilder.Build(doc, new RawInput { MimeType = "text/plain", RawText = raw });
            var body = plan.Chunks.Single(c => c.SectionKey == "body");

            Assert.That(body.TextNormalized.Replace("\r\n", "OK"), Does.Not.Contain("\n")); // only CRLF remains
            Assert.That(body.TextNormalized, Does.Contain("\r\n"));
            Assert.That(body.TextNormalized.EndsWith("\r\n\r\n"), Is.False);
        }

        [Test]
        public void LinePointers_Monotonic_AndWithinRawBounds()
        {
            var doc = new TestDoc();
            var raw = MakeLines(300, "beta");
            doc.Sections.Add(new IndexSection { Key = "steps", Heading = "Steps", Text = raw });

            var plan = RagChunkBuilder.Build(
                doc,
                new RawInput { MimeType = "text/plain", RawText = raw },
                new ChunkingOptions { TargetTokensPerChunk = 250, OverlapTokens = 40 });

            var steps = plan.Chunks.Where(c => c.SectionKey == "steps").ToList();
            Assert.That(steps, Is.Not.Empty);

            int? prevEnd = 0;
            foreach (var c in steps)
            {
                if (c.LineStart.HasValue) Assert.That(c.LineStart.Value, Is.GreaterThan(0));
                if (c.LineEnd.HasValue) Assert.That(c.LineEnd.Value, Is.GreaterThanOrEqualTo(c.LineStart ?? 1));
                if (c.LineEnd.HasValue) Assert.That(c.LineEnd.Value, Is.LessThanOrEqualTo(300));

                if (prevEnd.HasValue && c.LineStart.HasValue)
                    Assert.That(c.LineStart.Value, Is.GreaterThanOrEqualTo(prevEnd.Value - 50),
                        "start should not jump far back (overlap bound)");

                prevEnd = c.LineEnd;
            }
        }

        [Test]
        public void RawArtifact_Populated_WithSuggestedPath_AndSha()
        {
            var doc = new TestDoc { ContentSubtype = "UserGuide" };
            var raw = MakeLines(10, "gamma");

            var plan = RagChunkBuilder.Build(
                doc,
                new RawInput { MimeType = "text/markdown", RawText = raw },
                new ChunkingOptions { VersionOrDate = "2025-11-03", RawFileExtension = "md" });

            Assert.That(plan.Raw, Is.Not.Null);
            Assert.That(plan.Raw.IsText, Is.True);
            Assert.That(plan.Raw.SuggestedBlobPath,
                Does.Contain($"/{doc.OrgId}/{doc.ProjectId}/userguide/{doc.UniqueId}/2025-11-03/source.md").IgnoreCase);
            Assert.That(plan.Raw.SourceSha256, Is.Not.Null.And.Not.Empty);
            Assert.That(plan.Raw.MimeType, Is.EqualTo("text/markdown"));
        }

        // helpers
        private static List<string> LastNonEmptyLines(string s, int count)
            => s.Split(new[] { "\r\n" }, StringSplitOptions.None)
                .Reverse()
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .Take(count)
                .ToList();

        private static List<string> FirstNonEmptyLines(string s, int count)
            => s.Split(new[] { "\r\n" }, StringSplitOptions.None)
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .Take(count)
                .ToList();
    }
}