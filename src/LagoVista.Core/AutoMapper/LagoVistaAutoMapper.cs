using LagoVista.Core.Attributes;
using LagoVista.Core.AutoMapper.LagoVista.Core.AutoMapper;
using LagoVista.Core.Interfaces.AutoMapper;
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
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

        // Cache atomic steps only: GraphShape/ChildSteps are per-call.
        private readonly ConcurrentDictionary<(Type Source, Type Target), IReadOnlyList<AtomicMapStep>> _atomicStepsCache =
            new ConcurrentDictionary<(Type Source, Type Target), IReadOnlyList<AtomicMapStep>>();

        public LagoVistaAutoMapper(IEncryptedMapper encryptedMapper, IAtomicPlanBuilder atomicBuilder, IMapValueConverterRegistry converters)
        {
            _encryptedMapper = encryptedMapper ?? throw new ArgumentNullException(nameof(encryptedMapper));
            _atomicBuilder = atomicBuilder ?? throw new ArgumentNullException(nameof(atomicBuilder));
            _converters = converters ?? throw new ArgumentNullException(nameof(converters));

            _atomicExecutor = new AtomicStepExecutor(_converters);
        }

        public async Task<TTarget> CreateAsync<TSource, TTarget>(TSource source, EntityHeader org, EntityHeader user, Action<TSource, TTarget> afterMap = null, CancellationToken ct = default)
            where TTarget : class, new()
            where TSource : class
        {
            var target = new TTarget();
            await MapAsync(source, target, org, user, afterMap, ct).ConfigureAwait(false);
            return target;
        }

        public async Task<TTarget> CreateAsync<TSource, TTarget>(TSource source, EntityHeader org, EntityHeader user, Action<MappingPlanComposer<TSource, TTarget>> configurePlan, Action<TSource, TTarget> afterMap = null, CancellationToken ct = default)
            where TTarget : class, new()
            where TSource : class
        {
            var target = new TTarget();
            await MapAsync(source, target, org, user, configurePlan, afterMap, ct).ConfigureAwait(false);
            return target;
        }

        public async Task MapAsync<TSource, TTarget>(TSource source, TTarget target, EntityHeader org, EntityHeader user, Action<TSource, TTarget> afterMap = null, CancellationToken ct = default)
            where TTarget : class
            where TSource : class
        {
            await MapAsync(source, target, org, user, configurePlan: null, afterMap, ct).ConfigureAwait(false);
        }

        public async Task MapAsync<TSource, TTarget>( TSource source, TTarget target, EntityHeader org, EntityHeader user, Action<MappingPlanComposer<TSource, TTarget>> configurePlan,Action<TSource, TTarget> afterMap = null, CancellationToken ct = default)
            where TTarget : class
            where TSource : class
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (org == null) throw new ArgumentNullException(nameof(org));
            if (user == null) throw new ArgumentNullException(nameof(user));

            var plan = BuildPlanForCall<TSource, TTarget>(configurePlan);

            _atomicExecutor.Execute(source, target, plan.AtomicSteps);

            // Execute child graph steps for this call (GraphShape-driven).
            await ExecuteChildStepsAsync(source, target, plan.ChildSteps, org, user, ct).ConfigureAwait(false);

            afterMap?.Invoke(source, target);

            await ApplyCryptoAsync(typeof(TSource), typeof(TTarget), source, target, org, user, ct).ConfigureAwait(false);
        }

        private MappingPlan<TSource, TTarget> BuildPlanForCall<TSource, TTarget>(Action<MappingPlanComposer<TSource, TTarget>> configurePlan)
            where TTarget : class
            where TSource : class
        {
            var key = (typeof(TSource), typeof(TTarget));

            if (!_atomicStepsCache.TryGetValue(key, out var atomicSteps))
            {
                var atomicResult = _atomicBuilder.BuildAtomicSteps(typeof(TSource), typeof(TTarget));
                if (!atomicResult.Successful)
                    throw new InvalidOperationException(string.Join("\r\n", atomicResult.Errors.Select(err => err.Message)));

                atomicSteps = atomicResult.Result;
                _atomicStepsCache.TryAdd(key, atomicSteps);
            }


            IReadOnlyList<IChildMapStep> childSteps = Array.Empty<IChildMapStep>();
            if (configurePlan != null)
            {
                var composer = MappingPlans.For<TSource, TTarget>(_atomicBuilder);
                configurePlan(composer);

                // Build() will also validate the full reachable mapping graph (reflection-based).
                var planResult = composer.Build();
                if (!planResult.Successful)
                    throw new InvalidOperationException(string.Join("\r\n", planResult.Errors.Select(err => err.Message)));

                childSteps = planResult.Result.ChildSteps;
            }

            return new MappingPlan<TSource, TTarget>(atomicSteps, childSteps);
        }

        private async Task ExecuteChildStepsAsync(
            object source,
            object target,
            IReadOnlyList<IChildMapStep> childSteps,
            EntityHeader org,
            EntityHeader user,
            CancellationToken ct)
        {
            if (childSteps == null || childSteps.Count == 0)
                return;

            foreach (var step in childSteps)
            {
                switch (step.Kind)
                {
                    case ChildMapStepKind.Object:
                        await ExecuteChildObjectAsync(source, target, step, org, user, ct).ConfigureAwait(false);
                        break;

                    case ChildMapStepKind.Collection:
                        await ExecuteChildCollectionAsync(source, target, step, org, user, ct).ConfigureAwait(false);
                        break;

                    case ChildMapStepKind.EntityHeaderValue:
                        await ExecuteEntityHeaderValueAsync(source, target, step, org, user, ct).ConfigureAwait(false);
                        break;

                    default:
                        throw new NotSupportedException($"Unsupported child step kind: {step.Kind}.");
                }
            }
        }

        private async Task ExecuteChildObjectAsync(object source, object target, IChildMapStep step, EntityHeader org, EntityHeader user, CancellationToken ct)
        {
            var srcChild = step.SourceProperty.GetValue(source);
            if (srcChild == null)
            {
                step.TargetProperty.SetValue(target, null);
                return;
            }

            var dstChild = Activator.CreateInstance(step.ChildTargetType);
            await MapDynamicAsync(step.ChildSourceType, step.ChildTargetType, srcChild, dstChild, org, user, step.Children, ct).ConfigureAwait(false);
            step.TargetProperty.SetValue(target, dstChild);
        }

        private async Task ExecuteChildCollectionAsync(object source, object target, IChildMapStep step, EntityHeader org, EntityHeader user, CancellationToken ct)
        {
            var srcListObj = step.SourceProperty.GetValue(source);
            if (srcListObj == null)
            {
                step.TargetProperty.SetValue(target, null);
                return;
            }

            // Replace semantics: create a new list each time.
            var dstList = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(step.ChildTargetType));

            foreach (var srcItem in (IEnumerable)srcListObj)
            {
                if (srcItem == null)
                {
                    dstList.Add(null);
                    continue;
                }

                var dstItem = Activator.CreateInstance(step.ChildTargetType);
                await MapDynamicAsync(step.ChildSourceType, step.ChildTargetType, srcItem, dstItem, org, user, step.Children, ct).ConfigureAwait(false);
                dstList.Add(dstItem);
            }

            step.TargetProperty.SetValue(target, dstList);
        }

        private async Task ExecuteEntityHeaderValueAsync(object source, object target, IChildMapStep step, EntityHeader org, EntityHeader user, CancellationToken ct)
        {
            var srcValue = step.SourceProperty.GetValue(source);
            if (srcValue == null)
            {
                step.TargetProperty.SetValue(target, null);
                return;
            }

            // Build header summary from source via instance method ToEntityHeader().
            var toHeaderMethod = step.ChildSourceType.GetMethod(
                "ToEntityHeader",
                BindingFlags.Public | BindingFlags.Instance,
                binder: null,
                types: Type.EmptyTypes,
                modifiers: null);

            if (toHeaderMethod == null)
                throw new InvalidOperationException($"Type {step.ChildSourceType.Name} must implement public instance method ToEntityHeader().");

            var headerSummary = (EntityHeader)toHeaderMethod.Invoke(srcValue, Array.Empty<object>());
            if (headerSummary == null)
                throw new InvalidOperationException($"{step.ChildSourceType.Name}.ToEntityHeader() returned null.");

            // Create EntityHeader<TTargetValue> and copy summary fields.
            var genericHeaderType = typeof(EntityHeader<>).MakeGenericType(step.ChildTargetType);
            var dstHeader = Activator.CreateInstance(genericHeaderType);
            CopyEntityHeaderSummary(headerSummary, dstHeader);

            // Value is optional: only map if children are configured.
            if (step.Children != null && step.Children.Count > 0)
            {
                var dstValue = Activator.CreateInstance(step.ChildTargetType);
                await MapDynamicAsync(step.ChildSourceType, step.ChildTargetType, srcValue, dstValue, org, user, step.Children, ct).ConfigureAwait(false);

                var valueProp = genericHeaderType.GetProperty("Value", BindingFlags.Public | BindingFlags.Instance);
                valueProp?.SetValue(dstHeader, dstValue);
            }

            step.TargetProperty.SetValue(target, dstHeader);
        }

        private static void CopyEntityHeaderSummary(EntityHeader summary, object genericHeader)
        {
            // Copy common properties by name to avoid tight coupling to constructors.
            var srcType = summary.GetType();
            var dstType = genericHeader.GetType();

            foreach (var propName in new[] { "Id", "Text", "Key" })
            {
                var sp = srcType.GetProperty(propName, BindingFlags.Public | BindingFlags.Instance);
                var dp = dstType.GetProperty(propName, BindingFlags.Public | BindingFlags.Instance);
                if (sp != null && dp != null && dp.CanWrite)
                {
                    dp.SetValue(genericHeader, sp.GetValue(summary));
                }
            }
        }

        private async Task MapDynamicAsync(
            Type sourceType,
            Type targetType,
            object source,
            object target,
            EntityHeader org,
            EntityHeader user,
            IReadOnlyList<IChildMapStep> childSteps,
            CancellationToken ct)
        {
            var key = (sourceType, targetType);
            if (!_atomicStepsCache.TryGetValue(key, out var atomicSteps))
            {
                var atomicResult = _atomicBuilder.BuildAtomicSteps(sourceType, targetType);
                if (!atomicResult.Successful)
                    throw new InvalidOperationException(string.Join("\r\n", atomicResult.Errors.Select(err => err.Message)));

                atomicSteps = atomicResult.Result;
                _atomicStepsCache.TryAdd(key, atomicSteps);
            }

            _atomicExecutor.Execute(source, target, atomicSteps);
            await ExecuteChildStepsAsync(source, target, childSteps, org, user, ct).ConfigureAwait(false);
            await ApplyCryptoAsync(sourceType, targetType, source, target, org, user, ct).ConfigureAwait(false);
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

        private async Task InvokeGenericEncryptedMapper( string methodName, object domain, object dto, Type domainType,Type dtoType, EntityHeader org, EntityHeader user, CancellationToken ct)
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
