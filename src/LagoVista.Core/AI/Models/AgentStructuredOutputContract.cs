using LagoVista.Core.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LagoVista.Core.AI.Models
{
    /// <summary>
    /// Declares the structured output contract expected from an agent execution turn.
    /// The public contract is provider-agnostic; provider-specific request builders translate this shape as needed.
    /// </summary>
    public sealed class AgentStructuredOutputContract
    {
        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("instructions", NullValueHandling = NullValueHandling.Ignore)]
        public string Instructions { get; set; }

        [JsonProperty("required")]
        public bool Required { get; set; } = true;

        [JsonProperty("allowMultipleResults")]
        public bool AllowMultipleResults { get; set; } = false;

        [JsonProperty("schemas")]
        public List<AgentStructuredOutputSchema> Schemas { get; set; } = new List<AgentStructuredOutputSchema>();

        public void Validate()
        {
            if (String.IsNullOrWhiteSpace(Key))
                throw new InvalidOperationException("StructuredOutput.Key is required.");

            if (Schemas == null || Schemas.Count == 0)
                throw new InvalidOperationException("StructuredOutput.Schemas must contain at least one schema.");

            var duplicateSchemaKeys = Schemas.Where(schema => schema != null && !String.IsNullOrWhiteSpace(schema.Key)).GroupBy(schema => schema.Key).Where(group => group.Count() > 1).Select(group => group.Key).ToList();

            if (duplicateSchemaKeys.Count > 0)
                throw new InvalidOperationException($"StructuredOutput.Schemas contains duplicate schema keys: {String.Join(", ", duplicateSchemaKeys)}.");

            var defaultCount = Schemas.Count(schema => schema != null && schema.IsDefault);

            if (defaultCount > 1)
                throw new InvalidOperationException("StructuredOutput.Schemas may contain at most one default schema.");

            foreach (var schema in Schemas)
            {
                if (schema == null)
                    throw new InvalidOperationException("StructuredOutput.Schemas cannot contain null entries.");

                schema.Validate();
            }
        }
    }

    /// <summary>
    /// Describes one allowed structured output schema for an agent execution turn.
    /// </summary>
    public sealed class AgentStructuredOutputSchema
    {
        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("dotNetTypeName", NullValueHandling = NullValueHandling.Ignore)]
        public string DotNetTypeName { get; set; }

        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }

        [JsonProperty("jsonSchema")]
        public string JsonSchema { get; set; }

        [JsonProperty("isDefault")]
        public bool IsDefault { get; set; }

        public static AgentStructuredOutputSchema FromType<T>(string key, string description = null, bool isDefault = false)
        {
            var schema = JsonSchemaBuilder.FromType<T>();

            return new AgentStructuredOutputSchema
            {
                Key = key,
                DotNetTypeName = typeof(T).FullName,
                Description = description,
                JsonSchema = schema.ToString(Formatting.Indented),
                IsDefault = isDefault
            };
        }

        public void Validate()
        {
            if (String.IsNullOrWhiteSpace(Key))
                throw new InvalidOperationException("StructuredOutput.Schema.Key is required.");

            if (String.IsNullOrWhiteSpace(JsonSchema))
                throw new InvalidOperationException($"StructuredOutput.Schema.JsonSchema is required for schema '{Key}'.");
        }
    }

    /// <summary>
    /// Contains one model-produced structured output payload and validation state.
    /// </summary>
    public sealed class AgentStructuredOutput
    {
        [JsonProperty("contractKey")]
        public string ContractKey { get; set; }

        [JsonProperty("schemaKey")]
        public string SchemaKey { get; set; }

        [JsonProperty("dotNetTypeName", NullValueHandling = NullValueHandling.Ignore)]
        public string DotNetTypeName { get; set; }

        [JsonProperty("payload")]
        public JObject Payload { get; set; }

        [JsonProperty("isValid")]
        public bool IsValid { get; set; }

        [JsonProperty("validationErrors", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> ValidationErrors { get; set; } = new List<string>();

        public bool TryGetPayload<T>(out T payload)
        {
            payload = default;

            if (!IsValid || Payload == null)
                return false;

            payload = Payload.ToObject<T>();
            return payload != null;
        }

        public T GetPayload<T>()
        {
            if (!TryGetPayload<T>(out var payload))
                throw new InvalidOperationException($"Structured output payload could not be deserialized as {typeof(T).FullName}.");

            return payload;
        }

        public void AddValidationError(string error)
        {
            if (ValidationErrors == null)
                ValidationErrors = new List<string>();

            if (!String.IsNullOrWhiteSpace(error))
                ValidationErrors.Add(error);

            IsValid = false;
        }
    }
}
