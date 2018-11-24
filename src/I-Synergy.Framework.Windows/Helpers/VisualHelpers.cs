﻿using System;
using System.Collections.Generic;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media;

namespace ISynergy.Helpers
{
    public static class VisualHelpers
    {
        public static Visual GetVisual(this UIElement element)
        {
            return ElementCompositionPreview.GetElementVisual(element);
        }

        public static CompositionCommitBatch ApplyImplicitAnimation(this Visual target, TimeSpan duration)
        {
            var myBatch = target.Compositor.GetCommitBatch(CompositionBatchTypes.Animation);
            target.Opacity = 0.0f;
            ImplicitAnimationCollection implicitAnimationCollection = target.Compositor.CreateImplicitAnimationCollection();

            implicitAnimationCollection[nameof(Visual.Opacity)] = CreateOpacityAnimation(target.Compositor, duration);
            target.ImplicitAnimations = implicitAnimationCollection;
            return myBatch;
        }

        public static KeyFrameAnimation CreateOpacityAnimation(Compositor compositor, TimeSpan duration)
        {
            ScalarKeyFrameAnimation kf = compositor.CreateScalarKeyFrameAnimation();
            kf.InsertExpressionKeyFrame(1.0f, "this.FinalValue");
            kf.Duration = duration;
            kf.Target = "Opacity";
            return kf;
        }

        public static void SetSize(this Visual v, FrameworkElement element)
        {
            v.Size = new System.Numerics.Vector2((float)element.ActualWidth, (float)element.ActualHeight);
        }

        public static void FadeVisual(this Visual v, double seconds)
        {
            var fadeAnimation = CreateImplicitFadeAnimation(seconds);
            v.ImplicitAnimations = Window.Current.Compositor.CreateImplicitAnimationCollection();
            v.ImplicitAnimations.Add(nameof(Visual.Opacity), fadeAnimation);
        }

        // TODO: kill this function
        public static ScalarKeyFrameAnimation CreateOpacityAnimation(double seconds)
        {
            var animation = Window.Current.Compositor.CreateScalarKeyFrameAnimation();
            animation.Target = "Opacity";
            animation.Duration = TimeSpan.FromSeconds(seconds);
            animation.InsertKeyFrame(0, 0);
            animation.InsertKeyFrame(0.25f, 0);
            animation.InsertKeyFrame(1, 1);
            return animation;
        }

        public static ICompositionAnimationBase CreateOpacityAnimation(double seconds, float finalvalue)
        {
            var animation = Window.Current.Compositor.CreateScalarKeyFrameAnimation();
            animation.Target = nameof(Visual.Opacity);
            animation.Duration = TimeSpan.FromSeconds(seconds);
            animation.InsertKeyFrame(1, finalvalue);
            return animation;
        }

        public static ICompositionAnimationBase CreateAnimationGroup(CompositionAnimation listContentShowAnimations, ScalarKeyFrameAnimation listContentOpacityAnimations)
        {
            var group = Window.Current.Compositor.CreateAnimationGroup();
            group.Add(listContentShowAnimations);
            group.Add(listContentOpacityAnimations);
            return group;
        }

        public static void EnableLayoutImplicitAnimations(this UIElement element, TimeSpan t)
        {
            Compositor compositor;
            var result = element.GetVisual();
            compositor = result.Compositor;

            var elementImplicitAnimation = compositor.CreateImplicitAnimationCollection();
            elementImplicitAnimation[nameof(Visual.Offset)] = CreateOffsetAnimation(compositor, t);

            result.ImplicitAnimations = elementImplicitAnimation;
        }

        private static CompositionAnimation CreateImplicitFadeAnimation(double seconds)
        {
            var animation = Window.Current.Compositor.CreateScalarKeyFrameAnimation();
            animation.InsertExpressionKeyFrame(1.0f, "this.FinalValue");
            animation.Target = nameof(Visual.Opacity);
            animation.Duration = TimeSpan.FromSeconds(seconds);
            return animation;
        }

        private static KeyFrameAnimation CreateOffsetAnimation(Compositor compositor, TimeSpan duration)
        {
            Vector3KeyFrameAnimation kf = compositor.CreateVector3KeyFrameAnimation();
            kf.InsertExpressionKeyFrame(1.0f, "this.FinalValue");
            kf.Duration = duration;
            kf.Target = "Offset";
            return kf;
        }

