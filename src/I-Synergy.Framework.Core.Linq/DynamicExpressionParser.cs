using System;
using System.Linq.Expressions;
using ISynergy.Framework.Core.Linq.Parsers;
using ISynergy.Framework.Core.Validation;

namespace ISynergy.Framework.Core.Linq
{
    /// <summary>
    /// Helper class to convert an expression into an LambdaExpression
    /// </summary>
    public static class DynamicExpressionParser
    {
        /// <summary>
        /// Parses an expression into a LambdaExpression.
        /// </summary>
        /// <param name="parsingConfig">The Configuration for the parsing.</param>
        /// <param name="createParameterCtor">if set to <c>true</c> then also create a constructor for all the parameters. Note that this doesn't work for Linq-to-Database entities.</param>
        /// <param name="resultType">Type of the result. If not specified, it will be generated dynamically.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="values">An object array that contains zero or more objects which are used as replacement values.</param>
        /// <returns>The generated <see cref="LambdaExpression"/></returns>
        public static LambdaExpression ParseLambda(ParsingConfig parsingConfig, bool createParameterCtor, Type resultType, string expression, params object[] values)
        {
            Argument.IsNotNullOrEmpty(nameof(expression), expression);

            var parser = new ExpressionParser(new ParameterExpression[0], expression, values, parsingConfig);

            return Expression.Lambda(parser.Parse(resultType, createParameterCtor));
        }

        /// <summary>
        /// Parses an expression into a Typed Expression.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="parsingConfig">The Configuration for the parsing.</param>
        /// <param name="createParameterCtor">if set to <c>true</c> then also create a constructor for all the parameters. Note that this doesn't work for Linq-to-Database entities.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="values">An object array that contains zero or more objects which are used as replacement values.</param>
        /// <returns>The generated <see cref="Expression"/></returns>
        public static Expression<Func<TResult>> ParseLambda<TResult>(ParsingConfig parsingConfig, bool createParameterCtor, string expression, params object[] values)
        {
            return (Expression<Func<TResult>>)ParseLambda(parsingConfig, createParameterCtor, typeof(TResult), expression, values);
        }

        /// <summary>
        /// Parses an expression into a LambdaExpression.
        /// </summary>
        /// <param name="parsingConfig">The Configuration for the parsing.</param>
        /// <param name="createParameterCtor">if set to <c>true</c> then also create a constructor for all the parameters. Note that this doesn't work for Linq-to-Database entities.</param>
        /// <param name="parameters">A array from ParameterExpressions.</param>
        /// <param name="resultType">Type of the result. If not specified, it will be generated dynamically.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="values">An object array that contains zero or more objects which are used as replacement values.</param>
        /// <returns>The generated <see cref="LambdaExpression"/></returns>
        public static LambdaExpression ParseLambda(ParsingConfig parsingConfig, bool createParameterCtor, ParameterExpression[] parameters, Type resultType, string expression, params object[] values)
        {
            Argument.IsNotNull(nameof(parameters), parameters);
            Argument.HasNoNulls(nameof(parameters), parameters);
            Argument.IsNotNullOrEmpty(nameof(expression), expression);

            var parser = new ExpressionParser(parameters, expression, values, parsingConfig);

            var parsedExpression = parser.Parse(resultType, createParameterCtor);

            if (parsingConfig != null && parsingConfig.RenameParameterExpression && parameters.Length == 1)
            {
                var renamer = new ParameterExpressionRenamer(parser.LastLambdaItName);
                parsedExpression = renamer.Rename(parsedExpression, out var newParameterExpression);

                return Expression.Lambda(parsedExpression, new[] { newParameterExpression });
            }
            else
            {
                return Expression.Lambda(parsedExpression, parameters);
            }
        }

        /// <summary>
        /// Parses an expression into a Typed Expression.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="parsingConfig">The Configuration for the parsing.</param>
        /// <param name="createParameterCtor">if set to <c>true</c> then also create a constructor for all the parameters. Note that this doesn't work for Linq-to-Database entities.</param>
        /// <param name="parameters">A array from ParameterExpressions.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="values">An object array that contains zero or more objects which are used as replacement values.</param>
        /// <returns>The generated <see cref="Expression"/></returns>
        public static Expression<Func<TResult>> ParseLambda<TResult>(ParsingConfig parsingConfig, bool createParameterCtor, ParameterExpression[] parameters, string expression, params object[] values)
        {
            return (Expression<Func<TResult>>)ParseLambda(parsingConfig, createParameterCtor, parameters, typeof(TResult), expression, values);
        }

