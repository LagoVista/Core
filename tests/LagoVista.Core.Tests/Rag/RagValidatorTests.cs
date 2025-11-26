// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 47565987edc1161ce829fd8390eabdaa785d33e4e396abd86888d185dddf0149
// IndexVersion: 2
// --- END CODE INDEX META ---
// RagVectorPayloadValidatorTests.cs
    using System;
    using System.Collections.Generic;
    using System.Linq;
using LagoVista.Core.Utils.Types;
using LagoVista.Core.Utils.Types.Nuviot.RagIndexing;
using NUnit.Framework;

namespace LagoVista.Core.Tests.Rag
{
    [TestFixture]
        public class RagVectorPayloadValidatorTests
        {
            // ---------- Helpers ----------

            private static RagVectorPayload BaselineDomainDoc()
            {
                return new RagVectorPayload
                {
                    OrgNamespace = "org-123",
                    ProjectId = "proj-abc",
                    DocId = Guid.NewGuid().ToString(),
                    ContentTypeId = RagContentType.DomainDocument,
                    Subtype = "UserGuide",

                    SectionKey = "configure-sensors",
                    PartIndex = 1,
                    PartTotal = 3,

                    Title = "Configure Sensors",
                    Language = "en-US",
                    Priority = 3,
                    Audience = "RetailOwner",
                    Persona = "SmallRetailerOwner",
                    Stage = "Onboarding",
                    LabelSlugs = new List<string> { "alerting", "retail" },
                    LabelIds = new List<string> { "11111111-1111-1111-1111-111111111111", "22222222-2222-2222-2222-222222222222" },

                    BlobUri = "/org-123/proj-abc/userguide/doc-id/2025-11-06/source.md",
                    BlobVersionId = "v1",
                    SourceSha256 = new string('a', 64),
                    LineStart = 120,
                    LineEnd = 260,
                    CharStart = 5000,
                    CharEnd = 9000,

                    IndexVersion = 1,
                    EmbeddingModel = "text-embedding-3-large",
                    ContentHash = new string('b', 64),
                    ChunkSizeTokens = 650,
                    OverlapTokens = 80,
                    ContentLenChars = 8200,
                    IndexedUtc = DateTime.UtcNow,
                    UpdatedUtc = DateTime.UtcNow,

                    SourceSystem = "nuviot",
                    SourceObjectId = "doc-id"
                };
            }

            private static RagVectorPayload BaselineCode()
            {
                var p = BaselineDomainDoc();
                p.ContentTypeId = RagContentType.SourceCode;
                p.Subtype = "CSharp";
                p.Repo = "github.com/acme/nuviot";
                p.RepoBranch = "main";
                p.CommitSha = "3f6d9c1";
                p.Path = "src/Services/OrderService.cs";
                p.Symbol = "Nuviot.Services.OrderService.ProcessOrder";
                p.StartLine = 100;
                p.EndLine = 180;
                return p;
            }

            private static List<string> Validate(RagVectorPayload p, RagVectorPayloadValidator.ValidateOptions opt = null)
                => RagVectorPayloadValidator.Validate(p, opt);

            // ---------- Happy-path tests ----------

            [Test]
            public void DomainDoc_Valid_Passes()
            {
                var p = BaselineDomainDoc();
                var errs = Validate(p);
                Assert.That(errs, Is.Empty, string.Join("\n", errs));
            }

            [Test]
            public void CodeChunk_Valid_Passes()
            {
                var p = BaselineCode();
                var errs = Validate(p);
                Assert.That(errs, Is.Empty, string.Join("\n", errs));
            }

            // ---------- Identity / tenancy ----------

            [Test]
            public void Missing_OrgId_Fails()
            {
                var p = BaselineDomainDoc();
                p.OrgNamespace = null;

                var errs = Validate(p);
                Assert.That(errs, Has.Some.Contains("org_namespace is required."));
            }

