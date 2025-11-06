using System;
using NUnit.Framework;
using System.Linq;
using LagoVista.Core.Utils.Types;
using NUnit.Framework.Legacy;

namespace LagoVista.Core.Tests.Rag
{
    [TestFixture]
    public class CodePointId_NoOrg_Tests
    {
        private sealed class FakeChunk : RagChunk
        {
            public FakeChunk(string sectionKey, int partIndex, int partTotal, string title = null)
            {
                SectionKey = sectionKey;
                PartIndex = partIndex;
                PartTotal = partTotal;
                Title = title;
                TextNormalized = $"SECTION: {title ?? sectionKey}\r\ncontent p{partIndex}";
            }
        }

        private static RagChunkPlan MakePlan(string docId, params RagChunk[] chunks)
        {
            return new RagChunkPlan
            {
                DocId = docId,
                Chunks = chunks.ToList().AsReadOnly(),
                Raw = new RawArtifact
                {
                    SuggestedBlobPath = "/code/src/Services/OrderService.cs",
                    SourceSha256 = new string('c', 64),
                    MimeType = "text/plain"
                }
            };
        }

        [Test]
        public void Canonical_Normalizes_Repo_And_Path()
        {
            var c1 = CodeDocId.Canonical("GitHub.com/Acme/NuvIoT", @"src\\Services//OrderService.cs");
            var c2 = CodeDocId.Canonical("github.com/acme/nuviot", "src/Services/OrderService.cs");
            Assert.That(c1, Is.EqualTo(c2));
            StringAssert.DoesNotContain("\\", c1);
            StringAssert.Contains("github.com/acme/nuviot|src/services/orderservice.cs", c1);
        }

        [Test]
        public void FileDocIdV5_Is_Deterministic_And_Sensitive_To_Path_And_Repo()
        {
            var A1 = CodeDocId.FileDocIdV5(CodeDocId.Canonical("github.com/acme/nuviot", "src/Services/OrderService.cs"));
            var A2 = CodeDocId.FileDocIdV5(CodeDocId.Canonical("github.com/acme/nuviot", "src/Services/OrderService.cs"));
            var B1 = CodeDocId.FileDocIdV5(CodeDocId.Canonical("github.com/acme/nuviot", "src/Services/InventoryService.cs"));
            var C1 = CodeDocId.FileDocIdV5(CodeDocId.Canonical("github.com/acme/another", "src/Services/OrderService.cs"));

            Console.WriteLine(A1);
            Console.WriteLine(A2);
            Console.WriteLine(B1);
            Console.WriteLine(C1);

            Assert.That(A1, Is.EqualTo(A2));
            Assert.That(A1, Is.Not.EqualTo(B1));
            Assert.That(A1, Is.Not.EqualTo(C1));
        }

        [Test]
        public void FileDocIdHex128_Is_Deterministic_And_Proper_Length()
        {
            var c = CodeDocId.Canonical("github.com/acme/nuviot", "src/Services/OrderService.cs");
            var d1 = CodeDocId.FileDocIdHex128(c);
            var d2 = CodeDocId.FileDocIdHex128(c);
            Assert.That(d1, Is.EqualTo(d2));
            Assert.That(d1.Length, Is.EqualTo(32));
            StringAssert.IsMatch("^[0-9a-f]{32}$", d1);
        }

        [Test]
        public void CodePointId_Is_Stable_Across_SectionKey_Casing_And_Spacing()
        {
            var fileDocId = CodeDocId.FileDocIdV5(CodeDocId.Canonical("github.com/acme/nuviot", "src/Services/OrderService.cs"));
            var id1 = CodePointId.Build(fileDocId, "Configure Sensors", 2);
            var id2 = CodePointId.Build(fileDocId, "configure   sensors", 2);
            Assert.That(id1, Is.EqualTo(id2));
            StringAssert.Contains(":sec:configure-sensors#p2", id1);
        }

        [Test]
        public void PointIds_Do_Not_Change_With_Different_Commits()
        {
            var fileDocId = CodeDocId.FileDocIdV5(CodeDocId.Canonical("github.com/acme/nuviot", "src/Services/OrderService.cs"));
            var pidA = CodePointId.Build(fileDocId, "ProcessOrder", 1);
            var pidB = CodePointId.Build(fileDocId, "ProcessOrder", 1); // different commit not in ID
            Assert.That(pidA, Is.EqualTo(pidB));
        }

        [Test]
        public void Integration_TwoChunks_Produces_Deterministic_PointIds()
        {
            var fileDocId = CodeDocId.FileDocIdV5(CodeDocId.Canonical("github.com/acme/nuviot", "src/Services/OrderService.cs"));
            var c1 = new FakeChunk("OrderService", 1, 2, "OrderService Class");
            var c2 = new FakeChunk("OrderService", 2, 2, "ProcessOrder Method");
            var plan = MakePlan(fileDocId.ToString(), c1, c2);

            var id1 = CodePointId.Build(fileDocId, c1.SectionKey, c1.PartIndex);
            var id2 = CodePointId.Build(fileDocId, c2.SectionKey, c2.PartIndex);

            Assert.That(id1, Is.EqualTo(plan.DocId + ":sec:orderservice#p1").IgnoreCase);
            Assert.That(id2, Is.EqualTo(plan.DocId + ":sec:orderservice#p2").IgnoreCase);
        }
    }
}
