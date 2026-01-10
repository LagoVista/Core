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

                // Stable, commit-agnostic document id
                var fileDocId = CodeDocId.FileDocIdV5(canonical);

                // Qdrant point id (opaque)
                c.PointId = Guid.NewGuid().ToString("N");

                var text = c.TextNormalized ?? string.Empty;

                var title =
                    !string.IsNullOrWhiteSpace(code.TitleOverride) ? code.TitleOverride :
                    !string.IsNullOrWhiteSpace(c.Symbol) ? c.Symbol :
                    !string.IsNullOrWhiteSpace(c.Title) ? c.Title :
                    code.Path;

                var payload = new RagVectorPayload();

                // -------------------------
                // META (filterable / canonical)
                // -------------------------
                payload.Meta.OrgNamespace = ctx.OrgNamspace;
                payload.Meta.OrgId = ctx.OrgId;
                payload.Meta.ProjectId = ctx.ProjectId;
                payload.Meta.DocId = fileDocId.ToString();
           
                payload.Meta.ContentTypeId = RagContentType.SourceCode;
                payload.Meta.ContentType = RagContentType.SourceCode.ToString();
                payload.Meta.Subtype = string.IsNullOrWhiteSpace(code.Subtype) ? "CSharp" : code.Subtype;

                payload.Meta.SectionKey = c.SectionKey;
                payload.Meta.PartIndex = c.PartIndex;
                payload.Meta.PartTotal = c.PartTotal;

                payload.Meta.Title = title;
                payload.Meta.Language = code.Language;
                payload.Meta.Priority = 3;

                payload.Meta.IndexVersion = ctx.IndexVersion;
                payload.Meta.EmbeddingModel = ctx.EmbeddingModel;
                payload.Meta.ContentHash = Sha256(text);
                payload.Meta.ContentLenChars = text.Length;
                payload.Meta.IndexedUtc = DateTime.UtcNow;

                // Optional numeric timestamps if you add them
                // payload.Meta.IndexedUnix = new DateTimeOffset(payload.Meta.IndexedUtc).ToUnixTimeSeconds();

                // -------------------------
                // EXTRA (non-filter helpers)
                // -------------------------
                payload.Extra.FullDocumentBlobUri = plan.Raw?.SuggestedBlobPath;
                payload.Extra.SourceSha256 = plan.Raw?.SourceSha256;

                payload.Extra.LineStart = c.LineStart;
                payload.Extra.LineEnd = c.LineEnd;
                payload.Extra.CharStart = c.CharStart;
                payload.Extra.CharEnd = c.CharEnd;

                payload.Extra.Repo = code.Repo;
                payload.Extra.RepoBranch = code.RepoBranch;
                payload.Extra.CommitSha = code.CommitSha;
                payload.Extra.Path = code.Path;

                payload.Extra.Symbol = c.Symbol;
                payload.Extra.SymbolType = c.SymbolType;
                payload.Extra.StartLine = c.LineStart;
                payload.Extra.EndLine = c.LineEnd;

                // -------------------------
                // VALIDATION (unchanged semantics)
                // -------------------------
                var errs = RagVectorPayloadValidator.Validate(
                    payload,
                    new RagVectorPayloadValidator.ValidateOptions
                    {
                        RequireCodeRepoFields = true
                    });

                if (errs.Count > 0)
                {
                    throw new InvalidOperationException(
                        $"Invalid code payload for {payload.Meta.DocId}: {string.Join("; ", errs)}");
                }

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

        private static string Sha256(string text)
        {
            if (string.IsNullOrEmpty(text)) return null;

            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(text));
            var sb = new StringBuilder(bytes.Length * 2);
            foreach (var b in bytes) sb.Append(b.ToString("x2"));
            return sb.ToString();
        }
    }
}