            [Test]
            public void Missing_DocId_Fails()
            {
                var p = BaselineDomainDoc();
                p.DocId = "  ";

                var errs = Validate(p);
                Assert.That(errs, Has.Some.Contains("doc_id is required."));
            }

            // ---------- Classification ----------

            [Test]
            public void Unknown_ContentType_Fails()
            {
                var p = BaselineDomainDoc();
                p.ContentTypeId = RagContentType.Unknown;

                var errs = Validate(p);
                Assert.That(errs, Has.Some.Contains("content_type must be DomainDocument or Code."));
            }

            [Test]
            public void Missing_Subtype_Fails()
            {
                var p = BaselineDomainDoc();
                p.Subtype = null;

                var errs = Validate(p);
                Assert.That(errs, Has.Some.Contains("subtype is required."));
            }

            // ---------- Sectioning ----------

            [Test]
            public void Missing_SectionKey_Fails()
            {
                var p = BaselineDomainDoc();
                p.SectionKey = null;

                var errs = Validate(p);
                Assert.That(errs, Has.Some.Contains("section_key is required."));
            }

            [Test]
            public void PartIndex_And_Total_Invalid_Fail()
            {
                var p = BaselineDomainDoc();
                p.PartIndex = 0;  // invalid
                p.PartTotal = 0;  // invalid

                var errs = Validate(p);
                Assert.That(errs, Has.Some.Contains("part_index must be >= 1."));
                Assert.That(errs, Has.Some.Contains("part_total must be >= 1."));
            }

            [Test]
            public void PartIndex_Greater_Than_Total_Fails()
            {
                var p = BaselineDomainDoc();
                p.PartIndex = 5;
                p.PartTotal = 3;

                var errs = Validate(p);
                Assert.That(errs, Has.Some.Contains("part_index cannot be greater than part_total."));
            }

            // ---------- Core metadata ----------

            [Test]
            public void Priority_OutOfBounds_Fails()
            {
                var pLow = BaselineDomainDoc();
                pLow.Priority = 0;

                var errsLow = Validate(pLow, new RagVectorPayloadValidator.ValidateOptions { MinPriority = 1, MaxPriority = 5 });
                Assert.That(errsLow, Has.Some.Contains("priority must be >= 1."));

                var pHigh = BaselineDomainDoc();
                pHigh.Priority = 10;

                var errsHigh = Validate(pHigh, new RagVectorPayloadValidator.ValidateOptions { MinPriority = 1, MaxPriority = 5 });
                Assert.That(errsHigh, Has.Some.Contains("priority must be <= 5."));
            }

            [Test]
            public void Language_LooksTooLong_Warns()
            {
                var p = BaselineDomainDoc();
                p.Language = "this-is-way-too-long-to-be-a-locale";

                var errs = Validate(p);
                Assert.That(errs, Has.Some.Contains("language looks too long"));
            }

            [Test]
            public void LabelIds_Required_When_SlugsPresent_If_Option_Enabled()
            {
                var p = BaselineDomainDoc();
                p.LabelIds = new List<string>(); // empty

                var opt = new RagVectorPayloadValidator.ValidateOptions
                {
                    RequireLabelIdsWhenSlugsPresent = true
                };

                var errs = Validate(p, opt);
                Assert.That(errs, Has.Some.Contains("label_ids must be present and match label_slugs count"));
            }

            // ---------- Raw pointers / provenance ----------

            [Test]
            public void BlobUri_Required_When_Option_Enabled()
            {
                var p = BaselineDomainDoc();
                p.BlobUri = null;

                var opt = new RagVectorPayloadValidator.ValidateOptions { RequireBlobPointer = true };
                var errs = Validate(p, opt);
                Assert.That(errs, Has.Some.Contains("blob_uri is required."));
            }

