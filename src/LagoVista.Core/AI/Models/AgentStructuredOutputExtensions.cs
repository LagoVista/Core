using System;
using System.Linq;

namespace LagoVista.Core.AI.Models
{
    public static class AgentStructuredOutputExtensions
    {
        public static bool TryGetStructuredOutput(this AgentExecuteResponse response, string schemaKey, out AgentStructuredOutput output)
        {
            output = null;

            if (response?.StructuredOutputs == null || String.IsNullOrWhiteSpace(schemaKey))
                return false;

            output = response.StructuredOutputs.FirstOrDefault(item => item != null && item.SchemaKey == schemaKey);
            return output != null;
        }

        public static bool TryGetStructuredPayload<T>(this AgentExecuteResponse response, string schemaKey, out T payload)
        {
            payload = default;

            if (!response.TryGetStructuredOutput(schemaKey, out var output))
                return false;

            return output.TryGetPayload(out payload);
        }

        public static T GetStructuredPayload<T>(this AgentExecuteResponse response, string schemaKey)
        {
            if (!response.TryGetStructuredPayload<T>(schemaKey, out var payload))
                throw new InvalidOperationException($"Structured output '{schemaKey}' could not be found or deserialized as {typeof(T).FullName}.");

            return payload;
        }
    }
}
