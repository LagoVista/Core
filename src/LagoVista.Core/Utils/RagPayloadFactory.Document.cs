// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 36af0ad3212367d8012714101d7ac19155ee4e8bc1cce97397b13827480f908e
// IndexVersion: 2
// --- END CODE INDEX META ---
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

                var payload = new RagVectorPayload();

                // -------------------------
                // META (filterable / canonical)
                // -------------------------
                payload.Meta.OrgId = ctx.OrgId;
                payload.Meta.OrgNamespace = ctx.OrgNamspace;
                payload.Meta.ProjectId = ctx.ProjectId;
                payload.Meta.DocId = plan.DocId;

                payload.Meta.ContentTypeId = RagContentType.DomainDocument;
                payload.Meta.ContentType = RagContentType.DomainDocument.ToString();

                payload.Meta.Subtype = !string.IsNullOrWhiteSpace(docCtx.Subtype)
                    ? docCtx.Subtype
                    : doc.ContentSubtype;

                payload.Meta.SectionKey = c.SectionKey;
                payload.Meta.PartIndex = c.PartIndex;
                payload.Meta.PartTotal = c.PartTotal;

                payload.Meta.Title = title;
                payload.Meta.Language = !string.IsNullOrWhiteSpace(docCtx.Language) ? docCtx.Language : doc.Language;
                payload.Meta.Priority = doc.Priority;
                payload.Meta.Audience = doc.Audience;
                payload.Meta.Persona = doc.Persona;
                payload.Meta.Stage = doc.Stage;

                payload.Meta.LabelSlugs = doc.GetLabelSlugs() != null
                    ? doc.GetLabelSlugs().ToList()
                    : new List<string>();

                // -------------------------
                // EXTRA (non-filter helpers)
                // -------------------------
                payload.Extra.FullDocumentBlobUri = plan.Raw != null ? plan.Raw.SuggestedBlobPath : null;
                payload.Extra.SourceSha256 = plan.Raw != null ? plan.Raw.SourceSha256 : null;

                payload.Extra.LineStart = c.LineStart;
                payload.Extra.LineEnd = c.LineEnd;
                payload.Extra.CharStart = c.CharStart;
                payload.Extra.CharEnd = c.CharEnd;

                // -------------------------
                // Index / embedding (Meta)
                // -------------------------
                payload.Meta.IndexVersion = ctx.IndexVersion;
                payload.Meta.EmbeddingModel = ctx.EmbeddingModel;
                payload.Meta.ContentHash = Sha256(text);
                payload.Meta.ContentLenChars = text.Length;
                payload.Meta.IndexedUtc = DateTime.UtcNow;

                // Optional numeric timestamps if you add them
                // payload.Meta.IndexedUnix = new DateTimeOffset(payload.Meta.IndexedUtc).ToUnixTimeSeconds();

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
