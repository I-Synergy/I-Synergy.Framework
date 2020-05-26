using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using ISynergy.Framework.Core.Linq.Abstractions;
using ISynergy.Framework.Core.Linq.Binders;
using ISynergy.Framework.Core.Linq.Constants;
using ISynergy.Framework.Core.Linq.Exceptions;
using ISynergy.Framework.Core.Linq.Factories;
using ISynergy.Framework.Core.Linq.Helpers;
using ISynergy.Framework.Core.Linq.Parsers.SupportedMethods;
using ISynergy.Framework.Core.Linq.Parsers.SupportedOperands;
using ISynergy.Framework.Core.Linq.Tokenizer;
using ISynergy.Framework.Core.Validation;

namespace ISynergy.Framework.Core.Linq.Parsers
{
    /// <summary>
    /// ExpressionParser
    /// </summary>
    public class ExpressionParser
    {
        /// <summary>
        /// The method order by
        /// </summary>
        static readonly string methodOrderBy = nameof(Queryable.OrderBy);
        /// <summary>
        /// The method order by descending
        /// </summary>
        static readonly string methodOrderByDescending = nameof(Queryable.OrderByDescending);
        /// <summary>
        /// The method then by
        /// </summary>
        static readonly string methodThenBy = nameof(Queryable.ThenBy);
        /// <summary>
        /// The method then by descending
        /// </summary>
        static readonly string methodThenByDescending = nameof(Queryable.ThenByDescending);

        /// <summary>
        /// The parsing configuration
        /// </summary>
        private readonly ParsingConfig _parsingConfig;
        /// <summary>
        /// The method finder
        /// </summary>
        private readonly MethodFinder _methodFinder;
        /// <summary>
        /// The keywords helper
        /// </summary>
        private readonly IKeywordsHelper _keywordsHelper;
        /// <summary>
        /// The text parser
        /// </summary>
        private readonly TextParser _textParser;
        /// <summary>
        /// The expression helper
        /// </summary>
        private readonly IExpressionHelper _expressionHelper;
        /// <summary>
        /// The type finder
        /// </summary>
        private readonly ITypeFinder _typeFinder;
        /// <summary>
        /// The type converter factory
        /// </summary>
        private readonly ITypeConverterFactory _typeConverterFactory;
        /// <summary>
        /// The internals
        /// </summary>
        private readonly Dictionary<string, object> _internals;
        /// <summary>
        /// The symbols
        /// </summary>
        private readonly Dictionary<string, object> _symbols;

        /// <summary>
        /// The externals
        /// </summary>
        private IDictionary<string, object> _externals;
        /// <summary>
        /// It
        /// </summary>
        private ParameterExpression _it;
        /// <summary>
        /// The parent
        /// </summary>
        private ParameterExpression _parent;
        /// <summary>
        /// The root
        /// </summary>
        private ParameterExpression _root;
        /// <summary>
        /// The result type
        /// </summary>
        private Type _resultType;
        /// <summary>
        /// The create parameter ctor
        /// </summary>
        private bool _createParameterCtor;

        /// <summary>
        /// Gets name for the `it` field. By default this is set to the KeyWord value "it".
        /// </summary>
        /// <value>It name.</value>
        public string ItName { get; private set; } = KeywordsHelper.KEYWORD_IT;

        /// <summary>
        /// There was a problem when an expression contained multiple lambdas where
        /// the ItName was not cleared and freed for the next lambda. This variable
        /// stores the ItName of the last parsed lambda.
        /// Not used internally by ExpressionParser, but used to preserve compatibility of parsingConfig.RenameParameterExpression
        /// which was designed to only work with mono-lambda expressions.
        /// </summary>
        /// <value>The last name of the lambda it.</value>
        public string LastLambdaItName { get; private set; } = KeywordsHelper.KEYWORD_IT;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionParser" /> class.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="values">The values.</param>
        /// <param name="parsingConfig">The parsing configuration.</param>
        public ExpressionParser(ParameterExpression[] parameters, string expression, object[] values, ParsingConfig parsingConfig)
        {
            Argument.IsNotNullOrEmpty(nameof(expression), expression);

            _symbols = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            _internals = new Dictionary<string, object>();

            if (parameters != null)
            {
                ProcessParameters(parameters);
            }

            if (values != null)
            {
                ProcessValues(values);
            }

            _parsingConfig = parsingConfig ?? ParsingConfig.Default;

            _keywordsHelper = new KeywordsHelper(_parsingConfig);
            _textParser = new TextParser(_parsingConfig, expression);
            _methodFinder = new MethodFinder(_parsingConfig);
            _expressionHelper = new ExpressionHelper(_parsingConfig);
            _typeFinder = new TypeFinder(_parsingConfig, _keywordsHelper);
            _typeConverterFactory = new TypeConverterFactory(_parsingConfig);
        }

        /// <summary>
        /// Processes the parameters.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        void ProcessParameters(ParameterExpression[] parameters)
        {
            foreach (var pe in parameters.Where(p => !string.IsNullOrEmpty(p.Name)))
            {
                AddSymbol(pe.Name, pe);
            }

            // If there is only 1 ParameterExpression, do also allow access using 'it'
            if (parameters.Length == 1)
            {
                _parent = _it;
                _it = parameters[0];

                if (_root == null)
                {
                    _root = _it;
                }
            }
        }

        /// <summary>
        /// Processes the values.
        /// </summary>
        /// <param name="values">The values.</param>
        void ProcessValues(object[] values)
        {
            for (var i = 0; i < values.Length; i++)
            {
                var value = values[i];

                if (i == values.Length - 1 && value is IDictionary<string, object> externals)
                {
                    _externals = externals;
                }
                else
                {
                    AddSymbol("@" + i.ToString(CultureInfo.InvariantCulture), value);
                }
            }
        }

        /// <summary>
        /// Adds the symbol.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        private void AddSymbol(string name, object value)
        {
            if (_symbols.ContainsKey(name))
            {
                throw ParseError(Res.DuplicateIdentifier, name);
            }

            _symbols.Add(name, value);
        }

        /// <summary>
        /// Uses the TextParser to parse the string into the specified result type.
        /// </summary>
        /// <param name="resultType">Type of the result.</param>
        /// <param name="createParameterCtor">if set to <c>true</c> [create parameter ctor].</param>
        /// <returns>Expression</returns>
        public Expression Parse(Type resultType, bool createParameterCtor = true)
        {
            _resultType = resultType;
            _createParameterCtor = createParameterCtor;

            var exprPos = _textParser.CurrentToken.Pos;
            var expr = ParseConditionalOperator();

            if (resultType != null)
            {
                if ((expr = _parsingConfig.ExpressionPromoter.Promote(expr, resultType, true, false)) == null)
                {
                    throw ParseError(exprPos, Res.ExpressionTypeMismatch, TypeHelper.GetTypeName(resultType));
                }
            }

            _textParser.ValidateToken(TokenId.End, Res.SyntaxError);

            return expr;
        }

#pragma warning disable 0219
        /// <summary>
        /// Parses the ordering.
        /// </summary>
        /// <param name="forceThenBy">if set to <c>true</c> [force then by].</param>
        /// <returns>IList&lt;DynamicOrdering&gt;.</returns>
        internal IList<DynamicOrdering> ParseOrdering(bool forceThenBy = false)
        {
            var orderings = new List<DynamicOrdering>();
            while (true)
            {
                var expr = ParseConditionalOperator();
                var ascending = true;
                if (TokenIdentifierIs("asc") || TokenIdentifierIs("ascending"))
                {
                    _textParser.NextToken();
                }
                else if (TokenIdentifierIs("desc") || TokenIdentifierIs("descending"))
                {
                    _textParser.NextToken();
                    ascending = false;
                }

                string methodName;
                if (forceThenBy || orderings.Count > 0)
                {
                    methodName = ascending ? methodThenBy : methodThenByDescending;
                }
                else
                {
                    methodName = ascending ? methodOrderBy : methodOrderByDescending;
                }

                orderings.Add(new DynamicOrdering { Selector = expr, Ascending = ascending, MethodName = methodName });

                if (_textParser.CurrentToken.Id != TokenId.Comma)
                {
                    break;
                }

                _textParser.NextToken();
            }

            _textParser.ValidateToken(TokenId.End, Res.SyntaxError);
            return orderings;
        }
#pragma warning restore 0219

        // ?: operator
        /// <summary>
        /// Parses the conditional operator.
        /// </summary>
        /// <returns>Expression.</returns>
        Expression ParseConditionalOperator()
        {
            var errorPos = _textParser.CurrentToken.Pos;
            var expr = ParseNullCoalescingOperator();
            if (_textParser.CurrentToken.Id == TokenId.Question)
            {
                _textParser.NextToken();
                var expr1 = ParseConditionalOperator();
                _textParser.ValidateToken(TokenId.Colon, Res.ColonExpected);
                _textParser.NextToken();
                var expr2 = ParseConditionalOperator();
                expr = GenerateConditional(expr, expr1, expr2, errorPos);
            }
            return expr;
        }

        // ?? (null-coalescing) operator
        /// <summary>
        /// Parses the null coalescing operator.
        /// </summary>
        /// <returns>Expression.</returns>
        Expression ParseNullCoalescingOperator()
        {
            var expr = ParseLambdaOperator();
            if (_textParser.CurrentToken.Id == TokenId.NullCoalescing)
            {
                _textParser.NextToken();
                var right = ParseConditionalOperator();
                expr = Expression.Coalesce(expr, right);
            }
            return expr;
        }

