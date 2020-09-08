using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using ISynergy.Framework.Core.Linq.Abstractions;
using ISynergy.Framework.Core.Linq.Parsers;
using ISynergy.Framework.Core.Validation;

namespace ISynergy.Framework.Core.Linq.Helpers
{
    /// <summary>
    /// Class ExpressionHelper.
    /// Implements the <see cref="IExpressionHelper" />
    /// </summary>
    /// <seealso cref="IExpressionHelper" />
    internal class ExpressionHelper : IExpressionHelper
    {
        /// <summary>
        /// The constant expression wrapper
        /// </summary>
        private readonly IConstantExpressionWrapper _constantExpressionWrapper = new ConstantExpressionWrapper();
        /// <summary>
        /// The parsing configuration
        /// </summary>
        private readonly ParsingConfig _parsingConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionHelper"/> class.
        /// </summary>
        /// <param name="parsingConfig">The parsing configuration.</param>
        internal ExpressionHelper(ParsingConfig parsingConfig)
        {
            Argument.IsNotNull(nameof(parsingConfig), parsingConfig);

            _parsingConfig = parsingConfig;
        }

        /// <summary>
        /// Wraps the constant expression.
        /// </summary>
        /// <param name="argument">The argument.</param>
        public void WrapConstantExpression(ref Expression argument)
        {
            if (_parsingConfig.UseParameterizedNamesInDynamicQuery)
            {
                _constantExpressionWrapper.Wrap(ref argument);
            }
        }

        /// <summary>
        /// Converts the numeric type to biggest common type for binary operator.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        public void ConvertNumericTypeToBiggestCommonTypeForBinaryOperator(ref Expression left, ref Expression right)
        {
            if (left.Type == right.Type)
            {
                return;
            }

            if (left.Type == typeof(ulong) || right.Type == typeof(ulong))
            {
                right = right.Type != typeof(ulong) ? Expression.Convert(right, typeof(ulong)) : right;
                left = left.Type != typeof(ulong) ? Expression.Convert(left, typeof(ulong)) : left;
            }
            else if (left.Type == typeof(long) || right.Type == typeof(long))
            {
                right = right.Type != typeof(long) ? Expression.Convert(right, typeof(long)) : right;
                left = left.Type != typeof(long) ? Expression.Convert(left, typeof(long)) : left;
            }
            else if (left.Type == typeof(uint) || right.Type == typeof(uint))
            {
                right = right.Type != typeof(uint) ? Expression.Convert(right, typeof(uint)) : right;
                left = left.Type != typeof(uint) ? Expression.Convert(left, typeof(uint)) : left;
            }
            else if (left.Type == typeof(int) || right.Type == typeof(int))
            {
                right = right.Type != typeof(int) ? Expression.Convert(right, typeof(int)) : right;
                left = left.Type != typeof(int) ? Expression.Convert(left, typeof(int)) : left;
            }
            else if (left.Type == typeof(ushort) || right.Type == typeof(ushort))
            {
                right = right.Type != typeof(ushort) ? Expression.Convert(right, typeof(ushort)) : right;
                left = left.Type != typeof(ushort) ? Expression.Convert(left, typeof(ushort)) : left;
            }
            else if (left.Type == typeof(short) || right.Type == typeof(short))
            {
                right = right.Type != typeof(short) ? Expression.Convert(right, typeof(short)) : right;
                left = left.Type != typeof(short) ? Expression.Convert(left, typeof(short)) : left;
            }
            else if (left.Type == typeof(byte) || right.Type == typeof(byte))
            {
                right = right.Type != typeof(byte) ? Expression.Convert(right, typeof(byte)) : right;
                left = left.Type != typeof(byte) ? Expression.Convert(left, typeof(byte)) : left;
            }
        }

        /// <summary>
        /// Generates the add.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>Expression.</returns>
        public Expression GenerateAdd(Expression left, Expression right)
        {
            return Expression.Add(left, right);
        }

        /// <summary>
        /// Generates the string concat.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>Expression.</returns>
        public Expression GenerateStringConcat(Expression left, Expression right)
        {
            return GenerateStaticMethodCall("Concat", left, right);
        }

        /// <summary>
        /// Generates the subtract.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>Expression.</returns>
        public Expression GenerateSubtract(Expression left, Expression right)
        {
            return Expression.Subtract(left, right);
        }

        /// <summary>
        /// Generates the equal.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>Expression.</returns>
        public Expression GenerateEqual(Expression left, Expression right)
        {
            OptimizeForEqualityIfPossible(ref left, ref right);

            WrapConstantExpressions(ref left, ref right);

            return Expression.Equal(left, right);
        }

