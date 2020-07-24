using System;
using System.Collections.Generic;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media;

namespace ISynergy.Framework.Windows.Helpers
{
    /// <summary>
    /// Class VisualHelpers.
    /// </summary>
    public static class VisualHelpers
    {
        /// <summary>
        /// Gets the visual.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>Visual.</returns>
        public static Visual GetVisual(this UIElement element)
        {
            return ElementCompositionPreview.GetElementVisual(element);
        }

        /// <summary>
        /// Applies the implicit animation.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="duration">The duration.</param>
        /// <returns>CompositionCommitBatch.</returns>
        public static CompositionCommitBatch ApplyImplicitAnimation(this Visual target, TimeSpan duration)
        {
            var myBatch = target.Compositor.GetCommitBatch(CompositionBatchTypes.Animation);
            target.Opacity = 0.0f;
            var implicitAnimationCollection = target.Compositor.CreateImplicitAnimationCollection();

            implicitAnimationCollection[nameof(Visual.Opacity)] = CreateOpacityAnimation(target.Compositor, duration);
            target.ImplicitAnimations = implicitAnimationCollection;
            return myBatch;
        }

        /// <summary>
        /// Creates the opacity animation.
        /// </summary>
        /// <param name="compositor">The compositor.</param>
        /// <param name="duration">The duration.</param>
        /// <returns>KeyFrameAnimation.</returns>
        public static KeyFrameAnimation CreateOpacityAnimation(Compositor compositor, TimeSpan duration)
        {
            var kf = compositor.CreateScalarKeyFrameAnimation();
            kf.InsertExpressionKeyFrame(1.0f, "this.FinalValue");
            kf.Duration = duration;
            kf.Target = "Opacity";
            return kf;
        }

        /// <summary>
        /// Sets the size.
        /// </summary>
        /// <param name="v">The v.</param>
        /// <param name="element">The element.</param>
        public static void SetSize(this Visual v, FrameworkElement element)
        {
            v.Size = new System.Numerics.Vector2((float)element.ActualWidth, (float)element.ActualHeight);
        }

        /// <summary>
        /// Fades the visual.
        /// </summary>
        /// <param name="v">The v.</param>
        /// <param name="seconds">The seconds.</param>
        public static void FadeVisual(this Visual v, double seconds)
        {
            var fadeAnimation = CreateImplicitFadeAnimation(seconds);
            v.ImplicitAnimations = Window.Current.Compositor.CreateImplicitAnimationCollection();
            v.ImplicitAnimations.Add(nameof(Visual.Opacity), fadeAnimation);
        }

        // TODO: kill this function
        /// <summary>
        /// Creates the opacity animation.
        /// </summary>
        /// <param name="seconds">The seconds.</param>
        /// <returns>ScalarKeyFrameAnimation.</returns>
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

        /// <summary>
        /// Creates the opacity animation.
        /// </summary>
        /// <param name="seconds">The seconds.</param>
        /// <param name="finalvalue">The finalvalue.</param>
        /// <returns>ICompositionAnimationBase.</returns>
        public static ICompositionAnimationBase CreateOpacityAnimation(double seconds, float finalvalue)
        {
            var animation = Window.Current.Compositor.CreateScalarKeyFrameAnimation();
            animation.Target = nameof(Visual.Opacity);
            animation.Duration = TimeSpan.FromSeconds(seconds);
            animation.InsertKeyFrame(1, finalvalue);
            return animation;
        }

        /// <summary>
        /// Creates the animation group.
        /// </summary>
        /// <param name="listContentShowAnimations">The list content show animations.</param>
        /// <param name="listContentOpacityAnimations">The list content opacity animations.</param>
        /// <returns>ICompositionAnimationBase.</returns>
        public static ICompositionAnimationBase CreateAnimationGroup(CompositionAnimation listContentShowAnimations, ScalarKeyFrameAnimation listContentOpacityAnimations)
        {
            var group = Window.Current.Compositor.CreateAnimationGroup();
            group.Add(listContentShowAnimations);
            group.Add(listContentOpacityAnimations);
            return group;
        }

        /// <summary>
        /// Enables the layout implicit animations.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="t">The t.</param>
        public static void EnableLayoutImplicitAnimations(this UIElement element, TimeSpan t)
        {
            Compositor compositor;
            var result = element.GetVisual();
            compositor = result.Compositor;

            var elementImplicitAnimation = compositor.CreateImplicitAnimationCollection();
            elementImplicitAnimation[nameof(Visual.Offset)] = CreateOffsetAnimation(compositor, t);

            result.ImplicitAnimations = elementImplicitAnimation;
        }