        public static CompositionAnimation CreateHorizontalOffsetAnimation(double seconds, float offset, double delaySeconds, bool from)
        {
            var animation = Window.Current.Compositor.CreateScalarKeyFrameAnimation();
            if (delaySeconds != 0.0)
            {
                animation.DelayBehavior = AnimationDelayBehavior.SetInitialValueBeforeDelay;
                animation.DelayTime = TimeSpan.FromSeconds(delaySeconds);
            }

            animation.Duration = TimeSpan.FromSeconds(seconds);
            animation.Target = "Translation.X";
            if (from)
            {
                animation.InsertKeyFrame(0, offset);
                animation.InsertKeyFrame(1, 0);
            }
            else
            {
                animation.InsertKeyFrame(1, offset);
            }

            return animation;
        }

        public static CompositionAnimation CreateHorizontalOffsetAnimation(double seconds, float offset, double delaySeconds)
        {
            return CreateHorizontalOffsetAnimation(seconds, offset, delaySeconds, true);
        }

        public static CompositionAnimation CreateVerticalOffsetAnimation(double seconds, float offset, double delaySeconds, bool from)
        {
            var animation = Window.Current.Compositor.CreateScalarKeyFrameAnimation();
            if (delaySeconds != 0.0)
            {
                animation.DelayBehavior = AnimationDelayBehavior.SetInitialValueBeforeDelay;
                animation.DelayTime = TimeSpan.FromSeconds(delaySeconds);
            }

            animation.Duration = TimeSpan.FromSeconds(seconds);
            animation.Target = "Translation.Y";
            if (from)
            {
                animation.InsertKeyFrame(0, offset);
                animation.InsertKeyFrame(1, 0);
            }
            else
            {
                animation.InsertKeyFrame(1, offset);
            }

            return animation;
        }

        public static CompositionAnimation CreateVerticalOffsetAnimation(double seconds, float offset, double delaySeconds)
        {
            return CreateVerticalOffsetAnimation(seconds, offset, delaySeconds, true);
        }

        public static CompositionAnimation CreateVerticalOffsetAnimationFrom(double seconds, float offset)
        {
            return CreateVerticalOffsetAnimation(seconds, offset, 0.0f);
        }

        public static CompositionAnimation CreateVerticalOffsetAnimationTo(double seconds, float offset)
        {
            return CreateVerticalOffsetAnimation(seconds, offset, 0.0f, false);
        }

        public static T GetVisualChildByName<T>(this FrameworkElement root, string name)
            where T : FrameworkElement
        {
            var chil = VisualTreeHelper.GetChild(root, 0);
            FrameworkElement child = null;

            int count = VisualTreeHelper.GetChildrenCount(root);

            for (int i = 0; i < count && child is null; i++)
            {
                var current = (FrameworkElement)VisualTreeHelper.GetChild(root, i);
                if (current != null && current.Name != null && current.Name == name)
                {
                    child = current;
                    break;
                }
                else
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
        public static FrameworkElement FindDescendantByName(this DependencyObject element, string name)
        {
            if (element == null || string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            if (name.Equals((element as FrameworkElement)?.Name, StringComparison.OrdinalIgnoreCase))
            {
                return element as FrameworkElement;
            }

            var childCount = VisualTreeHelper.GetChildrenCount(element);
            for (int i = 0; i < childCount; i++)
            {
                var result = VisualTreeHelper.GetChild(element, i).FindDescendantByName(name);
                if (result != null)
                {
                    return result;
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
        public static T FindDescendant<T>(this DependencyObject element)
            where T : DependencyObject
        {
            T retValue = null;
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

                if (retValue != null)
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
        public static object FindDescendant(this DependencyObject element, Type type)
        {
            object retValue = null;
            var childrenCount = VisualTreeHelper.GetChildrenCount(element);

            for (var i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(element, i);
                if (child.GetType() == type)
                {
                    retValue = child;
                    break;
                }

                retValue = FindDescendant(child, type);

                if (retValue != null)
                {
                    break;
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

                foreach (T childofChild in child.FindDescendants<T>())
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
        public static FrameworkElement FindAscendantByName(this DependencyObject element, string name)
        {
            if (element == null || string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            var parent = VisualTreeHelper.GetParent(element);

            if (parent == null)
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
        public static T FindAscendant<T>(this DependencyObject element)
            where T : DependencyObject
        {
            var parent = VisualTreeHelper.GetParent(element);

            if (parent == null)
            {
                return null;
            }

            if (parent is T)
            {
                return parent as T;
            }

            return parent.FindAscendant<T>();
        }

        /// <summary>
        /// Find first visual ascendant control of a specified type.
        /// </summary>
        /// <param name="element">Child element.</param>
        /// <param name="type">Type of ascendant to look for.</param>
        /// <returns>Ascendant control or null if not found.</returns>
        public static object FindAscendant(this DependencyObject element, Type type)
        {
            var parent = VisualTreeHelper.GetParent(element);

            if (parent == null)
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

            while (parent != null)
            {
                yield return parent;
                parent = VisualTreeHelper.GetParent(parent);
            }
        }
    }
}
