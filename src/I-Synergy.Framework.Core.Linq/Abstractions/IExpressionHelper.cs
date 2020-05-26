﻿using System;
using System.Linq.Expressions;

namespace ISynergy.Framework.Core.Linq.Abstractions
{
    internal interface IExpressionHelper
    {
        void ConvertNumericTypeToBiggestCommonTypeForBinaryOperator(ref Expression left, ref Expression right);

        Expression GenerateAdd(Expression left, Expression right);

        Expression GenerateEqual(Expression left, Expression right);

        Expression GenerateGreaterThan(Expression left, Expression right);

        Expression GenerateGreaterThanEqual(Expression left, Expression right);

        Expression GenerateLessThan(Expression left, Expression right);

        Expression GenerateLessThanEqual(Expression left, Expression right);

        Expression GenerateNotEqual(Expression left, Expression right);

        Expression GenerateStringConcat(Expression left, Expression right);

        Expression GenerateSubtract(Expression left, Expression right);

        void OptimizeForEqualityIfPossible(ref Expression left, ref Expression right);

        Expression OptimizeStringForEqualityIfPossible(string text, Type type);

        bool TryGenerateAndAlsoNotNullExpression(Expression sourceExpression, bool addSelf, out Expression generatedExpression);

        void WrapConstantExpression(ref Expression argument);
    }
}