        // => operator - Added Support for projection operator
        /// <summary>
        /// Parses the lambda operator.
        /// </summary>
        /// <returns>Expression.</returns>
        Expression ParseLambdaOperator()
        {
            var expr = ParseOrOperator();
            if (_textParser.CurrentToken.Id == TokenId.Lambda && _it.Type == expr.Type)
            {
                _textParser.NextToken();
                if (_textParser.CurrentToken.Id == TokenId.Identifier || _textParser.CurrentToken.Id == TokenId.OpenParen)
                {
                    var right = ParseConditionalOperator();
                    return Expression.Lambda(right, new[] { (ParameterExpression)expr });
                }
                _textParser.ValidateToken(TokenId.OpenParen, Res.OpenParenExpected);
            }
            return expr;
        }

        // Or operator
        // - ||
        // - Or
        // - OrElse
        /// <summary>
        /// Parses the or operator.
        /// </summary>
        /// <returns>Expression.</returns>
        Expression ParseOrOperator()
        {
            var left = ParseAndOperator();
            while (_textParser.CurrentToken.Id == TokenId.DoubleBar)
            {
                var op = _textParser.CurrentToken;
                _textParser.NextToken();
                var right = ParseAndOperator();
                CheckAndPromoteOperands(typeof(ILogicalSignatures), op.Id, op.Text, ref left, ref right, op.Pos);
                left = Expression.OrElse(left, right);
            }
            return left;
        }

        // And operator
        // - &&
        // - And
        // - AndAlso
        /// <summary>
        /// Parses the and operator.
        /// </summary>
        /// <returns>Expression.</returns>
        Expression ParseAndOperator()
        {
            var left = ParseIn();
            while (_textParser.CurrentToken.Id == TokenId.DoubleAmphersand)
            {
                var op = _textParser.CurrentToken;
                _textParser.NextToken();
                var right = ParseComparisonOperator();
                CheckAndPromoteOperands(typeof(ILogicalSignatures), op.Id, op.Text, ref left, ref right, op.Pos);
                left = Expression.AndAlso(left, right);
            }
            return left;
        }

        // in operator for literals - example: "x in (1,2,3,4)"
        // in operator to mimic contains - example: "x in @0", compare to @0.Contains(x)
        // Adapted from ticket submitted by github user mlewis9548 
        /// <summary>
        /// Parses the in.
        /// </summary>
        /// <returns>Expression.</returns>
        Expression ParseIn()
        {
            var left = ParseLogicalAndOrOperator();
            var accumulate = left;

            while (TokenIdentifierIs("in"))
            {
                var op = _textParser.CurrentToken;

                _textParser.NextToken();
                if (_textParser.CurrentToken.Id == TokenId.OpenParen) // literals (or other inline list)
                {
                    while (_textParser.CurrentToken.Id != TokenId.CloseParen)
                    {
                        _textParser.NextToken();

                        // we need to parse unary expressions because otherwise 'in' clause will fail in use cases like 'in (-1, -1)' or 'in (!true)'
                        var right = ParseUnary();

                        // if the identifier is an Enum, try to convert the right-side also to an Enum.
                        if (left.Type.GetTypeInfo().IsEnum && right is ConstantExpression)
                        {
                            right = ParseEnumToConstantExpression(op.Pos, left.Type, right as ConstantExpression);
                        }

                        // else, check for direct type match
                        else if (left.Type != right.Type)
                        {
                            CheckAndPromoteOperands(typeof(IEqualitySignatures), TokenId.DoubleEqual, "==", ref left, ref right, op.Pos);
                        }

                        if (accumulate.Type != typeof(bool))
                        {
                            accumulate = _expressionHelper.GenerateEqual(left, right);
                        }
                        else
                        {
                            accumulate = Expression.OrElse(accumulate, _expressionHelper.GenerateEqual(left, right));
                        }

                        if (_textParser.CurrentToken.Id == TokenId.End)
                        {
                            throw ParseError(op.Pos, Res.CloseParenOrCommaExpected);
                        }
                    }
                }
                else if (_textParser.CurrentToken.Id == TokenId.Identifier) // a single argument
                {
                    var right = ParsePrimary();

                    if (!typeof(IEnumerable).IsAssignableFrom(right.Type))
                    {
                        throw ParseError(_textParser.CurrentToken.Pos, Res.IdentifierImplementingInterfaceExpected, typeof(IEnumerable));
                    }

                    var args = new[] { left };

                    if (_methodFinder.FindMethod(typeof(IEnumerableSignatures), nameof(IEnumerableSignatures.Contains), false, args, out var containsSignature) != 1)
                    {
                        throw ParseError(op.Pos, Res.NoApplicableAggregate, nameof(IEnumerableSignatures.Contains), string.Join(",", args.Select(a => a.Type.Name).ToArray()));
                    }

                    var typeArgs = new[] { left.Type };

                    args = new[] { right, left };

                    accumulate = Expression.Call(typeof(Enumerable), containsSignature.Name, typeArgs, args);
                }
                else
                {
                    throw ParseError(op.Pos, Res.OpenParenOrIdentifierExpected);
                }

                _textParser.NextToken();
            }

            return accumulate;
        }

        // &, | bitwise operators
        /// <summary>
        /// Parses the logical and or operator.
        /// </summary>
        /// <returns>Expression.</returns>
        Expression ParseLogicalAndOrOperator()
        {
            var left = ParseComparisonOperator();
            while (_textParser.CurrentToken.Id == TokenId.Amphersand || _textParser.CurrentToken.Id == TokenId.Bar)
            {
                var op = _textParser.CurrentToken;
                _textParser.NextToken();
                var right = ParseComparisonOperator();

                if (left.Type.GetTypeInfo().IsEnum)
                {
                    left = Expression.Convert(left, Enum.GetUnderlyingType(left.Type));
                }

                if (right.Type.GetTypeInfo().IsEnum)
                {
                    right = Expression.Convert(right, Enum.GetUnderlyingType(right.Type));
                }

                switch (op.Id)
                {
                    case TokenId.Amphersand:
                        int parseValue;
                        if (left.Type == typeof(string) && left.NodeType == ExpressionType.Constant && int.TryParse((string)((ConstantExpression)left).Value, out parseValue) && TypeHelper.IsNumericType(right.Type))
                        {
                            left = Expression.Constant(parseValue);
                        }
                        else if (right.Type == typeof(string) && right.NodeType == ExpressionType.Constant && int.TryParse((string)((ConstantExpression)right).Value, out parseValue) && TypeHelper.IsNumericType(left.Type))
                        {
                            right = Expression.Constant(parseValue);
                        }

                        // When at least one side of the operator is a string, consider it's a VB-style concatenation operator.
                        // Doesn't break any other function since logical AND with a string is invalid anyway.
                        if (left.Type == typeof(string) || right.Type == typeof(string))
                        {
                            left = _expressionHelper.GenerateStringConcat(left, right);
                        }
                        else
                        {
                            _expressionHelper.ConvertNumericTypeToBiggestCommonTypeForBinaryOperator(ref left, ref right);
                            left = Expression.And(left, right);
                        }
                        break;

                    case TokenId.Bar:
                        _expressionHelper.ConvertNumericTypeToBiggestCommonTypeForBinaryOperator(ref left, ref right);
                        left = Expression.Or(left, right);
                        break;
                }
            }
            return left;
        }

