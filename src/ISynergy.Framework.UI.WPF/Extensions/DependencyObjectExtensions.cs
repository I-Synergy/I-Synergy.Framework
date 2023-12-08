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
            foundChild = child.FindChild<T>();
            if (foundChild != null) break;
        }

        return (T)foundChild;
    }

    public static T FindChild<T>(this DependencyObject parent, string childName) where T : DependencyObject
    {
        // Confirm parent and childName are valid. 
        if (parent == null) return null;

        T foundChild = null;
        int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
        for (int i = 0; i < childrenCount; i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            var tmpFoundChild = GetChild<T>(childName, child);
            if (tmpFoundChild != null)
            {
                foundChild = tmpFoundChild;
                break;
            }
        }

        return foundChild;
    }

    private static T GetChild<T>(string childName, DependencyObject child) where T : DependencyObject
    {
        // If the child is not of the request child type child
        if ((child is T childType) == false)
        {
            // recursively drill down the tree
            return FindChild<T>(child, childName);
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
