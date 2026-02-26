using LagoVista.Core.Attributes;
using LagoVista.Core.AutoMapper.LagoVista.Core.AutoMapper;
using LagoVista.Core.Interfaces.AutoMapper;
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LagoVista.Core.AutoMapper
{
    /// <summary>
    /// Reflection-based atomic mapping plan builder.
    /// Mirrors the attribute-driven strategy used by ReflectionMappingPlanBuilder,
    /// but emits AtomicMapStep records for the new MappingPlan pipeline.
    ///
    /// Strict mode: if a conversion is required and no converter exists, we accumulate
    /// errors and throw MappingPlanBuildException.
    /// </summary>
    public sealed class ReflectionAtomicPlanBuilder : IAtomicPlanBuilder
    {
        private readonly IMapValueConverterRegistry _converters;

        public ReflectionAtomicPlanBuilder(IMapValueConverterRegistry converters)
        {
            _converters = converters ?? throw new ArgumentNullException(nameof(converters));
        }

        public InvokeResult<IReadOnlyList<AtomicMapStep>> BuildAtomicSteps(Type sourceType, Type targetType)
        {
            if (sourceType == null) throw new ArgumentNullException(nameof(sourceType));
            if (targetType == null) throw new ArgumentNullException(nameof(targetType));

            var errors = new List<string>();
            var steps = new List<AtomicMapStep>();

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
            var targetLookup = targetProps.ToDictionary(p => p.Name, p => p, StringComparer.OrdinalIgnoreCase);

            var mappedTargets = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            // Pass 0: IgnoreOnMapTo
            foreach (var tprop in targetProps)
            {
                if (tprop.GetCustomAttribute<IgnoreOnMapToAttribute>() != null)
                {
                    steps.Add(new AtomicMapStep(tprop, sourceProperty: null, kind: AtomicMapStepKind.Ignored));
                    mappedTargets.Add(tprop.Name);
                }

                if (tprop.GetCustomAttribute<ManualMappingAttribute>() != null)
                {
                    steps.Add(new AtomicMapStep(tprop, sourceProperty: null, kind: AtomicMapStepKind.Manual));
                    mappedTargets.Add(tprop.Name);
                }
            }

            // Pass 1: target-driven (MapFrom + same-name)
            foreach (var tprop in targetProps)
            {
                if (mappedTargets.Contains(tprop.Name))
                    continue;

                if (tprop.GetCustomAttribute<IgnoreOnMapToAttribute>() != null)
                    continue;

                if (tprop.GetCustomAttribute<ManualMappingAttribute>() != null)
                    continue;

                // Crypto plaintext properties are handled by crypto when decrypting.
                // We'll mark them later in the crypto pass.

                var mapFrom = tprop.GetCustomAttribute<MapFromAttribute>();
                var sourceName = mapFrom != null ? mapFrom.SourceProperty : tprop.Name;

                if (!sourceLookup.TryGetValue(sourceName, out var sprop))
                    continue;

                if (TryBuildAtomicStep(sourceType, targetType, sprop, tprop, AtomicMapStepKind.DirectAssign, AtomicMapStepKind.NullableDirectAssign, AtomicMapStepKind.ConverterAssign, errors, out var step))
                {
                    steps.Add(step);
                    mappedTargets.Add(tprop.Name);
                }
            }

            // Pass 1.5a: EntityHeader <-> (Id, Text) mapping
            // Pass 1.5a: Id + Text -> EntityHeader
            foreach (var tprop in targetProps)
            {
                if (mappedTargets.Contains(tprop.Name))
                    continue;

                if (tprop.GetCustomAttribute<IgnoreOnMapToAttribute>() != null)
                    continue;

                if (tprop.GetCustomAttribute<ManualMappingAttribute>() != null)
                    continue;

                var map = tprop.GetCustomAttribute<MapToNameAndIdAttribute>(inherit: true);
                if (map == null)
                    continue;

                if (!typeof(EntityHeader).IsAssignableFrom(tprop.PropertyType))
                    continue;

                if (!sourceLookup.TryGetValue(map.IdPropertyName, out var idSource))
                    continue;

                if (!sourceLookup.TryGetValue(map.TextPropertyName, out var textSource))
                    continue;

                steps.Add(new AtomicMapStep(tprop, idSource, AtomicMapStepKind.EntityHeaderFromIdText, sourceProperty2: textSource));
                mappedTargets.Add(tprop.Name);
            }

            // Pass 1.5b: MapTo fan-out for EntityHeader -> (Id, Text)
            foreach (var sprop in sourceProps)
            {
                var map = sprop.GetCustomAttribute<MapToNameAndIdAttribute>(inherit: true);
                if (map == null)
                    continue;

                // Forward: EntityHeaderPrimary -> DTO (fan-out into Id + Text)
                if (targetLookup.TryGetValue(map.IdPropertyName, out var idTarget) &&
                    !mappedTargets.Contains(idTarget.Name) &&
                    idTarget.GetCustomAttribute<IgnoreOnMapToAttribute>() == null &&
                    idTarget.GetCustomAttribute<ManualMappingAttribute>() == null)
                {
                    steps.Add(new AtomicMapStep(
                        targetProperty: idTarget,
                        sourceProperty: sprop,
                        kind: AtomicMapStepKind.MapToFanoutAssign,
                        converterType: typeof(EntityHeaderIdConverter)));
                        mappedTargets.Add(idTarget.Name);
                }

                if (targetLookup.TryGetValue(map.TextPropertyName, out var textTarget) &&
                    !mappedTargets.Contains(textTarget.Name) &&
                    textTarget.GetCustomAttribute<IgnoreOnMapToAttribute>() == null &&
                    textTarget.GetCustomAttribute<ManualMappingAttribute>() == null)
                {
                    steps.Add(new AtomicMapStep(
                         targetProperty: textTarget,
                         sourceProperty: sprop,
                         kind: AtomicMapStepKind.MapToFanoutAssign,
                         converterType: typeof(EntityHeaderTextConverter)));
                         mappedTargets.Add(textTarget.Name);
                }
            }

            // Pass 2: MapTo fan-out
            foreach (var sprop in sourceProps)
            {
                var mapTos = sprop.GetCustomAttributes<MapToAttribute>(inherit: true);
                foreach (var mapTo in mapTos)
                {
                    if (String.IsNullOrWhiteSpace(mapTo.TargetProperty))
                        continue;

                    if (!targetLookup.TryGetValue(mapTo.TargetProperty, out var tprop))
                        continue;

                    if (mappedTargets.Contains(tprop.Name))
                        continue;

                    if (tprop.GetCustomAttribute<IgnoreOnMapToAttribute>() != null)
                        continue;

                    if (tprop.GetCustomAttribute<ManualMappingAttribute>() != null)
                        continue;

                    if (TryBuildAtomicStep(sourceType, targetType, sprop, tprop, AtomicMapStepKind.MapToFanoutAssign, AtomicMapStepKind.MapToFanoutAssign, AtomicMapStepKind.MapToFanoutAssign, errors, out var step))
                    {
                        steps.Add(step);

                        DiagnosticsPrint(this.Tag(), $"Added step {sprop.Name}");
                        mappedTargets.Add(tprop.Name);
                    }
                }

                DiagnosticsPrint(this.Tag(), $"Processed MapTo for source property {sprop.Name}");
            }

            // Crypto marking (mirrors old builder intent)
            var canDecrypt = HasCryptoForDecrypt(sourceType, targetType);
            var canEncrypt = HasCryptoForEncrypt(sourceType, targetType);

            if (canDecrypt)
            {
                DiagnosticsPrint(this.Tag(), "Can Decrypt");
                // targetType is Domain; [EncryptedField] lives on target properties (plaintext props)
                foreach (var tprop in targetProps)
                {
                    var enc = tprop.GetCustomAttribute<EncryptedFieldAttribute>();
                    if (enc == null)
                    {
                        continue;
                    }

                    if (tprop.GetCustomAttribute<IgnoreOnMapToAttribute>() != null)
                        continue;

                    if (tprop.GetCustomAttribute<ManualMappingAttribute>() != null)
                        continue;

                    if (mappedTargets.Contains(tprop.Name))
                    {
                        DiagnosticsPrint(this.Tag(), "contains existing, removing old...");
                        mappedTargets.Remove(tprop.Name);
                        steps.Remove(steps.First(s => s.TargetProperty.Name.Equals(tprop.Name, StringComparison.OrdinalIgnoreCase)));
                    }

                    var srcProperty = sourceProps.FirstOrDefault(sp => sp.Name.Equals(enc.CiphertextProperty, StringComparison.OrdinalIgnoreCase));

                    steps.Add(new AtomicMapStep(tprop, srcProperty, kind: AtomicMapStepKind.Crypto));
                    mappedTargets.Add(tprop.Name);
                }
            }
            else
            {
                DiagnosticsPrint(this.Tag(), "No decryption capability");
            }   

            if (canEncrypt)
            {
                DiagnosticsPrint(this.Tag(), "Can Encrypt");

                // sourceType is Domain; [EncryptedField] lives on source properties (plaintext props)
                foreach (var sprop in sourceProps)
                {

                    var enc = sprop.GetCustomAttribute<EncryptedFieldAttribute>();
                    if (enc == null)
                        continue;

                    if (String.IsNullOrWhiteSpace(enc.CiphertextProperty))
                        continue;

                    if (!targetLookup.TryGetValue(enc.CiphertextProperty, out var cipherTargetProp))
                    {
                        errors.Add($"Crypto plan error for {sourceType.Name} -> {targetType.Name}: ciphertext property '{enc.CiphertextProperty}' not found on target '{targetType.Name}'.");
                        continue;
                    }

                    DiagnosticsPrint(this.Tag(), "found target");


                    if (mappedTargets.Contains(cipherTargetProp.Name))
                    {
                        DiagnosticsPrint(this.Tag(), "contains existing, removing old...");
                        mappedTargets.Remove(cipherTargetProp.Name);
                        steps.Remove(steps.First(s => s.TargetProperty.Name.Equals(cipherTargetProp.Name, StringComparison.OrdinalIgnoreCase)));
                    }

                    DiagnosticsPrint(this.Tag(), "contains existing, adding...");
                    steps.Add(new AtomicMapStep(cipherTargetProp, sourceProperty: sprop, kind: AtomicMapStepKind.Crypto));
                    mappedTargets.Add(cipherTargetProp.Name);
                }
            }
            else
            {
                DiagnosticsPrint(this.Tag(), "No encryption capability");
            }

            if (errors.Count > 0)
                throw new MappingPlanBuildException(errors);

            return InvokeResult<IReadOnlyList<AtomicMapStep>>.Create(steps);
        }

        private void DiagnosticsPrint(string tag, string message)
        {
           // Console.WriteLine($"{tag} {message}");
        }

        private bool TryBuildAtomicStep(Type sourceType, Type targetType, PropertyInfo sprop,
            PropertyInfo tprop,  AtomicMapStepKind directKind, AtomicMapStepKind nullableKind,
            AtomicMapStepKind converterKind, List<string> errors, out AtomicMapStep step)
        {
            step = null;

            var st = sprop.PropertyType;
            var tt = tprop.PropertyType;

            var targetMappingType = TargetTypes.FromProperty;
            if (tt.GetCustomAttribute<DateOnlydAttribute>() != null)
                targetMappingType = TargetTypes.DateOnly;

            if (tt.IsAssignableFrom(st))
            {
                step = new AtomicMapStep(tprop, sprop, directKind, targetType: targetMappingType);
                return true;
            }

            var stNonNull = Nullable.GetUnderlyingType(st) ?? st;
            var ttNonNull = Nullable.GetUnderlyingType(tt) ?? tt;

            if (ttNonNull.IsAssignableFrom(stNonNull))
            {
                step = new AtomicMapStep(tprop, sprop, nullableKind);
                return true;
            }

            // Child edges are validated by the graph validator (not by atomic conversion).
            // Emit a marker step so the plan can be introspected/debugged.
            if (IsChildEdge(st, tt))
            {
                step = new AtomicMapStep(tprop, sprop, AtomicMapStepKind.ChildLeaf);
                return true;
            }

            // Converter required
            if (_converters.CanConvert(st, tt))
            {
                var converter = _converters.GetConverter(st, tt);
                step = new AtomicMapStep(tprop, sprop, converterKind, converterType: converter.GetType(), targetType: targetMappingType);
                return true;
            }

            errors.Add($"No converter registered for {sourceType.Name}.{sprop.Name} ({st.Name}) -> {targetType.Name}.{tprop.Name} ({tt.Name}).");
            return false;
        }

        private static bool IsChildEdge(Type sourcePropType, Type targetPropType)
        {
            // EntityHeader<T> -> T (or DTO) should be treated as a child leaf.
            if (TryGetEntityHeaderGenericArg(sourcePropType, out var ehArg) && IsComplexObject(targetPropType))
                return true;

            // Non-generic EntityHeader is NOT a leaf (child edge).
            if (typeof(EntityHeader).IsAssignableFrom(sourcePropType) &&
                typeof(EntityHeader).IsAssignableFrom(targetPropType))
                return true;

            // Collections/lists are child edges (element types validated by graph validator).
            if (TryGetEnumerableElementType(sourcePropType, out _) && TryGetEnumerableElementType(targetPropType, out _))
                return true;

            // Complex object types are child edges.
            return IsComplexObject(sourcePropType) && IsComplexObject(targetPropType);
        }

        private static bool TryGetEntityHeaderGenericArg(Type t, out Type arg)
        {
            arg = null;
            if (t == null) return false;
            if (!t.IsGenericType) return false;

            if (t.GetGenericTypeDefinition() == typeof(EntityHeader<>))
            {
                arg = t.GetGenericArguments()[0];
                return true;
            }

            return false;
        }

        private static bool IsComplexObject(Type t)
        {
            t = Nullable.GetUnderlyingType(t) ?? t;

            if (t.IsPrimitive) return false;
            if (t.IsEnum) return false;

            if (t == typeof(string)) return false;
            if (t == typeof(decimal)) return false;
            if (t == typeof(DateTime) || t == typeof(DateTimeOffset) || t == typeof(Guid) || t == typeof(TimeSpan)) return false;

            if (t == typeof(object)) return false;
            if (t.IsArray) return false;

            // treat collections as non-complex here (handled separately)
            if (typeof(System.Collections.IEnumerable).IsAssignableFrom(t) && t != typeof(string)) return false;

            return t.IsClass;
        }

        private static bool TryGetEnumerableElementType(Type t, out Type elementType)
        {
            elementType = null;
            if (t == null) return false;
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

            var ienum = t.GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));

            if (ienum != null)
            {
                elementType = ienum.GetGenericArguments()[0];
                return true;
            }

            return false;
        }

        private static bool HasCryptoForDecrypt(Type sourceType, Type targetType)
        {
            var hasKey = sourceType.GetCustomAttributes(inherit: true).Any(a => a is EncryptionKeyAttribute);
            var hasEncryptedFields = targetType
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Any(p => p.GetCustomAttributes(inherit: true).Any(a => a is EncryptedFieldAttribute));
            return hasKey && hasEncryptedFields;
        }

        private static bool HasCryptoForEncrypt(Type sourceType, Type targetType)
        {
            var hasKey = targetType.GetCustomAttributes(inherit: true).Any(a => a is EncryptionKeyAttribute);
            var hasEncryptedFields = sourceType
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Any(p => p.GetCustomAttributes(inherit: true).Any(a => a is EncryptedFieldAttribute));
            return hasKey && hasEncryptedFields;
        }
    }
}