        // =, ==, !=, <>, >, >=, <, <= operators
        /// <summary>
        /// Parses the comparison operator.
        /// </summary>
        /// <returns>Expression.</returns>
        Expression ParseComparisonOperator()
        {
            var left = ParseShiftOperator();
            while (_textParser.CurrentToken.Id == TokenId.Equal || _textParser.CurrentToken.Id == TokenId.DoubleEqual ||
                   _textParser.CurrentToken.Id == TokenId.ExclamationEqual || _textParser.CurrentToken.Id == TokenId.LessGreater ||
                   _textParser.CurrentToken.Id == TokenId.GreaterThan || _textParser.CurrentToken.Id == TokenId.GreaterThanEqual ||
                   _textParser.CurrentToken.Id == TokenId.LessThan || _textParser.CurrentToken.Id == TokenId.LessThanEqual)
            {
                ConstantExpression constantExpr;
                TypeConverter typeConverter;
                var op = _textParser.CurrentToken;
                _textParser.NextToken();
                var right = ParseShiftOperator();
                var isEquality = op.Id == TokenId.Equal || op.Id == TokenId.DoubleEqual || op.Id == TokenId.ExclamationEqual || op.Id == TokenId.LessGreater;

                if (isEquality && (!left.Type.GetTypeInfo().IsValueType && !right.Type.GetTypeInfo().IsValueType || left.Type == typeof(Guid) && right.Type == typeof(Guid)))
                {
                    // If left or right is NullLiteral, just continue. Else check if the types differ.
                    if (!(Constants.IsNull(left) || Constants.IsNull(right)) && left.Type != right.Type)
                    {
                        if (left.Type.IsAssignableFrom(right.Type) || HasImplicitConversion(right.Type, left.Type))
                        {
                            right = Expression.Convert(right, left.Type);
                        }
                        else if (right.Type.IsAssignableFrom(left.Type) || HasImplicitConversion(left.Type, right.Type))
                        {
                            left = Expression.Convert(left, right.Type);
                        }
                        else
                        {
                            throw IncompatibleOperandsError(op.Text, left, right, op.Pos);
                        }
                    }
                }
                else if (TypeHelper.IsEnumType(left.Type) || TypeHelper.IsEnumType(right.Type))
                {
                    if (left.Type != right.Type)
                    {
                        Expression e;
                        if ((e = _parsingConfig.ExpressionPromoter.Promote(right, left.Type, true, false)) != null)
                        {
                            right = e;
                        }
                        else if ((e = _parsingConfig.ExpressionPromoter.Promote(left, right.Type, true, false)) != null)
                        {
                            left = e;
                        }
                        else if (TypeHelper.IsEnumType(left.Type) && (constantExpr = right as ConstantExpression) != null)
                        {
                            right = ParseEnumToConstantExpression(op.Pos, left.Type, constantExpr);
                        }
                        else if (TypeHelper.IsEnumType(right.Type) && (constantExpr = left as ConstantExpression) != null)
                        {
                            left = ParseEnumToConstantExpression(op.Pos, right.Type, constantExpr);
                        }
                        else
                        {
                            throw IncompatibleOperandsError(op.Text, left, right, op.Pos);
                        }
                    }
                }
                else if ((constantExpr = right as ConstantExpression) != null && constantExpr.Value is string stringValueR && (typeConverter = _typeConverterFactory.GetConverter(left.Type)) != null)
                {
                    right = Expression.Constant(typeConverter.ConvertFromInvariantString(stringValueR), left.Type);
                }
                else if ((constantExpr = left as ConstantExpression) != null && constantExpr.Value is string stringValueL && (typeConverter = _typeConverterFactory.GetConverter(right.Type)) != null)
                {
                    left = Expression.Constant(typeConverter.ConvertFromInvariantString(stringValueL), right.Type);
                }
                else
                {
                    var typesAreSameAndImplementCorrectInterface = false;
                    if (left.Type == right.Type)
                    {
                        var interfaces = left.Type.GetInterfaces().Where(x => x.GetTypeInfo().IsGenericType);
                        if (isEquality)
                        {
                            typesAreSameAndImplementCorrectInterface = interfaces.Any(x => x.GetGenericTypeDefinition() == typeof(IEquatable<>));
                        }
                        else
                        {
                            typesAreSameAndImplementCorrectInterface = interfaces.Any(x => x.GetGenericTypeDefinition() == typeof(IComparable<>));
                        }
                    }


                    if (!typesAreSameAndImplementCorrectInterface)
                    {
                        if (left.Type.GetTypeInfo().IsClass && right is ConstantExpression)
                        {
                            if (HasImplicitConversion(left.Type, right.Type))
                            {
                                left = Expression.Convert(left, right.Type);
                            }
                            else if (HasImplicitConversion(right.Type, left.Type))
                            {
                                right = Expression.Convert(right, left.Type);
                            }
                        }
                        else if (right.Type.GetTypeInfo().IsClass && left is ConstantExpression)
                        {
                            if (HasImplicitConversion(right.Type, left.Type))
                            {
                                right = Expression.Convert(right, left.Type);
                            }
                            else if (HasImplicitConversion(left.Type, right.Type))
                            {
                                left = Expression.Convert(left, right.Type);
                            }
                        }
                        else
                        {
                            CheckAndPromoteOperands(isEquality ? typeof(IEqualitySignatures) : typeof(IRelationalSignatures), op.Id, op.Text, ref left, ref right, op.Pos);
                        }
                    }
                }

                switch (op.Id)
                {
                    case TokenId.Equal:
                    case TokenId.DoubleEqual:
                        left = _expressionHelper.GenerateEqual(left, right);
                        break;
                    case TokenId.ExclamationEqual:
                    case TokenId.LessGreater:
                        left = _expressionHelper.GenerateNotEqual(left, right);
                        break;
                    case TokenId.GreaterThan:
                        left = _expressionHelper.GenerateGreaterThan(left, right);
                        break;
                    case TokenId.GreaterThanEqual:
                        left = _expressionHelper.GenerateGreaterThanEqual(left, right);
                        break;
                    case TokenId.LessThan:
                        left = _expressionHelper.GenerateLessThan(left, right);
                        break;
                    case TokenId.LessThanEqual:
                        left = _expressionHelper.GenerateLessThanEqual(left, right);
                        break;
                }
            }

            return left;
        }

        /// <summary>
        /// Determines whether [has implicit conversion] [the specified base type].
        /// </summary>
        /// <param name="baseType">Type of the base.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <returns><c>true</c> if [has implicit conversion] [the specified base type]; otherwise, <c>false</c>.</returns>
        private bool HasImplicitConversion(Type baseType, Type targetType)
        {
            var baseTypeHasConversion = baseType.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(mi => mi.Name == "op_Implicit" && mi.ReturnType == targetType)
                .Any(mi => mi.GetParameters().FirstOrDefault()?.ParameterType == baseType);

            if (baseTypeHasConversion)
            {
                return true;
            }

            return targetType.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(mi => mi.Name == "op_Implicit" && mi.ReturnType == targetType)
                .Any(mi => mi.GetParameters().FirstOrDefault()?.ParameterType == baseType);
        }

        /// <summary>
        /// Parses the enum to constant expression.
        /// </summary>
        /// <param name="pos">The position.</param>
        /// <param name="leftType">Type of the left.</param>
        /// <param name="constantExpr">The constant expr.</param>
        /// <returns>ConstantExpression.</returns>
        private ConstantExpression ParseEnumToConstantExpression(int pos, Type leftType, ConstantExpression constantExpr)
        {
            return Expression.Constant(ParseConstantExpressionToEnum(pos, leftType, constantExpr), leftType);
        }

        /// <summary>
        /// Parses the constant expression to enum.
        /// </summary>
        /// <param name="pos">The position.</param>
        /// <param name="leftType">Type of the left.</param>
        /// <param name="constantExpr">The constant expr.</param>
        /// <returns>System.Object.</returns>
        private object ParseConstantExpressionToEnum(int pos, Type leftType, ConstantExpression constantExpr)
        {
            try
            {
                if (constantExpr.Value is string stringValue)
                {
                    return Enum.Parse(TypeHelper.GetNonNullableType(leftType), stringValue, true);
                }
            }
            catch
            {
                throw ParseError(pos, Res.ExpressionTypeMismatch, leftType);
            }

            try
            {
                return Enum.ToObject(TypeHelper.GetNonNullableType(leftType), constantExpr.Value);
            }
            catch
            {
                throw ParseError(pos, Res.ExpressionTypeMismatch, leftType);
            }
        }

        // <<, >> operators
        /// <summary>
        /// Parses the shift operator.
        /// </summary>
        /// <returns>Expression.</returns>
        Expression ParseShiftOperator()
        {
            var left = ParseAdditive();
            while (_textParser.CurrentToken.Id == TokenId.DoubleLessThan || _textParser.CurrentToken.Id == TokenId.DoubleGreaterThan)
            {
                var op = _textParser.CurrentToken;
                _textParser.NextToken();
                var right = ParseAdditive();
                switch (op.Id)
                {
                    case TokenId.DoubleLessThan:
                        CheckAndPromoteOperands(typeof(IShiftSignatures), op.Id, op.Text, ref left, ref right, op.Pos);
                        left = Expression.LeftShift(left, right);
                        break;
                    case TokenId.DoubleGreaterThan:
                        CheckAndPromoteOperands(typeof(IShiftSignatures), op.Id, op.Text, ref left, ref right, op.Pos);
                        left = Expression.RightShift(left, right);
                        break;
                }
            }
            return left;
        }

        // +, - operators
        /// <summary>
        /// Parses the additive.
        /// </summary>
        /// <returns>Expression.</returns>
        Expression ParseAdditive()
        {
            var left = ParseMultiplicative();
            while (_textParser.CurrentToken.Id == TokenId.Plus || _textParser.CurrentToken.Id == TokenId.Minus)
            {
                var op = _textParser.CurrentToken;
                _textParser.NextToken();
                var right = ParseMultiplicative();
                switch (op.Id)
                {
                    case TokenId.Plus:
                        if (left.Type == typeof(string) || right.Type == typeof(string))
                        {
                            left = _expressionHelper.GenerateStringConcat(left, right);
                        }
                        else
                        {
                            CheckAndPromoteOperands(typeof(IAddSignatures), op.Id, op.Text, ref left, ref right, op.Pos);
                            left = _expressionHelper.GenerateAdd(left, right);
                        }
                        break;
                    case TokenId.Minus:
                        CheckAndPromoteOperands(typeof(ISubtractSignatures), op.Id, op.Text, ref left, ref right, op.Pos);
                        left = _expressionHelper.GenerateSubtract(left, right);
                        break;
                }
            }
            return left;
        }

        // *, /, %, mod operators
        /// <summary>
        /// Parses the multiplicative.
        /// </summary>
        /// <returns>Expression.</returns>
        Expression ParseMultiplicative()
        {
            var left = ParseUnary();
            while (_textParser.CurrentToken.Id == TokenId.Asterisk || _textParser.CurrentToken.Id == TokenId.Slash ||
                _textParser.CurrentToken.Id == TokenId.Percent || TokenIdentifierIs("mod"))
            {
                var op = _textParser.CurrentToken;
                _textParser.NextToken();
                var right = ParseUnary();
                CheckAndPromoteOperands(typeof(IArithmeticSignatures), op.Id, op.Text, ref left, ref right, op.Pos);
                switch (op.Id)
                {
                    case TokenId.Asterisk:
                        left = Expression.Multiply(left, right);
                        break;
                    case TokenId.Slash:
                        left = Expression.Divide(left, right);
                        break;
                    case TokenId.Percent:
                    case TokenId.Identifier:
                        left = Expression.Modulo(left, right);
                        break;
                }
            }
            return left;
        }

        // -, !, not unary operators
        /// <summary>
        /// Parses the unary.
        /// </summary>
        /// <returns>Expression.</returns>
        Expression ParseUnary()
        {
            if (_textParser.CurrentToken.Id == TokenId.Minus || _textParser.CurrentToken.Id == TokenId.Exclamation || TokenIdentifierIs("not"))
            {
                var op = _textParser.CurrentToken;
                _textParser.NextToken();
                if (op.Id == TokenId.Minus && (_textParser.CurrentToken.Id == TokenId.IntegerLiteral || _textParser.CurrentToken.Id == TokenId.RealLiteral))
                {
                    _textParser.CurrentToken.Text = "-" + _textParser.CurrentToken.Text;
                    _textParser.CurrentToken.Pos = op.Pos;
                    return ParsePrimary();
                }

                var expr = ParseUnary();
                if (op.Id == TokenId.Minus)
                {
                    CheckAndPromoteOperand(typeof(INegationSignatures), op.Text, ref expr, op.Pos);
                    expr = Expression.Negate(expr);
                }
                else
                {
                    CheckAndPromoteOperand(typeof(INotSignatures), op.Text, ref expr, op.Pos);
                    expr = Expression.Not(expr);
                }

                return expr;
            }

            return ParsePrimary();
        }

