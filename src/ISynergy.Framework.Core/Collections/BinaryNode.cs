using ISynergy.Framework.Core.Abstractions.Collections;

namespace ISynergy.Framework.Core.Collections;

/// <summary>
///   Tree node for binary trees.
/// </summary>
/// 
/// <typeparam name="TNode">The class type for the nodes of the tree.</typeparam>
/// 
[Serializable]
// S4035: this is a public base class that cannot be sealed without breaking derived node types such as
// RedBlackTreeNode<T>; the equality members are inherited by those extensible node types by design.
#pragma warning disable S4035 // public base class designed for inheritance
public class BinaryNode<TNode> : IEquatable<TNode>, IBinaryTreeNode<TNode>
    where TNode : BinaryNode<TNode>
{
    /// <summary>
    ///   Gets or sets the left subtree of this node.
    /// </summary>
    /// 
    public TNode? Left { get; set; }

    /// <summary>
    ///   Gets or sets the right subtree of this node.
    /// </summary>
    /// 
    public TNode? Right { get; set; }

    /// <summary>
    ///   Gets whether this node is a leaf (has no children).
    /// </summary>
    /// 
    public bool IsLeaf
    {
        get { return Left == null && Right == null; }
    }

    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <param name="other">An object to compare with this object.</param>
    /// <returns>
    /// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
    /// </returns>
    /// 
    public bool Equals(TNode? other)
    {
        return this == other;
    }

    /// <summary>
    ///   Gets or sets the collection of child nodes
    ///   under this node.
    /// </summary>
    /// 
    public TNode?[] Children
    {
        get { return [Left, Right]; }
        set
        {
            if (value.Length != 2)
                throw new ArgumentException("The array must have length 2.");
            Left = value[0];
            Right = value[1];
        }
    }

}
#pragma warning restore S4035
