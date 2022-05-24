using System.Windows;
using System.Windows.Media;

namespace ISynergy.Framework.UI.Extensions
{
    /// <summary>
    /// Extensions for dependency objects. 
    /// </summary>
    public static class DependencyObjectExtensions
    {
        /// <summary>
        /// Looks for a child control within a parent by type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self">The parent.</param>
        /// <returns>T.</returns>
        public static T FindChild<T>(this DependencyObject self) where T : DependencyObject
        {
            // confirm parent is valid.
            if (self is null) return default;
            if (self is T) return (T)self;

            DependencyObject foundChild = null;

            var childrenCount = VisualTreeHelper.GetChildrenCount(self);

            for (var i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(self, i);
                foundChild = FindChild<T>(child);
                if (foundChild != null) break;
            }

            return (T)foundChild;
        }
    }
}