        /// <summary>
        /// Parses the primary.
        /// </summary>
        /// <returns>Expression.</returns>
        /// <exception cref="NotSupportedException">An expression tree lambda may not contain a null propagating operator. Use the 'np()' or 'np(...)' (null-propagation) function instead.</exception>
        Expression ParsePrimary()
        {
            var expr = ParsePrimaryStart();
            while (true)
            {
                if (_textParser.CurrentToken.Id == TokenId.Dot)
                {
                    _textParser.NextToken();
                    expr = ParseMemberAccess(null, expr);
                }
                else if (_textParser.CurrentToken.Id == TokenId.NullPropagation)
                {
                    throw new NotSupportedException("An expression tree lambda may not contain a null propagating operator. Use the 'np()' or 'np(...)' (null-propagation) function instead.");
                }
                else if (_textParser.CurrentToken.Id == TokenId.OpenBracket)
                {
                    expr = ParseElementAccess(expr);
                }
                else
                {
                    break;
                }
            }
            return expr;
        }

        /// <summary>
        /// Parses the primary start.
        /// </summary>
        /// <returns>Expression.</returns>
        Expression ParsePrimaryStart()
        {
            switch (_textParser.CurrentToken.Id)
            {
                case TokenId.Identifier:
                    return ParseIdentifier();
                case TokenId.StringLiteral:
                    return ParseStringLiteral();
                case TokenId.IntegerLiteral:
                    return ParseIntegerLiteral();
                case TokenId.RealLiteral:
                    return ParseRealLiteral();
                case TokenId.OpenParen:
                    return ParseParenExpression();
                default:
                    throw ParseError(Res.ExpressionExpected);
            }
        }

        /// <summary>
        /// Parses the string literal.
        /// </summary>
        /// <returns>Expression.</returns>
        Expression ParseStringLiteral()
        {
            _textParser.ValidateToken(TokenId.StringLiteral);
            var quote = _textParser.CurrentToken.Text[0];
            var s = _textParser.CurrentToken.Text.Substring(1, _textParser.CurrentToken.Text.Length - 2);
            var index1 = 0;
            while (true)
            {
                var index2 = s.IndexOf(quote, index1);
                if (index2 < 0)
                {
                    break;
                }

                if (index2 + 1 < s.Length && s[index2 + 1] == quote)
                {
                    s = s.Remove(index2, 1);
                }
                index1 = index2 + 1;
            }

            if (quote == '\'')
            {
                if (s.Length != 1)
                {
                    throw ParseError(Res.InvalidCharacterLiteral);
                }
                _textParser.NextToken();
                return ConstantExpressionHelper.CreateLiteral(s[0], s);
            }
            _textParser.NextToken();
            return ConstantExpressionHelper.CreateLiteral(s, s);
        }

        /// <summary>
        /// Parses the integer literal.
        /// </summary>
        /// <returns>Expression.</returns>
        Expression ParseIntegerLiteral()
        {
            _textParser.ValidateToken(TokenId.IntegerLiteral);

            var text = _textParser.CurrentToken.Text;
            string qualifier = null;
            var last = text[text.Length - 1];
            var isHexadecimal = text.StartsWith(text[0] == '-' ? "-0x" : "0x", StringComparison.OrdinalIgnoreCase);
            var qualifierLetters = isHexadecimal
                                          ? new[] { 'U', 'u', 'L', 'l' }
                                          : new[] { 'U', 'u', 'L', 'l', 'F', 'f', 'D', 'd', 'M', 'm' };

            if (qualifierLetters.Contains(last))
            {
                int pos = text.Length - 1, count = 0;
                while (qualifierLetters.Contains(text[pos]))
                {
                    ++count;
                    --pos;
                }
                qualifier = text.Substring(text.Length - count, count);
                text = text.Substring(0, text.Length - count);
            }

            if (text[0] != '-')
            {
                if (isHexadecimal)
                {
                    text = text.Substring(2);
                }

                if (!ulong.TryParse(text, isHexadecimal ? NumberStyles.HexNumber : NumberStyles.Integer, _parsingConfig.NumberParseCulture, out var value))
                {
                    throw ParseError(Res.InvalidIntegerLiteral, text);
                }

                _textParser.NextToken();
                if (!string.IsNullOrEmpty(qualifier))
                {
                    if (qualifier == "U" || qualifier == "u") return ConstantExpressionHelper.CreateLiteral((uint)value, text);
                    if (qualifier == "L" || qualifier == "l") return ConstantExpressionHelper.CreateLiteral((long)value, text);

                    // in case of UL, just return
                    return ConstantExpressionHelper.CreateLiteral(value, text);
                }

                // if (value <= (int)short.MaxValue) return ConstantExpressionHelper.CreateLiteral((short)value, text);
                if (value <= int.MaxValue) return ConstantExpressionHelper.CreateLiteral((int)value, text);
                if (value <= uint.MaxValue) return ConstantExpressionHelper.CreateLiteral((uint)value, text);
                if (value <= long.MaxValue) return ConstantExpressionHelper.CreateLiteral((long)value, text);

                return ConstantExpressionHelper.CreateLiteral(value, text);
            }
            else
            {
                if (isHexadecimal)
                {
                    text = text.Substring(3);
                }

                if (!long.TryParse(text, isHexadecimal ? NumberStyles.HexNumber : NumberStyles.Integer, _parsingConfig.NumberParseCulture, out var value))
                {
                    throw ParseError(Res.InvalidIntegerLiteral, text);
                }

                if (isHexadecimal)
                {
                    value = -value;
                }

                _textParser.NextToken();
                if (!string.IsNullOrEmpty(qualifier))
                {
                    if (qualifier == "L" || qualifier == "l")
                        return ConstantExpressionHelper.CreateLiteral(value, text);

                    if (qualifier == "F" || qualifier == "f")
                        return TryParseAsFloat(text, qualifier[0]);

                    if (qualifier == "D" || qualifier == "d")
                        return TryParseAsDouble(text, qualifier[0]);

                    if (qualifier == "M" || qualifier == "m")
                        return TryParseAsDecimal(text, qualifier[0]);

                    throw ParseError(Res.MinusCannotBeAppliedToUnsignedInteger);
                }

                if (value <= int.MaxValue)
                {
                    return ConstantExpressionHelper.CreateLiteral((int)value, text);
                }

                return ConstantExpressionHelper.CreateLiteral(value, text);
            }
        }

        /// <summary>
        /// Parses the real literal.
        /// </summary>
        /// <returns>Expression.</returns>
        Expression ParseRealLiteral()
        {
            _textParser.ValidateToken(TokenId.RealLiteral);

            var text = _textParser.CurrentToken.Text;
            var qualifier = text[text.Length - 1];

            _textParser.NextToken();
            return TryParseAsFloat(text, qualifier);
        }

        /// <summary>
        /// Tries the parse as float.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="qualifier">The qualifier.</param>
        /// <returns>Expression.</returns>
        Expression TryParseAsFloat(string text, char qualifier)
        {
            if (qualifier == 'F' || qualifier == 'f')
            {
                if (float.TryParse(text.Substring(0, text.Length - 1), NumberStyles.Float, _parsingConfig.NumberParseCulture, out var f))
                {
                    return ConstantExpressionHelper.CreateLiteral(f, text);
                }
            }

            // not possible to find float qualifier, so try to parse as double
            return TryParseAsDecimal(text, qualifier);
        }

        /// <summary>
        /// Tries the parse as decimal.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="qualifier">The qualifier.</param>
        /// <returns>Expression.</returns>
        Expression TryParseAsDecimal(string text, char qualifier)
        {
            if (qualifier == 'M' || qualifier == 'm')
            {
                if (decimal.TryParse(text.Substring(0, text.Length - 1), NumberStyles.Number, _parsingConfig.NumberParseCulture, out var d))
                {
                    return ConstantExpressionHelper.CreateLiteral(d, text);
                }
            }

            // not possible to find float qualifier, so try to parse as double
            return TryParseAsDouble(text, qualifier);
        }

        /// <summary>
        /// Tries the parse as double.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="qualifier">The qualifier.</param>
        /// <returns>Expression.</returns>
        Expression TryParseAsDouble(string text, char qualifier)
        {
            double d;
            if (qualifier == 'D' || qualifier == 'd')
            {
                if (double.TryParse(text.Substring(0, text.Length - 1), NumberStyles.Number, _parsingConfig.NumberParseCulture, out d))
                {
                    return ConstantExpressionHelper.CreateLiteral(d, text);
                }
            }

            if (double.TryParse(text, NumberStyles.Number, _parsingConfig.NumberParseCulture, out d))
            {
                return ConstantExpressionHelper.CreateLiteral(d, text);
            }

            throw ParseError(Res.InvalidRealLiteral, text);
        }

        /// <summary>
        /// Parses the paren expression.
        /// </summary>
        /// <returns>Expression.</returns>
        Expression ParseParenExpression()
        {
            _textParser.ValidateToken(TokenId.OpenParen, Res.OpenParenExpected);
            _textParser.NextToken();
            var e = ParseConditionalOperator();
            _textParser.ValidateToken(TokenId.CloseParen, Res.CloseParenOrOperatorExpected);
            _textParser.NextToken();
            return e;
        }

