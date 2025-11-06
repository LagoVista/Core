// RagChunkBuilderTests.cs (NUnit 4.4)
using System;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using LagoVista.Core.Utils;
using LagoVista.Core.Utils.Types;
using LagoVista.Core.Models;

namespace LagoVista.Core.Tests.Rag
{
    public sealed class TestDoc : IRagIndexable
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string ContentSubtype { get; set; } = "UserGuide";

        public EntityHeader OwnerOrganization => EntityHeader.Create("803F1C881DB54495831415D53F48FA85", "Some Interesting org");


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

        private static string MakeLine(int? minLength = 50, int? maxLength = 1024)
        {
            var rand = new Random();
            int length = rand.Next(minLength ?? 50, maxLength ?? 1024);
            const string chars = "abcdefghijklmnopqrstuvwxyz ";
            return new string(Enumerable.Range(0, length).Select(_ => chars[rand.Next(chars.Length)]).ToArray());
        }
         

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
            Assert.That(front.PointId, Is.EqualTo(doc.Id + ":sec:front#p1"));
            Assert.That(front.Title, Is.EqualTo("Demo Guide"));
            Assert.That(front.TextNormalized, Does.Contain("SUMMARY:"));
        }



        // Very simple conservative estimator: ~4 chars per token
        private static int EstimateTokens(string s)
        {
            if (string.IsNullOrEmpty(s)) return 0;
            // Count newlines a bit heavier; trim to avoid trailing gaps
            var text = s.TrimEnd();
            var extra = text.Count(c => c == '\n' || c == '\r');
            return (int)Math.Ceiling((text.Length + extra) / 4.0);
        }

        [Test]
        public void SingleLongLine_SplitsWithOverlap()
        {
            var longLine = MakeLines(500, MakeLine(100, 65535));
            var chunks = ChunkSplitter.SplitWithOverlap(longLine, targetTokens: 600, overlapTokens: 80);

            Assert.That(chunks.Count, Is.GreaterThan(1));
            // Overlap check: tail of chunk[i] appears in head of chunk[i+1]
            for (int i = 0; i < chunks.Count - 1; i++)
            {
                var tokens = EstimateTokens(chunks[i]);

                Assert.That(tokens < 700);

                var tail = chunks[i].Substring(Math.Max(0, chunks[i].Length - 200));
                Assert.That(chunks[i + 1], Does.Contain(tail.Substring(0, 100)));
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
                Does.Contain($"/{doc.OwnerOrganization.Id}/{doc.Id}/userguide/{doc.Id}/2025-11-03/source.md").IgnoreCase);
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