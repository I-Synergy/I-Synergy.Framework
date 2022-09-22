using ISynergy.Framework.Core.Abstractions.Collections;
using System;

namespace ISynergy.Framework.Core.Collections
{
    /// <summary>
    /// Base class for tree structures.
    /// </summary>
    [Serializable]
    public class Tree<T> : TreeNode<T>
        where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Tree{T}"/> class.
        /// </summary>
        public Tree() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Tree{T}"/> class.
        /// </summary>
        /// <param name="rootValue">The root value.</param>
        public Tree(T rootValue)
            : base(rootValue) { }
    }
}
