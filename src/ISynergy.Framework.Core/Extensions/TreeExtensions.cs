using ISynergy.Framework.Core.Collections;
using ISynergy.Framework.Core.Validation;

namespace ISynergy.Framework.Core.Extensions;

/// <summary>
/// Class TreeExtensions.
/// </summary>
public static class TreeExtensions
{
    /// <summary>
    /// Gets the root node.
    /// </summary>
    /// <typeparam name="TKey">The type of the t key.</typeparam>
    /// <typeparam name="TModel">The type of the t model.</typeparam>
    /// <param name="node">The node.</param>
    /// <returns>TreeNode&lt;TKey, TModel&gt;.</returns>
    public static TreeNode<TKey, TModel> GetRootNode<TKey, TModel>(this TreeNode<TKey, TModel> node)
        where TKey : struct
        where TModel : class
    {
        Argument.IsNotNull(node);

        if (node.Parent is null)
            return node;

        return node.Parent.GetRootNode();
    }

    /// <summary>
    /// Finds the node.
    /// </summary>
    /// <typeparam name="TKey">The type of the t key.</typeparam>
    /// <typeparam name="TModel">The type of the t model.</typeparam>
    /// <param name="node">The node.</param>
    /// <param name="key">The key.</param>
    /// <returns>TreeNode&lt;TKey, TModel&gt;.</returns>
    public static TreeNode<TKey, TModel> FindNode<TKey, TModel>(this TreeNode<TKey, TModel> node, TKey key)
        where TKey : struct
        where TModel : class
    {
        Argument.IsNotNull(node);

        return node.FlattenAll().SingleOrDefault(q => q.Key.Equals(key));
    }

    /// <summary>
    /// Flattens all.
    /// </summary>
    /// <typeparam name="TKey">The type of the t key.</typeparam>
    /// <typeparam name="TModel">The type of the t model.</typeparam>
    /// <param name="node">The node.</param>
    /// <returns>IEnumerable&lt;TreeNode&lt;TKey, TModel&gt;&gt;.</returns>
    public static IEnumerable<TreeNode<TKey, TModel>> FlattenAll<TKey, TModel>(this TreeNode<TKey, TModel> node)
        where TKey : struct
        where TModel : class
    {
        Argument.IsNotNull(node);

        var parent = node.GetRootNode();
        return parent.Flatten();
    }

    /// <summary>
    /// Flattens the specified node.
    /// </summary>
    /// <typeparam name="TKey">The type of the t identifier.</typeparam>
    /// <typeparam name="TModel">The type of the t model.</typeparam>
    /// <param name="node">The node.</param>
    /// <returns>IEnumerable&lt;ITreeNode&gt;.</returns>
    public static IEnumerable<TreeNode<TKey, TModel>> Flatten<TKey, TModel>(this TreeNode<TKey, TModel> node)
        where TKey : struct
        where TModel : class
    {
        yield return node;

        var flattened = node.Children
                            .SelectMany(child => Flatten(child));

        foreach (var flattenedNode in flattened.EnsureNotNull())
        {
            yield return flattenedNode;
        }
    }

    /// <summary>
    /// Flattens to list.
    /// </summary>
    /// <typeparam name="TKey">The type of the t identifier.</typeparam>
    /// <typeparam name="TModel">The type of the t model.</typeparam>
    /// <param name="node">The tree.</param>
    /// <returns>IEnumerable&lt;ITreeNode&gt;.</returns>
    public static IEnumerable<TreeNode<TKey, TModel>> FlattenList<TKey, TModel>(this TreeNode<TKey, TModel> node)
        where TKey : struct
        where TModel : class =>
        node.Flatten().ToList();

    /// <summary>
    /// Flattens the values to list.
    /// </summary>
    /// <typeparam name="TKey">The type of the t identifier.</typeparam>
    /// <typeparam name="TModel">The type of the t model.</typeparam>
    /// <param name="node">The tree.</param>
    /// <returns>IEnumerable&lt;System.Object&gt;.</returns>
    public static IEnumerable<TModel> FlattenDataList<TKey, TModel>(this TreeNode<TKey, TModel> node)
        where TKey : struct
        where TModel : class =>
        node.Flatten().Select(s => s.Data).ToList();


    /// <summary>
    /// Converts to tree.
    /// </summary>
    /// <typeparam name="TKey">The type of the t identifier.</typeparam>
    /// <typeparam name="TModel">The type of the t model.</typeparam>
    /// <param name="nodes">The data.</param>
    /// <returns>TreeNode&lt;T&gt;.</returns>
    public static TreeNode<TKey, TModel> ToTree<TKey, TModel>(this IEnumerable<TreeNode<TKey, TModel>> nodes)
        where TKey : struct
        where TModel : class
    {
        var array = nodes.ToArray();

        if (array.Length > 0)
        {
            var tree = array[0] as Tree<TKey, TModel>;
            var parents = new Stack<TreeNode<TKey, TModel>>();
            parents.Push(tree);

            for (int i = 1; i < nodes.Count() - 1; i++)
            {
                while (array[i].FindNode(array[i].ParentKey) != parents.Peek())
                {
                    parents.Pop();
                }

                parents.Peek().Children.Add(array[i]);
                parents.Push(array[i]);
            }

            return tree;
        }
        return null;
    }
    /// <summary>
    /// Gets the node by identifier.
    /// </summary>
    /// <typeparam name="TKey">The type of the t identifier.</typeparam>
    /// <typeparam name="TModel">The type of the t model.</typeparam>
    /// <param name="nodes">The nodes.</param>
    /// <param name="key">The identifier.</param>
    /// <returns>TreeNode&lt;TKey, TModel&gt;.</returns>
    public static TreeNode<TKey, TModel> GetNodeById<TKey, TModel>(this IEnumerable<TreeNode<TKey, TModel>> nodes, TKey key)
        where TKey : struct
        where TModel : class
    {
        foreach (var node in nodes.EnsureNotNull())
        {
            if (node.IsSelected && node.Key.Equals(key))
                return node;

            var foundChild = node.Children.GetNodeById(key);

            if (foundChild != null)
                return foundChild;
        }

        return null;
    }

    /// <summary>
    /// Gets the selected node.
    /// </summary>
    /// <typeparam name="TKey">The type of the t identifier.</typeparam>
    /// <typeparam name="TModel">The type of the t model.</typeparam>
    /// <param name="nodes">The nodes.</param>
    /// <returns>TreeNode&lt;TKey, TModel&gt;.</returns>
    public static TreeNode<TKey, TModel> GetSelectedNode<TKey, TModel>(this IEnumerable<TreeNode<TKey, TModel>> nodes)
        where TKey : struct
        where TModel : class
    {
        foreach (var node in nodes.EnsureNotNull())
        {
            if (node.IsSelected)
                return node;

            var selectedChild = node.Children.GetSelectedNode();

            if (selectedChild != null)
                return selectedChild;
        }

        return null;
    }
}
