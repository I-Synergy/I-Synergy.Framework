using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;

namespace ISynergy.Framework.UI.Helpers
{
    /// <summary>
    /// Class NavigationAnimationHelpers.
    /// </summary>
    public static class NavigationAnimationHelpers
    {
        /// <summary>
        /// Navigates the with fade outgoing.
        /// </summary>
        /// <param name="frame">The frame.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="destination">The destination.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool NavigateWithFadeOutgoing(this Frame frame, object parameter, Type destination)
        {
            ImplicitHideFrameContent(frame);

            return frame.Navigate(destination, parameter);
        }

        /// <summary>
        /// Implicits the content of the hide frame.
        /// </summary>
        /// <param name="frame">The frame.</param>
        private static void ImplicitHideFrameContent(Frame frame)
        {
            if (frame.Content != null)
            {
                SetImplicitHide(frame.Content as UIElement);
            }
        }

        /// <summary>
        /// Sets the implicit hide.
        /// </summary>
        /// <param name="thisPtr">The this PTR.</param>
        private static void SetImplicitHide(UIElement thisPtr)
        {
            ElementCompositionPreview.SetImplicitHideAnimation(thisPtr, VisualHelpers.CreateOpacityAnimation(0.4, 0));
            Canvas.SetTop(thisPtr, 1);
        }
    }
}
