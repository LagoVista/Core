using System.Collections.Generic;
using LagoVista.Core.Models;
using Newtonsoft.Json;

namespace LagoVista.Core.AI.Models
{
    public class AgentExecuteRequest
    {
        public EntityHeader AgentContext { get; set; }

        public EntityHeader ConversationContext { get; set; }

        public string SessionId { get; set; }

        public string PreviousTurnId { get; set; }
        public string CurrentTurnId { get; set; }
        public string ResponseContinuationId { get; set; }

        public string Mode { get; set; }

        public bool Streaming { get; set; } = false;

        public string SystemPrompt { get; set; }
        public string Instruction { get; set; }

        public string WorkspaceId { get; set; }

        public string Repo { get; set; }

        public string Language { get; set; }


        public RagScopeFilter RagScopeFilter { get; set; } = new RagScopeFilter();

        public List<ActiveFile> ActiveFiles { get; set; } = new List<ActiveFile>();

        /// <summary>
        /// JSON-serialized array of tool definitions to be passed directly to the LLM `tools` parameter.
        /// This is owned by the client (VS Code extension) for v1.
        /// </summary>
        public string ToolsJson { get; set; }

        /// <summary>
        /// Optional tool choice hint for the LLM.
        /// If null or empty, default to 'auto'.
        /// If set to a tool name, the server should set `tool_choice` to that tool.
        /// </summary>
        public string ToolChoiceName { get; set; }

        /// <summary>
        /// JSON-serialized array of tool results previously produced by the client or server.
        /// Shape should mirror the "tool_output" / "tool_result" the LLM expects.
        /// </summary>
        public string ToolResultsJson { get; set; }

        /// <summary>
        /// Optional strongly-typed convenience mirror of ToolResultsJson.
        /// </summary>
        public List<AgentToolCall> ToolResults { get; set; } = new List<AgentToolCall>();

        public List<ImageAttachment> ImageAttachments { get; set; } = new List<ImageAttachment>();
    }

    public class ImageAttachment
    {
        public string Id { get; set; }
        public string MimeType { get; set; }
        public string DataBase64 { get; set; }
    }


    /// <summary>
    /// Top-level filter object passed to Qdrant as "filter".
    /// This is what you'll treat as your RAG scope definition.
    /// </summary>
    public class RagScopeFilter
    {
        // All conditions must match (AND)
        [JsonProperty("must", NullValueHandling = NullValueHandling.Ignore)]
        public List<RagScopeCondition> Must { get; set; }

        // None of the conditions may match (NOT)
        [JsonProperty("must_not", NullValueHandling = NullValueHandling.Ignore)]
        public List<RagScopeCondition> MustNot { get; set; }

        // At least one should match (OR)
        [JsonProperty("should", NullValueHandling = NullValueHandling.Ignore)]
        public List<RagScopeCondition> Should { get; set; }

        // At least MinCount of the conditions should match
        [JsonProperty("min_should", NullValueHandling = NullValueHandling.Ignore)]
        public RagScopeMinShould MinShould { get; set; }

        public RagScopeFilter()
        {
            Must = new List<RagScopeCondition>();
            MustNot = new List<RagScopeCondition>();
            Should = new List<RagScopeCondition>();
        }

        #region Helper factory methods (optional)

        public static RagScopeFilter Match(string key, object value)
        {
            var filter = new RagScopeFilter();
            filter.Must.Add(RagScopeCondition.MatchIndex(key, value));
            return filter;
        }

        public static RagScopeFilter Range(string key, double? gte = null, double? lte = null)
        {
            var filter = new RagScopeFilter();
            filter.Must.Add(RagScopeCondition.RangeInIndex(key, gte, lte));
            return filter;
        }

        #endregion
    }

    /// <summary>
    /// Single condition. Only one of Match / Range / HasId / Nested / IsEmpty / IsNull
    /// is typically used at a time (Qdrant treats this as a "one-of" union).
    /// </summary>
    public class RagScopeCondition
    {
        // For field-based filters
        [JsonProperty("key", NullValueHandling = NullValueHandling.Ignore)]
        public string Key { get; set; }

        [JsonProperty("match", NullValueHandling = NullValueHandling.Ignore)]
        public RagScopeMatch Match { get; set; }

        [JsonProperty("range", NullValueHandling = NullValueHandling.Ignore)]
        public RagScopeRange Range { get; set; }

