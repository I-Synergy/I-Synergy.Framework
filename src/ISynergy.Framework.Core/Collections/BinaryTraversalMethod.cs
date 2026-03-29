namespace ISynergy.Framework.Core.Collections;

/// <summary>
/// Tree enumeration method delegate.
/// </summary>
/// <typeparam name="TNode">The class type for the nodes of the tree.</typeparam>
/// <param name="tree">The binary tree to be traversed.</param>
/// <returns>An enumerator traversing the tree.</returns>
public delegate IEnumerator<TNode> BinaryTraversalMethod<TNode>(BinaryTree<TNode> tree)
    where TNode : BinaryNode<TNode>;
