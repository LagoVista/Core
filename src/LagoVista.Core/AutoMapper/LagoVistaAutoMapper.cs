using LagoVista.Core.Attributes;
using LagoVista.Core.Interfaces.AutoMapper;
using LagoVista.Core.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Core.AutoMapper
{

    public sealed class LagoVistaAutoMapper : ILagoVistaAutoMapper
    {
        private static readonly ConcurrentDictionary<(Type Source, Type Target), object> _plans = new ConcurrentDictionary<(Type Source, Type Target), object>();

        private readonly IEncryptedMapper _encryptedMapper;
        private readonly IMapValueConverterRegistry _converters;

        public LagoVistaAutoMapper(IEncryptedMapper encryptedMapper, IMapValueConverterRegistry converters)
        {
            _encryptedMapper = encryptedMapper ?? throw new ArgumentNullException(nameof(encryptedMapper));
            _converters = converters ?? throw new ArgumentNullException(nameof(converters));
        }

        public async Task<TTarget> CreateAsync<TSource, TTarget>(TSource source, EntityHeader org, EntityHeader user, Action<TSource, TTarget> afterMap = null, CancellationToken ct = default)
            where TTarget : class, new()
            where TSource : class
        {
            var target = new TTarget();
            await MapAsync(source, target, org, user, afterMap, ct).ConfigureAwait(false);
            return target;
        }

        public async Task MapAsync<TSource, TTarget>(TSource source, TTarget target, EntityHeader org, EntityHeader user, Action<TSource, TTarget> afterMap = null, CancellationToken ct = default)
            where TTarget : class
            where TSource : class
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (org == null) throw new ArgumentNullException(nameof(org));
            if (user == null) throw new ArgumentNullException(nameof(user));

            var plan = GetOrBuildPlan<TSource, TTarget>();
            plan.Apply(source, target);

            if (afterMap != null)
                afterMap(source, target);

            await ApplyCryptoAsync(source, target, org, user, ct).ConfigureAwait(false);
        }

        private async Task ApplyCryptoAsync<TSource, TTarget>(TSource source, TTarget target, EntityHeader org, EntityHeader user, CancellationToken ct)
            where TTarget : class
            where TSource : class
        {
            // V1 rule: try crypto in both directions, but only if attributes exist in that pair.
            // If a pair has no crypto attributes, EncryptedMapper will throw unless we guard it.
            // So we do a light reflection check once per type-pair via plan flags.

            var plan = GetOrBuildPlan<TSource, TTarget>();

            if (plan.CanDecrypt)
                await _encryptedMapper.MapDecryptAsync(target, source, org, user, ct).ConfigureAwait(false);

            if (plan.CanEncrypt)
                await _encryptedMapper.MapEncryptAsync(source, target, org, user, ct).ConfigureAwait(false);
        }

        private Plan<TSource, TTarget> GetOrBuildPlan<TSource, TTarget>()
        {
            var key = (typeof(TSource), typeof(TTarget));
            if (!_plans.TryGetValue(key, out var planObj))
            {
                planObj = BuildPlan<TSource, TTarget>();
                _plans.TryAdd(key, planObj);
            }

            return (Plan<TSource, TTarget>)planObj;
        }

        private Plan<TSource, TTarget> BuildPlan<TSource, TTarget>()
        {
            var sourceType = typeof(TSource);
            var targetType = typeof(TTarget);

            var sourceProps = sourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.CanRead).ToArray();
            var targetProps = targetType.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.CanWrite).ToArray();

            var sourceLookup = sourceProps.ToDictionary(p => p.Name, p => p, StringComparer.OrdinalIgnoreCase);
            var targetLookup = targetProps.ToDictionary(p => p.Name, p => p, StringComparer.OrdinalIgnoreCase);

            var assignments = new List<Action<TSource, TTarget>>();

            foreach (var tprop in targetProps)
            {
                if (tprop.GetCustomAttribute<MapIgnoreAttribute>() != null)
                    continue;

                if (tprop.GetIndexParameters().Length > 0)
                    continue;

                var mapFrom = tprop.GetCustomAttribute<MapFromAttribute>();
                var sourceName = mapFrom != null ? mapFrom.SourceProperty : tprop.Name;

                if (!sourceLookup.TryGetValue(sourceName, out var sprop))
                    continue;

                if (sprop.GetIndexParameters().Length > 0)
                    continue;

                var assign = BuildAssignment<TSource, TTarget>(sprop, tprop);
                if (assign != null)
                    assignments.Add(assign);
            }

            var canDecrypt = HasCryptoForDecrypt(sourceType, targetType);
            var canEncrypt = HasCryptoForEncrypt(sourceType, targetType);

            return new Plan<TSource, TTarget>(assignments.ToArray(), canDecrypt, canEncrypt);
        }

        private static bool HasCryptoForDecrypt(Type sourceType, Type targetType)
        {
            // Decrypt means: source is DTO (has [EncryptionKey]) and target has [EncryptedField] props.
            var hasKey = sourceType.GetCustomAttributes(inherit: true).Any(a => a is EncryptionKeyAttribute);
            var hasEncryptedFields = targetType.GetProperties(BindingFlags.Public | BindingFlags.Instance).Any(p => p.GetCustomAttributes(inherit: true).Any(a => a is EncryptedFieldAttribute));
            return hasKey && hasEncryptedFields;
        }

        private static bool HasCryptoForEncrypt(Type sourceType, Type targetType)
        {
            // Encrypt means: target is DTO (has [EncryptionKey]) and source has [EncryptedField] props.
            var hasKey = targetType.GetCustomAttributes(inherit: true).Any(a => a is EncryptionKeyAttribute);
            var hasEncryptedFields = sourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance).Any(p => p.GetCustomAttributes(inherit: true).Any(a => a is EncryptedFieldAttribute));
            return hasKey && hasEncryptedFields;
        }

        private Action<TSource, TTarget> BuildAssignment<TSource, TTarget>(PropertyInfo sprop, PropertyInfo tprop)
        {
            var st = sprop.PropertyType;
            var tt = tprop.PropertyType;

            if (tt.IsAssignableFrom(st))
                return (s, t) => tprop.SetValue(t, sprop.GetValue(s));

            var stNonNull = Nullable.GetUnderlyingType(st) ?? st;
            var ttNonNull = Nullable.GetUnderlyingType(tt) ?? tt;

            if (ttNonNull.IsAssignableFrom(stNonNull))
                return (s, t) => tprop.SetValue(t, sprop.GetValue(s));

            // NEW: try converter registry
            return (s, t) =>
            {
                var raw = sprop.GetValue(s);
                if (!_converters.TryConvert(raw, tt, out var converted))
                    return;

                tprop.SetValue(t, converted);
            };
        }

        private sealed class Plan<TSource, TTarget>
        {
            private readonly Action<TSource, TTarget>[] _assignments;

            public bool CanDecrypt { get; }
            public bool CanEncrypt { get; }

            public Plan(Action<TSource, TTarget>[] assignments, bool canDecrypt, bool canEncrypt)
            {
                _assignments = assignments ?? Array.Empty<Action<TSource, TTarget>>();
                CanDecrypt = canDecrypt;
                CanEncrypt = canEncrypt;
            }

            public void Apply(TSource source, TTarget target)
            {
                for (var i = 0; i < _assignments.Length; ++i)
                    _assignments[i](source, target);
            }
        }
    }
}
