using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace ISynergy.Framework.Core.Linq.Helpers
{
    /// <summary>
    /// Class ConstantExpressionHelper.
    /// </summary>
    internal static class ConstantExpressionHelper
    {
        /// <summary>
        /// The expressions
        /// </summary>
        private static readonly ConcurrentDictionary<object, Expression> Expressions = new ConcurrentDictionary<object, Expression>();
        /// <summary>
        /// The literals
        /// </summary>
        private static readonly ConcurrentDictionary<Expression, string> Literals = new ConcurrentDictionary<Expression, string>();

        /// <summary>
        /// Tries the get text.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="text">The text.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool TryGetText(Expression expression, out string text)
        {
            return Literals.TryGetValue(expression, out text);
        }

        /// <summary>
        /// Creates the literal.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="text">The text.</param>
        /// <returns>Expression.</returns>
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
