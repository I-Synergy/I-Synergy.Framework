using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace ISynergy.Framework.UI.Commands
{
    /// <summary>
    /// Class DoubleTappedCommand.
    /// </summary>
    public static class DoubleTappedCommand
    {
        /// <summary>
        /// The command property
        /// </summary>
        public static readonly DependencyProperty CommandProperty = 
            DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(DoubleTappedCommand), new PropertyMetadata(null, OnCommandPropertyChanged));

        /// <summary>
        /// The command parameter property
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.RegisterAttached("CommandParameter", typeof(object), typeof(DoubleTappedCommand), new PropertyMetadata(null));

        /// <summary>
        /// Sets the command.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="value">The value.</param>
        public static void SetCommand(DependencyObject d, ICommand value)
        {
            d.SetValue(CommandProperty, value);
        }

        /// <summary>
        /// Sets the command parameter.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="value">The value.</param>
        public static void SetCommandParameter(DependencyObject d, object value)
        {
            d.SetValue(CommandParameterProperty, value);
        }

        /// <summary>
        /// Gets the command.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <returns>ICommand.</returns>
        public static ICommand GetCommand(DependencyObject d) => (ICommand)d.GetValue(CommandProperty);

        /// <summary>
        /// Gets the command parameter.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <returns>System.Object.</returns>
        public static object GetCommandParameter(DependencyObject d) => d.GetValue(CommandParameterProperty);

        /// <summary>
        /// Handles the <see cref="E:CommandPropertyChanged" /> event.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnCommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ListViewBase control)
            {
                control.DoubleTapped += OnDoubleTapped;
            }
        }

        /// <summary>
        /// Handles the <see cref="E:DoubleTapped" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DoubleTappedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnDoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            if(sender is ListViewBase control)
            {
                var command = GetCommand(control);

                if (command != null && command.CanExecute(GetCommandParameter(control)))
                {
                    command.Execute(GetCommandParameter(control));
                }
            }
        }
    }
}
