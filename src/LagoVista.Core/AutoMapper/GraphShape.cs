using LagoVista.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace LagoVista.Core.AutoMapper
{
    public sealed class GraphShape<TSource, TTarget>
    {
        private readonly List<ChildMapStep> _steps = new List<ChildMapStep>();

        public IReadOnlyList<IChildMapStep> Steps => _steps;

        public GraphShape<TSource, TTarget> IncludeChild<TChildSource, TChildTarget>(Expression<Func<TTarget, TChildTarget>> targetProp, Expression<Func<TSource, TChildSource>> sourceProp, Action<GraphShape<TChildSource, TChildTarget>> configure = null)
        {
            var tprop = ExprProp.Get(targetProp);
            var sprop = ExprProp.Get(sourceProp);

            IReadOnlyList<IChildMapStep> children = Array.Empty<IChildMapStep>();
            if (configure != null)
            {
                var nested = new GraphShape<TChildSource, TChildTarget>();
                configure(nested);
                children = (IReadOnlyList<IChildMapStep>)nested.Steps;
            }

            _steps.Add(new ChildMapStep(ChildMapStepKind.Object, tprop, sprop, typeof(TChildSource), typeof(TChildTarget), children));
            return this;
        }

        public GraphShape<TSource, TTarget> IncludeList<TChildSource, TChildTarget>(Expression<Func<TTarget, List<TChildTarget>>> targetProp, Expression<Func<TSource, List<TChildSource>>> sourceProp, Action<GraphShape<TChildSource, TChildTarget>> configure = null)
        {
            var tprop = ExprProp.Get(targetProp);
            var sprop = ExprProp.Get(sourceProp);

            IReadOnlyList<IChildMapStep> children = Array.Empty<IChildMapStep>();
            if (configure != null)
            {
                var nested = new GraphShape<TChildSource, TChildTarget>();
                configure(nested);
                children = (IReadOnlyList<IChildMapStep>)nested.Steps;
            }

            _steps.Add(new ChildMapStep(ChildMapStepKind.Collection, tprop, sprop, typeof(TChildSource), typeof(TChildTarget), children));
            return this;
        }

        public GraphShape<TSource, TTarget> IncludeEntityHeaderValue<TChildSource, TChildTarget>(Expression<Func<TTarget, EntityHeader<TChildTarget>>> targetProp, Expression<Func<TSource, TChildSource>> sourceProp, Action<GraphShape<TChildSource, TChildTarget>> configure = null)
        {
            var tprop = ExprProp.Get(targetProp);
            var sprop = ExprProp.Get(sourceProp);

            IReadOnlyList<IChildMapStep> children = Array.Empty<IChildMapStep>();
            if (configure != null)
            {
                var nested = new GraphShape<TChildSource, TChildTarget>();
                configure(nested);
                children = (IReadOnlyList<IChildMapStep>)nested.Steps;
            }

            _steps.Add(new ChildMapStep(ChildMapStepKind.EntityHeaderValue, tprop, sprop, typeof(TChildSource), typeof(TChildTarget), children));
            return this;
        }
    }
}
