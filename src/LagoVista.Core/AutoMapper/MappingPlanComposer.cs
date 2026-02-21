using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.AutoMapper
{
    using global::LagoVista.Core.Models;
    using global::LagoVista.Core.Validation;
    using System;
    using System.Collections.Generic;

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

                var plan = new MappingPlan<TSource, TTarget>(atomic.Result, (IReadOnlyList<IChildMapStep>)_shape.Steps);
                return InvokeResult<MappingPlan<TSource, TTarget>>.Create(plan);
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
