using LagoVista.Core.Attributes;
using LagoVista.Core.AutoMapper.LagoVista.Core.AutoMapper;
using LagoVista.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static LagoVista.Core.AutoMapper.MappingVerifier;

namespace LagoVista.Core.AutoMapper
{
    /// <summary>
    /// Validates mapping viability across the entire reachable object graph between a source and target type.
    /// This is a plan-build time validation step (GraphShape is execution-only).
    ///
    /// Rules:
    /// - Recurse into complex objects, lists/collections (element type), and non-generic EntityHeader.
    /// - EntityHeader&lt;T&gt; is treated as a leaf.
    /// - Dictionaries are not supported (error).
    /// - IgnoreOnMapToAttribute on the TARGET property skips recursion/validation for that property.
    /// - Cycles (repeated (sourceType,targetType) pairs) are treated as errors.
    /// </summary>
    internal sealed class ReflectionMappingPlanGraphValidator
    {
        private readonly IAtomicPlanBuilder _atomicBuilder;

        public ReflectionMappingPlanGraphValidator(IAtomicPlanBuilder atomicBuilder)
        {
            _atomicBuilder = atomicBuilder ?? throw new ArgumentNullException(nameof(atomicBuilder));
        }

        public IReadOnlyList<string> Validate(Type sourceType, Type targetType)
        {
            if (sourceType == null) throw new ArgumentNullException(nameof(sourceType));
            if (targetType == null) throw new ArgumentNullException(nameof(targetType));

            var errors = new List<string>();
            var visiting = new HashSet<(Type Source, Type Target)>();

            ValidatePair(sourceType, targetType, errors, visiting, path: $"{sourceType.Name}->{targetType.Name}");
            return errors;
        }

        private void ValidatePair(Type sourceType, Type targetType, List<string> errors, HashSet<(Type Source, Type Target)> visiting, string path)
        {
            var key = (Source: sourceType, Target: targetType);
            if (!visiting.Add(key))
            {
                errors.Add($"Cycle detected in mapping graph at {path} ({sourceType.FullName} -> {targetType.FullName}).");
                return;
            }

            try
            {
                // Atomic validation for this pair (strict builder throws MappingPlanBuildException)
                try
                {
                    _atomicBuilder.BuildAtomicSteps(sourceType, targetType);
                }
                catch (MappingPlanBuildException ex)
                {
                    foreach (var err in ex.Errors)
                        errors.Add($"{path}: {err}");
                }

                var sourceProps = sourceType
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.CanRead)
                    .Where(p => p.GetIndexParameters().Length == 0)
                    .ToArray();

                var targetProps = targetType
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.CanWrite)
                    .Where(p => p.GetIndexParameters().Length == 0)
                    .ToArray();

                var sourceLookup = sourceProps.ToDictionary(p => p.Name, p => p, StringComparer.OrdinalIgnoreCase);

