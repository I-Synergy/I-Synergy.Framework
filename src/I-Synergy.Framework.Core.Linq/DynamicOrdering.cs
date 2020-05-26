using System.Linq.Expressions;

namespace ISynergy.Framework.Core.Linq
{
    /// <summary>
    /// Class DynamicOrdering.
    /// </summary>
    internal class DynamicOrdering
    {
        /// <summary>
        /// The selector
        /// </summary>
        public Expression Selector;
        /// <summary>
        /// The ascending
        /// </summary>
        public bool Ascending;
        /// <summary>
        /// The method name
        /// </summary>
        public string MethodName;
    }
}
