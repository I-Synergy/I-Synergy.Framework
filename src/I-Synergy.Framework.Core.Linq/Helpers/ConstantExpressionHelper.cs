﻿using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace ISynergy.Framework.Core.Linq.Helpers
{
    internal static class ConstantExpressionHelper
    {
        private static readonly ConcurrentDictionary<object, Expression> Expressions = new ConcurrentDictionary<object, Expression>();
        private static readonly ConcurrentDictionary<Expression, string> Literals = new ConcurrentDictionary<Expression, string>();

        public static bool TryGetText(Expression expression, out string text)
        {
            return Literals.TryGetValue(expression, out text);
        }

        public static Expression CreateLiteral(object value, string text)
        {
            if (!Expressions.ContainsKey(value))
            {
                var constantExpression = Expression.Constant(value);

                Expressions.TryAdd(value, constantExpression);
                Literals.TryAdd(constantExpression, text);
            }

            return Expressions[value];
        }
    }
}