        /// <summary>
        /// Generates the not equal.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>Expression.</returns>
        public Expression GenerateNotEqual(Expression left, Expression right)
        {
            OptimizeForEqualityIfPossible(ref left, ref right);

            WrapConstantExpressions(ref left, ref right);

            return Expression.NotEqual(left, right);
        }

        /// <summary>
        /// Generates the greater than.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>Expression.</returns>
        public Expression GenerateGreaterThan(Expression left, Expression right)
        {
            if (left.Type == typeof(string))
            {
                return Expression.GreaterThan(GenerateStaticMethodCall("Compare", left, right), Expression.Constant(0));
            }

            if (left.Type.GetTypeInfo().IsEnum || right.Type.GetTypeInfo().IsEnum)
            {
                var leftPart = left.Type.GetTypeInfo().IsEnum ? Expression.Convert(left, Enum.GetUnderlyingType(left.Type)) : left;
                var rightPart = right.Type.GetTypeInfo().IsEnum ? Expression.Convert(right, Enum.GetUnderlyingType(right.Type)) : right;
                return Expression.GreaterThan(leftPart, rightPart);
            }

            WrapConstantExpressions(ref left, ref right);

            return Expression.GreaterThan(left, right);
        }

        /// <summary>
        /// Generates the greater than equal.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>Expression.</returns>
        public Expression GenerateGreaterThanEqual(Expression left, Expression right)
        {
            if (left.Type == typeof(string))
            {
                return Expression.GreaterThanOrEqual(GenerateStaticMethodCall("Compare", left, right), Expression.Constant(0));
            }

            if (left.Type.GetTypeInfo().IsEnum || right.Type.GetTypeInfo().IsEnum)
            {
                return Expression.GreaterThanOrEqual(left.Type.GetTypeInfo().IsEnum ? Expression.Convert(left, Enum.GetUnderlyingType(left.Type)) : left,
                    right.Type.GetTypeInfo().IsEnum ? Expression.Convert(right, Enum.GetUnderlyingType(right.Type)) : right);
            }

            WrapConstantExpressions(ref left, ref right);

            return Expression.GreaterThanOrEqual(left, right);
        }

        /// <summary>
        /// Generates the less than.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>Expression.</returns>
        public Expression GenerateLessThan(Expression left, Expression right)
        {
            if (left.Type == typeof(string))
            {
                return Expression.LessThan(GenerateStaticMethodCall("Compare", left, right), Expression.Constant(0));
            }

            if (left.Type.GetTypeInfo().IsEnum || right.Type.GetTypeInfo().IsEnum)
            {
                return Expression.LessThan(left.Type.GetTypeInfo().IsEnum ? Expression.Convert(left, Enum.GetUnderlyingType(left.Type)) : left,
                    right.Type.GetTypeInfo().IsEnum ? Expression.Convert(right, Enum.GetUnderlyingType(right.Type)) : right);
            }

            WrapConstantExpressions(ref left, ref right);

            return Expression.LessThan(left, right);
        }

        /// <summary>
        /// Generates the less than equal.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>Expression.</returns>
        public Expression GenerateLessThanEqual(Expression left, Expression right)
        {
            if (left.Type == typeof(string))
            {
                return Expression.LessThanOrEqual(GenerateStaticMethodCall("Compare", left, right), Expression.Constant(0));
            }

            if (left.Type.GetTypeInfo().IsEnum || right.Type.GetTypeInfo().IsEnum)
            {
                return Expression.LessThanOrEqual(left.Type.GetTypeInfo().IsEnum ? Expression.Convert(left, Enum.GetUnderlyingType(left.Type)) : left,
                    right.Type.GetTypeInfo().IsEnum ? Expression.Convert(right, Enum.GetUnderlyingType(right.Type)) : right);
            }

            WrapConstantExpressions(ref left, ref right);

            return Expression.LessThanOrEqual(left, right);
        }

        /// <summary>
        /// Optimizes for equality if possible.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        public void OptimizeForEqualityIfPossible(ref Expression left, ref Expression right)
        {
            // The goal here is to provide the way to convert some types from the string form in a way that is compatible with Linq to Entities.
            // The Expression.Call(typeof(Guid).GetMethod("Parse"), right); does the job only for Linq to Object but Linq to Entities.
            var leftType = left.Type;
            var rightType = right.Type;

            if (rightType == typeof(string) && right.NodeType == ExpressionType.Constant)
            {
                right = OptimizeStringForEqualityIfPossible((string)((ConstantExpression)right).Value, leftType) ?? right;
            }

            if (leftType == typeof(string) && left.NodeType == ExpressionType.Constant)
            {
                left = OptimizeStringForEqualityIfPossible((string)((ConstantExpression)left).Value, rightType) ?? left;
            }
        }

