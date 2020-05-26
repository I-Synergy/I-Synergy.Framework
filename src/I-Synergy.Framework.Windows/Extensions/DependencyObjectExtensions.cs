using System.Collections.Generic;
using System.Reflection;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace ISynergy.Framework.Windows.Extensions
{
    public static class DependencyObjectExtensions
    {
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

        public static T FindParent<T>(this DependencyObject startNode) where T : DependencyObject
        {
            var parentObject = VisualTreeHelper.GetParent(startNode);
            if (parentObject is null) return null;

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
