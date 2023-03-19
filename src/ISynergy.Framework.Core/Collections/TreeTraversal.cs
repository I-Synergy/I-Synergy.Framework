namespace ISynergy.Framework.Core.Collections
{
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

            var queue = new Queue<TNode>(new[] { tree.Root });

            while (queue.Count != 0)
            {
                if (queue.Dequeue() is TNode current)
                {
                    yield return current;

                    if (current.Left is TNode leftNode)
                        queue.Enqueue(leftNode);

                    if (current.Right is TNode rightNode)
                        queue.Enqueue(rightNode);
                }
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

            TNode current = tree.Root;

            while (stack.Count != 0 || current is not null)
            {
                if (current is not null)
                {
                    stack.Push(current);
                    yield return current;
                    current = current.Left;
                }
                else
                {
                    current = stack.Pop();
                    current = current.Right;
                }
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

            TNode current = tree.Root;

            while (stack.Count != 0 || current is not null)
            {
                if (current is not null)
                {
                    stack.Push(current);
                    current = current.Left;
                }
                else
                {
                    current = stack.Pop();
                    yield return current;
                    current = current.Right;
                }
            }
        }

        /// <summary>
        /// Post-order tree traversal method.
        /// </summary>
        /// <typeparam name="TNode">The type of the t node.</typeparam>
        /// <param name="tree">The tree.</param>
        /// <returns>IEnumerator&lt;TNode&gt;.</returns>
        /// <exception cref="System.InvalidOperationException"></exception>
        public static IEnumerator<TNode> PostOrder<TNode>(BinaryTree<TNode> tree)
            where TNode : BinaryNode<TNode>, new()
        {
            if (tree.Root is null)
                yield break;

            var stack = new Stack<TNode>(new[] { tree.Root });

            TNode previous = tree.Root;

            while (stack.Count != 0)
            {
                TNode current = stack.Peek();

                if (previous == current || previous.Left == current || previous.Right == current)
                {
                    if (current.Left is not null)
                        stack.Push(current.Left);
                    else if (current.Right is not null)
                        stack.Push(current.Right);
                    else
                    {
                        yield return stack.Pop();
                    }
                }
                else if (current.Left == previous)
                {
                    if (current.Right is not null)
                        stack.Push(current.Right);
                    else
                    {
                        yield return stack.Pop();
                    }
                }
                else if (current.Right == previous)
                {
                    yield return stack.Pop();
                }
                else
                {
                    throw new InvalidOperationException();
                }

                previous = current;
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

            while (stack.Count != 0)
            {
                if (stack.Pop() is TNode current)
                {
                    yield return current;

                    if (current.Left is TNode leftNode)
                        stack.Push(leftNode);

                    if (current.Right is TNode rightNode)
                        stack.Push(rightNode);
                }
            }
        }
    }
}
