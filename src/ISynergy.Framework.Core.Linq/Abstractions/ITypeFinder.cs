using System;
using System.Linq.Expressions;

namespace ISynergy.Framework.Core.Linq.Abstractions
{
    /// <summary>
    /// Interface ITypeFinder
    /// </summary>
    interface ITypeFinder
    {
        /// <summary>
        /// Finds the name of the type by.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="expressions">The expressions.</param>
        /// <param name="forceUseCustomTypeProvider">if set to <c>true</c> [force use custom type provider].</param>
        /// <returns>Type.</returns>
        Type FindTypeByName(string name, ParameterExpression[] expressions, bool forceUseCustomTypeProvider);
    }
}
