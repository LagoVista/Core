using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Attributes
{
    
    /// <summary>
    /// Put this on domain properties that must be represented in the DTO persistence layer.
    /// (Avoids having to ignore every transient/computed domain property.)
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class MapRequiredAttribute : Attribute
    {
        public string? Reason { get; }
        public MapRequiredAttribute(string? reason = null) => Reason = reason;
    }
}
