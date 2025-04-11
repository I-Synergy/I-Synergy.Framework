namespace ISynergy.Framework.Core.Collections;

/// <summary>
/// Tree enumeration method delegate.
/// </summary>
/// <typeparam name="TNode">The class type for the nodes of the tree.</typeparam>
/// <param name="tree">The binary tree to be traversed.</param>
/// <returns>An enumerator traversing the tree.</returns>
public delegate IEnumerator<TNode> BinaryTraversalMethod<TNode>(BinaryTree<TNode> tree)
    where TNode : BinaryNode<TNode>;

/// <summary>
/// Static class with tree traversal methods.
/// </summary>
public static class TreeTraversal
{
    /// <summary>
    /// Breadth-first tree traversal method.
    /// </summary>
    /// <typeparam name="TNode">The type of the t node.</typeparam>
    /// <param name="tree">The tree.</param>
    /// <returns>IEnumerator&lt;TNode&gt;.</returns>
    public static IEnumerator<TNode> BreadthFirst<TNode>(BinaryTree<TNode> tree)
        where TNode : BinaryNode<TNode>, new()
    {
        if (tree.Root is null)
            yield break;

        var queue = new Queue<TNode>();
        queue.Enqueue(tree.Root);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            yield return current;

            if (current.Left is not null)
                queue.Enqueue(current.Left);

            if (current.Right is not null)
                queue.Enqueue(current.Right);
        }
    }

    /// <summary>
    /// Pre-order tree traversal method.
    /// </summary>
    /// <typeparam name="TNode">The type of the t node.</typeparam>
    /// <param name="tree">The tree.</param>
    /// <returns>IEnumerator&lt;TNode&gt;.</returns>
    public static IEnumerator<TNode> PreOrder<TNode>(BinaryTree<TNode> tree)
        where TNode : BinaryNode<TNode>, new()
    {
        if (tree.Root is null)
            yield break;

        var stack = new Stack<TNode>();
        stack.Push(tree.Root);

        while (stack.Count > 0)
        {
            var current = stack.Pop();
            yield return current;

            // Push right first so that left is processed first (LIFO stack)
            if (current.Right is not null)
                stack.Push(current.Right);

            if (current.Left is not null)
                stack.Push(current.Left);
        }
    }

    /// <summary>
    /// In-order tree traversal method.
    /// </summary>
    /// <typeparam name="TNode">The type of the t node.</typeparam>
    /// <param name="tree">The tree.</param>
    /// <returns>IEnumerator&lt;TNode&gt;.</returns>
    public static IEnumerator<TNode> InOrder<TNode>(BinaryTree<TNode> tree)
        where TNode : BinaryNode<TNode>, new()
    {
        if (tree.Root is null)
            yield break;

        var stack = new Stack<TNode>();
        TNode? current = tree.Root;

        while (stack.Count > 0 || current is not null)
        {
            // Traverse to leftmost node
            while (current is not null)
            {
                stack.Push(current);
                current = current.Left;
            }

            // Visit node
            current = stack.Pop();
            yield return current;

            // Move to right subtree
            current = current.Right;
        }
    }

    /// <summary>
    /// Post-order tree traversal method.
    /// </summary>
    /// <typeparam name="TNode">The type of the t node.</typeparam>
    /// <param name="tree">The tree.</param>
    /// <returns>IEnumerator&lt;TNode&gt;.</returns>
    public static IEnumerator<TNode> PostOrder<TNode>(BinaryTree<TNode> tree)
        where TNode : BinaryNode<TNode>, new()
    {
        if (tree.Root is null)
            yield break;

        var stack1 = new Stack<TNode>();
        var stack2 = new Stack<TNode>();

        stack1.Push(tree.Root);

        // First, we'll collect nodes in reverse post-order (root, right, left)
        while (stack1.Count > 0)
        {
            var current = stack1.Pop();
            stack2.Push(current);

            if (current.Left is not null)
                stack1.Push(current.Left);

            if (current.Right is not null)
                stack1.Push(current.Right);
        }

        // Then yield them in correct post-order (left, right, root)
        while (stack2.Count > 0)
        {
            yield return stack2.Pop();
        }
    }

    /// <summary>
    /// Depth-first tree traversal method.
    /// </summary>
    /// <typeparam name="TNode">The type of the t node.</typeparam>
    /// <param name="tree">The tree.</param>
    /// <returns>IEnumerator&lt;TNode&gt;.</returns>
    public static IEnumerator<TNode> DepthFirst<TNode>(BinaryTree<TNode> tree)
        where TNode : BinaryNode<TNode>
    {
        if (tree.Root is null)
            yield break;

        var stack = new Stack<TNode>();
        stack.Push(tree.Root);

        while (stack.Count > 0)
        {
            var current = stack.Pop();
            yield return current;

            // Push left first so that right is processed first (LIFO stack)
            if (current.Left is not null)
                stack.Push(current.Left);

            if (current.Right is not null)
                stack.Push(current.Right);
        }
    }
}
