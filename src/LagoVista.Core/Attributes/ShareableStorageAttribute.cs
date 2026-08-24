using System;

namespace LagoVista.Core.Attributes
{
    /// <summary>
    /// Marks an entity type as eligible for shared/public document storage semantics.
    /// Records remain owned by their organization, but public records may be read across organizations.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class ShareableStorageAttribute : Attribute
    {
    }
}
