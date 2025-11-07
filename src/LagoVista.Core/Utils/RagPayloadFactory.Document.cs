using LagoVista.Core.Utils.Types.Nuviot.RagIndexing;
using LagoVista.Core.Utils.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace LagoVista.Core.Utils
{
    public static partial class RagPayloadFactory
    {
        /// <summary>
        /// Document ingestion: builds payloads for non-code artifacts (user guides, FAQs, templates, etc.).
        /// </summary>
        public static IReadOnlyList<PayloadBuildResult> FromDocumentPlan(
            IRagIndexable doc,
            RagChunkPlan plan,
            IngestContext ctx,
            DocumentArtifactContext docCtx)
        {
            if (doc == null) throw new ArgumentNullException(nameof(doc));
            if (plan == null) throw new ArgumentNullException(nameof(plan));
            if (ctx == null) throw new ArgumentNullException(nameof(ctx));
            if (docCtx == null) docCtx = new DocumentArtifactContext();

            var results = new List<PayloadBuildResult>(plan.Chunks != null ? plan.Chunks.Count : 0);

            foreach (var c in plan.Chunks ?? Enumerable.Empty<RagChunk>())
            {
                var pointId = RagPointId.BuildForDocument(plan.DocId, c.SectionKey, c.PartIndex);

                if (string.IsNullOrWhiteSpace(c.PointId)) c.PointId = pointId;
                var text = c.TextNormalized ?? string.Empty;

                var title = !string.IsNullOrWhiteSpace(docCtx.TitleOverride) ? docCtx.TitleOverride
                          : (!string.IsNullOrWhiteSpace(c.Title) ? c.Title
                          : doc.ContentSubtype);

                var payload = new RagVectorPayload
                {
                    // Identity / tenancy
                    OrgId = ctx.OrgId,
                    ProjectId = ctx.ProjectId,
                    DocId = plan.DocId,

                    // Classification
                    ContentType = RagContentType.DomainDocument,
                    Subtype = !string.IsNullOrWhiteSpace(docCtx.Subtype) ? docCtx.Subtype : doc.ContentSubtype,

                    // Sectioning
                    SectionKey = c.SectionKey,
                    PartIndex = c.PartIndex,
                    PartTotal = c.PartTotal,

                    // Core meta
                    Title = title,
                    Language = !string.IsNullOrWhiteSpace(docCtx.Language) ? docCtx.Language : doc.Language,
                    Priority = doc.Priority,
                    Audience = doc.Audience,
                    Persona = doc.Persona,
                    Stage = doc.Stage,
                    LabelSlugs = doc.GetLabelSlugs() != null ? doc.GetLabelSlugs().ToList() : new List<string>(),

                    // Provenance (blob + offsets)
                    BlobUri = plan.Raw != null ? plan.Raw.SuggestedBlobPath : null,
                    SourceSha256 = plan.Raw != null ? plan.Raw.SourceSha256 : null,
                    LineStart = c.LineStart,
                    LineEnd = c.LineEnd,
                    CharStart = c.CharStart,
                    CharEnd = c.CharEnd,

                    // Index / embedding
                    IndexVersion = ctx.IndexVersion,
                    EmbeddingModel = ctx.EmbeddingModel,
                    ContentHash = Sha256(text),
                    ContentLenChars = text.Length,
                    IndexedUtc = DateTime.UtcNow
                };

                var errs = RagVectorPayloadValidator.Validate(payload, new RagVectorPayloadValidator.ValidateOptions
                {
                    RequireCodeRepoFields = false // doc mode
                });
                if (errs.Count > 0)
                    throw new InvalidOperationException("Invalid document payload for " + pointId + ": " + string.Join("; ", errs));

                results.Add(new PayloadBuildResult
                {
                    PointId = pointId,
                    Payload = payload,
                    TextForEmbedding = text,
                    EstimatedTokens = EstimateTokens(text),
                    Vector = c.Vector
                });
            }

            return results;
        }

        // Helpers reused by both entry points
        private static string Sha256(string text)
        {
            using (var sha = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(text ?? string.Empty);
                var hash = sha.ComputeHash(bytes);
                var sb = new StringBuilder(hash.Length * 2);
                for (int i = 0; i < hash.Length; i++) sb.Append(hash[i].ToString("x2"));
                return sb.ToString();
            }
        }

        private static int EstimateTokens(string s)
        {
            if (string.IsNullOrEmpty(s)) return 0;
            var extra = 0;
            for (int i = 0; i < s.Length; i++)
            {
                var ch = s[i];
                if (ch == '\n' || ch == '\r') extra++;
            }
            return (int)Math.Ceiling((s.Length + extra) / 4.0);
        }
    }
}
