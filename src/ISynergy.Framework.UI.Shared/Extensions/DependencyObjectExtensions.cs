using System.Collections.Generic;
using System.Reflection;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace ISynergy.Framework.UI.Extensions
{
    /// <summary>
    /// Class DependencyObjectExtensions.
    /// </summary>
    public static class DependencyObjectExtensions
    {
        /// <summary>
        /// Finds the children.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="startNode">The start node.</param>
        /// <param name="results">The results.</param>
        public static void FindChildren<T>(this DependencyObject startNode, List<T> results) where T : DependencyObject
        {
            var count = VisualTreeHelper.GetChildrenCount(startNode);
            for (var i = 0; i < count; i++)
            {
                var current = VisualTreeHelper.GetChild(startNode, i);
                if (current.GetType().Equals(typeof(T)) || current.GetType().GetTypeInfo().IsSubclassOf(typeof(T)))
                {
                    var asType = (T)current;
                    results.Add(asType);
                }
                current.FindChildren(results);
            }
        }

        /// <summary>
        /// Finds the parent.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="startNode">The start node.</param>
        /// <returns>T.</returns>
        public static T FindParent<T>(this DependencyObject startNode) where T : DependencyObject
        {
            var parentObject = VisualTreeHelper.GetParent(startNode);
            if (parentObject is null) return default;

            if (parentObject is T parent)
            {
                return parent;
            }
            else
            {
                return FindParent<T>(parentObject);
            }
        }
    }
}