                foreach (var tprop in targetProps)
                {
                    if (tprop.GetCustomAttribute<IgnoreOnMapToAttribute>() != null)
                        continue;

                    var mapFrom = tprop.GetCustomAttribute<MapFromAttribute>();
                    var sourceName = mapFrom != null ? mapFrom.SourceProperty : tprop.Name;

                    if (!sourceLookup.TryGetValue(sourceName, out var sprop))
                        continue;

                    // Determine if this property represents a child edge we should recurse into.
                    if (!TryGetChildEdgeTypes(sprop.PropertyType, tprop.PropertyType, out var childKind, out var childSourceType, out var childTargetType, out var childError))
                    {
                        if (childError != null)
                            errors.Add($"{path}: {childError}");

                        continue;
                    }

                    // Leaf edges: no recursion.
                    if (childKind == null)
                        continue;

                    ValidatePair(childSourceType, childTargetType, errors, visiting, path: $"{path}.{tprop.Name}({childKind})");
                }
            }
            finally
            {
                visiting.Remove(key);
            }
        }

        private static bool TryGetChildEdgeTypes(Type sourcePropType, Type targetPropType, out ChildMapStepKind? kind, out Type childSourceType, out Type childTargetType, out string error)
        {
            kind = null;
            childSourceType = null;
            childTargetType = null;
            error = null;

            // Leaf primitives/nullables
            if (IsLeafType(sourcePropType) || IsLeafType(targetPropType))
                return true;

            // Dictionaries are not supported
            if (IsDictionaryType(sourcePropType) || IsDictionaryType(targetPropType))
            {
                error = $"Dictionary mapping not supported for property types '{sourcePropType.Name}' -> '{targetPropType.Name}'.";
                return false;
            }

            // EntityHeader<T> is leaf
            if (IsEntityHeaderOfT(sourcePropType) || IsEntityHeaderOfT(targetPropType))
            {
                // both sides are EntityHeader<T> => leaf
                if (IsEntityHeaderOfT(sourcePropType) && IsEntityHeaderOfT(targetPropType))
                    return true;

                // EntityHeader<T> -> ComplexObject => recurse into T -> ComplexObject
                if (IsEntityHeaderOfT(sourcePropType) && IsComplexObject(targetPropType))
                {
                    kind = ChildMapStepKind.Object;
                    childSourceType = sourcePropType.GetGenericArguments()[0];
                    childTargetType = targetPropType;
                    return true;
                }

                // ComplexObject -> EntityHeader<T> (if this ever happens) => recurse into ComplexObject -> T
                if (IsComplexObject(sourcePropType) && IsEntityHeaderOfT(targetPropType))
                {
                    kind = ChildMapStepKind.Object;
                    childSourceType = sourcePropType;
                    childTargetType = targetPropType.GetGenericArguments()[0];
                    return true;
                }

                return true;
            }

            // Non-generic EntityHeader: recurse into EntityHeader -> EntityHeader
            if (typeof(EntityHeader).IsAssignableFrom(sourcePropType) && typeof(EntityHeader).IsAssignableFrom(targetPropType))
            {
                kind = ChildMapStepKind.EntityHeaderValue;
                childSourceType = sourcePropType;
                childTargetType = targetPropType;
                return true;
            }

            // Lists/collections: recurse into element type
            if (TryGetEnumerableElementType(sourcePropType, out var srcElem) && TryGetEnumerableElementType(targetPropType, out var dstElem))
            {
                kind = ChildMapStepKind.Collection;
                childSourceType = srcElem;
                childTargetType = dstElem;
                return true;
            }

            // Complex object: recurse
            if (IsComplexObject(sourcePropType) && IsComplexObject(targetPropType))
            {
                kind = ChildMapStepKind.Object;
                childSourceType = sourcePropType;
                childTargetType = targetPropType;
                return true;
            }

            // Otherwise: treat as leaf (atomic builder will have already validated conversion requirements)
            return true;
        }

        private static bool IsLeafType(Type t)
        {
            t = Nullable.GetUnderlyingType(t) ?? t;

            if (t.IsPrimitive)
                return true;

            if (t.IsEnum)
                return true;

            return t == typeof(string)
                || t == typeof(decimal)
                || t == typeof(DateTime)
                || t == typeof(DateTimeOffset)
                || t == typeof(Guid)
                || t == typeof(TimeSpan);
        }

        private static bool IsComplexObject(Type t)
        {
            t = Nullable.GetUnderlyingType(t) ?? t;

            if (IsLeafType(t)) return false;
            if (t == typeof(object)) return false;
            if (t.IsArray) return false;
            if (typeof(System.Collections.IEnumerable).IsAssignableFrom(t) && t != typeof(string)) return false;

            return t.IsClass;
        }

        private static bool TryGetEnumerableElementType(Type t, out Type elementType)
        {
            elementType = null;
            if (t == typeof(string)) return false;

            if (t.IsArray)
            {
                elementType = t.GetElementType();
                return elementType != null;
            }

            if (t.IsGenericType)
            {
                var genDef = t.GetGenericTypeDefinition();
                if (genDef == typeof(List<>))
                {
                    elementType = t.GetGenericArguments()[0];
                    return true;
                }
            }

            // Allow IEnumerable<T> / ICollection<T> / IList<T>
            var ienum = t.GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));

            if (ienum != null)
            {
                elementType = ienum.GetGenericArguments()[0];
                return true;
            }

            return false;
        }

        private static bool IsDictionaryType(Type t)
        {
            if (t == null) return false;

            if (t.IsGenericType)
            {
                var genDef = t.GetGenericTypeDefinition();
                if (genDef == typeof(Dictionary<,>))
                    return true;
            }

            return t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDictionary<,>));
        }

        private static bool IsEntityHeaderOfT(Type t)
        {
            if (t == null) return false;
            if (!t.IsGenericType) return false;
            return t.GetGenericTypeDefinition() == typeof(EntityHeader<>);
        }


        public IEnumerable<MappingPair> EnumeratePairs(Type sourceType, Type targetType)
        {
            var visiting = new HashSet<(Type Source, Type Target)>();
            foreach (var pair in EnumeratePairsImpl(sourceType, targetType, visiting, path: $"{sourceType.Name}->{targetType.Name}", depth: 0))
                yield return pair;
        }

        private IEnumerable<MappingPair> EnumeratePairsImpl(
            Type sourceType,
            Type targetType,
            HashSet<(Type Source, Type Target)> visiting,
            string path,
            int depth)
        {
            var key = (Source: sourceType, Target: targetType);
            if (!visiting.Add(key))
                throw new MappingPlanBuildException(new[] { $"Cycle detected in mapping graph at {path} ({sourceType.FullName} -> {targetType.FullName})." });

            yield return new MappingPair(sourceType, targetType, path, depth);

            try
            {
                // Reuse the SAME child-discovery logic you already have in ValidatePair:
                // - build sourceLookup/targetProps
                // - for each mapped property, call TryGetChildEdgeTypes(...)
                // - if it returns a child kind + child types, recurse

                var sourceProps = sourceType
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.CanRead)
                    .Where(p => p.GetIndexParameters().Length == 0)
                    .ToArray();

                var targetProps = targetType
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.CanWrite)
                    .Where(p => p.GetIndexParameters().Length == 0)
                    .ToArray();

                var sourceLookup = sourceProps.ToDictionary(p => p.Name, p => p, StringComparer.OrdinalIgnoreCase);

                foreach (var tprop in targetProps)
                {
                    if (tprop.GetCustomAttribute<IgnoreOnMapToAttribute>() != null)
                        continue;

                    var mapFrom = tprop.GetCustomAttribute<MapFromAttribute>();
                    var sourceName = mapFrom != null ? mapFrom.SourceProperty : tprop.Name;

                    if (!sourceLookup.TryGetValue(sourceName, out var sprop))
                        continue;

                    if (!TryGetChildEdgeTypes(sprop.PropertyType, tprop.PropertyType,
                            out var childKind, out var childSourceType, out var childTargetType, out var childError))
                    {
                        if (childError != null)
                            throw new MappingPlanBuildException(new[] { $"{path}: {childError}" });

                        continue;
                    }

                    if (childKind == null)
                        continue;

                    foreach (var childPair in EnumeratePairsImpl(
                                 childSourceType,
                                 childTargetType,
                                 visiting,
                                 path: $"{path}.{tprop.Name}({childKind})",
                                 depth: depth + 1))
                    {
                        yield return childPair;
                    }
                }
            }
            finally
            {
                visiting.Remove(key);
            }
        }
    }
}
