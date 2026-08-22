using LagoVista.Core.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LagoVista.Core.AI.Models
{
    /// <summary>
    /// AGN-000040 outbound Client Directive delivered to a hosting client.
    /// </summary>
    public class ClientDirective
    {
        [JsonProperty("directiveId")]
        public string DirectiveId { get; set; }

        [JsonProperty("action")]
        public string Action { get; set; }

        [JsonProperty("preamble", NullValueHandling = NullValueHandling.Ignore)]
        public string Preamble { get; set; }

        [JsonProperty("payload", NullValueHandling = NullValueHandling.Ignore)]
        public JObject Payload { get; set; }
    }

    /// <summary>
    /// AGN-000040 constrained result returned by a client for one result-bearing Client Directive.
    /// Result is directive-defined and enum-like. At most one value form may be populated.
    /// </summary>
    public sealed class ClientDirectiveResult
    {
        [JsonProperty("directiveId")]
        public string DirectiveId { get; set; }

        [JsonProperty("action")]
        public string Action { get; set; }

        [JsonProperty("result")]
        public string Result { get; set; }

        [JsonProperty("scalar", NullValueHandling = NullValueHandling.Ignore)]
        public ClientDirectiveScalarValue Scalar { get; set; }

        [JsonProperty("multiSelect", NullValueHandling = NullValueHandling.Ignore)]
        public ClientDirectiveMultiSelectValue MultiSelect { get; set; }

        [JsonProperty("entityHeader", NullValueHandling = NullValueHandling.Ignore)]
        public EntityHeader EntityHeader { get; set; }

        public void Validate()
        {
            if (String.IsNullOrWhiteSpace(DirectiveId))
                throw new InvalidOperationException("ClientDirectiveResult.DirectiveId is required.");

            if (String.IsNullOrWhiteSpace(Action))
                throw new InvalidOperationException("ClientDirectiveResult.Action is required.");

            if (String.IsNullOrWhiteSpace(Result))
                throw new InvalidOperationException("ClientDirectiveResult.Result is required.");

            var populatedValueCount = (Scalar != null ? 1 : 0) + (MultiSelect != null ? 1 : 0) + (EntityHeader != null ? 1 : 0);
            if (populatedValueCount > 1)
                throw new InvalidOperationException("ClientDirectiveResult may include at most one of Scalar, MultiSelect, or EntityHeader.");

            Scalar?.Validate();
            MultiSelect?.Validate();
        }
    }

    /// <summary>
    /// Exactly one scalar representation may be populated.
    /// </summary>
    public sealed class ClientDirectiveScalarValue
    {
        [JsonProperty("stringValue", NullValueHandling = NullValueHandling.Ignore)]
        public string StringValue { get; set; }

        [JsonProperty("numberValue", NullValueHandling = NullValueHandling.Ignore)]
        public decimal? NumberValue { get; set; }

        [JsonProperty("flagValue", NullValueHandling = NullValueHandling.Ignore)]
        public bool? FlagValue { get; set; }

        public void Validate()
        {
            var populatedValueCount = (StringValue != null ? 1 : 0) + (NumberValue.HasValue ? 1 : 0) + (FlagValue.HasValue ? 1 : 0);
            if (populatedValueCount != 1)
                throw new InvalidOperationException("ClientDirectiveScalarValue must include exactly one of StringValue, NumberValue, or FlagValue.");
        }
    }

    /// <summary>
    /// Multi-select results are homogeneous and contain either strings or numbers.
    /// </summary>
    public sealed class ClientDirectiveMultiSelectValue
    {
        [JsonProperty("stringValues", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> StringValues { get; set; }

        [JsonProperty("numberValues", NullValueHandling = NullValueHandling.Ignore)]
        public List<decimal> NumberValues { get; set; }

        public void Validate()
        {
            var hasStrings = StringValues != null && StringValues.Count > 0;
            var hasNumbers = NumberValues != null && NumberValues.Count > 0;

            if (hasStrings == hasNumbers)
                throw new InvalidOperationException("ClientDirectiveMultiSelectValue must include exactly one non-empty StringValues or NumberValues collection.");

            if (hasStrings && StringValues.Any(value => value == null))
                throw new InvalidOperationException("ClientDirectiveMultiSelectValue.StringValues may not contain null values.");
        }
    }
}