        /// <summary>
        /// Parses the identifier.
        /// </summary>
        /// <returns>Expression.</returns>
        Expression ParseIdentifier()
        {
            _textParser.ValidateToken(TokenId.Identifier);

            if (_keywordsHelper.TryGetValue(_textParser.CurrentToken.Text, out var value))
            {
                var typeValue = value as Type;
                if (typeValue != null)
                {
                    return ParseTypeAccess(typeValue);
                }

                switch (value)
                {
                    case KeywordsHelper.KEYWORD_IT:
                    case KeywordsHelper.SYMBOL_IT:
                        return ParseIt();

                    case KeywordsHelper.KEYWORD_PARENT:
                    case KeywordsHelper.SYMBOL_PARENT:
                        return ParseParent();

                    case KeywordsHelper.KEYWORD_ROOT:
                    case KeywordsHelper.SYMBOL_ROOT:
                        return ParseRoot();

                    case KeywordsHelper.FUNCTION_IIF:
                        return ParseFunctionIif();

                    case KeywordsHelper.FUNCTION_ISNULL:
                        return ParseFunctionIsNull();

                    case KeywordsHelper.FUNCTION_NEW:
                        return ParseNew();

                    case KeywordsHelper.FUNCTION_NULLPROPAGATION:
                        return ParseFunctionNullPropagation();

                    case KeywordsHelper.FUNCTION_IS:
                        return ParseFunctionIs();

                    case KeywordsHelper.FUNCTION_AS:
                        return ParseFunctionAs();

                    case KeywordsHelper.FUNCTION_CAST:
                        return ParseFunctionCast();
                }

                _textParser.NextToken();

                return (Expression)value;
            }

            if (_symbols.TryGetValue(_textParser.CurrentToken.Text, out value) ||
                _externals != null && _externals.TryGetValue(_textParser.CurrentToken.Text, out value) ||
                _internals.TryGetValue(_textParser.CurrentToken.Text, out value))
            {
                if (!(value is Expression expr))
                {
                    expr = Expression.Constant(value);
                }
                else
                {
                    if (expr is LambdaExpression lambda)
                    {
                        return ParseLambdaInvocation(lambda);
                    }
                }

                _textParser.NextToken();

                return expr;
            }

            if (_it != null)
            {
                return ParseMemberAccess(null, _it);
            }

            throw ParseError(Res.UnknownIdentifier, _textParser.CurrentToken.Text);
        }

        /// <summary>
        /// Parses it.
        /// </summary>
        /// <returns>Expression.</returns>
        Expression ParseIt()
        {
            if (_it == null)
            {
                throw ParseError(Res.NoItInScope);
            }
            _textParser.NextToken();
            return _it;
        }

        /// <summary>
        /// Parses the parent.
        /// </summary>
        /// <returns>Expression.</returns>
        Expression ParseParent()
        {
            if (_parent == null)
            {
                throw ParseError(Res.NoParentInScope);
            }
            _textParser.NextToken();
            return _parent;
        }

        /// <summary>
        /// Parses the root.
        /// </summary>
        /// <returns>Expression.</returns>
        Expression ParseRoot()
        {
            if (_root == null)
            {
                throw ParseError(Res.NoRootInScope);
            }
            _textParser.NextToken();
            return _root;
        }

        // isnull(a,b) function
        /// <summary>
        /// Parses the function is null.
        /// </summary>
        /// <returns>Expression.</returns>
        Expression ParseFunctionIsNull()
        {
            var errorPos = _textParser.CurrentToken.Pos;
            _textParser.NextToken();
            var args = ParseArgumentList();
            if (args.Length != 2)
            {
                throw ParseError(errorPos, Res.IsNullRequiresTwoArgs);
            }

            return Expression.Coalesce(args[0], args[1]);
        }

        // iif(test, ifTrue, ifFalse) function
        /// <summary>
        /// Parses the function iif.
        /// </summary>
        /// <returns>Expression.</returns>
        Expression ParseFunctionIif()
        {
            var errorPos = _textParser.CurrentToken.Pos;
            _textParser.NextToken();

            var args = ParseArgumentList();
            if (args.Length != 3)
            {
                throw ParseError(errorPos, Res.IifRequiresThreeArgs);
            }

            return GenerateConditional(args[0], args[1], args[2], errorPos);
        }

        // np(...) function
        /// <summary>
        /// Parses the function null propagation.
        /// </summary>
        /// <returns>Expression.</returns>
        Expression ParseFunctionNullPropagation()
        {
            var errorPos = _textParser.CurrentToken.Pos;
            _textParser.NextToken();

            var args = ParseArgumentList();

            if (args.Length != 1 && args.Length != 2)
            {
                throw ParseError(errorPos, Res.NullPropagationRequiresCorrectArgs);
            }

            if (args[0] is MemberExpression memberExpression)
            {
                var hasDefaultParameter = args.Length == 2;
                var expressionIfFalse = hasDefaultParameter ? args[1] : Expression.Constant(null);

                if (_expressionHelper.TryGenerateAndAlsoNotNullExpression(memberExpression, hasDefaultParameter, out var generatedExpression))
                {
                    return GenerateConditional(generatedExpression, memberExpression, expressionIfFalse, errorPos);
                }

                return memberExpression;
            }

            throw ParseError(errorPos, Res.NullPropagationRequiresMemberExpression);
        }

        // Is(...) function
        /// <summary>
        /// Parses the function is.
        /// </summary>
        /// <returns>Expression.</returns>
        Expression ParseFunctionIs()
        {
            var errorPos = _textParser.CurrentToken.Pos;
            var functionName = _textParser.CurrentToken.Text;
            _textParser.NextToken();

            var args = ParseArgumentList();

            if (args.Length != 1)
            {
                throw ParseError(errorPos, Res.FunctionRequiresOneArg, functionName);
            }

            var resolvedType = ResolveTypeFromArgumentExpression(functionName, args[0]);

            return Expression.TypeIs(_it, resolvedType);
        }

        // As(...) function
        /// <summary>
        /// Parses the function as.
        /// </summary>
        /// <returns>Expression.</returns>
        Expression ParseFunctionAs()
        {
            var errorPos = _textParser.CurrentToken.Pos;
            var functionName = _textParser.CurrentToken.Text;
            _textParser.NextToken();

            var args = ParseArgumentList();

            if (args.Length != 1)
            {
                throw ParseError(errorPos, Res.FunctionRequiresOneArg, functionName);
            }

            var resolvedType = ResolveTypeFromArgumentExpression(functionName, args[0]);

            return Expression.TypeAs(_it, resolvedType);
        }

        // Cast(...) function
        /// <summary>
        /// Parses the function cast.
        /// </summary>
        /// <returns>Expression.</returns>
        Expression ParseFunctionCast()
        {
            var errorPos = _textParser.CurrentToken.Pos;
            var functionName = _textParser.CurrentToken.Text;
            _textParser.NextToken();

            var args = ParseArgumentList();

            if (args.Length != 1)
            {
                throw ParseError(errorPos, Res.FunctionRequiresOneArg, functionName);
            }

            var resolvedType = ResolveTypeFromArgumentExpression(functionName, args[0]);

            return Expression.ConvertChecked(_it, resolvedType);
        }

        /// <summary>
        /// Generates the conditional.
        /// </summary>
        /// <param name="test">The test.</param>
        /// <param name="expressionIfTrue">The expression if true.</param>
        /// <param name="expressionIfFalse">The expression if false.</param>
        /// <param name="errorPos">The error position.</param>
        /// <returns>Expression.</returns>
        Expression GenerateConditional(Expression test, Expression expressionIfTrue, Expression expressionIfFalse, int errorPos)
        {
            if (test.Type != typeof(bool))
            {
                throw ParseError(errorPos, Res.FirstExprMustBeBool);
            }

            if (expressionIfTrue.Type != expressionIfFalse.Type)
            {
                // If expressionIfTrue is a null constant and expressionIfFalse is ValueType:
                // - create nullable constant from expressionIfTrue with type from expressionIfFalse
                // - convert expressionIfFalse to nullable (unless it's already nullable)
                if (Constants.IsNull(expressionIfTrue) && expressionIfFalse.Type.GetTypeInfo().IsValueType)
                {
                    var nullableType = TypeHelper.ToNullableType(expressionIfFalse.Type);
                    expressionIfTrue = Expression.Constant(null, nullableType);
                    if (!TypeHelper.IsNullableType(expressionIfFalse.Type))
                    {
                        expressionIfFalse = Expression.Convert(expressionIfFalse, nullableType);
                    }

                    return Expression.Condition(test, expressionIfTrue, expressionIfFalse);
                }

                // If expressionIfFalse is a null constant and expressionIfTrue is a ValueType:
                // - create nullable constant from expressionIfFalse with type from expressionIfTrue
                // - convert expressionIfTrue to nullable (unless it's already nullable)
                if (Constants.IsNull(expressionIfFalse) && expressionIfTrue.Type.GetTypeInfo().IsValueType)
                {
                    var nullableType = TypeHelper.ToNullableType(expressionIfTrue.Type);
                    expressionIfFalse = Expression.Constant(null, nullableType);
                    if (!TypeHelper.IsNullableType(expressionIfTrue.Type))
                    {
                        expressionIfTrue = Expression.Convert(expressionIfTrue, nullableType);
                    }

                    return Expression.Condition(test, expressionIfTrue, expressionIfFalse);
                }

                var expr1As2 = !Constants.IsNull(expressionIfFalse) ? _parsingConfig.ExpressionPromoter.Promote(expressionIfTrue, expressionIfFalse.Type, true, false) : null;
                var expr2As1 = !Constants.IsNull(expressionIfTrue) ? _parsingConfig.ExpressionPromoter.Promote(expressionIfFalse, expressionIfTrue.Type, true, false) : null;
                if (expr1As2 != null && expr2As1 == null)
                {
                    expressionIfTrue = expr1As2;
                }
                else if (expr2As1 != null && expr1As2 == null)
                {
                    expressionIfFalse = expr2As1;
                }
                else
                {
                    var type1 = !Constants.IsNull(expressionIfTrue) ? expressionIfTrue.Type.Name : "null";
                    var type2 = !Constants.IsNull(expressionIfFalse) ? expressionIfFalse.Type.Name : "null";
                    if (expr1As2 != null)
                    {
                        throw ParseError(errorPos, Res.BothTypesConvertToOther, type1, type2);
                    }

                    throw ParseError(errorPos, Res.NeitherTypeConvertsToOther, type1, type2);
                }
            }

            return Expression.Condition(test, expressionIfTrue, expressionIfFalse);
        }

