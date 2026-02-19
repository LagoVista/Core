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
    public sealed class MapIgnoreAttribute : Attribute
    {
        public string? Reason { get; }
        public MapIgnoreAttribute(string? reason = null) => Reason = reason;
    }
}