            [Test]
            public void BlobUri_NotRequired_When_Option_Disabled()
            {
                var p = BaselineDomainDoc();
                p.BlobUri = null;

                var opt = new RagVectorPayloadValidator.ValidateOptions { RequireBlobPointer = false };
                var errs = Validate(p, opt);
                // Should not include blob_uri error
                Assert.That(errs.Any(e => e.IndexOf("blob_uri is required", StringComparison.OrdinalIgnoreCase) >= 0), Is.False);
            }

            [Test]
            public void Line_And_Char_Ranges_Invalid_Fail()
            {
                var p = BaselineDomainDoc();
                p.LineStart = 0;          // invalid (must be >=1)
                p.LineEnd = -5;           // invalid
                p.CharStart = -1;         // invalid (must be >=0)
                p.CharEnd = -2;           // invalid

                var errs = Validate(p);
                Assert.That(errs, Has.Some.Contains("line_start must be >= 1."));
                Assert.That(errs, Has.Some.Contains("line_end must be >= 1."));
                Assert.That(errs, Has.Some.Contains("char_start must be >= 0."));
                Assert.That(errs, Has.Some.Contains("char_end must be >= 0."));
            }

            [Test]
            public void LineEnd_Less_Than_LineStart_Fails()
            {
                var p = BaselineDomainDoc();
                p.LineStart = 100;
                p.LineEnd = 90;

                var errs = Validate(p);
                Assert.That(errs, Has.Some.Contains("line_end must be >= line_start."));
            }

            [Test]
            public void CharEnd_Less_Than_CharStart_Fails()
            {
                var p = BaselineDomainDoc();
                p.CharStart = 2000;
                p.CharEnd = 1999;

                var errs = Validate(p);
                Assert.That(errs, Has.Some.Contains("char_end must be >= char_start."));
            }

            [Test]
            public void SourceSha256_Invalid_Length_Fails()
            {
                var p = BaselineDomainDoc();
                p.SourceSha256 = "abc123"; // too short

                var errs = Validate(p);
                Assert.That(errs, Has.Some.Contains("source_sha256 must be 64 hex chars"));
            }

            // ---------- Index / embedding ----------

            [Test]
            public void IndexVersion_Must_Be_Positive()
            {
                var p = BaselineDomainDoc();
                p.IndexVersion = 0;

                var errs = Validate(p);
                Assert.That(errs, Has.Some.Contains("index_version must be >= 1."));
            }

            [Test]
            public void EmbeddingModel_Required()
            {
                var p = BaselineDomainDoc();
                p.EmbeddingModel = null;

                var errs = Validate(p);
                Assert.That(errs, Has.Some.Contains("embedding_model is required."));
            }

            [Test]
            public void ContentHash_Required_And_64Hex()
            {
                var p1 = BaselineDomainDoc();
                p1.ContentHash = null;

                var e1 = Validate(p1);
                Assert.That(e1, Has.Some.Contains("content_hash is required."));

                var p2 = BaselineDomainDoc();
                p2.ContentHash = "1234";

                var e2 = Validate(p2);
                Assert.That(e2, Has.Some.Contains("content_hash must be 64 hex chars"));
            }

            [Test]
            public void IndexedUtc_Must_Be_Set()
            {
                var p = BaselineDomainDoc();
                p.IndexedUtc = default(DateTime);

                var errs = Validate(p);
                Assert.That(errs, Has.Some.Contains("indexed_utc must be set"));
            }

            [Test]
            public void ChunkSizeTokens_And_ContentLenChars_Not_Negative()
            {
                var p = BaselineDomainDoc();
                p.ChunkSizeTokens = 0;  // not negative but validator requires > 0 when provided
                p.OverlapTokens = -1;   // invalid
                p.ContentLenChars = -10; // invalid

                var errs = Validate(p);
                Assert.That(errs, Has.Some.Contains("chunk_size_tokens must be > 0"));
                Assert.That(errs, Has.Some.Contains("overlap_tokens cannot be negative"));
                Assert.That(errs, Has.Some.Contains("content_len_chars cannot be negative"));
            }

