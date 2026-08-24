using System;

namespace LagoVista.Core.Attributes
{
    /// <summary>
    /// Marks an entity type for storage in a dedicated document collection/container
    /// named from the entity type rather than a shared EntityBase collection.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class DedicatedStorageCollectionAttribute : Attribute
    {
    }
}
