using System;
using System.Collections.Generic;
using System.Reflection;

namespace LagoVista.Core.AutoMapper
{
    public abstract class MappingPlan
    {
        public abstract Type SourceType { get; }
        public abstract Type TargetType { get; }

        public IReadOnlyList<AtomicMapStep> AtomicSteps { get; protected set; } = Array.Empty<AtomicMapStep>();
        public IReadOnlyList<IChildMapStep> ChildSteps { get; protected set; } = Array.Empty<IChildMapStep>();
    }

    public sealed class MappingPlan<TSource, TTarget> : MappingPlan
    {
        public override Type SourceType => typeof(TSource);
        public override Type TargetType => typeof(TTarget);

        public MappingPlan(IReadOnlyList<AtomicMapStep> atomicSteps, IReadOnlyList<IChildMapStep> childSteps)
        {
            AtomicSteps = atomicSteps ?? throw new ArgumentNullException(nameof(atomicSteps));
            ChildSteps = childSteps ?? throw new ArgumentNullException(nameof(childSteps));
        }
    }

    public sealed class AtomicMapStep
    {
        public PropertyInfo TargetProperty { get; }
        public PropertyInfo SourceProperty { get; }
        public AtomicMapStepKind Kind { get; }
        public Type ConverterTargetType { get; }

        public AtomicMapStep(PropertyInfo targetProperty, PropertyInfo sourceProperty, AtomicMapStepKind kind, Type converterTargetType = null)
        {
            TargetProperty = targetProperty ?? throw new ArgumentNullException(nameof(targetProperty));
            SourceProperty = sourceProperty;
            Kind = kind;
            ConverterTargetType = converterTargetType;
        }
    }

    public enum AtomicMapStepKind
    {
        DirectAssign,
        NullableDirectAssign,
        ConverterAssign,
        MapToFanoutAssign,
        Ignored,
        Crypto
    }

    public interface IChildMapStep
    {
        ChildMapStepKind Kind { get; }

        PropertyInfo TargetProperty { get; }
        PropertyInfo SourceProperty { get; }

        Type ChildSourceType { get; }
        Type ChildTargetType { get; }

        IReadOnlyList<IChildMapStep> Children { get; }
    }

    public enum ChildMapStepKind
    {
        Object,
        Collection,
        EntityHeaderValue
    }

    public sealed class ChildMapStep : IChildMapStep
    {
        public ChildMapStepKind Kind { get; }
        public PropertyInfo TargetProperty { get; }
        public PropertyInfo SourceProperty { get; }
        public Type ChildSourceType { get; }
        public Type ChildTargetType { get; }
        public IReadOnlyList<IChildMapStep> Children { get; }

        public ChildMapStep(ChildMapStepKind kind, PropertyInfo targetProperty, PropertyInfo sourceProperty, Type childSourceType, Type childTargetType, IReadOnlyList<IChildMapStep> children)
        {
            Kind = kind;
            TargetProperty = targetProperty ?? throw new ArgumentNullException(nameof(targetProperty));
            SourceProperty = sourceProperty ?? throw new ArgumentNullException(nameof(sourceProperty));
            ChildSourceType = childSourceType ?? throw new ArgumentNullException(nameof(childSourceType));
            ChildTargetType = childTargetType ?? throw new ArgumentNullException(nameof(childTargetType));
            Children = children ?? Array.Empty<IChildMapStep>();
        }
    }
}