            // ---------- SourceCode-specific ----------

            [Test]
            public void CodeChunk_Requires_Repo_Path_CommitSha()
            {
                var p = BaselineCode();
                p.Repo = null;
                p.Path = null;
                p.CommitSha = null;

                var errs = Validate(p, new RagVectorPayloadValidator.ValidateOptions { RequireCodeRepoFields = true });
                Assert.That(errs, Has.Some.Contains("repo is required."));
                Assert.That(errs, Has.Some.Contains("path is required."));
                Assert.That(errs, Has.Some.Contains("commit_sha is required."));
            }

            [Test]
            public void CodeChunk_Invalid_CommitSha_Fails()
            {
                var p = BaselineCode();
                p.CommitSha = "xyz"; // too short, and not hex

                var errs = Validate(p);
                Assert.That(errs, Has.Some.Contains("commit_sha should be hex (>=7 chars)."));
            }

            [Test]
            public void CodeChunk_StartLine_EndLine_Range_Validated()
            {
                var p = BaselineCode();
                p.StartLine = 0;
                p.EndLine = -1;

                var errs = Validate(p);
                Assert.That(errs, Has.Some.Contains("start_line must be >= 1."));
                Assert.That(errs, Has.Some.Contains("end_line must be >= 1."));
            }

            [Test]
            public void CodeChunk_EndLine_Less_Than_StartLine_Fails()
            {
                var p = BaselineCode();
                p.StartLine = 100;
                p.EndLine = 50;

                var errs = Validate(p);
                Assert.That(errs, Has.Some.Contains("end_line must be >= start_line."));
            }

            [Test]
            public void CodeChunk_When_RepoFields_Not_Required_Passes_Without_Repo()
            {
                var p = BaselineCode();
                p.Repo = null;
                p.Path = null;
                p.CommitSha = null;

                var opt = new RagVectorPayloadValidator.ValidateOptions { RequireCodeRepoFields = false };
                var errs = Validate(p, opt);
                Assert.That(errs, Is.Empty, string.Join("\n", errs));
            }

            // ---------- Mixed options / toggles ----------

            [Test]
            public void Disable_PartBounds_Allows_Weird_PartIndex()
            {
                var p = BaselineDomainDoc();
                p.PartIndex = 5;
                p.PartTotal = 3;

                var opt = new RagVectorPayloadValidator.ValidateOptions { EnforcePartBounds = false };
                var errs = Validate(p, opt);
                // Should not include part_index/part_total errors
                Assert.That(errs.Any(e => e.IndexOf("part_index", StringComparison.OrdinalIgnoreCase) >= 0), Is.False);
                Assert.That(errs.Any(e => e.IndexOf("part_total", StringComparison.OrdinalIgnoreCase) >= 0), Is.False);
            }

            [Test]
            public void Priority_Range_Can_Be_Disabled()
            {
                var p = BaselineDomainDoc();
                p.Priority = 999;

                var opt = new RagVectorPayloadValidator.ValidateOptions { MinPriority = null, MaxPriority = null };
                var errs = Validate(p, opt);
                // No priority errors when bounds disabled
                Assert.That(errs.Any(e => e.IndexOf("priority must be", StringComparison.OrdinalIgnoreCase) >= 0), Is.False);
            }

        [Test]
        public void BuildForDocument_IsDeterministic_And_Valid()
        {
            var docId = Guid.NewGuid().ToString();
            var id1 = RagPointId.BuildForDocument(docId, "Configure Sensors", 2);
            var id2 = RagPointId.BuildForDocument(docId, "configure   sensors", 2);
            Assert.That(id1, Is.EqualTo(id2)); // slug normalization
            NUnit.Framework.Legacy.StringAssert.Contains(":sec:configure-sensors#p2", id1);
        }


    }
}
