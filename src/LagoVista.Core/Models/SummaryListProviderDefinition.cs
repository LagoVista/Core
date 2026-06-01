using LagoVista.Core.Attributes;
using LagoVista.Core.Models.UIMetaData;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace LagoVista.Core.Models
{
    public class SummaryListProviderDefinition
    {
        public string ModelName { get; set; }

        public string Scope { get; set; }

        public string Description { get; set; }

        public Type EntityType { get; set; }

        public Type SummaryType { get; set; }

        public Type ServiceType { get; set; }

        public MethodInfo Method { get; set; }

        public string MethodName { get; set; }

        public IReadOnlyList<SummaryListProviderParameterDefinition> Parameters { get; set; }
    }

    public class SummaryListProviderParameterDefinition
    {
        public string Name { get; set; }

        public Type ParameterType { get; set; }
    }

    public class GetSummaryRecordsRequest
    {
        public string ModelName { get; set; }

        public ListRequest ListRequest { get; set; }

        public string Category { get; set; }

        public string Scope { get; set; } = SummaryListProviderScopes.Organization;
    }
}
