using System.Windows.Input;

#if (NETFX_CORE || HAS_UNO)
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
#elif (NET5_0 && WINDOWS)
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
#endif

namespace ISynergy.Framework.UI.Controls
{
    /// <summary>
    /// Attached property for UIElement to handle the LostFocus events.
    /// </summary>
    [Bindable]
    public static class UIElementAttached
    {
        /// <summary>
        /// Gets the LostFocus Command property.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static ICommand GetLostFocusCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(LostFocusCommandProperty);
        }

        /// <summary>
        /// Gets the LostFocus Command Parameter property
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static object GetLostFocusCommandParameter(DependencyObject obj)
        {
            return obj.GetValue(LostFocusCommandParameterProperty);
        }

        /// <summary>
        /// Sets the LostFocus Command property.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetLostFocusCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(LostFocusCommandProperty, value);
        }

        /// <summary>
        /// Sets the LostFocus Command Parameter property
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetLostFocusCommandParameter(DependencyObject obj, object value)
        {
            obj.SetValue(LostFocusCommandParameterProperty, value);
        }

        /// <summary>
        /// The LostFocus Command dependency property.
        /// </summary>
        public static readonly DependencyProperty LostFocusCommandProperty =
            DependencyProperty.RegisterAttached(
                "LostFocusCommand",
                typeof(ICommand),
                typeof(UIElementAttached),
                new PropertyMetadata(null, new PropertyChangedCallback(OnLostFocusCommandChanged)));

        /// <summary>
        /// The LostFocus Command Parameter dependency property.
        /// </summary>
        public static readonly DependencyProperty LostFocusCommandParameterProperty =
            DependencyProperty.RegisterAttached(
                "LostFocusCommandParameter",
                typeof(object),
                typeof(UIElementAttached),
                new PropertyMetadata(null));

        /// <summary>
        /// Attaches and detaches the handler of the LostFocus event
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnLostFocusCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement control)
            {
                if ((e.OldValue == null) && (e.NewValue != null))
                {
                    control.LostFocus += Control_LostFocus;
                }
                else if ((e.OldValue != null) && (e.NewValue == null))
                {
                    control.LostFocus -= Control_LostFocus;
                }
            }
        }

        /// <summary>
        /// Handles the LostFocus event by executing the given command.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Control_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is UIElement target && target.GetValue(LostFocusCommandProperty) is ICommand command)
            {
                var parameter = target.GetValue(LostFocusCommandParameterProperty);

                if (command.CanExecute(parameter))
                    command.Execute(parameter);
            }
        }
    }
}
