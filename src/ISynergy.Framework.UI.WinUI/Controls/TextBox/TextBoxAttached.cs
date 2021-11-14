#if WINDOWS_UWP
using ISynergy;
using ISynergy.Framework;
using ISynergy.Framework.UI;
using ISynergy.Framework.UI.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#else
using ISynergy;
using ISynergy.Framework;
using ISynergy.Framework.UI;
using ISynergy.Framework.UI.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endif

namespace ISynergy.Framework.UI.Controls
{
    /// <summary>
    /// Class TextBoxAttached.
    /// </summary>
    public static class TextBoxAttached
    {
        /// <summary>
        /// Gets the automatic selectable.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool GetAutoSelectable(DependencyObject obj)
        {
            return (bool)obj.GetValue(AutoSelectableProperty);
        }

        /// <summary>
        /// Sets the automatic selectable.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetAutoSelectable(DependencyObject obj, bool value)
        {
            obj.SetValue(AutoSelectableProperty, value);
        }

        /// <summary>
        /// The automatic selectable property
        /// </summary>
        public static readonly DependencyProperty AutoSelectableProperty =
            DependencyProperty.RegisterAttached(
                "AutoSelectable",
                typeof(bool),
                typeof(TextBoxAttached),
                new PropertyMetadata(false, AutoFocusableChangedHandler));

        /// <summary>
        /// Automatics the focusable changed handler.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void AutoFocusableChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue && d is TextBox textBox)
            {
                if ((bool)e.NewValue)
                {
                    textBox.GotFocus += OnGotFocusHandler;
                }
                else
                {
                    textBox.GotFocus -= OnGotFocusHandler;
                }
            }
        }

        /// <summary>
        /// Handles the <see cref="E:GotFocusHandler" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private static void OnGotFocusHandler(object sender, RoutedEventArgs e)
        {
            (sender as TextBox)?.SelectAll();
        }
    }
}
