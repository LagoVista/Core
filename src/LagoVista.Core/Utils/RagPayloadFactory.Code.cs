// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 4677e189e98954c464e22fd132116fe707717183cae45605f7987fcf83d246bd
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
        /// Builds payloads for code artifacts only.
        /// Excludes fields such as Audience, Persona, Stage, Labels, etc.
        /// </summary>
        public static IReadOnlyList<PayloadBuildResult> FromCodePlan(
            RagChunkPlan plan,
            IngestContext ctx,
            CodeArtifactContext code)
        {
            if (plan == null) throw new ArgumentNullException(nameof(plan));
            if (ctx == null) throw new ArgumentNullException(nameof(ctx));
            if (code == null) throw new ArgumentNullException(nameof(code));

            var results = new List<PayloadBuildResult>(plan.Chunks?.Count ?? 0);

            foreach (var c in plan.Chunks ?? Enumerable.Empty<RagChunk>())
            {
                var canonical = CodeDocId.Canonical(code.Repo, code.Path);

                // Option A: GUID v5 (recommended)
                var fileDocId = CodeDocId.FileDocIdV5(canonical);

                // Option B: 32-hex short ID
                // var fileDocIdHex = CodeDocId.FileDocIdHex128(canonical);  // if you prefer string id

                c.PointId = Guid.NewGuid().ToString().ToLower();

                // Build stable point id (commit-agnostic)
          //      var pointId = CodePointId.Build(fileDocId, c.SectionKey, c.PartIndex);
                // (optional) also stamp it back onto the chunk for traceability:
            //    if (string.IsNullOrWhiteSpace(c.PointId)) c.PointId = pointId;
                var text = c.TextNormalized ?? string.Empty;

                var title = !string.IsNullOrWhiteSpace(code.TitleOverride) ? code.TitleOverride
                          : (!string.IsNullOrWhiteSpace(c.Symbol) ? c.Symbol
                          : (!string.IsNullOrWhiteSpace(c.Title) ? c.Title : code.Path));

                var payload = new RagVectorPayload
                {
                    // Identity / tenancy
                    OrgId = ctx.OrgId,
                    ProjectId = ctx.ProjectId,
                    DocId = fileDocId.ToString(),

                    // Classification
                    ContentTypeId = RagContentType.SourceCode,
                    ContentType = RagContentType.SourceCode.ToString(),
                    Subtype = string.IsNullOrWhiteSpace(code.Subtype) ? "CSharp" : code.Subtype,

                    // Sectioning
                    SectionKey = c.SectionKey,
                    PartIndex = c.PartIndex,
                    PartTotal = c.PartTotal,

                    // Core meta
                    Title = title,
                    Language = code.Language,
                    Priority = 3, // neutral default

                    // Provenance
                    BlobUri = plan.Raw?.SuggestedBlobPath,
                    SourceSha256 = plan.Raw?.SourceSha256,
                    LineStart = c.LineStart,
                    LineEnd = c.LineEnd,
                    CharStart = c.CharStart,
                    CharEnd = c.CharEnd,

                    // Index / embedding
                    IndexVersion = ctx.IndexVersion,
                    EmbeddingModel = ctx.EmbeddingModel,
                    ContentHash = Sha256(text),
                    ContentLenChars = text.Length,
                    IndexedUtc = DateTime.UtcNow,

                    // SourceCode-specific
                    Repo = code.Repo,
                    RepoBranch = code.RepoBranch,
                    CommitSha = code.CommitSha,
                    Path = code.Path,
                    Symbol = c.Symbol,
                    SymbolType = c.SymbolType,
                    StartLine = c.LineStart,
                    EndLine = c.LineEnd
                };

                var errs = RagVectorPayloadValidator.Validate(payload,
                    new RagVectorPayloadValidator.ValidateOptions { RequireCodeRepoFields = true });
                if (errs.Count > 0)
                    throw new InvalidOperationException($"Invalid code payload for {c.DocId}: {string.Join("; ", errs)}");

                results.Add(new PayloadBuildResult
                {
                    PointId = c.PointId,
                    Payload = payload,
                    TextForEmbedding = c.TextNormalized,
                    EstimatedTokens = EstimateTokens(text),
                    Vector = c.Vector
                    
                });
            }

            return results;
        }
    }
}
 