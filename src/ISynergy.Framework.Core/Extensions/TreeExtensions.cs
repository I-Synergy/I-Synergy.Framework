using ISynergy.Framework.Core.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ISynergy.Framework.Core.Extensions
{
    /// <summary>
    /// Class TreeExtensions.
    /// </summary>
    public static class TreeExtensions
    {
        /// <summary>
        /// Flattens the specified node.
        /// </summary>
        /// <typeparam name="TKey">The type of the t identifier.</typeparam>
        /// <typeparam name="TModel">The type of the t model.</typeparam>
        /// <param name="tree">The node.</param>
        /// <returns>IEnumerable&lt;ITreeNode&gt;.</returns>
        public static IEnumerable<TreeNode<TKey, TModel>> Flatten<TKey, TModel>(this TreeNode<TKey, TModel> tree)
            where TKey : struct
            where TModel : class
        {
            yield return tree;

            var flattened = tree.Children
                                .SelectMany(child => Flatten(child));

            foreach (var flattenedNode in flattened)
            {
                yield return flattenedNode;
            }
        }

        /// <summary>
        /// Flattens to list.
        /// </summary>
        /// <typeparam name="TKey">The type of the t identifier.</typeparam>
        /// <typeparam name="TModel">The type of the t model.</typeparam>
        /// <param name="tree">The tree.</param>
        /// <returns>IEnumerable&lt;ITreeNode&gt;.</returns>
        public static IEnumerable<TreeNode<TKey, TModel>> FlattenList<TKey, TModel>(this TreeNode<TKey, TModel> tree)
            where TKey : struct
            where TModel : class =>
            tree.Flatten().ToList();

        /// <summary>
        /// Flattens the values to list.
        /// </summary>
        /// <typeparam name="TKey">The type of the t identifier.</typeparam>
        /// <typeparam name="TModel">The type of the t model.</typeparam>
        /// <param name="tree">The tree.</param>
        /// <returns>IEnumerable&lt;System.Object&gt;.</returns>
        public static IEnumerable<TModel> FlattenDataList<TKey, TModel>(this TreeNode<TKey, TModel> tree)
            where TKey : struct
            where TModel : class =>
            tree.Flatten().Select(s => s.Data).ToList();


        /// <summary>
        /// Converts to tree.
        /// </summary>
        /// <typeparam name="TKey">The type of the t identifier.</typeparam>
        /// <typeparam name="TModel">The type of the t model.</typeparam>
        /// <param name="data">The data.</param>
        /// <returns>TreeNode&lt;T&gt;.</returns>
        public static TreeNode<TKey, TModel> ToTree<TKey, TModel>(this IEnumerable<TreeNode<TKey, TModel>> data)
            where TKey : struct
            where TModel : class
        {
            var array = data.ToArray();

            if(array.Length > 0)
            {
                var tree = array[0] as Tree<TKey, TModel>;
                var parents = new Stack<TreeNode<TKey, TModel>>();
                parents.Push(tree);

                for (int i = 1; i < data.Count() - 1; i++)
                {
                    while (array[i].Parent != parents.Peek())
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
            foreach (var node in nodes)
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
            foreach (var node in nodes)
            {
                if (node.IsSelected)
                    return node;

                var selectedChild = node.Children.GetSelectedNode();

                if (selectedChild != null)
                    return selectedChild;
            }

            return null;
        }

        /// <summary>
        /// Expands the parent nodes.
        /// </summary>
        /// <typeparam name="TKey">The type of the t identifier.</typeparam>
        /// <typeparam name="TModel">The type of the t model.</typeparam>
        /// <param name="node">The node.</param>
        public static void ExpandParentNodes<TKey, TModel>(this TreeNode<TKey, TModel> node)
            where TKey : struct
            where TModel : class
        {
            if (node.Parent != null)
            {
                node.Parent.IsExpanded = true;
                ExpandParentNodes(node.Parent);
            }
        }

        /// <summary>
        /// Toggles the expanded.
        /// </summary>
        /// <typeparam name="TKey">The type of the t identifier.</typeparam>
        /// <typeparam name="TModel">The type of the t model.</typeparam>
        /// <param name="nodes">The nodes.</param>
        /// <param name="isExpanded">if set to <c>true</c> [is expanded].</param>
        public static void ToggleExpanded<TKey, TModel>(IEnumerable<TreeNode<TKey, TModel>> nodes, bool isExpanded)
            where TKey : struct
            where TModel : class
        {
            foreach (var node in nodes)
            {
                node.IsExpanded = isExpanded;
                ToggleExpanded(node.Children, isExpanded);
            }
        }
    }
}
