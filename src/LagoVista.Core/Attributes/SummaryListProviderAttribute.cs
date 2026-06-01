using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class SummaryListProviderAttribute : Attribute
    {
        public SummaryListProviderAttribute(string modelName)
        {
            if (String.IsNullOrWhiteSpace(modelName))
                throw new ArgumentNullException(nameof(modelName));

            ModelName = modelName;
        }

        public string ModelName { get; }

        public string Scope { get; set; } = SummaryListProviderScopes.Organization;

        public string Description { get; set; }
    }

    public static class SummaryListProviderScopes
    {
        public const string Organization = "Organization";
    }
}
