using ISynergy.Framework.UI.Extensions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ISynergy.Framework.UI.Behaviors
{
    /// <summary>
    /// Behavior for auto focus
    /// </summary>
    public class AutoFocusBehavior
    {
        /// <summary>
        /// Gets the automatic selectable.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool GetAutoFocusable(DependencyObject obj)
        {
            return (bool)obj.GetValue(AutoFocusableProperty);
        }

        /// <summary>
        /// Sets the automatic selectable.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetAutoFocusable(DependencyObject obj, bool value)
        {
            obj.SetValue(AutoFocusableProperty, value);
        }

        /// <summary>
        /// The automatic selectable property
        /// </summary>
        public static readonly DependencyProperty AutoFocusableProperty =
            DependencyProperty.RegisterAttached(
                "AutoFocusable",
                typeof(bool),
                typeof(AutoFocusBehavior),
                new PropertyMetadata(false, AutoFocusableChangedHandler));

        /// <summary>
        /// Automatics the focusable changed handler.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void AutoFocusableChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TextBox textBox = null;

            if (d is TextBox control)
                textBox = control;
            else
                textBox = d.FindChild<TextBox>();

            if (e.NewValue != e.OldValue && textBox is not null)
            {
                if ((bool)e.NewValue)
                {
                    textBox.Loaded += TextBox_Loaded;
                }
                else
                {
                    textBox.Loaded -= TextBox_Loaded;
                }
            }
        }

        /// <summary>
        /// Handles the OnLoaded event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void TextBox_Loaded(object sender, RoutedEventArgs e)
        {
            TextBox textBox = null;

            if (sender is TextBox control)
                textBox = control;
            else
                textBox = (sender as DependencyObject).FindChild<TextBox>();

            if (textBox is not null && textBox.IsLoaded && textBox.Visibility == Visibility.Visible)
            {
                textBox.Focus();
                Keyboard.Focus(textBox);
            }
        }
    }
}
