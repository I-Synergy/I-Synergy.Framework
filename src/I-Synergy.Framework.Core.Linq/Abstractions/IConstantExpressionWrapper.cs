using System.Linq.Expressions;

namespace ISynergy.Framework.Core.Linq.Abstractions
{
    /// <summary>
    /// Interface IConstantExpressionWrapper
    /// </summary>
    internal interface IConstantExpressionWrapper
    {
        /// <summary>
        /// Wraps the specified expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        void Wrap(ref Expression expression);
    }
}