        // new (...) function
        /// <summary>
        /// Parses the new.
        /// </summary>
        /// <returns>Expression.</returns>
        Expression ParseNew()
        {
            _textParser.NextToken();
            if (_textParser.CurrentToken.Id != TokenId.OpenParen &&
                _textParser.CurrentToken.Id != TokenId.OpenCurlyParen &&
                _textParser.CurrentToken.Id != TokenId.OpenBracket &&
                _textParser.CurrentToken.Id != TokenId.Identifier)
            {
                throw ParseError(Res.OpenParenOrIdentifierExpected);
            }

            Type newType = null;
            if (_textParser.CurrentToken.Id == TokenId.Identifier)
            {
                var newTypeName = _textParser.CurrentToken.Text;
                _textParser.NextToken();

                while (_textParser.CurrentToken.Id == TokenId.Dot || _textParser.CurrentToken.Id == TokenId.Plus)
                {
                    var sep = _textParser.CurrentToken.Text;
                    _textParser.NextToken();
                    if (_textParser.CurrentToken.Id != TokenId.Identifier)
                    {
                        throw ParseError(Res.IdentifierExpected);
                    }
                    newTypeName += sep + _textParser.CurrentToken.Text;
                    _textParser.NextToken();
                }

                newType = _typeFinder.FindTypeByName(newTypeName, new[] { _it, _parent, _root }, false);
                if (newType == null)
                {
                    throw ParseError(_textParser.CurrentToken.Pos, Res.TypeNotFound, newTypeName);
                }

                if (_textParser.CurrentToken.Id != TokenId.OpenParen &&
                    _textParser.CurrentToken.Id != TokenId.OpenBracket &&
                    _textParser.CurrentToken.Id != TokenId.OpenCurlyParen)
                {
                    throw ParseError(Res.OpenParenExpected);
                }
            }

            var arrayInitializer = false;
            if (_textParser.CurrentToken.Id == TokenId.OpenBracket)
            {
                _textParser.NextToken();
                _textParser.ValidateToken(TokenId.CloseBracket, Res.CloseBracketExpected);
                _textParser.NextToken();
                _textParser.ValidateToken(TokenId.OpenCurlyParen, Res.OpenCurlyParenExpected);
                arrayInitializer = true;
            }

            _textParser.NextToken();

            var properties = new List<DynamicProperty>();
            var expressions = new List<Expression>();

            while (_textParser.CurrentToken.Id != TokenId.CloseParen && _textParser.CurrentToken.Id != TokenId.CloseCurlyParen)
            {
                var exprPos = _textParser.CurrentToken.Pos;
                var expr = ParseConditionalOperator();
                if (!arrayInitializer)
                {
                    string propName;
                    if (TokenIdentifierIs("as"))
                    {
                        _textParser.NextToken();
                        propName = GetIdentifier();
                        _textParser.NextToken();
                    }
                    else
                    {
                        if (!TryGetMemberName(expr, out propName))
                        {
                            throw ParseError(exprPos, Res.MissingAsClause);
                        }
                    }

                    properties.Add(new DynamicProperty(propName, expr.Type));
                }

                expressions.Add(expr);

                if (_textParser.CurrentToken.Id != TokenId.Comma)
                {
                    break;
                }

                _textParser.NextToken();
            }

            if (_textParser.CurrentToken.Id != TokenId.CloseParen && _textParser.CurrentToken.Id != TokenId.CloseCurlyParen)
            {
                throw ParseError(Res.CloseParenOrCommaExpected);
            }
            _textParser.NextToken();

            if (arrayInitializer)
            {
                return CreateArrayInitializerExpression(expressions, newType);
            }

            return CreateNewExpression(properties, expressions, newType);
        }

        /// <summary>
        /// Creates the array initializer expression.
        /// </summary>
        /// <param name="expressions">The expressions.</param>
        /// <param name="newType">The new type.</param>
        /// <returns>Expression.</returns>
        private Expression CreateArrayInitializerExpression(List<Expression> expressions, Type newType)
        {
            if (expressions.Count == 0)
            {
                return Expression.NewArrayInit(newType ?? typeof(object));
            }

            if (newType != null)
            {
                return Expression.NewArrayInit(newType, expressions.Select(expression => _parsingConfig.ExpressionPromoter.Promote(expression, newType, true, true)));
            }

            return Expression.NewArrayInit(expressions.All(expression => expression.Type == expressions[0].Type) ? expressions[0].Type : typeof(object), expressions);
        }

        /// <summary>
        /// Creates the new expression.
        /// </summary>
        /// <param name="properties">The properties.</param>
        /// <param name="expressions">The expressions.</param>
        /// <param name="newType">The new type.</param>
        /// <returns>Expression.</returns>
        private Expression CreateNewExpression(List<DynamicProperty> properties, List<Expression> expressions, Type newType)
        {
            // http://solutionizing.net/category/linq/
            var type = newType ?? _resultType;

            if (type == null)
            {
                if (_parsingConfig != null && _parsingConfig.UseDynamicObjectClassForAnonymousTypes)
                {
                    type = typeof(DynamicClass);
                    var typeForKeyValuePair = typeof(KeyValuePair<string, object>);
                    var constructorForKeyValuePair = typeForKeyValuePair.GetTypeInfo().DeclaredConstructors.First();
                    var arrayIndexParams = new List<Expression>();
                    for (var i = 0; i < expressions.Count; i++)
                    {
                        // Just convert the expression always to an object expression.
                        var boxingExpression = Expression.Convert(expressions[i], typeof(object));
                        var parameter = Expression.New(constructorForKeyValuePair, (Expression)Expression.Constant(properties[i].Name), boxingExpression);

                        arrayIndexParams.Add(parameter);
                    }

                    // Create an expression tree that represents creating and initializing a one-dimensional array of type KeyValuePair<string, object>.
                    var newArrayExpression = Expression.NewArrayInit(typeof(KeyValuePair<string, object>), arrayIndexParams);

                    // Get the "public DynamicClass(KeyValuePair<string, object>[] propertylist)" constructor
                    var constructor = type.GetTypeInfo().DeclaredConstructors.First();
                    return Expression.New(constructor, newArrayExpression);
                }

                type = DynamicClassFactory.CreateType(properties, _createParameterCtor);
            }

            IEnumerable<PropertyInfo> propertyInfos = type.GetProperties();
            if (type.GetTypeInfo().BaseType == typeof(DynamicClass))
            {
                propertyInfos = propertyInfos.Where(x => x.Name != "Item");
            }

            var propertyTypes = propertyInfos.Select(p => p.PropertyType).ToArray();
            var ctor = type.GetConstructor(propertyTypes);
            if (ctor != null && ctor.GetParameters().Length == expressions.Count)
            {
                var expressionsPromoted = new List<Expression>();

                // Loop all expressions and promote if needed
                for (var i = 0; i < propertyTypes.Length; i++)
                {
                    var propertyType = propertyTypes[i];
                    // Type expressionType = expressions[i].Type;

                    // Promote from Type to Nullable Type if needed
                    expressionsPromoted.Add(_parsingConfig.ExpressionPromoter.Promote(expressions[i], propertyType, true, true));
                }

                return Expression.New(ctor, expressionsPromoted, (IEnumerable<MemberInfo>)propertyInfos);
            }

            var bindings = new MemberBinding[properties.Count];
            for (var i = 0; i < bindings.Length; i++)
            {
                var property = type.GetProperty(properties[i].Name);
                var propertyType = property.PropertyType;
                // Type expressionType = expressions[i].Type;

                // Promote from Type to Nullable Type if needed
                bindings[i] = Expression.Bind(property, _parsingConfig.ExpressionPromoter.Promote(expressions[i], propertyType, true, true));
            }

            return Expression.MemberInit(Expression.New(type), bindings);
        }

        /// <summary>
        /// Parses the lambda invocation.
        /// </summary>
        /// <param name="lambda">The lambda.</param>
        /// <returns>Expression.</returns>
        Expression ParseLambdaInvocation(LambdaExpression lambda)
        {
            var errorPos = _textParser.CurrentToken.Pos;
            _textParser.NextToken();
            var args = ParseArgumentList();
            if (_methodFinder.FindMethod(lambda.Type, nameof(Expression.Invoke), false, args, out var _) != 1)
            {
                throw ParseError(errorPos, Res.ArgsIncompatibleWithLambda);
            }

            return Expression.Invoke(lambda, args);
        }

