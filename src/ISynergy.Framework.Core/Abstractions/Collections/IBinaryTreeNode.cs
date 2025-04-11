using ISynergy.Framework.Core.Collections;

namespace ISynergy.Framework.Core.Abstractions.Collections;

/// <summary>
///   Common interface for tree nodes, such as <see cref="BinaryTree{TNode}"/> and <see cref="RedBlackTree{T}"/>.
/// </summary>
/// 
/// <typeparam name="TNode">The type of the tree node.</typeparam>
/// 
/// <seealso cref="BinaryTree{TNode}"/>
/// <seealso cref="RedBlackTree{T}"/>
/// 
public interface IBinaryTreeNode<TNode>
    where TNode : IBinaryTreeNode<TNode>
{
    /// <summary>
    ///   Gets or sets the collection of child nodes
    ///   under this node.
    /// </summary>
    /// 
    TNode?[] Children { get; set; }

    /// <summary>
    ///   Gets whether this node is a leaf (has no children).
    /// </summary>
    /// 
    bool IsLeaf { get; }
}
