using System.Linq.Expressions;
using System.Reflection;

namespace ISynergy.Framework.Core.Linq.Extensions.Tests.Helpers
{
    /// <summary>
    /// Class ExpressionExtensions.
    /// </summary>
    public static class ExpressionExtensions
    {
        /// <summary>
        /// Converts to debugview.
        /// </summary>
        /// <param name="exp">The exp.</param>
        /// <returns>System.String.</returns>
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
