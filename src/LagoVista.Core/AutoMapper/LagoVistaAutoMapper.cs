using LagoVista.Core.Attributes;
using LagoVista.Core.AutoMapper.LagoVista.Core.AutoMapper;
using LagoVista.Core.Interfaces.AutoMapper;
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Core.AutoMapper
{
    public sealed class LagoVistaAutoMapper : ILagoVistaAutoMapper
    {
        private readonly IEncryptedMapper _encryptedMapper;
        private readonly IAtomicPlanBuilder _atomicBuilder;
        private readonly IMapValueConverterRegistry _converters;

        private readonly AtomicStepExecutor _atomicExecutor;

        private readonly ConcurrentDictionary<(Type Source, Type Target), object> _plans =
            new ConcurrentDictionary<(Type Source, Type Target), object>();

        public LagoVistaAutoMapper(IEncryptedMapper encryptedMapper, IAtomicPlanBuilder atomicBuilder, IMapValueConverterRegistry converters)
        {
            _encryptedMapper = encryptedMapper ?? throw new ArgumentNullException(nameof(encryptedMapper));
            _atomicBuilder = atomicBuilder ?? throw new ArgumentNullException(nameof(atomicBuilder));
            _converters = converters ?? throw new ArgumentNullException(nameof(converters));

            _atomicExecutor = new AtomicStepExecutor(_converters);
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

            var plan = GetOrBuildPlan<TSource, TTarget>();

            _atomicExecutor.Execute(source, target, plan.AtomicSteps);

            afterMap?.Invoke(source, target);

            await ApplyCryptoAsync(typeof(TSource), typeof(TTarget), source, target, org, user, ct).ConfigureAwait(false);
        }

        private MappingPlan<TSource, TTarget> GetOrBuildPlan<TSource, TTarget>()
            where TTarget : class
            where TSource : class
        {
            var key = (typeof(TSource), typeof(TTarget));
            if (!_plans.TryGetValue(key, out var planObj))
            {
                var result = MappingPlans.For<TSource, TTarget>(_atomicBuilder).Build();
                if (!result.Successful)
                    throw new InvalidOperationException(string.Join("\r\n", result.Errors.Select(err => err.Message)));

                planObj = result.Result;
                _plans.TryAdd(key, planObj);
            }

            return (MappingPlan<TSource, TTarget>)planObj;
        }

        private async Task ApplyCryptoAsync(
            Type sourceType,
            Type targetType,
            object source,
            object target,
            EntityHeader org,
            EntityHeader user,
            CancellationToken ct)
        {
            // Keep same behavior as the atomic builder: crypto applicability is implied by attribute placement.
            if (HasCryptoForDecrypt(sourceType, targetType))
                await InvokeGenericEncryptedMapper(nameof(IEncryptedMapper.MapDecryptAsync), domain: target, dto: source, targetType, sourceType, org, user, ct).ConfigureAwait(false);

            if (HasCryptoForEncrypt(sourceType, targetType))
                await InvokeGenericEncryptedMapper(nameof(IEncryptedMapper.MapEncryptAsync), domain: source, dto: target, sourceType, targetType, org, user, ct).ConfigureAwait(false);
        }

        private async Task InvokeGenericEncryptedMapper(
            string methodName,
            object domain,
            object dto,
            Type domainType,
            Type dtoType,
            EntityHeader org,
            EntityHeader user,
            CancellationToken ct)
        {
            var method = typeof(IEncryptedMapper).GetMethods()
                .FirstOrDefault(m => m.Name == methodName && m.IsGenericMethodDefinition);

            if (method == null)
                throw new InvalidOperationException($"Could not find generic method {methodName} on IEncryptedMapper.");

            var closed = method.MakeGenericMethod(domainType, dtoType);
            var task = (Task)closed.Invoke(_encryptedMapper, new object[] { domain, dto, org, user, ct });
            await task.ConfigureAwait(false);
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
