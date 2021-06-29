using ISynergy.Framework.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace ISynergy.Framework.Core.Collections
{
    /// <summary>
    ///   Possible node colors for <see cref="RedBlackTree{T}"/>s.
    /// </summary>
    /// 
    public enum RedBlackTreeNodeType
    {
        /// <summary>
        ///   Red node.
        /// </summary>
        /// 
        Red,

        /// <summary>
        ///   Black node.
        /// </summary>
        /// 
        Black
    }

    /// <summary>
    ///   <see cref="RedBlackTree{T}"/> node.
    /// </summary>
    /// 
    /// <typeparam name="T">The type of the value to be stored.</typeparam>
    /// 
    [Serializable]
    public class RedBlackTreeNode<T> : BinaryNode<RedBlackTreeNode<T>>
    {
        RedBlackTreeNode<T> parent;

        RedBlackTreeNodeType color;

        T value;

        /// <summary>
        ///   Constructs a new empty node.
        /// </summary>
        /// 
        public RedBlackTreeNode()
        {

        }

        /// <summary>
        ///   Constructs a node containing the given <param name="value"/>.
        /// </summary>
        /// 
        public RedBlackTreeNode(T value)
        {
            this.value = value;
        }

        /// <summary>
        ///   Gets or sets a reference to this node's parent node.
        /// </summary>
        /// 
        public RedBlackTreeNode<T> Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        /// <summary>
        ///   Gets or sets this node's color.
        /// </summary>
        /// 
        public RedBlackTreeNodeType Color
        {
            get { return color; }
            set { color = value; }
        }

        /// <summary>
        ///   Gets or sets the value associated with this node.
        /// </summary>
        /// 
        public T Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            if (color == RedBlackTreeNodeType.Black)
                return "Black: {0)".Format(Value);
            return "Red: {0)".Format(Value);
        }
    }

    /// <summary>
    ///   <see cref="RedBlackTree{T}"/> node.
    /// </summary>
    /// 
    /// <typeparam name="TKey">The type of the key that identifies the value.</typeparam>
    /// <typeparam name="TValue">The type of the values stored in this node.</typeparam>
    /// 
    [Serializable]
    public class RedBlackTreeNode<TKey, TValue> : RedBlackTreeNode<KeyValuePair<TKey, TValue>>
    {
        /// <summary>
        ///   Constructs a new empty node.
        /// </summary>
        /// 
        public RedBlackTreeNode()
        {

        }

        /// <summary>
        ///   Constructs a new node containing the given <param name="key">
        ///   key</param> and <param name="value">value</param> pair.
        /// </summary>
        /// 
        public RedBlackTreeNode(TKey key, TValue value)
            : base(new KeyValuePair<TKey, TValue>(key, value))
        {
        }

        /// <summary>
        ///   Constructs a new node containing the given
        ///   <param name="item">key and value pair</param>.
        /// </summary>
        /// 
        public RedBlackTreeNode(KeyValuePair<TKey, TValue> item)
            : base(item)
        {
        }
    }
}
