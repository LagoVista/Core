using LagoVista.Core.Attributes;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Interfaces.AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace LagoVista.Core.AutoMapper
{
    public sealed class ReflectionMappingPlanBuilder : IMappingPlanBuilder
    {
      
        private readonly IMapValueConverterRegistry _converters;

        public ReflectionMappingPlanBuilder(IMapValueConverterRegistry converters)
        {
            _converters = converters ?? throw new ArgumentNullException(nameof(converters));
        }

        public IMappingPlan Build(Type sourceType, Type targetType)
        {
            if (sourceType == null) throw new ArgumentNullException(nameof(sourceType));
            if (targetType == null) throw new ArgumentNullException(nameof(targetType));

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

            var actions = new List<Action<object, object>>();
            var bindings = new List<MappingBinding>();

            // Track which target properties we actually mapped (so MapTo doesn’t double-map).
            var mappedTargets = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            // Pass 0: record ignored target props (useful for verifier / plan introspection)
            for (var i = 0; i < targetProps.Length; ++i)
            {
                var tprop = targetProps[i];
                if (tprop.GetCustomAttribute<IgnoreOnMapToAttribute>() != null)
                {
                    bindings.Add(new MappingBinding(tprop.Name, sourceProperty: "", MappingBindingKind.Ignored));
                    mappedTargets.Add(tprop.Name);
                }
            }

            // Pass 1: target-driven (MapFrom + same-name)
            for (var i = 0; i < targetProps.Length; ++i)
            {
                var tprop = targetProps[i];

                var encryptedAttr = tprop.GetCustomAttribute<EncryptedFieldAttribute>();
                if (encryptedAttr != null)
                {
                    var sourceHasKey = sourceType.GetCustomAttributes(true)
                                                 .Any(a => a is EncryptionKeyAttribute);

                    if (sourceHasKey)
                        continue; // Crypto will handle it
                }

                if (mappedTargets.Contains(tprop.Name))
                    continue;

                if (tprop.GetCustomAttribute<IgnoreOnMapToAttribute>() != null)
                    continue;

                var mapFrom = tprop.GetCustomAttribute<MapFromAttribute>();
                var sourceName = mapFrom != null ? mapFrom.SourceProperty : tprop.Name;

                if (!sourceLookup.TryGetValue(sourceName, out var sprop))
                    continue;

                var assign = BuildAssignment(sprop, tprop);
                if (assign == null)
                    continue;

                // If this target property is encrypted and source has EncryptionKey,
                // consider it handled by crypto.
               

                actions.Add(assign);
                bindings.Add(new MappingBinding(tprop.Name, sprop.Name, MappingBindingKind.Direct));
                mappedTargets.Add(tprop.Name);
            }

            // Pass 2: MapTo fan-out (fill gaps)
            for (var i = 0; i < sourceProps.Length; ++i)
            {
                var sprop = sourceProps[i];

                var encryptedAttr = sprop.GetCustomAttribute<EncryptedFieldAttribute>();
                if (encryptedAttr != null)
                {
                    var targetHasKey = targetType.GetCustomAttributes(true)
                                                 .Any(a => a is EncryptionKeyAttribute);

                    if(String.IsNullOrEmpty(encryptedAttr.CiphertextProperty))

                    if (targetHasKey)
                        continue; // Crypto will handle it
                }

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

                    var assign = BuildAssignment(sprop, tprop);
                    if (assign == null)
                        continue;                    

                    actions.Add(assign);
                    bindings.Add(new MappingBinding(tprop.Name, sprop.Name, MappingBindingKind.MapToFanout));
                    mappedTargets.Add(tprop.Name);
                }
            }

            var canDecrypt = HasCryptoForDecrypt(sourceType, targetType);
            var canEncrypt = HasCryptoForEncrypt(sourceType, targetType);

            // NEW: add Crypto bindings so verifier knows these are handled later by EncryptedMapper.
            // - Decrypt direction: source is DTO (has [EncryptionKey]), target is Domain (has [EncryptedField])
            //   => mark domain plaintext properties as Crypto-bound.
            // - Encrypt direction: source is Domain (has [EncryptedField]), target is DTO (has [EncryptionKey])
            //   => mark DTO ciphertext properties as Crypto-bound (and optionally salt properties if you ever set them).
            if (canDecrypt)
            {
                // targetType is Domain here, so encrypted attributes live on target props.
                foreach (var tprop in targetProps)
                {
                    if (mappedTargets.Contains(tprop.Name))
                        continue;

                    if (tprop.GetCustomAttribute<IgnoreOnMapToAttribute>() != null)
                        continue;

                    var enc = tprop.GetCustomAttribute<EncryptedFieldAttribute>();
                    if (enc == null)
                        continue;

                    // This property is populated by crypto decryption, not plan.Apply.
                    bindings.Add(new MappingBinding(tprop.Name, enc.CiphertextProperty, MappingBindingKind.Crypto));
                    mappedTargets.Add(tprop.Name);
                }
            }

            if (canEncrypt)
            {
                // targetType is DTO here; ciphertext property names are referenced by [EncryptedField] on source (Domain).
                foreach (var sprop in sourceProps)
                {
                    var enc = sprop.GetCustomAttribute<EncryptedFieldAttribute>();
                    if (enc == null)
                        continue;

                    // Mark ciphertext target property as handled by crypto (dto.{CiphertextProperty} is set during Encrypt).
                    // We intentionally do NOT add an action; EncryptedMapper will set it.
                    if (!String.IsNullOrWhiteSpace(enc.CiphertextProperty))
                    {
                        // If the ciphertext property doesn't exist, that's a real error: fail fast here
                        // so you get a clean build-time exception instead of "missing mapping".
                        if (!targetLookup.ContainsKey(enc.CiphertextProperty))
                            throw new InvalidOperationException(
                                $"Crypto plan error for {sourceType.Name} -> {targetType.Name}: ciphertext property '{enc.CiphertextProperty}' not found on DTO '{targetType.Name}'.");

                        if (!mappedTargets.Contains(enc.CiphertextProperty))
                        {
                            bindings.Add(new MappingBinding(enc.CiphertextProperty, sprop.Name, MappingBindingKind.Crypto));
                            mappedTargets.Add(enc.CiphertextProperty);
                        }
                    }

                    // Optional: If encryption ever sets salt property, mark it too.
                    // Right now your EncryptedMapper reads salt from DTO (it doesn't set it),
                    // so we do NOT mark salt as Crypto-bound.
                    // If you later start writing salt, uncomment this.

                    /*
                    if (!String.IsNullOrWhiteSpace(enc.SaltProperty))
                    {
                        if (!targetLookup.ContainsKey(enc.SaltProperty))
                            throw new InvalidOperationException(
                                $"Crypto plan error for {sourceType.Name} -> {targetType.Name}: salt property '{enc.SaltProperty}' not found on DTO '{targetType.Name}'.");

                        if (!mappedTargets.Contains(enc.SaltProperty))
                        {
                            bindings.Add(new MappingBinding(enc.SaltProperty, sprop.Name, MappingBindingKind.Crypto));
                            mappedTargets.Add(enc.SaltProperty);
                        }
                    }
                    */
                }

            }
            
            return new CompiledMappingPlan(sourceType, targetType, actions.ToArray(), bindings, canDecrypt, canEncrypt);
        }

        private Action<object, object> BuildAssignment(PropertyInfo sprop, PropertyInfo tprop)
        {
            var st = sprop.PropertyType;
            var tt = tprop.PropertyType;

            // Direct assign
            if (tt.IsAssignableFrom(st))
                return (s, t) => tprop.SetValue(t, sprop.GetValue(s));

            var stNonNull = Nullable.GetUnderlyingType(st) ?? st;
            var ttNonNull = Nullable.GetUnderlyingType(tt) ?? tt;

            // Nullable-compatible assign
            if (ttNonNull.IsAssignableFrom(stNonNull))
                return (s, t) => tprop.SetValue(t, sprop.GetValue(s));

            // Converter fallback (matches current behavior: silent no-op if no converter)
            return (s, t) =>
            {
                var raw = sprop.GetValue(s);
                if (!_converters.TryConvert(raw, tt, out var converted))
                    return;

                tprop.SetValue(t, converted);
            };
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

        private sealed class CompiledMappingPlan : IMappingPlan
        {
            private readonly Action<object, object>[] _actions;
            private readonly IReadOnlyList<MappingBinding> _bindings;

            public Type SourceType { get; }
            public Type TargetType { get; }

            public bool CanDecrypt { get; }
            public bool CanEncrypt { get; }

            public IReadOnlyList<MappingBinding> Bindings => _bindings;

            public CompiledMappingPlan(
                Type sourceType,
                Type targetType,
                Action<object, object>[] actions,
                IReadOnlyList<MappingBinding> bindings,
                bool canDecrypt,
                bool canEncrypt)
            {
                SourceType = sourceType;
                TargetType = targetType;

                _actions = actions ?? Array.Empty<Action<object, object>>();
                _bindings = bindings ?? Array.Empty<MappingBinding>();

                CanDecrypt = canDecrypt;
                CanEncrypt = canEncrypt;
            }

            public void Apply(object source, object target)
            {
                for (var i = 0; i < _actions.Length; ++i)
                    _actions[i](source, target);
            }
        }
    }
}
