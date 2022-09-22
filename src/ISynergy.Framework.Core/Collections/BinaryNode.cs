using ISynergy.Framework.Core.Abstractions.Collections;
using System;

namespace ISynergy.Framework.Core.Collections
{
    /// <summary>
    ///   Tree node for binary trees.
    /// </summary>
    /// 
    /// <typeparam name="TNode">The class type for the nodes of the tree.</typeparam>
    /// 
    [Serializable]
    public class BinaryNode<TNode> : IEquatable<TNode>, IBinaryTreeNode<TNode> // TODO: Try to remove IEquatable
        where TNode : BinaryNode<TNode>
    {
        /// <summary>
        ///   Gets or sets the left subtree of this node.
        /// </summary>
        /// 
        public TNode Left { get; set; }

        /// <summary>
        ///   Gets or sets the right subtree of this node.
        /// </summary>
        /// 
        public TNode Right { get; set; }

        /// <summary>
        ///   Gets whether this node is a leaf (has no children).
        /// </summary>
        /// 
        public bool IsLeaf
        {
            get { return Left == default(TNode) && Right == default(TNode); }
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        /// 
        public bool Equals(TNode other)
        {
            return this == other;
        }

        /// <summary>
        ///   Gets or sets the collection of child nodes
        ///   under this node.
        /// </summary>
        /// 
        public TNode[] Children
        {
            get { return new[] { Left, Right }; }
            set
            {
                if (value.Length != 2)
                    throw new ArgumentException("The array must have length 2.");
                Left = value[0];
                Right = value[1];
            }
        }

    }
}
