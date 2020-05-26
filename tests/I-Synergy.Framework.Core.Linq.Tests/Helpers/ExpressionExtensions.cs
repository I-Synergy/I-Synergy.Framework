﻿using System.Linq.Expressions;
using System.Reflection;

namespace ISynergy.Framework.Core.Linq.Extensions.Tests.Helpers
{
    public static class ExpressionExtensions
    {
        public static string ToDebugView(this Expression exp)
        {
            if (exp == null)
            {
                return null;
            }

            var propertyInfo = typeof(Expression).GetProperty("DebugView", BindingFlags.Instance | BindingFlags.NonPublic);
            return propertyInfo.GetValue(exp) as string;
        }
    }
}