        /// <summary>
        /// Creates the implicit fade animation.
        /// </summary>
        /// <param name="seconds">The seconds.</param>
        /// <returns>CompositionAnimation.</returns>
        private static CompositionAnimation CreateImplicitFadeAnimation(double seconds)
        {
            var animation = Window.Current.Compositor.CreateScalarKeyFrameAnimation();
            animation.InsertExpressionKeyFrame(1.0f, "this.FinalValue");
            animation.Target = nameof(Visual.Opacity);
            animation.Duration = TimeSpan.FromSeconds(seconds);
            return animation;
        }

        /// <summary>
        /// Creates the offset animation.
        /// </summary>
        /// <param name="compositor">The compositor.</param>
        /// <param name="duration">The duration.</param>
        /// <returns>KeyFrameAnimation.</returns>
        private static KeyFrameAnimation CreateOffsetAnimation(Compositor compositor, TimeSpan duration)
        {
            var kf = compositor.CreateVector3KeyFrameAnimation();
            kf.InsertExpressionKeyFrame(1.0f, "this.FinalValue");
            kf.Duration = duration;
            kf.Target = "Offset";
            return kf;
        }

        /// <summary>
        /// Creates the horizontal offset animation.
        /// </summary>
        /// <param name="seconds">The seconds.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="delaySeconds">The delay seconds.</param>
        /// <param name="from">if set to <c>true</c> [from].</param>
        /// <returns>CompositionAnimation.</returns>
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

        /// <summary>
        /// Creates the horizontal offset animation.
        /// </summary>
        /// <param name="seconds">The seconds.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="delaySeconds">The delay seconds.</param>
        /// <returns>CompositionAnimation.</returns>
        public static CompositionAnimation CreateHorizontalOffsetAnimation(double seconds, float offset, double delaySeconds)
        {
            return CreateHorizontalOffsetAnimation(seconds, offset, delaySeconds, true);
        }

        /// <summary>
        /// Creates the vertical offset animation.
        /// </summary>
        /// <param name="seconds">The seconds.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="delaySeconds">The delay seconds.</param>
        /// <param name="from">if set to <c>true</c> [from].</param>
        /// <returns>CompositionAnimation.</returns>
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

        /// <summary>
        /// Creates the vertical offset animation.
        /// </summary>
        /// <param name="seconds">The seconds.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="delaySeconds">The delay seconds.</param>
        /// <returns>CompositionAnimation.</returns>
        public static CompositionAnimation CreateVerticalOffsetAnimation(double seconds, float offset, double delaySeconds)
        {
            return CreateVerticalOffsetAnimation(seconds, offset, delaySeconds, true);
        }

        /// <summary>
        /// Creates the vertical offset animation from.
        /// </summary>
        /// <param name="seconds">The seconds.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>CompositionAnimation.</returns>
        public static CompositionAnimation CreateVerticalOffsetAnimationFrom(double seconds, float offset)
        {
            return CreateVerticalOffsetAnimation(seconds, offset, 0.0f);
        }

        /// <summary>
        /// Creates the vertical offset animation to.
        /// </summary>
        /// <param name="seconds">The seconds.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>CompositionAnimation.</returns>
        public static CompositionAnimation CreateVerticalOffsetAnimationTo(double seconds, float offset)
        {
            return CreateVerticalOffsetAnimation(seconds, offset, 0.0f, false);
        }

        /// <summary>
        /// Gets the name of the visual child by.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="root">The root.</param>
        /// <param name="name">The name.</param>
        /// <returns>T.</returns>
        public static T GetVisualChildByName<T>(this FrameworkElement root, string name)
            where T : FrameworkElement
        {
            var chil = VisualTreeHelper.GetChild(root, 0);
            FrameworkElement child = null;

            var count = VisualTreeHelper.GetChildrenCount(root);

            for (var i = 0; i < count && child is null; i++)
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
        /// Find descendant <see cref="FrameworkElement" /> control using its name.
        /// </summary>
        /// <param name="element">Parent element.</param>
        /// <param name="name">Name of the control to find</param>
        /// <returns>Descendant control or null if not found.</returns>
        public static FrameworkElement FindDescendantByName(this DependencyObject element, string name)
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

                foreach (var childofChild in child.FindDescendants<T>())
                {
                    yield return childofChild;
                }
            }
        }

        /// <summary>
        /// Find visual ascendant <see cref="FrameworkElement" /> control using its name.
        /// </summary>
        /// <param name="element">Parent element.</param>
        /// <param name="name">Name of the control to find</param>
        /// <returns>Descendant control or null if not found.</returns>
        public static FrameworkElement FindAscendantByName(this DependencyObject element, string name)
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
        public static T FindAscendant<T>(this DependencyObject element)
            where T : DependencyObject
        {
            var parent = VisualTreeHelper.GetParent(element);

            if (parent is null)
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

            while (parent != null)
            {
                yield return parent;
                parent = VisualTreeHelper.GetParent(parent);
            }
        }
    }
}
