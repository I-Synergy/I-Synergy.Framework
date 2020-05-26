using System;
using System.Linq.Expressions;

namespace ISynergy.Framework.Core.Linq.Abstractions
{
    interface ITypeFinder
    {
        Type FindTypeByName(string name, ParameterExpression[] expressions, bool forceUseCustomTypeProvider);
    }
}
