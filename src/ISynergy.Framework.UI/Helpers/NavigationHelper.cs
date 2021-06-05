#if (NETFX_CORE || HAS_UNO)
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
#elif (NET5_0 && WINDOWS)
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
#endif

namespace ISynergy.Framework.UI.Helpers
{
    /// <summary>
    /// Class NavigationHelper.
    /// </summary>
    public static class NavigationHelper
    {
        /// <summary>
        /// Looks for a child control within a parent by name
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="name">The name.</param>
        /// <returns>DependencyObject.</returns>
        public static DependencyObject FindChild(DependencyObject parent, string name)
        {
            // confirm parent and name are valid.
            if (parent is null || string.IsNullOrEmpty(name)) return null;

            if (parent is FrameworkElement frameworkElement && frameworkElement.Name == name) return parent;

            DependencyObject result = null;

            var childrenCount = VisualTreeHelper.GetChildrenCount(parent);

            for (var i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                result = FindChild(child, name);
                if (result != null) break;
            }

            return result;
        }

        /// <summary>
        /// Looks for a child control within a parent by type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent">The parent.</param>
        /// <returns>T.</returns>
        public static T FindChild<T>(DependencyObject parent) where T : DependencyObject
        {
            // confirm parent is valid.
            if (parent is null) return default;
            if (parent is T) return (T)parent;

            DependencyObject foundChild = null;

            var childrenCount = VisualTreeHelper.GetChildrenCount(parent);

            for (var i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                foundChild = FindChild<T>(child);
                if (foundChild != null) break;
            }

            return (T)foundChild;
        }
    }
}
