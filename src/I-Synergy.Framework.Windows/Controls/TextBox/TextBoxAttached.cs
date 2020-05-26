using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ISynergy.Framework.Windows.Controls
{
    public static class TextBoxAttached
    {
        public static bool GetAutoSelectable(DependencyObject obj)
        {
            return (bool)obj.GetValue(AutoSelectableProperty);
        }

        public static void SetAutoSelectable(DependencyObject obj, bool value)
        {
            obj.SetValue(AutoSelectableProperty, value);
        }

        public static readonly DependencyProperty AutoSelectableProperty =
            DependencyProperty.RegisterAttached(
                "AutoSelectable",
                typeof(bool),
                typeof(TextBoxAttached),
                new PropertyMetadata(false, AutoFocusableChangedHandler));

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

        private static void OnGotFocusHandler(object sender, RoutedEventArgs e)
        {
            (sender as TextBox)?.SelectAll();
        }
    }
}
