using System;
using System.Linq.Expressions;

namespace ISynergy.Framework.Core.Linq.Abstractions
{
    /// <summary>
    /// Interface IExpressionHelper
    /// </summary>
    internal interface IExpressionHelper
    {
        /// <summary>
        /// Converts the numeric type to biggest common type for binary operator.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        void ConvertNumericTypeToBiggestCommonTypeForBinaryOperator(ref Expression left, ref Expression right);

        /// <summary>
        /// Generates the add.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>Expression.</returns>
        Expression GenerateAdd(Expression left, Expression right);

        /// <summary>
        /// Generates the equal.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>Expression.</returns>
        Expression GenerateEqual(Expression left, Expression right);

        /// <summary>
        /// Generates the greater than.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>Expression.</returns>
        Expression GenerateGreaterThan(Expression left, Expression right);

        /// <summary>
        /// Generates the greater than equal.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>Expression.</returns>
        Expression GenerateGreaterThanEqual(Expression left, Expression right);

        /// <summary>
        /// Generates the less than.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>Expression.</returns>
        Expression GenerateLessThan(Expression left, Expression right);

        /// <summary>
        /// Generates the less than equal.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>Expression.</returns>
        Expression GenerateLessThanEqual(Expression left, Expression right);

        /// <summary>
        /// Generates the not equal.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>Expression.</returns>
        Expression GenerateNotEqual(Expression left, Expression right);

        /// <summary>
        /// Generates the string concat.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>Expression.</returns>
        Expression GenerateStringConcat(Expression left, Expression right);

        /// <summary>
        /// Generates the subtract.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>Expression.</returns>
        Expression GenerateSubtract(Expression left, Expression right);

        /// <summary>
        /// Optimizes for equality if possible.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        void OptimizeForEqualityIfPossible(ref Expression left, ref Expression right);

        /// <summary>
        /// Optimizes the string for equality if possible.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="type">The type.</param>
        /// <returns>Expression.</returns>
        Expression OptimizeStringForEqualityIfPossible(string text, Type type);

        /// <summary>
        /// Tries the generate and also not null expression.
        /// </summary>
        /// <param name="sourceExpression">The source expression.</param>
        /// <param name="addSelf">if set to <c>true</c> [add self].</param>
        /// <param name="generatedExpression">The generated expression.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool TryGenerateAndAlsoNotNullExpression(Expression sourceExpression, bool addSelf, out Expression generatedExpression);

        /// <summary>
        /// Wraps the constant expression.
        /// </summary>
        /// <param name="argument">The argument.</param>
        void WrapConstantExpression(ref Expression argument);
    }
}
