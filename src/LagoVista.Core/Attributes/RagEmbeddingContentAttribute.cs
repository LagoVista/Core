using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class RagEmbeddingContentAttribute : Attribute
    {
        public RagEmbeddingContentAttribute()
        {
        }

        /// <summary>
        /// Optional stable reference type override.
        ///
        /// When omitted, the reference type is derived from the property name.
        /// For example, ExampleUtterances becomes "example-utterances".
        /// </summary>
        public string ReferenceType { get; set; }

        /// <summary>
        /// Relative retrieval priority for vectors generated from this property.
        /// Higher values may be used to influence ranking or diagnostics.
        /// </summary>
        public int Priority { get; set; } = 50;
    }
}
