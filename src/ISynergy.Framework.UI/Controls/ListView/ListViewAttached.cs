using System.Windows.Input;

#if (NETFX_CORE || HAS_UNO)
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
#elif (NET5_0 && WINDOWS)
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
#endif

namespace ISynergy.Framework.UI.Controls
{
    /// <summary>
    /// Attached property for ListView to handle the DoubleTapped event.
    /// </summary>
    [Bindable]
    public static class ListViewAttached
    {
        /// <summary>
        /// Gets the DoubleTapped Command property.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static ICommand GetDoubleTappedCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(DoubleTappedCommandProperty);
        }

        /// <summary>
        /// Sets the DoubleTapped Command property.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetDoubleTappedCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(DoubleTappedCommandProperty, value);
        }

        /// <summary>
        /// The DoubleTapped Command dependency property.
        /// </summary>
        public static readonly DependencyProperty DoubleTappedCommandProperty =
            DependencyProperty.RegisterAttached(
                "DoubleTappedCommand",
                typeof(ICommand),
                typeof(ListViewAttached),
                new PropertyMetadata(null, new PropertyChangedCallback(OnDoubleTappedCommandChanged)));

        /// <summary>
        /// Gets the Double Tapped Command Parameter property.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static object GetDoubleTappedCommandParameter(DependencyObject obj)
        {
            return obj.GetValue(DoubleTappedCommandParameterProperty);
        }

        /// <summary>
        /// Sets the Double Tapped Command property.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetDoubleTappedCommandParameter(DependencyObject obj, object value)
        {
            obj.SetValue(DoubleTappedCommandParameterProperty, value);
        }

        /// <summary>
        /// The Tapped Command dependency property.
        /// </summary>
        public static readonly DependencyProperty DoubleTappedCommandParameterProperty =
            DependencyProperty.RegisterAttached(
                "DoubleTappedCommandParameter",
                typeof(object),
                typeof(ListViewAttached),
                new PropertyMetadata(null));

        /// <summary>
        /// Attaches and detaches the handler of the DoubleTapped event
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnDoubleTappedCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ListView control && control.SelectionMode == ListViewSelectionMode.Single)
            {
                if ((e.OldValue == null) && (e.NewValue != null))
                {
                    control.DoubleTapped += Control_DoubleTapped;
                }
                else if ((e.OldValue != null) && (e.NewValue == null))
                {
                    control.DoubleTapped -= Control_DoubleTapped;
                }
            }
        }

        /// <summary>
        /// Handles the DoubleTapped event by executing the given command.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Control_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            if (sender is ListView target && target.GetValue(DoubleTappedCommandProperty) is ICommand command)
            {
                var parameter = target.GetValue(DoubleTappedCommandParameterProperty);

                if (command.CanExecute(parameter))
                    command.Execute(parameter);
            }

            e.Handled = true;
        }
    }
}
