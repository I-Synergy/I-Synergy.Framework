using System.Linq.Expressions;

namespace ISynergy.Framework.Core.Linq.Parsers
{
    /// <summary>
    /// Class Constants.
    /// </summary>
    internal static class Constants
    {
        /// <summary>
        /// Determines whether the specified exp is null.
        /// </summary>
        /// <param name="exp">The exp.</param>
        /// <returns><c>true</c> if the specified exp is null; otherwise, <c>false</c>.</returns>
        public static bool IsNull(Expression exp)
        {
            return exp is ConstantExpression cExp && cExp.Value == null;
        }
    }
}
