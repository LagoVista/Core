using System;

namespace LagoVista.Core.Attributes
{
    /// <summary>
    /// Prevents automatic ProducedArtifact generation when an entity is saved through document storage.
    /// The entity itself is still persisted normally.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class SkipProducedArtifactGenerationAttribute : Attribute
    {
    }
}
