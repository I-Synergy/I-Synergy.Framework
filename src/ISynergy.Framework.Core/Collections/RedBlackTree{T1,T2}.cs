using System;
using System.Collections.Generic;
using System.Text;

namespace ISynergy.Framework.Core.Collections
{
    /// <summary>
    ///   Red-black tree specialized for key-based value retrieval.
    /// </summary>
    /// 
    /// <remarks>
    ///   See <see cref="RedBlackTree{T}"/>.
    /// </remarks>
    /// 
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// 
    [Serializable]
    public class RedBlackTree<TKey, TValue> : RedBlackTree<KeyValuePair<TKey, TValue>>
    {

        /// <summary>
        ///   Constructs a new <see cref="RedBlackTree&lt;T&gt;"/> using the default
        ///   <see cref="IComparer{T}"/> for the key type <typeparamref name="TKey"/>.
        /// </summary>
        /// 
        public RedBlackTree()
            : base(KeyValuePairComparer<TKey, TValue>.Default)
        {
        }

        /// <summary>
        ///   Constructs a new <see cref="RedBlackTree&lt;T&gt;"/> using 
        ///   the provided <see cref="IComparer{T}"/> implementation.
        /// </summary>
        /// 
        /// <param name="comparer">
        ///   The element comparer to be used to order elements in the tree.</param>
        /// 
        public RedBlackTree(IComparer<KeyValuePair<TKey, TValue>> comparer)
            : base(comparer)
        {
        }

        /// <summary>
        ///   Constructs a new <see cref="RedBlackTree&lt;T&gt;"/> using the default
        ///   <see cref="IComparer{T}"/> for the key type <typeparamref name="TKey"/>.
        /// </summary>
        /// 
        /// <param name="allowDuplicates">
        ///   Pass <c>true</c> to allow duplicate elements 
        ///   in the tree; <c>false</c> otherwise.</param>
        /// 
        public RedBlackTree(bool allowDuplicates)
            : base(KeyValuePairComparer<TKey, TValue>.Default, allowDuplicates)
        {
        }

        /// <summary>
        ///   Constructs a new <see cref="RedBlackTree&lt;T&gt;"/> using 
        ///   the provided <see cref="IComparer{T}"/> implementation.
        /// </summary>
        /// 
        /// <param name="comparer">
        ///   The element comparer to be used to order elements in the tree.</param>
        /// <param name="allowDuplicates">
        ///   Pass <c>true</c> to allow duplicate elements 
        ///   in the tree; <c>false</c> otherwise.</param>
        /// 
        public RedBlackTree(IComparer<KeyValuePair<TKey, TValue>> comparer, bool allowDuplicates)
            : base(comparer, allowDuplicates)
        {
        }

    }
}
