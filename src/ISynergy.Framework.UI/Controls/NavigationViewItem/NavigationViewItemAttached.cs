using System.Windows.Input;
using Microsoft.UI.Xaml.Controls;

#if (NETFX_CORE || HAS_UNO)
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
#elif (NET5_0 && WINDOWS)
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
#endif

namespace ISynergy.Framework.UI.Controls
{
    /// <summary>
    /// Attached property for ListView to handle the Tapped event.
    /// </summary>
    [Bindable]
    public class NavigationViewItemAttached
    {
        /// <summary>
        /// Gets the Tapped Command property.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static ICommand GetTappedCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(TappedCommandProperty);
        }

        /// <summary>
        /// Sets the Tapped Command property.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetTappedCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(TappedCommandProperty, value);
        }

        /// <summary>
        /// The Tapped Command dependency property.
        /// </summary>
        public static readonly DependencyProperty TappedCommandProperty =
            DependencyProperty.RegisterAttached(
                "TappedCommand",
                typeof(ICommand),
                typeof(NavigationViewItemAttached),
                new PropertyMetadata(null, new PropertyChangedCallback(OnTappedCommandChanged)));

        /// <summary>
        /// Gets the Tapped Command Parameter property.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static object GetTappedCommandParameter(DependencyObject obj)
        {
            return obj.GetValue(TappedCommandParameterProperty);
        }

        /// <summary>
        /// Sets the Tapped Command property.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetTappedCommandParameter(DependencyObject obj, object value)
        {
            obj.SetValue(TappedCommandParameterProperty, value);
        }

        /// <summary>
        /// The Tapped Command dependency property.
        /// </summary>
        public static readonly DependencyProperty TappedCommandParameterProperty =
            DependencyProperty.RegisterAttached(
                "TappedCommandParameter",
                typeof(object),
                typeof(NavigationViewItemAttached),
                new PropertyMetadata(null));

        /// <summary>
        /// Attaches and detaches the handler of the Tapped event
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnTappedCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NavigationViewItem control)
            {
                if ((e.OldValue == null) && (e.NewValue != null))
                {
                    control.Tapped += Control_Tapped;
                }
                else if ((e.OldValue != null) && (e.NewValue == null))
                {
                    control.Tapped -= Control_Tapped;
                }
            }
        }

        /// <summary>
        /// Handles the Tapped event by executing the given command.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Control_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (sender is NavigationViewItem target && target.GetValue(TappedCommandProperty) is ICommand command)
            {
                var parameter = target.GetValue(TappedCommandParameterProperty);

                if (command.CanExecute(parameter))
                    command.Execute(parameter);
            }

            e.Handled = true;
        }
    }
}
