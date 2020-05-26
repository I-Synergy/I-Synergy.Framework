using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;

namespace ISynergy.Framework.Core.Linq.Binders
{
    /// <summary>
    /// Based on From SqlLinq by dkackman. https://github.com/dkackman/SqlLinq/blob/210b594e37f14061424397368ed750ce547c21e7/License.md
    /// </summary>
    /// <seealso cref="GetMemberBinder" />
    internal class DynamicGetMemberBinder : GetMemberBinder
    {
        /// <summary>
        /// The indexer
        /// </summary>
        private static readonly PropertyInfo Indexer = typeof(IDictionary<string, object>).GetProperty("Item");

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicGetMemberBinder"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public DynamicGetMemberBinder(string name)
            : base(name, true)
        {
        }

        /// <summary>
        /// Fallbacks the get member.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="errorSuggestion">The error suggestion.</param>
        /// <returns>DynamicMetaObject.</returns>
        /// <exception cref="InvalidOperationException">Target object is not an ExpandoObject</exception>
        public override DynamicMetaObject FallbackGetMember(DynamicMetaObject target, DynamicMetaObject errorSuggestion)
        {
            if (!(target.Value is IDictionary<string, object> dictionary))
            {
                throw new InvalidOperationException("Target object is not an ExpandoObject");
            }

            return DynamicMetaObject.Create(dictionary, Expression.MakeIndex(Expression.Constant(dictionary), Indexer, new Expression[] { Expression.Constant(Name) }));
        }
    }
}
