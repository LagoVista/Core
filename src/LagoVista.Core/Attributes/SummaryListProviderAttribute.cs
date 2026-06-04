using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class SummaryListProviderAttribute : Attribute
    {
        public SummaryListProviderAttribute(Type modelType)
        {
            ModelType = modelType ?? throw new ArgumentNullException(nameof(modelType));
        }

        public Type ModelType { get; }

        public string ModelName
        {
            get { return ModelType.Name; }
        }

        public string Scope { get; set; } = SummaryListProviderScopes.Organization;

        public string Description { get; set; }

        public Type ServiceType { get; set; }
    }

    public static class SummaryListProviderScopes
    {
        public const string Organization = "Organization";
    }
}