        /// <summary>
        /// Optimizes the string for equality if possible.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="type">The type.</param>
        /// <returns>Expression.</returns>
        public Expression OptimizeStringForEqualityIfPossible(string text, Type type)
        {
            if (type == typeof(DateTime) && DateTime.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateTime))
            {
                return Expression.Constant(dateTime, typeof(DateTime));
            }
            if (type == typeof(Guid) && Guid.TryParse(text, out var guid))
            {
                return Expression.Constant(guid, typeof(Guid));
            }
            return null;
        }

        /// <summary>
        /// Gets the static method.
        /// </summary>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>MethodInfo.</returns>
        private MethodInfo GetStaticMethod(string methodName, Expression left, Expression right)
        {
            var methodInfo = left.Type.GetMethod(methodName, new[] { left.Type, right.Type });
            if (methodInfo == null)
            {
                methodInfo = right.Type.GetMethod(methodName, new[] { left.Type, right.Type });
            }

            return methodInfo;
        }

        /// <summary>
        /// Generates the static method call.
        /// </summary>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>Expression.</returns>
        private Expression GenerateStaticMethodCall(string methodName, Expression left, Expression right)
        {
            return Expression.Call(null, GetStaticMethod(methodName, left, right), new[] { left, right });
        }

        /// <summary>
        /// Wraps the constant expressions.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        private void WrapConstantExpressions(ref Expression left, ref Expression right)
        {
            if (_parsingConfig.UseParameterizedNamesInDynamicQuery)
            {
                _constantExpressionWrapper.Wrap(ref left);
                _constantExpressionWrapper.Wrap(ref right);
            }
        }

        /// <summary>
        /// Tries the generate and also not null expression.
        /// </summary>
        /// <param name="sourceExpression">The source expression.</param>
        /// <param name="addSelf">if set to <c>true</c> [add self].</param>
        /// <param name="generatedExpression">The generated expression.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool TryGenerateAndAlsoNotNullExpression(Expression sourceExpression, bool addSelf, out Expression generatedExpression)
        {
            var expressions = CollectExpressions(addSelf, sourceExpression);

            if (expressions.Count == 1 && !(expressions[0] is MethodCallExpression))
            {
                generatedExpression = sourceExpression;
                return false;
            }

            // Reverse the list
            expressions.Reverse();

            // Convert all expressions into '!= null'
            var binaryExpressions = expressions.Select(expression => Expression.NotEqual(expression, Expression.Constant(null))).ToArray();

            // Convert all binary expressions into `AndAlso(...)`
            generatedExpression = binaryExpressions[0];
            for (var i = 1; i < binaryExpressions.Length; i++)
            {
                generatedExpression = Expression.AndAlso(generatedExpression, binaryExpressions[i]);
            }

            return true;
        }

        /// <summary>
        /// Gets the member expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>Expression.</returns>
        private static Expression GetMemberExpression(Expression expression)
        {
            if (expression is MemberExpression memberExpression)
            {
                return memberExpression;
            }

            if (expression is ParameterExpression parameterExpression)
            {
                return parameterExpression;
            }

            if (expression is MethodCallExpression methodCallExpression)
            {
                return methodCallExpression;
            }

            if (expression is LambdaExpression lambdaExpression)
            {
                if (lambdaExpression.Body is MemberExpression bodyAsMemberExpression)
                {
                    return bodyAsMemberExpression;
                }

                if (lambdaExpression.Body is UnaryExpression bodyAsUnaryExpression)
                {
                    return bodyAsUnaryExpression.Operand;
                }
            }

            return null;
        }

        /// <summary>
        /// Collects the expressions.
        /// </summary>
        /// <param name="addSelf">if set to <c>true</c> [add self].</param>
        /// <param name="sourceExpression">The source expression.</param>
        /// <returns>List&lt;Expression&gt;.</returns>
        private static List<Expression> CollectExpressions(bool addSelf, Expression sourceExpression)
        {
            var expression = GetMemberExpression(sourceExpression);

            var list = new List<Expression>();

            if (addSelf && expression is MemberExpression memberExpressionFirst)
            {
                if (TypeHelper.IsNullableType(memberExpressionFirst.Type) || !memberExpressionFirst.Type.GetTypeInfo().IsValueType)
                {
                    list.Add(sourceExpression);
                }
            }

            while (expression is MemberExpression memberExpression)
            {
                expression = GetMemberExpression(memberExpression.Expression);
                if (expression is MemberExpression)
                {
                    list.Add(expression);
                }
            }

            if (expression is ParameterExpression)
            {
                list.Add(expression);
            }

            if (expression is MethodCallExpression)
            {
                list.Add(expression);
            }

            return list;
        }
    }
}
