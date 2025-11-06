using System;
using System.Collections.Generic;
using System.Linq;
using LagoVista.Core.Models.Drawing;
using LagoVista.Core.Utils.Types.Nuviot.RagIndexing;
using LagoVista.Core.Utils.Types;
using NUnit.Framework;
using LagoVista.Core.Models;
using NUnit.Framework.Legacy;
using LagoVista.Core.Utils;


namespace LagoVista.Core.Tests.Rag
{
    [TestFixture]
    public class RagPayloadFactoryTests
    {
        // -----------------------------
        // Minimal fakes / helpers
        // -----------------------------
        private sealed class FakeDoc : IRagIndexable
        {
            public string Id { get; set; } = Guid.NewGuid().ToString();
            public string ContentSubtype { get; set; } = "UserGuide";
            public string Language { get; set; } = "en-US";
            public int Priority { get; set; } = 3;
            public string Audience { get; set; } = "RetailOwner";
            public string Persona { get; set; } = "SmallRetailerOwner";
            public string Stage { get; set; } = "Onboarding";

            public EntityHeader OwnerOrganization => EntityHeader.Create("803F1C881DB54495831415D53F48FA85", "Some Interesting org");


            public IEnumerable<IndexSection> GetBodySections()
            {
                throw new NotImplementedException();
            }

            public string GetFrontMatter()
            {
                throw new NotImplementedException();
            }

            public IEnumerable<string> GetLabelSlugs() => new[] { "alerting", "retail" };
        }

        private sealed class FakeChunk : RagChunk
        {
            public FakeChunk(string sectionKey, int partIndex, int partTotal, string text, string title = null,
                             int? ls = null, int? le = null, int? cs = null, int? ce = null)
            {
                SectionKey = sectionKey;
                PartIndex = partIndex;
                PartTotal = partTotal;
                Title = title;
                TextNormalized = text;
                LineStart = ls;
                LineEnd = le;
                CharStart = cs;
                CharEnd = ce;
            }
        }

        private static RagChunkPlan MakePlan(string docId, RawArtifact raw, params RagChunk[] chunks)
        {
            return new RagChunkPlan
            {
                DocId = docId,
                Chunks = chunks.ToList().AsReadOnly(),
                Raw = raw
            };
        }

        private static RawArtifact Raw(string blobPath = "/blob/code/src/Services/OrderService.cs", string sha = null)
            => new RawArtifact { SuggestedBlobPath = blobPath, SourceSha256 = sha ?? new string('a', 64), MimeType = "text/plain" };

        private static IngestContext Ctx() => new IngestContext
        {
            OrgId = "org-123",
            ProjectId = "proj-abc",
            EmbeddingModel = "text-embedding-3-large",
            IndexVersion = 1
        };

        // -----------------------------
        // Documents
        // -----------------------------

        [Test]
        public void FromDocumentPlan_Builds_Deterministic_PointIds_And_Valid_Payloads()
        {
            var doc = new FakeDoc();
            var plan = MakePlan(
                doc.Id,
                Raw("/docs/userguide.md", new string('b', 64)),
                new FakeChunk("Introduction", 1, 2, "intro text", "Intro", 1, 25, 0, 120),
                new FakeChunk("Introduction", 2, 2, "more intro", null, 26, 60, 121, 300));

            var results = RagPayloadFactory.FromDocumentPlan(doc, plan, Ctx(),
                new DocumentArtifactContext { Subtype = "UserGuide", Language = "en-US" });

            Assert.That(results, Has.Count.EqualTo(2));

            // Deterministic PointIds (slug + part index)
            Assert.That(results[0].PointId, Is.EqualTo($"{plan.DocId}:sec:introduction#p1"));
            Assert.That(results[1].PointId, Is.EqualTo($"{plan.DocId}:sec:introduction#p2"));

            // Payload basics
            var p1 = results[0].Payload;
            Assert.That(p1.ContentType, Is.EqualTo(RagContentType.DomainDocument));
            Assert.That(p1.Subtype, Is.EqualTo("UserGuide"));
            Assert.That(p1.OrgId, Is.EqualTo("org-123"));
            Assert.That(p1.ProjectId, Is.EqualTo("proj-abc"));
            Assert.That(p1.DocId, Is.EqualTo(plan.DocId));
            Assert.That(p1.SectionKey, Is.EqualTo("Introduction"));
            Assert.That(p1.PartIndex, Is.EqualTo(1));
            Assert.That(p1.PartTotal, Is.EqualTo(2));
            Assert.That(p1.BlobUri, Is.EqualTo("/docs/userguide.md"));
            Assert.That(p1.SourceSha256, Is.EqualTo(new string('b', 64)));
            Assert.That(p1.LineStart, Is.EqualTo(1));
            Assert.That(p1.LineEnd, Is.EqualTo(25));
            Assert.That(p1.CharStart, Is.EqualTo(0));
            Assert.That(p1.CharEnd, Is.EqualTo(120));
            Assert.That(p1.ContentHash, Is.Not.Null.And.Length.EqualTo(64)); // sha256 hex
            Assert.That(p1.IndexVersion, Is.EqualTo(1));
            Assert.That(p1.EmbeddingModel, Is.EqualTo("text-embedding-3-large"));

            // ToDictionary omits nulls
            var dict = p1.ToDictionary();
            Assert.That(!dict.ContainsKey("label_ids"), "Null/empty collections should be omitted.");
            Assert.That(dict["content_type"], Is.EqualTo("DomainDocument"));
        }

