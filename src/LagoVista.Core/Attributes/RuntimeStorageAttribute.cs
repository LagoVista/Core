using System;

namespace LagoVista.Core.Attributes
{
    /// <summary>
    /// Marks an entity as runtime document data so document storage can route the
    /// entity type to its own provider-specific collection/container.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class RuntimeStorageAttribute : Attribute
    {
    }
}