        /// <summary>
        /// Parses an expression into a LambdaExpression.
        /// </summary>
        /// <param name="createParameterCtor">if set to <c>true</c> then also create a constructor for all the parameters. Note that this doesn't work for Linq-to-Database entities.</param>
        /// <param name="itType">The main type from the dynamic class expression.</param>
        /// <param name="resultType">Type of the result. If not specified, it will be generated dynamically.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="values">An object array that contains zero or more objects which are used as replacement values.</param>
        /// <returns>The generated <see cref="LambdaExpression"/></returns>
        public static LambdaExpression ParseLambda(bool createParameterCtor, Type itType, Type resultType, string expression, params object[] values)
        {
            Argument.IsNotNull(nameof(itType), itType);
            Argument.IsNotNullOrEmpty(nameof(expression), expression);

            return ParseLambda(createParameterCtor, new[] { Expression.Parameter(itType, string.Empty) }, resultType, expression, values);
        }

        /// <summary>
        /// Parses an expression into a Typed Expression.
        /// </summary>
        /// <typeparam name="T">The `it`-Type.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="parsingConfig">The Configuration for the parsing.</param>
        /// <param name="createParameterCtor">if set to <c>true</c> then also create a constructor for all the parameters. Note that this doesn't work for Linq-to-Database entities.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="values">An object array that contains zero or more objects which are used as replacement values.</param>
        /// <returns>The generated <see cref="Expression"/></returns>
        public static Expression<Func<T, TResult>> ParseLambda<T, TResult>(ParsingConfig parsingConfig, bool createParameterCtor, string expression, params object[] values)
        {
            Argument.IsNotNullOrEmpty(nameof(expression), expression);

            return (Expression<Func<T, TResult>>)ParseLambda(parsingConfig, createParameterCtor, new[] { Expression.Parameter(typeof(T), string.Empty) }, typeof(TResult), expression, values);
        }

        /// <summary>
        /// Parses an expression into a LambdaExpression. (Also create a constructor for all the parameters. Note that this doesn't work for Linq-to-Database entities.)
        /// </summary>
        /// <param name="parsingConfig">The Configuration for the parsing.</param>
        /// <param name="resultType">Type of the result. If not specified, it will be generated dynamically.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="values">An object array that contains zero or more objects which are used as replacement values.</param>
        /// <returns>The generated <see cref="LambdaExpression"/></returns>
        public static LambdaExpression ParseLambda(ParsingConfig parsingConfig, Type resultType, string expression, params object[] values)
        {
            return ParseLambda(parsingConfig, true, resultType, expression, values);
        }

        /// <summary>
        /// Parses an expression into a LambdaExpression. (Also create a constructor for all the parameters. Note that this doesn't work for Linq-to-Database entities.)
        /// </summary>
        /// <param name="resultType">Type of the result. If not specified, it will be generated dynamically.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="values">An object array that contains zero or more objects which are used as replacement values.</param>
        /// <returns>The generated <see cref="LambdaExpression"/></returns>
        public static LambdaExpression ParseLambda(Type resultType, string expression, params object[] values)
        {
            Argument.IsNotNullOrEmpty(nameof(expression), expression);

            return ParseLambda(null, true, resultType, expression, values);
        }

        /// <summary>
        /// Parses an expression into a LambdaExpression. (Also create a constructor for all the parameters. Note that this doesn't work for Linq-to-Database entities.)
        /// </summary>
        /// <param name="itType">The main type from the dynamic class expression.</param>
        /// <param name="resultType">Type of the result. If not specified, it will be generated dynamically.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="values">An object array that contains zero or more objects which are used as replacement values.</param>
        /// <returns>The generated <see cref="LambdaExpression"/></returns>
        public static LambdaExpression ParseLambda(Type itType, Type resultType, string expression, params object[] values)
        {
            return ParseLambda(true, itType, resultType, expression, values);
        }

