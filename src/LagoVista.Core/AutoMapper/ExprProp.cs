using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace LagoVista.Core.AutoMapper
{
    internal static class ExprProp
    {
        public static PropertyInfo Get<TObj, TProp>(Expression<Func<TObj, TProp>> expr)
        {
            if (expr == null) throw new ArgumentNullException(nameof(expr));

            Expression body = expr.Body;

            if (body is UnaryExpression unary && unary.NodeType == ExpressionType.Convert)
                body = unary.Operand;

            if (!(body is MemberExpression member))
                throw new InvalidOperationException("Expression must be a simple property access.");

            if (!(member.Member is PropertyInfo prop))
                throw new InvalidOperationException("Expression must target a property.");

            if (!(member.Expression is ParameterExpression))
                throw new InvalidOperationException("Expression must be a direct property access.");

            return prop;
        }
    }
}
