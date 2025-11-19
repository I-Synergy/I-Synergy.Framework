using System;
using System.Collections.Generic;
using System.Reflection;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace ISynergy.Framework.UI.Extensions;

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
    public static T? FindParent<T>(this DependencyObject startNode) where T : DependencyObject
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

    /// <summary>
    /// Get visual child by name.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="root"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static T? GetVisualChildByName<T>(this FrameworkElement? root, string name)
        where T : FrameworkElement
    {
        if (root is null) return null;
        
        FrameworkElement? child = null;

        var count = VisualTreeHelper.GetChildrenCount(root);

        for (var i = 0; i < count && child is null; i++)
        {
            var current = VisualTreeHelper.GetChild(root, i) as FrameworkElement;
            if (current is not null && current.Name is not null && current.Name == name)
            {
                child = current;
                break;
            }
            else if (current is not null)
            {
                child = current.GetVisualChildByName<FrameworkElement>(name);
            }
        }

        return child as T;
    }

    /// <summary>
    /// Find descendant <see cref="FrameworkElement"/> control using its name.
    /// </summary>
    /// <param name="element">Parent element.</param>
    /// <param name="name">Name of the control to find</param>
    /// <returns>Descendant control or null if not found.</returns>
    public static FrameworkElement? FindDescendantByName(this DependencyObject? element, string name)
    {
        if (element is null || string.IsNullOrWhiteSpace(name))
        {
            return null;
        }

        if (name.Equals((element as FrameworkElement)?.Name, StringComparison.OrdinalIgnoreCase))
        {
            return element as FrameworkElement;
        }

        var childCount = VisualTreeHelper.GetChildrenCount(element);
        for (var i = 0; i < childCount; i++)
        {
            var child = VisualTreeHelper.GetChild(element, i);
            if (child is not null)
            {
                var result = child.FindDescendantByName(name);
                if (result is not null)
                {
                    return result;
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Find first descendant control of a specified type.
    /// </summary>
    /// <typeparam name="T">Type to search for.</typeparam>
    /// <param name="element">Parent element.</param>
    /// <returns>Descendant control or null if not found.</returns>
    public static T? FindDescendant<T>(this DependencyObject element)
        where T : DependencyObject
    {
        T? retValue = default;
        var childrenCount = VisualTreeHelper.GetChildrenCount(element);

        for (var i = 0; i < childrenCount; i++)
        {
            var child = VisualTreeHelper.GetChild(element, i);
            if (child is T type)
            {
                retValue = type;
                break;
            }

            retValue = FindDescendant<T>(child);

            if (retValue is not null)
            {
                break;
            }
        }

        return retValue;
    }

    /// <summary>
    /// Find first descendant control of a specified type.
    /// </summary>
    /// <param name="element">Parent element.</param>
    /// <param name="type">Type of descendant.</param>
    /// <returns>Descendant control or null if not found.</returns>
    public static object? FindDescendant(this DependencyObject element, Type type)
    {
        object? retValue = null;
        var childrenCount = VisualTreeHelper.GetChildrenCount(element);

        for (var i = 0; i < childrenCount; i++)
        {
            var child = VisualTreeHelper.GetChild(element, i);
            if (child is not null && child.GetType() == type)
            {
                retValue = child;
                break;
            }

            if (child is not null)
            {
                retValue = FindDescendant(child, type);

                if (retValue is not null)
                {
                    break;
                }
            }
        }

        return retValue;
    }

    /// <summary>
    /// Find all descendant controls of the specified type.
    /// </summary>
    /// <typeparam name="T">Type to search for.</typeparam>
    /// <param name="element">Parent element.</param>
    /// <returns>Descendant controls or empty if not found.</returns>
    public static IEnumerable<T> FindDescendants<T>(this DependencyObject element)
        where T : DependencyObject
    {
        var childrenCount = VisualTreeHelper.GetChildrenCount(element);

        for (var i = 0; i < childrenCount; i++)
        {
            var child = VisualTreeHelper.GetChild(element, i);
            if (child is T type)
            {
                yield return type;
            }

            foreach (var childofChild in child.FindDescendants<T>())
            {
                yield return childofChild;
            }
        }
    }

    /// <summary>
    /// Find visual ascendant <see cref="FrameworkElement"/> control using its name.
    /// </summary>
    /// <param name="element">Parent element.</param>
    /// <param name="name">Name of the control to find</param>
    /// <returns>Descendant control or null if not found.</returns>
    public static FrameworkElement? FindAscendantByName(this DependencyObject element, string name)
    {
        if (element is null || string.IsNullOrWhiteSpace(name))
        {
            return null;
        }

        var parent = VisualTreeHelper.GetParent(element);

        if (parent is null)
        {
            return null;
        }

        if (name.Equals((parent as FrameworkElement)?.Name, StringComparison.OrdinalIgnoreCase))
        {
            return parent as FrameworkElement;
        }

        return parent.FindAscendantByName(name);
    }

    /// <summary>
    /// Find first visual ascendant control of a specified type.
    /// </summary>
    /// <typeparam name="T">Type to search for.</typeparam>
    /// <param name="element">Child element.</param>
    /// <returns>Ascendant control or null if not found.</returns>
    public static T? FindAscendant<T>(this DependencyObject element)
        where T : DependencyObject
    {
        var parent = VisualTreeHelper.GetParent(element);

        if (parent is null)
        {
            return default;
        }

        if (parent is T result)
        {
            return result;
        }

        return parent.FindAscendant<T>();
    }

    /// <summary>
    /// Find first visual ascendant control of a specified type.
    /// </summary>
    /// <param name="element">Child element.</param>
    /// <param name="type">Type of ascendant to look for.</param>
    /// <returns>Ascendant control or null if not found.</returns>
    public static object? FindAscendant(this DependencyObject element, Type type)
    {
        var parent = VisualTreeHelper.GetParent(element);

        if (parent is null)
        {
            return null;
        }

        if (parent.GetType() == type)
        {
            return parent;
        }

        return parent.FindAscendant(type);
    }

    /// <summary>
    /// Find all visual ascendants for the element.
    /// </summary>
    /// <param name="element">Child element.</param>
    /// <returns>A collection of parent elements or null if none found.</returns>
    public static IEnumerable<DependencyObject> FindAscendants(this DependencyObject element)
    {
        var parent = VisualTreeHelper.GetParent(element);

        while (parent is not null)
        {
            yield return parent;
            parent = VisualTreeHelper.GetParent(parent);
        }
    }
}
