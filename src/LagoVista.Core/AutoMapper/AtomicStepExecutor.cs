using LagoVista.Core.Interfaces.AutoMapper;
using LagoVista.Core.Models;
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
                        case AtomicMapStepKind.EntityHeaderFromIdText:
                            AssignEntityHeaderFromIdText(source, target, step);
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

        private static void AssignEntityHeaderFromIdText(object source, object target, AtomicMapStep step)
        {
            if (step.SourceProperty == null)
                return;

            if (step.SourceProperty2 == null)
                throw new InvalidOperationException($"Atomic step '{AtomicMapStepKind.EntityHeaderFromIdText}' requires SourceProperty2 (Text).");

            var idObj = step.SourceProperty.GetValue(source);
            var textObj = step.SourceProperty2.GetValue(source);

            // IDs are always strings in this mapping family; treat null as null.
            var id = idObj as string;
            var text = textObj as string;

            var tt = step.TargetProperty.PropertyType;

            // Fast path: non-generic EntityHeader
            if (tt == typeof(EntityHeader))
            {
                step.TargetProperty.SetValue(target, new EntityHeader { Id = id, Text = text });
                return;
            }

            // Support EntityHeader<T> as well (we only set Id/Text on the base class).
            if (tt.IsGenericType && tt.GetGenericTypeDefinition() == typeof(EntityHeader<>))
            {
                var eh = (EntityHeader)Activator.CreateInstance(tt);
                eh.Id = id;
                eh.Text = text;
                step.TargetProperty.SetValue(target, eh);
                return;
            }

            throw new InvalidOperationException(
                $"Atomic step '{AtomicMapStepKind.EntityHeaderFromIdText}' requires target type EntityHeader or EntityHeader<T>, but was '{tt.FullName}'.");
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

            var raw = step.SourceProperty.GetValue(source);

            if (step.ConverterType != null)
            {
                if (!_converters.TryConvert(raw, step.TargetProperty.PropertyType, step.ConverterType, out var converted))
                {
                    var st = raw?.GetType()?.Name ?? step.SourceProperty.PropertyType.Name;
                    var tt = step.TargetProperty.PropertyType.Name;
                    throw new InvalidOperationException($"Planned converter '{step.ConverterType.Name}' could not convert {st} -> {tt}.");
                }

                step.TargetProperty.SetValue(target, converted);
                return;
            }

            if (!_converters.TryConvert(raw, step.TargetProperty.PropertyType, out var fallback))
            {
                var st = raw?.GetType()?.Name ?? step.SourceProperty.PropertyType.Name;
                var tt = step.TargetProperty.PropertyType.Name;
                throw new InvalidOperationException($"No converter registered at runtime for {st} -> {tt}.");
            }

            step.TargetProperty.SetValue(target, fallback);
        }
    }
}