        /// <summary>
        /// Parses the type access.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>Expression.</returns>
        Expression ParseTypeAccess(Type type)
        {
            var errorPos = _textParser.CurrentToken.Pos;
            _textParser.NextToken();

            if (_textParser.CurrentToken.Id == TokenId.Question)
            {
                if (!type.GetTypeInfo().IsValueType || TypeHelper.IsNullableType(type))
                {
                    throw ParseError(errorPos, Res.TypeHasNoNullableForm, TypeHelper.GetTypeName(type));
                }

                type = typeof(Nullable<>).MakeGenericType(type);
                _textParser.NextToken();
            }

            // This is a shorthand for explicitly converting a string to something
            var shorthand = _textParser.CurrentToken.Id == TokenId.StringLiteral;
            if (_textParser.CurrentToken.Id == TokenId.OpenParen || shorthand)
            {
                var args = shorthand ? new[] { ParseStringLiteral() } : ParseArgumentList();

                // If only 1 argument, and if the type is a ValueType and argType is also a ValueType, just Convert
                if (args.Length == 1)
                {
                    var argType = args[0].Type;

                    if (type.GetTypeInfo().IsValueType && TypeHelper.IsNullableType(type) && argType.GetTypeInfo().IsValueType)
                    {
                        return Expression.Convert(args[0], type);
                    }
                }

                switch (_methodFinder.FindBestMethod(type.GetConstructors(), args, out var method))
                {
                    case 0:
                        if (args.Length == 1)
                        {
                            return GenerateConversion(args[0], type, errorPos);
                        }

                        throw ParseError(errorPos, Res.NoMatchingConstructor, TypeHelper.GetTypeName(type));

                    case 1:
                        return Expression.New((ConstructorInfo)method, args);

                    default:
                        throw ParseError(errorPos, Res.AmbiguousConstructorInvocation, TypeHelper.GetTypeName(type));
                }
            }

            _textParser.ValidateToken(TokenId.Dot, Res.DotOrOpenParenOrStringLiteralExpected);
            _textParser.NextToken();

            return ParseMemberAccess(type, null);
        }

        /// <summary>
        /// Generates the conversion.
        /// </summary>
        /// <param name="expr">The expr.</param>
        /// <param name="type">The type.</param>
        /// <param name="errorPos">The error position.</param>
        /// <returns>Expression.</returns>
        private Expression GenerateConversion(Expression expr, Type type, int errorPos)
        {
            var exprType = expr.Type;
            if (exprType == type)
            {
                return expr;
            }

            if (exprType.GetTypeInfo().IsValueType && type.GetTypeInfo().IsValueType)
            {
                if ((TypeHelper.IsNullableType(exprType) || TypeHelper.IsNullableType(type)) && TypeHelper.GetNonNullableType(exprType) == TypeHelper.GetNonNullableType(type))
                {
                    return Expression.Convert(expr, type);
                }

                if ((TypeHelper.IsNumericType(exprType) || TypeHelper.IsEnumType(exprType)) && TypeHelper.IsNumericType(type) || TypeHelper.IsEnumType(type))
                {
                    return Expression.ConvertChecked(expr, type);
                }
            }

            if (exprType.IsAssignableFrom(type) || type.IsAssignableFrom(exprType) || exprType.GetTypeInfo().IsInterface || type.GetTypeInfo().IsInterface)
            {
                return Expression.Convert(expr, type);
            }

            // Try to Parse the string rather than just generate the convert statement
            if (expr.NodeType == ExpressionType.Constant && exprType == typeof(string))
            {
                var text = (string)((ConstantExpression)expr).Value;

                var typeConvertor = _typeConverterFactory.GetConverter(type);
                if (typeConvertor != null)
                {
                    var value = typeConvertor.ConvertFromInvariantString(text);
                    return Expression.Constant(value, type);
                }
            }

            // Check if there are any explicit conversion operators on the source type which fit the requirement (cast to the return type).
            var explicitOperatorAvailable = exprType.GetTypeInfo().GetDeclaredMethods("op_Explicit").Any(m => m.ReturnType == type);
            if (explicitOperatorAvailable)
            {
                return Expression.Convert(expr, type);
            }

            throw ParseError(errorPos, Res.CannotConvertValue, TypeHelper.GetTypeName(exprType), TypeHelper.GetTypeName(type));
        }

        /// <summary>
        /// Parses the member access.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="instance">The instance.</param>
        /// <returns>Expression.</returns>
        Expression ParseMemberAccess(Type type, Expression instance)
        {
            if (instance != null)
            {
                type = instance.Type;
            }

            var errorPos = _textParser.CurrentToken.Pos;
            var id = GetIdentifier();
            _textParser.NextToken();

            if (_textParser.CurrentToken.Id == TokenId.OpenParen)
            {
                if (instance != null && type != typeof(string))
                {
                    var enumerableType = TypeHelper.FindGenericType(typeof(IEnumerable<>), type);
                    if (enumerableType != null)
                    {
                        var elementType = enumerableType.GetTypeInfo().GenericTypeArguments[0];
                        return ParseAggregate(instance, elementType, id, errorPos, TypeHelper.FindGenericType(typeof(IQueryable<>), type) != null);
                    }
                }

                var args = ParseArgumentList();
                switch (_methodFinder.FindMethod(type, id, instance == null, args, out var mb))
                {
                    case 0:
                        throw ParseError(errorPos, Res.NoApplicableMethod, id, TypeHelper.GetTypeName(type));

                    case 1:
                        var method = (MethodInfo)mb;
                        if (!PredefinedTypesHelper.IsPredefinedType(_parsingConfig, method.DeclaringType) && !(method.IsPublic && PredefinedTypesHelper.IsPredefinedType(_parsingConfig, method.ReturnType)))
                        {
                            throw ParseError(errorPos, Res.MethodsAreInaccessible, TypeHelper.GetTypeName(method.DeclaringType));
                        }

                        if (method.ReturnType == typeof(void))
                        {
                            throw ParseError(errorPos, Res.MethodIsVoid, id, TypeHelper.GetTypeName(method.DeclaringType));
                        }

                        return Expression.Call(instance, method, args);

                    default:
                        throw ParseError(errorPos, Res.AmbiguousMethodInvocation, id, TypeHelper.GetTypeName(type));
                }
            }

            if (type.GetTypeInfo().IsEnum)
            {
                var @enum = Enum.Parse(type, id, true);

                return Expression.Constant(@enum);
            }

            var member = FindPropertyOrField(type, id, instance == null);
            if (member is PropertyInfo property)
            {
                return Expression.Property(instance, property);
            }

            if (member is FieldInfo field)
            {
                return Expression.Field(instance, field);
            }

            if (type == typeof(object))
            {
                return Expression.Dynamic(new DynamicGetMemberBinder(id), type, instance);
            }

            if (!_parsingConfig.DisableMemberAccessToIndexAccessorFallback)
            {
                var indexerMethod = instance.Type.GetMethod("get_Item", new[] { typeof(string) });
                if (indexerMethod != null)
                {
                    return Expression.Call(instance, indexerMethod, Expression.Constant(id));
                }
            }

            if (_textParser.CurrentToken.Id == TokenId.Lambda && _it.Type == type)
            {
                // This might be an internal variable for use within a lambda expression, so store it as such
                _internals.Add(id, _it);
                var _previousItName = ItName;

                // Also store ItName (only once)
                if (string.Equals(ItName, KeywordsHelper.KEYWORD_IT))
                {
                    ItName = id;
                }

                // next
                _textParser.NextToken();

                LastLambdaItName = ItName;
                var exp = ParseConditionalOperator();

                // Restore previous context and clear internals
                _internals.Remove(id);
                ItName = _previousItName;

                return exp;
            }

            throw ParseError(errorPos, Res.UnknownPropertyOrField, id, TypeHelper.GetTypeName(type));
        }

        /// <summary>
        /// Parses the aggregate.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="elementType">Type of the element.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="errorPos">The error position.</param>
        /// <param name="isQueryable">if set to <c>true</c> [is queryable].</param>
        /// <returns>Expression.</returns>
        Expression ParseAggregate(Expression instance, Type elementType, string methodName, int errorPos, bool isQueryable)
        {
            var oldParent = _parent;

            var outerIt = _it;
            var innerIt = Expression.Parameter(elementType, string.Empty);

            _parent = _it;

            if (methodName == "Contains" || methodName == "Skip" || methodName == "Take")
            {
                // for any method that acts on the parent element type, we need to specify the outerIt as scope.
                _it = outerIt;
            }
            else
            {
                _it = innerIt;
            }

            var args = ParseArgumentList();

            _it = outerIt;
            _parent = oldParent;

            if (!_methodFinder.ContainsMethod(typeof(IEnumerableSignatures), methodName, false, args))
            {
                throw ParseError(errorPos, Res.NoApplicableAggregate, methodName, string.Join(",", args.Select(a => a.Type.Name).ToArray()));
            }

            var callType = typeof(Enumerable);
            if (isQueryable && _methodFinder.ContainsMethod(typeof(IQueryableSignatures), methodName, false, args))
            {
                callType = typeof(Queryable);
            }

            Type[] typeArgs;
            if (new[] { "OfType", "Cast" }.Contains(methodName))
            {
                if (args.Length != 1)
                {
                    throw ParseError(_textParser.CurrentToken.Pos, Res.FunctionRequiresOneArg, methodName);
                }

                typeArgs = new[] { ResolveTypeFromArgumentExpression(methodName, args[0]) };
                args = Array.Empty<Expression>();
            }
            else if (new[] { "Min", "Max", "Select", "OrderBy", "OrderByDescending", "ThenBy", "ThenByDescending", "GroupBy" }.Contains(methodName))
            {
                if (args.Length == 2)
                {
                    typeArgs = new[] { elementType, args[0].Type, args[1].Type };
                }
                else
                {
                    typeArgs = new[] { elementType, args[0].Type };
                }
            }
            else if (methodName == "SelectMany")
            {
                var type = Expression.Lambda(args[0], innerIt).Body.Type;
                var interfaces = type.GetInterfaces().Union(new[] { type });
                var interfaceType = interfaces.Single(i => i.Name == typeof(IEnumerable<>).Name);
                var resultType = interfaceType.GetTypeInfo().GenericTypeArguments[0];
                typeArgs = new[] { elementType, resultType };
            }
            else
            {
                typeArgs = new[] { elementType };
            }

            if (args.Length == 0)
            {
                args = new[] { instance };
            }
            else
            {
                if (new[] { "Contains", "Take", "Skip", "DefaultIfEmpty" }.Contains(methodName))
                {
                    args = new[] { instance, args[0] };
                }
                else
                {
                    if (args.Length == 2)
                    {
                        args = new[] { instance, Expression.Lambda(args[0], innerIt), Expression.Lambda(args[1], innerIt) };
                    }
                    else
                    {
                        args = new[] { instance, Expression.Lambda(args[0], innerIt) };
                    }
                }
            }

            return Expression.Call(callType, methodName, typeArgs, args);
        }

