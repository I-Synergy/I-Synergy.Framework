using System;
using System.Linq.Expressions;
using System.Reflection;
using ISynergy.Framework.Core.Linq.Abstractions;
using ISynergy.Framework.Core.Linq.Helpers;

namespace ISynergy.Framework.Core.Linq.Parsers
{
    public class ExpressionPromoter : IExpressionPromoter
    {
        private readonly NumberParser _numberParser;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionPromoter"/> class.
        /// </summary>
        /// <param name="config">The ParsingConfig.</param>
        public ExpressionPromoter(ParsingConfig config)
        {
            _numberParser = new NumberParser(config);
        }

        /// <inheritdoc cref="IExpressionPromoter.Promote(Expression, Type, bool, bool)"/>
        public virtual Expression Promote(Expression expr, Type type, bool exact, bool convertExpr)
        {
            if (expr.Type == type)
            {
                return expr;
            }


            if (expr is ConstantExpression ce)
            {
                if (Constants.IsNull(ce))
                {
                    if (!type.GetTypeInfo().IsValueType || TypeHelper.IsNullableType(type))
                    {
                        return Expression.Constant(null, type);
                    }
                }
                else
                {
                    if (ConstantExpressionHelper.TryGetText(ce, out var text))
                    {
                        var target = TypeHelper.GetNonNullableType(type);
                        object value = null;

                        if (ce.Type == typeof(int) || ce.Type == typeof(uint) || ce.Type == typeof(long) || ce.Type == typeof(ulong))
                        {
                            value = _numberParser.ParseNumber(text, target);

                            // Make sure an enum value stays an enum value
                            if (target.GetTypeInfo().IsEnum)
                            {
                                value = Enum.ToObject(target, value);
                            }
                        }
                        else if (ce.Type == typeof(double))
                        {
                            if (target == typeof(decimal))
                            {
                                value = _numberParser.ParseNumber(text, target);
                            }
                        }
                        else if (ce.Type == typeof(string))
                        {
                            value = TypeHelper.ParseEnum(text, target);
                        }
                        if (value != null)
                        {
                            return Expression.Constant(value, type);
                        }
                    }
                }
            }

            if (TypeHelper.IsCompatibleWith(expr.Type, type))
            {
                if (type.GetTypeInfo().IsValueType || exact || expr.Type.GetTypeInfo().IsValueType && convertExpr)
                {
                    return Expression.Convert(expr, type);
                }

                return expr;
            }

            return null;
        }
    }
}