        /// <summary>
        /// Parses an expression into a LambdaExpression. (Also create a constructor for all the parameters. Note that this doesn't work for Linq-to-Database entities.)
        /// </summary>
        /// <param name="parsingConfig">The Configuration for the parsing.</param>
        /// <param name="itType">The main type from the dynamic class expression.</param>
        /// <param name="resultType">Type of the result. If not specified, it will be generated dynamically.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="values">An object array that contains zero or more objects which are used as replacement values.</param>
        /// <returns>The generated <see cref="LambdaExpression"/></returns>
        public static LambdaExpression ParseLambda(ParsingConfig parsingConfig, Type itType, Type resultType, string expression, params object[] values)
        {
            return ParseLambda(parsingConfig, true, itType, resultType, expression, values);
        }

        /// <summary>
        /// Parses an expression into a LambdaExpression.
        /// </summary>
        /// <param name="parsingConfig">The Configuration for the parsing.</param>
        /// <param name="createParameterCtor">if set to <c>true</c> then also create a constructor for all the parameters. Note that this doesn't work for Linq-to-Database entities.</param>
        /// <param name="itType">The main type from the dynamic class expression.</param>
        /// <param name="resultType">Type of the result. If not specified, it will be generated dynamically.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="values">An object array that contains zero or more objects which are used as replacement values.</param>
        /// <returns>The generated <see cref="LambdaExpression"/></returns>
        public static LambdaExpression ParseLambda(ParsingConfig parsingConfig, bool createParameterCtor, Type itType, Type resultType, string expression, params object[] values)
        {
            Argument.IsNotNull(nameof(itType), itType);
            Argument.IsNotNullOrEmpty(nameof(expression), expression);

            return ParseLambda(parsingConfig, createParameterCtor, new[] { Expression.Parameter(itType, string.Empty) }, resultType, expression, values);
        }

        /// <summary>
        /// Parses an expression into a LambdaExpression. (Also create a constructor for all the parameters. Note that this doesn't work for Linq-to-Database entities.)
        /// </summary>
        /// <param name="parameters">A array from ParameterExpressions.</param>
        /// <param name="resultType">Type of the result. If not specified, it will be generated dynamically.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="values">An object array that contains zero or more objects which are used as replacement values.</param>
        /// <returns>The generated <see cref="LambdaExpression"/></returns>
        public static LambdaExpression ParseLambda(ParameterExpression[] parameters, Type resultType, string expression, params object[] values)
        {
            return ParseLambda(null, true, parameters, resultType, expression, values);
        }

        /// <summary>
        /// Parses an expression into a LambdaExpression. (Also create a constructor for all the parameters. Note that this doesn't work for Linq-to-Database entities.)
        /// </summary>
        /// <param name="parsingConfig">The Configuration for the parsing.</param>
        /// <param name="parameters">A array from ParameterExpressions.</param>
        /// <param name="resultType">Type of the result. If not specified, it will be generated dynamically.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="values">An object array that contains zero or more objects which are used as replacement values.</param>
        /// <returns>The generated <see cref="LambdaExpression"/></returns>
        public static LambdaExpression ParseLambda(ParsingConfig parsingConfig, ParameterExpression[] parameters, Type resultType, string expression, params object[] values)
        {
            return ParseLambda(parsingConfig, true, parameters, resultType, expression, values);
        }

        /// <summary>
        /// Parses an expression into a LambdaExpression.
        /// </summary>
        /// <param name="createParameterCtor">if set to <c>true</c> then also create a constructor for all the parameters. Note that this doesn't work for Linq-to-Database entities.</param>
        /// <param name="parameters">A array from ParameterExpressions.</param>
        /// <param name="resultType">Type of the result. If not specified, it will be generated dynamically.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="values">An object array that contains zero or more objects which are used as replacement values.</param>
        /// <returns>The generated <see cref="LambdaExpression"/></returns>
        public static LambdaExpression ParseLambda(bool createParameterCtor, ParameterExpression[] parameters, Type resultType, string expression, params object[] values)
        {
            return ParseLambda(null, createParameterCtor, parameters, resultType, expression, values);
        }
    }
}