        /// <summary>
        /// Resolves the type from argument expression.
        /// </summary>
        /// <param name="functionName">Name of the function.</param>
        /// <param name="argumentExpression">The argument expression.</param>
        /// <returns>Type.</returns>
        private Type ResolveTypeFromArgumentExpression(string functionName, Expression argumentExpression)
        {
            var typeName = (argumentExpression as ConstantExpression)?.Value as string;
            if (string.IsNullOrEmpty(typeName))
            {
                throw ParseError(_textParser.CurrentToken.Pos, Res.FunctionRequiresOneNotNullArg, functionName, typeName);
            }

            var resultType = _typeFinder.FindTypeByName(typeName, new[] { _it, _parent, _root }, true);
            if (resultType == null)
            {
                throw ParseError(_textParser.CurrentToken.Pos, Res.TypeNotFound, typeName);
            }

            return resultType;
        }

        /// <summary>
        /// Parses the argument list.
        /// </summary>
        /// <returns>Expression[].</returns>
        Expression[] ParseArgumentList()
        {
            _textParser.ValidateToken(TokenId.OpenParen, Res.OpenParenExpected);
            _textParser.NextToken();
            var args = _textParser.CurrentToken.Id != TokenId.CloseParen ? ParseArguments() : Array.Empty<Expression>();
            _textParser.ValidateToken(TokenId.CloseParen, Res.CloseParenOrCommaExpected);
            _textParser.NextToken();
            return args;
        }

        /// <summary>
        /// Parses the arguments.
        /// </summary>
        /// <returns>Expression[].</returns>
        Expression[] ParseArguments()
        {
            var argList = new List<Expression>();
            while (true)
            {
                var argumentExpression = ParseConditionalOperator();

                _expressionHelper.WrapConstantExpression(ref argumentExpression);

                argList.Add(argumentExpression);

                if (_textParser.CurrentToken.Id != TokenId.Comma)
                {
                    break;
                }

                _textParser.NextToken();
            }

            return argList.ToArray();
        }

        /// <summary>
        /// Parses the element access.
        /// </summary>
        /// <param name="expr">The expr.</param>
        /// <returns>Expression.</returns>
        Expression ParseElementAccess(Expression expr)
        {
            var errorPos = _textParser.CurrentToken.Pos;
            _textParser.ValidateToken(TokenId.OpenBracket, Res.OpenParenExpected);
            _textParser.NextToken();

            var args = ParseArguments();
            _textParser.ValidateToken(TokenId.CloseBracket, Res.CloseBracketOrCommaExpected);
            _textParser.NextToken();

            if (expr.Type.IsArray)
            {
                if (expr.Type.GetArrayRank() != 1 || args.Length != 1)
                {
                    throw ParseError(errorPos, Res.CannotIndexMultiDimArray);
                }
                var index = _parsingConfig.ExpressionPromoter.Promote(args[0], typeof(int), true, false);

                if (index == null)
                {
                    throw ParseError(errorPos, Res.InvalidIndex);
                }

                return Expression.ArrayIndex(expr, index);
            }

            switch (_methodFinder.FindIndexer(expr.Type, args, out var mb))
            {
                case 0:
                    throw ParseError(errorPos, Res.NoApplicableIndexer,
                        TypeHelper.GetTypeName(expr.Type));
                case 1:
                    return Expression.Call(expr, (MethodInfo)mb, args);

                default:
                    throw ParseError(errorPos, Res.AmbiguousIndexerInvocation, TypeHelper.GetTypeName(expr.Type));
            }
        }

        /// <summary>
        /// Converts to nullabletype.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>Type.</returns>
        internal static Type ToNullableType(Type type)
        {
            Argument.IsNotNull(nameof(type), type);

            if (!type.GetTypeInfo().IsValueType || TypeHelper.IsNullableType(type))
            {
                throw ParseError(-1, Res.TypeHasNoNullableForm, TypeHelper.GetTypeName(type));
            }

            return typeof(Nullable<>).MakeGenericType(type);
        }

        /// <summary>
        /// Tries the name of the get member.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="memberName">Name of the member.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        static bool TryGetMemberName(Expression expression, out string memberName)
        {
            var memberExpression = expression as MemberExpression;
            if (memberExpression == null && expression.NodeType == ExpressionType.Coalesce)
            {
                memberExpression = (expression as BinaryExpression).Left as MemberExpression;
            }

            if (memberExpression != null)
            {
                memberName = memberExpression.Member.Name;
                return true;
            }

            memberName = null;
            return false;
        }

        /// <summary>
        /// Checks the and promote operand.
        /// </summary>
        /// <param name="signatures">The signatures.</param>
        /// <param name="opName">Name of the op.</param>
        /// <param name="expr">The expr.</param>
        /// <param name="errorPos">The error position.</param>
        void CheckAndPromoteOperand(Type signatures, string opName, ref Expression expr, int errorPos)
        {
            Expression[] args = { expr };

            if (!_methodFinder.ContainsMethod(signatures, "F", false, args))
            {
                throw IncompatibleOperandError(opName, expr, errorPos);
            }

            expr = args[0];
        }

        /// <summary>
        /// Gets the name of the overloaded operation.
        /// </summary>
        /// <param name="tokenId">The token identifier.</param>
        /// <returns>System.String.</returns>
        static string GetOverloadedOperationName(TokenId tokenId)
        {
            switch (tokenId)
            {
                case TokenId.DoubleEqual:
                case TokenId.Equal:
                    return "op_Equality";
                case TokenId.ExclamationEqual:
                    return "op_Inequality";
                default:
                    return null;
            }
        }

        /// <summary>
        /// Checks the and promote operands.
        /// </summary>
        /// <param name="signatures">The signatures.</param>
        /// <param name="opId">The op identifier.</param>
        /// <param name="opName">Name of the op.</param>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <param name="errorPos">The error position.</param>
        void CheckAndPromoteOperands(Type signatures, TokenId opId, string opName, ref Expression left, ref Expression right, int errorPos)
        {
            Expression[] args = { left, right };

            // support operator overloading
            var nativeOperation = GetOverloadedOperationName(opId);
            var found = false;

            if (nativeOperation != null)
            {
                // first try left operand's equality operators
                found = _methodFinder.ContainsMethod(left.Type, nativeOperation, true, args);
                if (!found)
                    found = _methodFinder.ContainsMethod(right.Type, nativeOperation, true, args);
            }

            if (!found && !_methodFinder.ContainsMethod(signatures, "F", false, args))
            {
                throw IncompatibleOperandsError(opName, left, right, errorPos);
            }

            left = args[0];
            right = args[1];
        }

        /// <summary>
        /// Incompatibles the operand error.
        /// </summary>
        /// <param name="opName">Name of the op.</param>
        /// <param name="expr">The expr.</param>
        /// <param name="errorPos">The error position.</param>
        /// <returns>Exception.</returns>
        static Exception IncompatibleOperandError(string opName, Expression expr, int errorPos)
        {
            return ParseError(errorPos, Res.IncompatibleOperand, opName, TypeHelper.GetTypeName(expr.Type));
        }

        /// <summary>
        /// Incompatibles the operands error.
        /// </summary>
        /// <param name="opName">Name of the op.</param>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <param name="errorPos">The error position.</param>
        /// <returns>Exception.</returns>
        static Exception IncompatibleOperandsError(string opName, Expression left, Expression right, int errorPos)
        {
            return ParseError(errorPos, Res.IncompatibleOperands, opName, TypeHelper.GetTypeName(left.Type), TypeHelper.GetTypeName(right.Type));
        }

        /// <summary>
        /// Finds the property or field.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="memberName">Name of the member.</param>
        /// <param name="staticAccess">if set to <c>true</c> [static access].</param>
        /// <returns>MemberInfo.</returns>
        static MemberInfo FindPropertyOrField(Type type, string memberName, bool staticAccess)
        {
            foreach (var t in TypeHelper.GetSelfAndBaseTypes(type))
            {
                // Try to find a property with the specified memberName
                MemberInfo member = t.GetTypeInfo().DeclaredProperties.FirstOrDefault(x => x.Name.ToLowerInvariant() == memberName.ToLowerInvariant());
                if (member != null)
                {
                    return member;
                }

                // If no property is found, try to get a field with the specified memberName
                member = t.GetTypeInfo().DeclaredFields.FirstOrDefault(x => (x.IsStatic || !staticAccess) && x.Name.ToLowerInvariant() == memberName.ToLowerInvariant());
                if (member != null)
                {
                    return member;
                }

                // No property or field is found, try the base type.
            }
            return null;
        }

        /// <summary>
        /// Tokens the identifier is.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool TokenIdentifierIs(string id)
        {
            return _textParser.CurrentToken.Id == TokenId.Identifier && string.Equals(id, _textParser.CurrentToken.Text, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <returns>System.String.</returns>
        string GetIdentifier()
        {
            _textParser.ValidateToken(TokenId.Identifier, Res.IdentifierExpected);
            var id = _textParser.CurrentToken.Text;
            if (id.Length > 1 && id[0] == '@')
            {
                id = id.Substring(1);
            }

            return id;
        }

        /// <summary>
        /// Parses the error.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>Exception.</returns>
        Exception ParseError(string format, params object[] args)
        {
            return ParseError(_textParser?.CurrentToken.Pos ?? 0, format, args);
        }

        /// <summary>
        /// Parses the error.
        /// </summary>
        /// <param name="pos">The position.</param>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>Exception.</returns>
        static Exception ParseError(int pos, string format, params object[] args)
        {
            return new ParseException(string.Format(CultureInfo.CurrentCulture, format, args), pos);
        }
    }
}
