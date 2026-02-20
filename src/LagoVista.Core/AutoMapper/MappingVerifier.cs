using LagoVista.Core.Attributes;
using LagoVista.Core.Interfaces.AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LagoVista.Core.AutoMapper
{
    public static class MappingVerifier
    {
        public static void Verify<TSource, TTarget>(IMappingPlanBuilder planBuilder = null, IMapValueConverterRegistry converters = null)
           where TSource : class
           where TTarget : class
        {

            Verify(typeof(TSource), typeof(TTarget), planBuilder, converters);
            Verify(typeof(TTarget), typeof(TSource), planBuilder, converters);
        }

        public static void Verify(
            Type sourceType,
            Type targetType,
            IMappingPlanBuilder planBuilder,
            IMapValueConverterRegistry converters = null)
        {
            if (sourceType == null) throw new ArgumentNullException(nameof(sourceType));
            if (targetType == null) throw new ArgumentNullException(nameof(targetType));

            // Keep your current behavior of defaulting / augmenting converters.
            if (converters == null)
            {
                converters = ConvertersStartup.DefaultConverterRegistery;
            }
            else
            {
                converters.AddRange(ConvertersStartup.DefaultConverterRegistery.Converters.ToArray());
            }

            if (planBuilder == null)
                planBuilder = new ReflectionMappingPlanBuilder(converters);

            var plan = planBuilder.Build(sourceType, targetType);

            // Target properties that are in-scope for verification.
            var targetProps = targetType
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanWrite)
                .Where(p => p.GetIndexParameters().Length == 0)
                .ToArray();

            // What the plan says it will handle.
            var boundTargets = new HashSet<string>(
                plan.Bindings
                    .Where(b => b.Kind != MappingBindingKind.Ignored)
                    .Select(b => b.TargetProperty),
                StringComparer.OrdinalIgnoreCase);

            var ignoredTargets = new HashSet<string>(
                plan.Bindings
                    .Where(b => b.Kind == MappingBindingKind.Ignored)
                    .Select(b => b.TargetProperty),
                StringComparer.OrdinalIgnoreCase);

            // Optional: detect duplicates (two bindings to same target).
            var dupes = plan.Bindings
                .Where(b => b.Kind != MappingBindingKind.Ignored)
                .GroupBy(b => b.TargetProperty, StringComparer.OrdinalIgnoreCase)
                .Where(g => g.Count() > 1)
                .ToArray();

            var errors = new List<string>();

            if (dupes.Length > 0)
            {
                foreach (var dupe in dupes)
                {
                    errors.Add(
                        $"Multiple bindings for target '{targetType.Name}.{dupe.Key}': " +
                        string.Join(", ", dupe.Select(b => $"{sourceType.Name}.{b.SourceProperty} ({b.Kind})")));
                }
            }

            // Core policy: every writable target prop must be resolved or explicitly ignored.
            for (var i = 0; i < targetProps.Length; ++i)
            {
                var tprop = targetProps[i];

                // Respect IgnoreOnMapToAttribute as "ignored" even if builder doesn’t emit it for some reason.
                if (tprop.GetCustomAttribute<IgnoreOnMapToAttribute>() != null)
                    continue;

                if (ignoredTargets.Contains(tprop.Name))
                    continue;

                if (boundTargets.Contains(tprop.Name))
                    continue;

                // Not resolved.
                errors.Add(
                    $"Missing source for target '{targetType.Name}.{tprop.Name}'. " +
                    $"Use [MapFrom(\"...\")], [MapIgnore], or [MapTo] (or update plan builder rules).");
            }

            // Optional: type compatibility check based on plan bindings.
            // This gives better diagnostics than “missing source” when a binding exists but is nonsensical.
            // (Your builder’s runtime behavior is permissive, but verifier can still flag impossible type pairs.)
            var sourceProps = sourceType
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead)
                .Where(p => p.GetIndexParameters().Length == 0)
                .ToDictionary(p => p.Name, p => p, StringComparer.OrdinalIgnoreCase);

            var targetPropsLookup = targetProps.ToDictionary(p => p.Name, p => p, StringComparer.OrdinalIgnoreCase);

            foreach (var binding in plan.Bindings.Where(b => b.Kind != MappingBindingKind.Ignored))
            {
                if (!targetPropsLookup.TryGetValue(binding.TargetProperty, out var tprop))
                    continue;

                if (!sourceProps.TryGetValue(binding.SourceProperty, out var sprop))
                    continue;

                if (!IsCompatiblePerMapper(sprop.PropertyType, tprop.PropertyType, converters))
                {
                    errors.Add(
                        $"Type mismatch '{sourceType.Name}.{sprop.Name}' ({PrettyType(sprop.PropertyType)}) -> " +
                        $"'{targetType.Name}.{tprop.Name}' ({PrettyType(tprop.PropertyType)}). " +
                        $"Add/adjust converter or change types.");
                }
            }

            if (errors.Count > 0)
            {
                throw new InvalidOperationException(
                    $"Auto-mapper verification failed for {sourceType.Name} -> {targetType.Name}:\\n" +
                    string.Join("\\n", errors.Select(e => " - " + e)) + Environment.NewLine);
            }
        }

        private static bool IsCompatiblePerMapper(Type st, Type tt, IMapValueConverterRegistry converters)
        {
            if (tt.IsAssignableFrom(st))
                return true;

            var stNonNull = Nullable.GetUnderlyingType(st) ?? st;
            var ttNonNull = Nullable.GetUnderlyingType(tt) ?? tt;

            if (ttNonNull.IsAssignableFrom(stNonNull))
                return true;

            return converters.CanConvert(st, tt);
        }

        private static string PrettyType(Type t)
        {
            if (!t.IsGenericType) return t.Name;
            var def = t.GetGenericTypeDefinition();
            var args = string.Join(", ", t.GetGenericArguments().Select(PrettyType));
            var name = def.Name.Split('`')[0];
            return $"{name}<{args}>";
        }
    }
}
