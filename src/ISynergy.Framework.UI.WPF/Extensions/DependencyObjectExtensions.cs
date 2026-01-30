using System.Windows;
using System.Windows.Media;

namespace ISynergy.Framework.UI.Extensions;

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
    public static T? FindDescendant<T>(this DependencyObject self) where T : DependencyObject
    {
        // confirm parent is valid.
        if (self is null)
            return default;

        if (self is T)
            return (T)self;

        var childrenCount = VisualTreeHelper.GetChildrenCount(self);

        for (var i = 0; i < childrenCount; i++)
        {
            var child = VisualTreeHelper.GetChild(self, i);
            var foundChild = child.FindDescendant<T>();

            if (foundChild is not null)
                return (T)foundChild;
        }

        return null;
    }

    public static T? FindDescendant<T>(this DependencyObject parent, string childName) where T : DependencyObject
    {
        // Confirm parent and childName are valid. 
        if (parent == null)
            return null;

        int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
        for (int i = 0; i < childrenCount; i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            var tmpFoundChild = GetDescendant<T>(childName, child);
            if (tmpFoundChild is not null)
            {
                var foundChild = tmpFoundChild;
                return foundChild;
            }
        }

        return null;
    }

    private static T? GetDescendant<T>(string childName, DependencyObject child) where T : DependencyObject
    {
        // If the child is not of the request child type child
        if ((child is T childType) == false)
        {
            // recursively drill down the tree
            return FindDescendant<T>(child, childName);
        }

        if (string.IsNullOrEmpty(childName) == false)
        {
            // If the child's name is set for search
            if (child is FrameworkElement frameworkElement && frameworkElement.Name == childName)
            {
                return (T)child;
            }

            return null;
        }

        // child element found.
        return (T)child;
    }
}
