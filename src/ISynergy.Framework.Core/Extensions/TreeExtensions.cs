using ISynergy.Framework.Core.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

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
        /// <typeparam name="T"></typeparam>
        /// <param name="tree">The node.</param>
        /// <returns>IEnumerable&lt;ITreeNode&gt;.</returns>
        public static IEnumerable<TreeNode<T>> Flatten<T>(this TreeNode<T> tree)
            where T : class
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
        /// <typeparam name="T"></typeparam>
        /// <param name="tree">The tree.</param>
        /// <returns>IEnumerable&lt;ITreeNode&gt;.</returns>
        public static IEnumerable<TreeNode<T>> FlattenList<T>(this TreeNode<T> tree)
            where T : class =>
            tree.Flatten().ToList();

        /// <summary>
        /// Flattens the values to list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tree">The tree.</param>
        /// <returns>IEnumerable&lt;System.Object&gt;.</returns>
        public static IEnumerable<T> FlattenValuesList<T>(this TreeNode<T> tree)
            where T : class =>
            tree.Flatten().Select(s => s.Data).ToList();


        /// <summary>
        /// Converts to tree.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">The data.</param>
        /// <returns>TreeNode&lt;T&gt;.</returns>
        public static TreeNode<T> ToTree<T>(this IEnumerable<TreeNode<T>> data)
            where T : class
        {
            var array = data.ToArray();

            if(array.Length > 0)
            {
                var tree = array[0] as Tree<T>;
                var parents = new Stack<TreeNode<T>>();
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
    }
}
