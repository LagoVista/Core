using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace LagoVista.Core.AI.Models
{
    /// <summary>
    /// AGN-034 — AgentExecuteRequest Streamlining
    ///
    /// Two request scenarios (presence-based, no explicit discriminator):
    ///
    /// 1) User Turn Request:
    ///    - Requires: SessionId, TurnId
    ///    - Must include at least one of: Instruction, InputArtifacts, ClipboardImages
    ///    - Must NOT include: ToolResults
    ///
    /// 2) Tool Continuation Submission:
    ///    - Requires: SessionId, TurnId, ToolResults (non-empty)
    ///    - Must NOT include: Instruction, InputArtifacts, ClipboardImages, RagScope, SolutionContextText, context hints, Streaming
    ///
    /// Mode/provider continuation/correlation/tracing are server-owned and MUST NOT appear here.
    /// </summary>
    public sealed class AgentExecuteRequest
    {
        /// <summary>
        /// Identified the AgentContext that should be used.
        /// Optional - only valid on first turn.  If not provided defaults from the organziation will be used.
        /// </summary>
        public string AgentContextId { get; set; }

        /// <summary>
        /// Identifies the conversation context that should be used within the AgentContext specifed above
        /// Optional - will not be valid without an AgentContextId
        /// </summary>
        public string ConversationContextId { get; set; }

        /// <summary>
        /// Identifies the session ("room") in which execution occurs.
        /// Required for all requests.
        /// </summary>
        public string SessionId { get; set; }

        /// <summary>
        /// Identifies the turn within the session.
        /// Required for all requests.
        /// </summary>
        public string TurnId { get; set; }

        /// <summary>
        /// Delivery preference only. Defaults to false when omitted.
        /// If true, the client MUST support streamed handling.
        /// Allowed only on User Turn Requests.
        /// </summary>
        public bool Streaming { get; set; } = false;

        /// <summary>
        /// Primary user input. UTF-8 text; Markdown canonical.
        /// Optional on User Turn Requests (files/images may convey intent).
        /// Forbidden on Tool Continuation Submissions.
        /// </summary>
        public string Instruction { get; set; }

        /// <summary>
        /// Files/artifacts supplied by the client for agent reasoning.
        /// Allowed only on User Turn Requests.
        /// Forbidden on Tool Continuation Submissions.
        /// </summary>
        public List<InputArtifact> InputArtifacts { get; set; } = new List<InputArtifact>();

        /// <summary>
        /// Images pasted from the clipboard (provenance-blind).
        /// Allowed only on User Turn Requests.
        /// Forbidden on Tool Continuation Submissions.
        /// </summary>
        public List<ClipboardImage> ClipboardImages { get; set; } = new List<ClipboardImage>();

        /// <summary>
        /// Platform-agnostic retrieval hint conditions.
        /// Allowed only on User Turn Requests.
        /// Forbidden on Tool Continuation Submissions.
        /// </summary>
        public RagScope RagScope { get; set; }

        /// <summary>
        /// Optional free-form solution/workspace context text.
        /// Session-latched (server stores/replaces when present; unchanged when absent).
        /// Allowed only on User Turn Requests.
        /// Forbidden on Tool Continuation Submissions.
        /// </summary>
        public string SolutionContextText { get; set; }

        /// <summary>
        /// Optional advisory workspace hint. Do not rely on for correctness.
        /// Allowed only on User Turn Requests.
        /// Forbidden on Tool Continuation Submissions.
        /// </summary>
        public string WorkspaceId { get; set; }

        /// <summary>
        /// Optional advisory repository hint. Do not rely on for correctness.
        /// Allowed only on User Turn Requests.
        /// Forbidden on Tool Continuation Submissions.
        /// </summary>
        public string Repo { get; set; }

        /// <summary>
        /// Optional advisory language hint (e.g., "csharp", "typescript").
        /// Allowed only on User Turn Requests.
        /// Forbidden on Tool Continuation Submissions.
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Tool execution results submitted by the client for continuation.
        /// Required (non-empty) on Tool Continuation Submissions.
        /// Forbidden on User Turn Requests.
        /// </summary>
        public List<ToolResultSubmission> ToolResults { get; set; } = new List<ToolResultSubmission>();

        /// <summary>
        /// Convenience helper (not required by AGN-034). Use to route validation.
        /// </summary>
        [JsonIgnore]
        public bool IsToolContinuation => ToolResults != null && ToolResults.Count > 0;

        /// <summary>
        /// Minimal, opinionated validation per AGN-034.
        /// Throws InvalidOperationException if contract rules are violated.
        /// </summary>
        public void Validate()
        {
            if (String.IsNullOrWhiteSpace(SessionId))
                throw new InvalidOperationException("SessionId is required.");

            if (String.IsNullOrWhiteSpace(TurnId))
                throw new InvalidOperationException("TurnId is required.");

            if (IsToolContinuation)
            {
                // Tool Continuation Submission rules
                if (!String.IsNullOrWhiteSpace(Instruction))
                    throw new InvalidOperationException("Instruction is forbidden on tool continuation submissions.");

                if (InputArtifacts != null && InputArtifacts.Count > 0)
                    throw new InvalidOperationException("InputArtifacts is forbidden on tool continuation submissions.");

                if (ClipboardImages != null && ClipboardImages.Count > 0)
                    throw new InvalidOperationException("ClipboardImages is forbidden on tool continuation submissions.");

                if (RagScope != null)
                    throw new InvalidOperationException("RagScope is forbidden on tool continuation submissions.");

                if (!String.IsNullOrWhiteSpace(SolutionContextText))
                    throw new InvalidOperationException("SolutionContextText is forbidden on tool continuation submissions.");

                if (!String.IsNullOrWhiteSpace(WorkspaceId) || !String.IsNullOrWhiteSpace(Repo) || !String.IsNullOrWhiteSpace(Language))
                    throw new InvalidOperationException("Workspace/environment hints are forbidden on tool continuation submissions.");

                if (Streaming)
                    throw new InvalidOperationException("Streaming is forbidden on tool continuation submissions.");

                foreach (var tr in ToolResults)
                    tr.Validate();
            }
            else
            {
                // User Turn Request rules
                if (ToolResults != null && ToolResults.Count > 0)
                    throw new InvalidOperationException("ToolResults is forbidden on user turn requests.");

                var hasInstruction = !String.IsNullOrWhiteSpace(Instruction);
                var hasArtifacts = InputArtifacts != null && InputArtifacts.Count > 0;
                var hasClipboardImages = ClipboardImages != null && ClipboardImages.Count > 0;

                if (!hasInstruction && !hasArtifacts && !hasClipboardImages)
                    throw new InvalidOperationException("User turn requests must include at least one of: Instruction, InputArtifacts, ClipboardImages.");

                if (hasArtifacts)
                {
                    foreach (var a in InputArtifacts)
                        a.Validate();
                }

                if (hasClipboardImages)
                {
                    foreach (var img in ClipboardImages)
                        img.Validate();
                }

                RagScope?.Validate();
            }
        }
    }

    public sealed class InputArtifact
    {
        /// <summary>
        /// Path relative to the VS Code opened root folder.
        /// This is the canonical identity in the request.
        /// </summary>
        public string RelativePath { get; set; }

        /// <summary>
        /// Convenience display name. Often derived from RelativePath.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Contents as UTF-8 text by default, or base64-encoded bytes when Encoding == "base64".
        /// </summary>
        public string Contents { get; set; }

        /// <summary>
        /// Optional encoding marker: "utf8" (default) or "base64".
        /// </summary>
        public string Encoding { get; set; } = InputArtifactEncoding.Utf8;

        /// <summary>
        /// Optional language hint (e.g., "csharp", "typescript").
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Optional mime type hint (e.g., "text/plain", "application/json", "image/png").
        /// </summary>
        public string MimeType { get; set; }

        /// <summary>
        /// Required. Indicates how the artifact was sourced by the client.
        /// Values: "ide" | "user".
        /// </summary>
        public string Origin { get; set; }

        public void Validate()
        {
            if (String.IsNullOrWhiteSpace(RelativePath))
                throw new InvalidOperationException("InputArtifact.RelativePath is required.");

            if (String.IsNullOrWhiteSpace(FileName))
                throw new InvalidOperationException("InputArtifact.FileName is required.");

            if (String.IsNullOrWhiteSpace(Contents))
                throw new InvalidOperationException("InputArtifact.Contents is required.");

            if (String.IsNullOrWhiteSpace(Origin) || (Origin != InputArtifactOrigin.Ide && Origin != InputArtifactOrigin.User))
                throw new InvalidOperationException($"InputArtifact.Origin must be '{InputArtifactOrigin.Ide}' or '{InputArtifactOrigin.User}'.");

            if (!String.IsNullOrWhiteSpace(Encoding) && Encoding != InputArtifactEncoding.Utf8 && Encoding != InputArtifactEncoding.Base64)
                throw new InvalidOperationException($"InputArtifact.Encoding must be '{InputArtifactEncoding.Utf8}' or '{InputArtifactEncoding.Base64}'.");

            // Defensive: no absolute paths in the contract
            if (RelativePath.Contains(":\\") || RelativePath.StartsWith("/") || RelativePath.StartsWith("\\"))
                throw new InvalidOperationException("InputArtifact.RelativePath must be relative (no absolute paths).");
        }
    }

    public static class InputArtifactOrigin
    {
        public const string Ide = "ide";
        public const string User = "user";
    }

    public static class InputArtifactEncoding
    {
        public const string Utf8 = "utf8";
        public const string Base64 = "base64";
    }

    public sealed class ClipboardImage
    {
        public string Id { get; set; }
        public string MimeType { get; set; }
        public string DataBase64 { get; set; }

        public void Validate()
        {
            if (String.IsNullOrWhiteSpace(Id))
                throw new InvalidOperationException("ClipboardImage.Id is required.");

            if (String.IsNullOrWhiteSpace(MimeType))
                throw new InvalidOperationException("ClipboardImage.MimeType is required.");

            if (String.IsNullOrWhiteSpace(DataBase64))
                throw new InvalidOperationException("ClipboardImage.DataBase64 is required.");
        }
    }

    public sealed class ToolResultSubmission
    {
        public string ToolCallId { get; set; }
        public int ExecutionMs { get; set; }

        /// <summary>
        /// JSON string payload (opaque to contract).
        /// Exactly one of ResultJson or ErrorMessage must be provided.
        /// </summary>
        public string ResultJson { get; set; }

        /// <summary>
        /// Tool failure message. Exactly one of ResultJson or ErrorMessage must be provided.
        /// </summary>
        public string ErrorMessage { get; set; }

        public void Validate()
        {
            if (String.IsNullOrWhiteSpace(ToolCallId))
                throw new InvalidOperationException("ToolResultSubmission.ToolCallId is required.");

            if (ExecutionMs < 0)
                throw new InvalidOperationException("ToolResultSubmission.ExecutionMs must be >= 0.");

            var hasResult = !String.IsNullOrWhiteSpace(ResultJson);
            var hasError = !String.IsNullOrWhiteSpace(ErrorMessage);

            if (hasResult == hasError)
                throw new InvalidOperationException("ToolResultSubmission must include exactly one of ResultJson or ErrorMessage.");
        }
    }

    public sealed class RagScope
    {
        public List<RagScopeCondition> Conditions { get; set; } = new List<RagScopeCondition>();

        public void Validate()
        {
            if (Conditions == null) return;

            foreach (var c in Conditions)
                c.Validate();
        }
    }

    public sealed class RagScopeCondition
    {
        public string Key { get; set; }

        /// <summary>
        /// Allowed operators: ==, !=, contains, does_not_contain
        /// </summary>
        public string Operator { get; set; }

        /// <summary>
        /// Any-of semantics. All conditions are ANDed.
        /// </summary>
        public List<string> Values { get; set; } = new List<string>();

        public void Validate()
        {
            if (String.IsNullOrWhiteSpace(Key))
                throw new InvalidOperationException("RagScopeCondition.Key is required.");

            if (String.IsNullOrWhiteSpace(Operator) ||
                (Operator != RagScopeOperator.Equals &&
                 Operator != RagScopeOperator.NotEquals &&
                 Operator != RagScopeOperator.Contains &&
                 Operator != RagScopeOperator.DoesNotContain))
            {
                throw new InvalidOperationException($"RagScopeCondition.Operator must be one of: {RagScopeOperator.Equals}, {RagScopeOperator.NotEquals}, {RagScopeOperator.Contains}, {RagScopeOperator.DoesNotContain}.");
            }

            if (Values == null || Values.Count == 0)
                throw new InvalidOperationException("RagScopeCondition.Values must be non-empty.");
        }
    }

    public static class RagScopeOperator
    {
        public const string Equals = "==";
        public const string NotEquals = "!=";
        public const string Contains = "contains";
        public const string DoesNotContain = "does_not_contain";
    }
}
