using System.Linq.Expressions;

namespace ISynergy.Framework.Core.Linq.Parsers
{
    internal static class Constants
    {
        public static bool IsNull(Expression exp)
        {
            return exp is ConstantExpression cExp && cExp.Value == null;
        }
    }
}