        // -----------------------------
        // Code
        // -----------------------------

        [Test]
        public void FromCodePlan_Builds_CommitAgnostic_PointIds_And_Excludes_DocOnly_Fields()
        {
            // Stable per-file GUID from repo+path
            var canonical = CodeDocId.Canonical("github.com/acme/nuviot", "src/Services/OrderService.cs");
            var fileDocId = CodeDocId.FileDocIdV5(canonical);

            var plan = MakePlan(
                fileDocId.ToString(),
                Raw("/code/src/Services/OrderService.cs", new string('c', 64)),
                new FakeChunk("OrderService", 1, 2, "class body", "OrderService Class", 1, 80, 0, 1200),
                new FakeChunk("OrderService", 2, 2, "ProcessOrder body", "ProcessOrder", 81, 160, 1201, 2500));

            var codeCtx = new CodeArtifactContext
            {
                Repo = "github.com/acme/nuviot",
                RepoBranch = "main",
                CommitSha = "3f6d9c1a",
                Path = "src/Services/OrderService.cs",
 
                Subtype = "CSharp",
                Language = "en"
            };

            var results = RagPayloadFactory.FromCodePlan(plan, Ctx(), codeCtx);

            Assert.That(results, Has.Count.EqualTo(2));

            // Commit-agnostic ID (no commit in the ID)
            Assert.That(results[0].PointId, Is.EqualTo($"{fileDocId}:sec:orderservice#p1"));
            Assert.That(results[1].PointId, Is.EqualTo($"{fileDocId}:sec:orderservice#p2"));

            // Payload properties
            var p = results[0].Payload;
            Assert.That(p.ContentType, Is.EqualTo(RagContentType.Code));
            Assert.That(p.Subtype, Is.EqualTo("CSharp"));
            Assert.That(p.DocId, Is.EqualTo(fileDocId.ToString()));
            Assert.That(p.Repo, Is.EqualTo("github.com/acme/nuviot"));
            Assert.That(p.Path, Is.EqualTo("src/Services/OrderService.cs"));
            Assert.That(p.CommitSha, Is.EqualTo("3f6d9c1a"));

            // Code payload should NOT carry doc-only fields
            Assert.That(p.Audience, Is.Null);
            Assert.That(p.Persona, Is.Null);
            Assert.That(p.Stage, Is.Null);
            Assert.That(p.LabelSlugs == null || p.LabelSlugs.Count == 0, Is.True);

            // Provenance offsets mapped
            Assert.That(p.StartLine, Is.EqualTo(1));
            Assert.That(p.EndLine, Is.EqualTo(80));
            Assert.That(p.LineStart, Is.EqualTo(1));
            Assert.That(p.LineEnd, Is.EqualTo(80));
            Assert.That(p.CharStart, Is.EqualTo(0));
            Assert.That(p.CharEnd, Is.EqualTo(1200));

            // ToDictionary contains core fields; commit_sha in payload (not in ID)
            var dict = p.ToDictionary();
            Assert.That(dict["commit_sha"], Is.EqualTo("3f6d9c1a"));
            StringAssert.DoesNotContain("3f6d9c1a", results[0].PointId);
        }

        [Test]
        public void FromCodePlan_Throws_When_Required_Code_Fields_Missing()
        {
            var canonical = CodeDocId.Canonical("github.com/acme/nuviot", "src/Services/OrderService.cs");
            var fileDocId = CodeDocId.FileDocIdV5(canonical);

            var plan = MakePlan(fileDocId.ToString(), Raw(),
                new FakeChunk("OrderService", 1, 1, "class body", "OrderService Class"));

            var badCtx = new CodeArtifactContext
            {
                Repo = "github.com/acme/nuviot",
                RepoBranch = "main",
                CommitSha = null, // missing
                Path = "src/Services/OrderService.cs"
            };

            Assert.Throws<InvalidOperationException>(() =>
            {
                RagPayloadFactory.FromCodePlan(plan, Ctx(), badCtx);
            }, "Missing commit SHA should fail validation.");
        }

        // -----------------------------
        // Determinism across runs
        // -----------------------------

        [Test]
        public void FromDocumentPlan_Deterministic_PointIds_Across_Runs()
        {
            var doc = new FakeDoc();
            var raw = Raw("/docs/userguide.md", new string('d', 64));

            var plan1 = MakePlan(doc.Id, raw,
                new FakeChunk("Overview", 1, 2, "aaa", "Overview"),
                new FakeChunk("Overview", 2, 2, "bbb"));

            var plan2 = MakePlan(doc.Id, raw,
                new FakeChunk("Overview", 1, 2, "aaa", "Overview"),
                new FakeChunk("Overview", 2, 2, "bbb"));

            var r1 = RagPayloadFactory.FromDocumentPlan(doc, plan1, Ctx(), new DocumentArtifactContext());
            var r2 = RagPayloadFactory.FromDocumentPlan(doc, plan2, Ctx(), new DocumentArtifactContext());

            CollectionAssert.AreEqual(r1.Select(x => x.PointId), r2.Select(x => x.PointId));
        }
    }
}
