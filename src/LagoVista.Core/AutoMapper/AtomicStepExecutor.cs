using LagoVista.Core.Interfaces.AutoMapper;
using System;
using System.Collections.Generic;

namespace LagoVista.Core.AutoMapper
{
    /// <summary>
    /// Executes AtomicMapStep records for a single source/target object pair.
    /// Child recursion is handled separately (ChildSteps).
    /// </summary>
    public sealed class AtomicStepExecutor
    {
        private readonly IMapValueConverterRegistry _converters;

        public AtomicStepExecutor(IMapValueConverterRegistry converters)
        {
            _converters = converters ?? throw new ArgumentNullException(nameof(converters));
        }

        public void Execute(object source, object target, IReadOnlyList<AtomicMapStep> steps)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (steps == null) throw new ArgumentNullException(nameof(steps));

            for (var i = 0; i < steps.Count; i++)
            {
                var step = steps[i];
                if (step.ConverterType != null)
                {
                    AssignWithConverter(source, target, step);
                }
                else
                {
                    switch (step.Kind)
                    {
                        case AtomicMapStepKind.Manual:
                        case AtomicMapStepKind.Ignored:
                        case AtomicMapStepKind.Crypto:
                        case AtomicMapStepKind.ChildLeaf:
                            // Not executed here.
                            continue;

                        case AtomicMapStepKind.DirectAssign:
                        case AtomicMapStepKind.NullableDirectAssign:
                        case AtomicMapStepKind.MapToFanoutAssign:
                            AssignDirect(source, target, step);
                            continue;

                        case AtomicMapStepKind.ConverterAssign:
                            AssignWithConverter(source, target, step);
                            continue;

                        default:
                            throw new NotSupportedException($"Unsupported atomic step kind: {step.Kind}.");
                    }
                }
            }
        }

        private static void AssignDirect(object source, object target, AtomicMapStep step)
        {
            if (step.SourceProperty == null)
                return;

            var value = step.SourceProperty.GetValue(source);
            step.TargetProperty.SetValue(target, value);
        }

        private void AssignWithConverter(object source, object target, AtomicMapStep step)
        {
            if (step.SourceProperty == null)
                return;

            var value = step.SourceProperty.GetValue(source);
            if (!_converters.TryConvert(value, step.TargetProperty.PropertyType, out var converted))
            {
                var st = value?.GetType()?.Name ?? step.SourceProperty.PropertyType.Name;
                var tt = step.TargetProperty.PropertyType.Name;
                throw new InvalidOperationException($"No converter registered at runtime for {st} -> {tt}.");
            }

            step.TargetProperty.SetValue(target, converted);
        }
    }
}