        [JsonProperty("values_count", NullValueHandling = NullValueHandling.Ignore)]
        public RagScopeValuesCount ValuesCount { get; set; }

        // has_id condition
        [JsonProperty("has_id", NullValueHandling = NullValueHandling.Ignore)]
        public List<object> HasId { get; set; }

        // Empty / null field checks
        [JsonProperty("is_empty", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsEmpty { get; set; }

        [JsonProperty("is_null", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsNull { get; set; }

        // Nested filter (for arrays of objects)
        [JsonProperty("nested", NullValueHandling = NullValueHandling.Ignore)]
        public RagScopeNested Nested { get; set; }

        public RagScopeCondition()
        {
        }

        public static RagScopeCondition MatchIndex(string key, object value)
        {
            return new RagScopeCondition
            {
                Key = key,
                Match = new RagScopeMatch { Value = value }
            };
        }

        public static RagScopeCondition MatchAnyIndex(string key, IEnumerable<object> values)
        {
            return new RagScopeCondition
            {
                Key = key,
                Match = new RagScopeMatch { Any = new List<object>(values) }
            };
        }

        public static RagScopeCondition RangeInIndex(string key, double? gte = null, double? lte = null)
        {
            return new RagScopeCondition
            {
                Key = key,
                Range = new RagScopeRange
                {
                    Gte = gte,
                    Lte = lte
                }
            };
        }

        public static RagScopeCondition HasIds(IEnumerable<object> ids)
        {
            return new RagScopeCondition
            {
                HasId = new List<object>(ids)
            };
        }
    }

    /// <summary>
    /// Match condition: exact value, any-of, except, or full-text style (text / phrase).
    /// Maps to Qdrant's "match" object. :contentReference[oaicite:1]{index=1}
    /// </summary>
    public class RagScopeMatch
    {
        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        public object Value { get; set; }

        [JsonProperty("any", NullValueHandling = NullValueHandling.Ignore)]
        public List<object> Any { get; set; }

        [JsonProperty("except", NullValueHandling = NullValueHandling.Ignore)]
        public List<object> Except { get; set; }

        [JsonProperty("text", NullValueHandling = NullValueHandling.Ignore)]
        public string Text { get; set; }

        [JsonProperty("phrase", NullValueHandling = NullValueHandling.Ignore)]
        public string Phrase { get; set; }
    }

    /// <summary>
    /// Numeric or datetime range. Qdrant allows both numeric and date ranges.
    /// </summary>
    public class RagScopeRange
    {
        [JsonProperty("gt", NullValueHandling = NullValueHandling.Ignore)]
        public double? Gt { get; set; }

        [JsonProperty("gte", NullValueHandling = NullValueHandling.Ignore)]
        public double? Gte { get; set; }

        [JsonProperty("lt", NullValueHandling = NullValueHandling.Ignore)]
        public double? Lt { get; set; }

        [JsonProperty("lte", NullValueHandling = NullValueHandling.Ignore)]
        public double? Lte { get; set; }
    }

    /// <summary>
    /// Minimum-should clause: at least MinCount of Conditions must match.
    /// </summary>
    public class RagScopeMinShould
    {
        [JsonProperty("min_count")]
        public int MinCount { get; set; }

        [JsonProperty("conditions")]
        public List<RagScopeCondition> Conditions { get; set; }

        public RagScopeMinShould()
        {
            Conditions = new List<RagScopeCondition>();
        }
    }

    /// <summary>
    /// Condition on array-of-objects payloads (nested filter).
    /// </summary>
    public class RagScopeNested
    {
        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("filter")]
        public RagScopeFilter Filter { get; set; }
    }

    /// <summary>
    /// Filter by number of values in an array field.
    /// </summary>
    public class RagScopeValuesCount
    {
        [JsonProperty("gt", NullValueHandling = NullValueHandling.Ignore)]
        public int? Gt { get; set; }

        [JsonProperty("gte", NullValueHandling = NullValueHandling.Ignore)]
        public int? Gte { get; set; }

        [JsonProperty("lt", NullValueHandling = NullValueHandling.Ignore)]
        public int? Lt { get; set; }

        [JsonProperty("lte", NullValueHandling = NullValueHandling.Ignore)]
        public int? Lte { get; set; }
    }
}
