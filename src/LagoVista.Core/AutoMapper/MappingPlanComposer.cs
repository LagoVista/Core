using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.AutoMapper
{
    using global::LagoVista.Core.Models;
    using global::LagoVista.Core.Validation;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    namespace LagoVista.Core.AutoMapper
    {
        public interface IAtomicPlanBuilder
        {
            InvokeResult<IReadOnlyList<AtomicMapStep>> BuildAtomicSteps(Type sourceType, Type targetType);
        }

        public sealed class MappingPlanComposer<TSource, TTarget>
        {
            private readonly IAtomicPlanBuilder _atomicBuilder;
            private readonly GraphShape<TSource, TTarget> _shape = new GraphShape<TSource, TTarget>();

            public MappingPlanComposer(IAtomicPlanBuilder atomicBuilder)
            {
                _atomicBuilder = atomicBuilder ?? throw new ArgumentNullException(nameof(atomicBuilder));
            }

            public MappingPlanComposer<TSource, TTarget> IncludeChild<TChildSource, TChildTarget>(System.Linq.Expressions.Expression<Func<TTarget, TChildTarget>> targetProp, System.Linq.Expressions.Expression<Func<TSource, TChildSource>> sourceProp, Action<GraphShape<TChildSource, TChildTarget>> configure = null)
            {
                _shape.IncludeChild(targetProp, sourceProp, configure);
                return this;
            }

            public MappingPlanComposer<TSource, TTarget> IncludeList<TChildSource, TChildTarget>(System.Linq.Expressions.Expression<Func<TTarget, List<TChildTarget>>> targetProp, System.Linq.Expressions.Expression<Func<TSource, List<TChildSource>>> sourceProp, Action<GraphShape<TChildSource, TChildTarget>> configure = null)
            {
                _shape.IncludeList(targetProp, sourceProp, configure);
                return this;
            }

            public MappingPlanComposer<TSource, TTarget> IncludeEntityHeaderValue<TChildSource, TChildTarget>(System.Linq.Expressions.Expression<Func<TTarget, EntityHeader<TChildTarget>>> targetProp, System.Linq.Expressions.Expression<Func<TSource, TChildSource>> sourceProp, Action<GraphShape<TChildSource, TChildTarget>> configure = null)
            {
                _shape.IncludeEntityHeaderValue(targetProp, sourceProp, configure);
                return this;
            }

            public InvokeResult<MappingPlan<TSource, TTarget>> Build()
            {
                var atomic = _atomicBuilder.BuildAtomicSteps(typeof(TSource), typeof(TTarget));
                if (!atomic.Successful)
                    return InvokeResult<MappingPlan<TSource, TTarget>>.FromErrors(atomic.Errors.ToArray());

                    // Validate the full reachable mapping graph via reflection (GraphShape is execution-only).
                var graphValidator = new ReflectionMappingPlanGraphValidator(_atomicBuilder);
                var graphErrors = graphValidator.Validate(typeof(TSource), typeof(TTarget));
                if (graphErrors.Count > 0)
                    return InvokeResult<MappingPlan<TSource, TTarget>>.FromErrors(graphErrors.Select(err => new ErrorMessage() { Message = err}).ToArray());

                var plan = new MappingPlan<TSource, TTarget>(atomic.Result, (IReadOnlyList<IChildMapStep>)_shape.Steps);
                return InvokeResult<MappingPlan<TSource, TTarget>>.Create(plan);
            }

            public MappingPlanComposer<TSource, TTarget> Include<TProp>(
           Expression<Func<TTarget, TProp>> targetProp,
           Action<object> configure = null)
            {
                if (targetProp == null) throw new ArgumentNullException(nameof(targetProp));

                var targetPi = GetPropertyInfo(targetProp);
                var sourcePi = typeof(TSource).GetProperty(targetPi.Name, BindingFlags.Instance | BindingFlags.Public);
                if (sourcePi == null)
                    throw new InvalidOperationException(
                        $"Include({targetPi.Name}) failed: no matching source property '{targetPi.Name}' on {typeof(TSource).Name}.");

                // Decide kind based on TARGET property type
                var tt = targetPi.PropertyType;

                // List<T>
                if (IsListOfT(tt, out var targetItemType))
                {
                    if (!IsListOfT(sourcePi.PropertyType, out var sourceItemType))
                        throw new InvalidOperationException(
                            $"Include({targetPi.Name}) failed: target is List<{targetItemType.Name}> but source is {sourcePi.PropertyType.Name}.");

                    // Build lambdas: (TTarget t) => t.Prop, (TSource s) => s.Prop
                    var tLambda = BuildPropertyLambda<TTarget>(targetPi);
                    var sLambda = BuildPropertyLambda<TSource>(sourcePi);

                    // Call IncludeList<TChildSource, TChildTarget>(Expression<Func<TTarget, List<TChildTarget>>>, Expression<Func<TSource, List<TChildSource>>>, Action<GraphShape<...>>)
                    var mi = typeof(MappingPlanComposer<TSource, TTarget>)
                        .GetMethod(nameof(IncludeList), BindingFlags.Instance | BindingFlags.Public)
                        .MakeGenericMethod(sourceItemType, targetItemType);

                    return (MappingPlanComposer<TSource, TTarget>)mi.Invoke(this, new object[] { tLambda, sLambda, WrapGraphConfigure(sourceItemType, targetItemType, configure) });
                }

                // EntityHeader<T>
                if (IsEntityHeaderOfT(tt, out var targetValueType))
                {
                    // Source side can be either TChildSource (e.g., CustomerDTO) or EntityHeader<TChildSource> depending on your conventions.
                    // Your existing signature is Expression<Func<TSource, TChildSource>> (not EntityHeader).
                    var sourceChildType = sourcePi.PropertyType;

                    var tLambda = BuildPropertyLambda<TTarget>(targetPi); // returns Expression<Func<TTarget, EntityHeader<TTargetValue>>>
                    var sLambda = BuildPropertyLambda<TSource>(sourcePi); // returns Expression<Func<TSource, TChildSource>>

                    var mi = typeof(MappingPlanComposer<TSource, TTarget>)
                        .GetMethod(nameof(IncludeEntityHeaderValue), BindingFlags.Instance | BindingFlags.Public)
                        .MakeGenericMethod(sourceChildType, targetValueType);

                    return (MappingPlanComposer<TSource, TTarget>)mi.Invoke(this, new object[] { tLambda, sLambda, WrapGraphConfigure(sourceChildType, targetValueType, configure) });
                }

                // Default: object child
                {
                    var sourceChildType = sourcePi.PropertyType;
                    var targetChildType = targetPi.PropertyType;

                    var tLambda = BuildPropertyLambda<TTarget>(targetPi);
                    var sLambda = BuildPropertyLambda<TSource>(sourcePi);

                    var mi = typeof(MappingPlanComposer<TSource, TTarget>)
                        .GetMethod(nameof(IncludeChild), BindingFlags.Instance | BindingFlags.Public)
                        .MakeGenericMethod(sourceChildType, targetChildType);

                    return (MappingPlanComposer<TSource, TTarget>)mi.Invoke(this, new object[] { tLambda, sLambda, WrapGraphConfigure(sourceChildType, targetChildType, configure) });
                }
            }

            private static PropertyInfo GetPropertyInfo<T, TProp>(Expression<Func<T, TProp>> expr)
            {
                if (expr.Body is MemberExpression me && me.Member is PropertyInfo pi) return pi;

                if (expr.Body is UnaryExpression ue && ue.Operand is MemberExpression me2 && me2.Member is PropertyInfo pi2) return pi2;

                throw new InvalidOperationException("Expression must be a simple property access like x => x.Customer.");
            }

            private static LambdaExpression BuildPropertyLambda<TDeclaring>(PropertyInfo pi)
            {
                var param = Expression.Parameter(typeof(TDeclaring), "x");
                var body = Expression.Property(param, pi);
                return Expression.Lambda(body, param);
            }

            private static bool IsListOfT(Type t, out Type itemType)
            {
                if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(List<>))
                {
                    itemType = t.GetGenericArguments()[0];
                    return true;
                }

                itemType = null;
                return false;
            }

            private static bool IsEntityHeaderOfT(Type t, out Type valueType)
            {
                if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(EntityHeader<>))
                {
                    valueType = t.GetGenericArguments()[0];
                    return true;
                }

                valueType = null;
                return false;
            }

            private static object WrapGraphConfigure(Type childSourceType, Type childTargetType, Action<object> configure)
            {
                if (configure == null) return null;

                // We need an Action<GraphShape<TChildSource, TChildTarget>> at runtime.
                // We wrap it by passing the GraphShape instance as object to user code.
                var graphShapeType = typeof(GraphShape<,>).MakeGenericType(childSourceType, childTargetType);

                var actionType = typeof(Action<>).MakeGenericType(graphShapeType);

                return Delegate.CreateDelegate(
                    actionType,
                    configure.Target,
                    configure.Method);
            }
        }

        public static class MappingPlans
        {
            public static MappingPlanComposer<TSource, TTarget> For<TSource, TTarget>(IAtomicPlanBuilder atomicBuilder)
            {
                return new MappingPlanComposer<TSource, TTarget>(atomicBuilder);
            }
        }
    }
}
