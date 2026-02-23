using LagoVista.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class MapFromAttribute : Attribute
    {
        public string SourceProperty { get; }

        public MapFromAttribute(string sourceProperty)
        {
            SourceProperty = sourceProperty ?? throw new ArgumentNullException(nameof(sourceProperty));
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public sealed class MapToAttribute : Attribute
    {
        public string TargetProperty { get; }
        public MapToAttribute(string targetProperty)
            => TargetProperty = targetProperty ?? throw new ArgumentNullException(nameof(targetProperty));
    }


    [AttributeUsage(AttributeTargets.Property)]
    public sealed class MapToIgnoreAttribute : Attribute
    {
        public string? Reason { get; }
        public MapToIgnoreAttribute(string reason = null) => Reason = reason;
    }


    [AttributeUsage(AttributeTargets.Property)]
    public sealed class IgnoreOnMapToAttribute : Attribute
    {
        public string? Reason { get; }
        public IgnoreOnMapToAttribute(string reason = null) => Reason = reason;
    }

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ManualMappingAttribute : Attribute
    {
        public string? How { get; }
        public ManualMappingAttribute(string how = null) => How = how;
    }


    /// <summary>
    /// Put this on domain properties that must be represented in the DTO persistence layer.
    /// (Avoids having to ignore every transient/computed domain property.)
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class MapRequiredAttribute : Attribute
    {
        public string? Reason { get; }
        public MapRequiredAttribute(string reason = null) => Reason = reason;
    }

    /// <summary>
    /// Put this on domain properties that must be represented in the DTO persistence layer.
    /// (Avoids having to ignore every transient/computed domain property.)
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class DateOnlydAttribute : Attribute
    {
    }
}
