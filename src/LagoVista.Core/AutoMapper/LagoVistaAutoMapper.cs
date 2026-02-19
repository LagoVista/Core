using LagoVista.Core.Interfaces.AutoMapper;
using LagoVista.Core.Models;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Core.AutoMapper
{

    public sealed class LagoVistaAutoMapper : ILagoVistaAutoMapper
    {
        private readonly IEncryptedMapper _encryptedMapper;
        private readonly IMappingPlanBuilder _planBuilder;

        // Prefer instance cache (avoids cross-test / cross-container weirdness)
        private readonly ConcurrentDictionary<(Type Source, Type Target), IMappingPlan> _plans =
            new ConcurrentDictionary<(Type Source, Type Target), IMappingPlan>();

        public LagoVistaAutoMapper(IEncryptedMapper encryptedMapper, IMappingPlanBuilder planBuilder)
        {
            _encryptedMapper = encryptedMapper ?? throw new ArgumentNullException(nameof(encryptedMapper));
            _planBuilder = planBuilder ?? throw new ArgumentNullException(nameof(planBuilder));
        }

        public async Task<TTarget> CreateAsync<TSource, TTarget>(
            TSource source,
            EntityHeader org,
            EntityHeader user,
            Action<TSource, TTarget> afterMap = null,
            CancellationToken ct = default)
            where TTarget : class, new()
            where TSource : class
        {
            var target = new TTarget();
            await MapAsync(source, target, org, user, afterMap, ct).ConfigureAwait(false);
            return target;
        }

        public async Task MapAsync<TSource, TTarget>(
            TSource source,
            TTarget target,
            EntityHeader org,
            EntityHeader user,
            Action<TSource, TTarget> afterMap = null,
            CancellationToken ct = default)
            where TTarget : class
            where TSource : class
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (org == null) throw new ArgumentNullException(nameof(org));
            if (user == null) throw new ArgumentNullException(nameof(user));

            var plan = GetOrBuildPlan(typeof(TSource), typeof(TTarget));

            plan.Apply(source, target);

            afterMap?.Invoke(source, target);

            await ApplyCryptoAsync(plan, source, target, org, user, ct).ConfigureAwait(false);
        }

        private IMappingPlan GetOrBuildPlan(Type sourceType, Type targetType)
        {
            var key = (sourceType, targetType);
            return _plans.GetOrAdd(key, k => _planBuilder.Build(k.Source, k.Target));
        }

        private async Task ApplyCryptoAsync<TSource, TTarget>(
            IMappingPlan plan,
            TSource source,
            TTarget target,
            EntityHeader org,
            EntityHeader user,
            CancellationToken ct)
            where TTarget : class
            where TSource : class
        {
            // Decrypt: DTO -> Domain (target has [EncryptedField], source has [EncryptionKey])
            if (plan.CanDecrypt)
                await _encryptedMapper.MapDecryptAsync(target, source, org, user, ct).ConfigureAwait(false);

            // Encrypt: Domain -> DTO (source has [EncryptedField], target has [EncryptionKey])
            if (plan.CanEncrypt)
                await _encryptedMapper.MapEncryptAsync(source, target, org, user, ct).ConfigureAwait(false);
        }
    }
}
