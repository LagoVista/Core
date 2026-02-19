using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Interfaces.AutoMapper
{
    public interface IMappingPlan
    {
        Type SourceType { get; }
        Type TargetType { get; }

        bool CanDecrypt { get; }
        bool CanEncrypt { get; }

        IReadOnlyList<MappingBinding> Bindings { get; } // for verification/debug
        void Apply(object source, object target);        // for runtime
    }

    public class MappingBinding
    {
        public MappingBinding(string targetProperty, string sourceProperty, MappingBindingKind kind)
        {
            TargetProperty = targetProperty ?? throw new ArgumentNullException(nameof(targetProperty));
            SourceProperty = sourceProperty ?? throw new ArgumentNullException(nameof(sourceProperty));
            Kind = kind;
        }

        public string TargetProperty { get; set; }
        public string SourceProperty { get; set; }
        public MappingBindingKind Kind { get; set; }
    }

    public enum MappingBindingKind
    {
        Direct,        // sourceProp -> targetProp (maybe via converter)
        MapToFanout,   // resolved via [MapTo]
        Ignored        // [MapIgnore] (or future ignore rules)
    }

}
