using System;
using System.Collections.Generic;
using System.Text;

namespace ISynergy.Framework.Core.Collections
{
    /// <summary>
    ///   Common interface for tree nodes, such as <see cref="BinaryTree{TNode}"/> and <see cref="TreeNode{TNode}"/>.
    /// </summary>
    /// 
    /// <typeparam name="TNode">The type of the tree node.</typeparam>
    /// 
    /// <seealso cref="BinaryTree{TNode}"/>
    /// <seealso cref="TreeNode{TNode}"/>
    /// <seealso cref="RedBlackTree{T}"/>
    /// 
    public interface ITreeNode<TNode>
        where TNode : ITreeNode<TNode>
    {
        /// <summary>
        ///   Gets or sets the collection of child nodes
        ///   under this node.
        /// </summary>
        /// 
        TNode[] Children { get; set; }

        /// <summary>
        ///   Gets whether this node is a leaf (has no children).
        /// </summary>
        /// 
        bool IsLeaf { get; }
    }
}
