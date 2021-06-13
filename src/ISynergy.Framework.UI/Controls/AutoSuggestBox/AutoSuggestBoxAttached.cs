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
    /// Attached property for AutoSuggestBox to handle the QuerySubmitted event.
    /// </summary>
    [Bindable]
    public static class AutoSuggestBoxAttached
    {
        /// <summary>
        /// Gets the QuerySubmitted Command property.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static ICommand GetQuerySubmittedCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(QuerySubmittedCommandProperty);
        }

        /// <summary>
        /// Gets the QuerySubmitted Command Parameter property
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static object GetQuerySubmittedCommandParameter(DependencyObject obj)
        {
            return obj.GetValue(QuerySubmittedCommandParameterProperty);
        }

        /// <summary>
        /// Sets the QuerySubmittedCommand property.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetQuerySubmittedCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(QuerySubmittedCommandProperty, value);
        }

        /// <summary>
        /// Sets the QuerySubmitted Command Parameter property
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetQuerySubmittedCommandParameter(DependencyObject obj, object value)
        {
            obj.SetValue(QuerySubmittedCommandParameterProperty, value);
        }

        /// <summary>
        /// The QuerySubmitted Command dependency property.
        /// </summary>
        public static readonly DependencyProperty QuerySubmittedCommandProperty =
            DependencyProperty.RegisterAttached(
                "QuerySubmittedCommand",
                typeof(ICommand),
                typeof(AutoSuggestBoxAttached),
                new PropertyMetadata(null, new PropertyChangedCallback(OnQuerySubmittedCommandChanged)));

        /// <summary>
        /// The QuerySubmitted Command Parameter dependency property.
        /// </summary>
        public static readonly DependencyProperty QuerySubmittedCommandParameterProperty =
            DependencyProperty.RegisterAttached(
                "QuerySubmittedCommandParameter",
                typeof(object),
                typeof(AutoSuggestBoxAttached),
                new PropertyMetadata(null));

        /// <summary>
        /// Gets the TextChanged Command property.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static ICommand GetTextChangedCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(TextChangedCommandProperty);
        }

        /// <summary>
        /// Gets the TextChanged Command Parameter property
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static object GetTextChangedCommandParameter(DependencyObject obj)
        {
            return obj.GetValue(TextChangedCommandParameterProperty);
        }

        /// <summary>
        /// Sets the TextChanged Command property.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetTextChangedCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(TextChangedCommandProperty, value);
        }

        /// <summary>
        /// Sets the TextChanged Command Parameter property
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetTextChangedCommandParameter(DependencyObject obj, object value)
        {
            obj.SetValue(TextChangedCommandParameterProperty, value);
        }

        /// <summary>
        /// The TextChanged Command dependency property.
        /// </summary>
        public static readonly DependencyProperty TextChangedCommandProperty =
            DependencyProperty.RegisterAttached(
                "TextChangedCommand",
                typeof(ICommand),
                typeof(AutoSuggestBoxAttached),
                new PropertyMetadata(null, new PropertyChangedCallback(OnTextChangedCommandChanged)));

        /// <summary>
        /// The TextChanged Command Parameter dependency property.
        /// </summary>
        public static readonly DependencyProperty TextChangedCommandParameterProperty =
            DependencyProperty.RegisterAttached(
                "TextChangedCommandParameter",
                typeof(object),
                typeof(AutoSuggestBoxAttached),
                new PropertyMetadata(null));

        /// <summary>
        /// Gets the SuggestionChosen Command property.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static ICommand GetSuggestionChosenCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(SuggestionChosenCommandProperty);
        }

        /// <summary>
        /// Gets the SuggestionChosen Command Parameter property
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static object GetSuggestionChosenCommandParameter(DependencyObject obj)
        {
            return obj.GetValue(SuggestionChosenCommandParameterProperty);
        }

        /// <summary>
        /// Sets the SuggestionChosen property.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetSuggestionChosenCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(SuggestionChosenCommandProperty, value);
        }

        /// <summary>
        /// Sets the SuggestionChosen Command Parameter property
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetSuggestionChosenCommandParameter(DependencyObject obj, object value)
        {
            obj.SetValue(SuggestionChosenCommandParameterProperty, value);
        }

        /// <summary>
        /// The SuggestionChosen Command dependency property.
        /// </summary>
        public static readonly DependencyProperty SuggestionChosenCommandProperty =
            DependencyProperty.RegisterAttached(
                "SuggestionChosenCommand",
                typeof(ICommand),
                typeof(AutoSuggestBoxAttached),
                new PropertyMetadata(null, new PropertyChangedCallback(OnSuggestionChosenCommandChanged)));

        /// <summary>
        /// The SuggestionChosen Command Parameter dependency property.
        /// </summary>
        public static readonly DependencyProperty SuggestionChosenCommandParameterProperty =
            DependencyProperty.RegisterAttached(
                "SuggestionChosenCommandParameter",
                typeof(object),
                typeof(AutoSuggestBoxAttached),
                new PropertyMetadata(null));

        /// <summary>
        /// Attaches and detaches the handler of the QuerySubmitted event
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnQuerySubmittedCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AutoSuggestBox control)
            {
                if ((e.OldValue == null) && (e.NewValue != null))
                {
                    control.QuerySubmitted += Control_QuerySubmitted;
                }
                else if ((e.OldValue != null) && (e.NewValue == null))
                {
                    control.QuerySubmitted -= Control_QuerySubmitted;
                }
            }
        }

        /// <summary>
        /// Attaches and detaches the handler of the SuggestionChosen event
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnSuggestionChosenCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AutoSuggestBox control)
            {
                if ((e.OldValue == null) && (e.NewValue != null))
                {
                    control.SuggestionChosen += Control_SuggestionChosen;
                }
                else if ((e.OldValue != null) && (e.NewValue == null))
                {
                    control.SuggestionChosen -= Control_SuggestionChosen;
                }
            }
        }

        /// <summary>
        /// Attaches and detaches the handler of the TextChanged event
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnTextChangedCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AutoSuggestBox control)
            {
                if ((e.OldValue == null) && (e.NewValue != null))
                {
                    control.TextChanged += Control_TextChanged;
                }
                else if ((e.OldValue != null) && (e.NewValue == null))
                {
                    control.TextChanged -= Control_TextChanged;
                }
            }
        }

        /// <summary>
        /// Handles the QuerySubmitted event by executing the given command.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void Control_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (sender is AutoSuggestBox target && target.GetValue(QuerySubmittedCommandProperty) is ICommand command)
            {
                var parameter = target.GetValue(QuerySubmittedCommandParameterProperty);

                if (command.CanExecute(parameter))
                    command.Execute(parameter);
            }
        }

        /// <summary>
        /// Handles the SuggestionChosen event by executing the given command.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void Control_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            if (sender is AutoSuggestBox target && target.GetValue(SuggestionChosenCommandProperty) is ICommand command)
            {
                var parameter = target.GetValue(SuggestionChosenCommandParameterProperty);

                if (command.CanExecute(parameter))
                    command.Execute(parameter);
            }
        }

        /// <summary>
        /// Handles the TextChanged event by executing the given command.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void Control_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (sender is AutoSuggestBox target && target.GetValue(TextChangedCommandProperty) is ICommand command)
            {
                var parameter = target.GetValue(TextChangedCommandParameterProperty);

                if (command.CanExecute(parameter))
                    command.Execute(parameter);
            }
        }
    }
}
