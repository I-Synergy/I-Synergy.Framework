using Microsoft.Xaml.Interactivity;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;

namespace ISynergy.Framework.UI.Actions
{
    /// <summary>
    /// Class OpenFlyoutAction.
    /// Implements the <see cref="DependencyObject" />
    /// Implements the <see cref="IAction" />
    /// </summary>
    /// <seealso cref="DependencyObject" />
    /// <seealso cref="IAction" />
    public partial class OpenFlyoutAction : DependencyObject, IAction
    {
        /// <summary>
        /// Identifies the <seealso cref="Command" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
            nameof(Command),
            typeof(ICommand),
            typeof(OpenFlyoutAction),
            new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <seealso cref="CommandParameter" /> dependency property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register(
            nameof(CommandParameter),
            typeof(object),
            typeof(OpenFlyoutAction),
            new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <seealso cref="InputConverter" /> dependency property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        public static readonly DependencyProperty InputConverterProperty = DependencyProperty.Register(
            nameof(InputConverter),
            typeof(IValueConverter),
            typeof(OpenFlyoutAction),
            new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <seealso cref="InputConverterParameter" /> dependency property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        public static readonly DependencyProperty InputConverterParameterProperty = DependencyProperty.Register(
            nameof(InputConverterParameter),
            typeof(object),
            typeof(OpenFlyoutAction),
            new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <seealso cref="InputConverterLanguage" /> dependency property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        public static readonly DependencyProperty InputConverterLanguageProperty = DependencyProperty.Register(
            nameof(InputConverterLanguage),
            typeof(string),
            typeof(OpenFlyoutAction),
            new PropertyMetadata(string.Empty)); // Empty string means the invariant culture.

        /// <summary>
        /// Gets or sets the command this action should invoke. This is a dependency property.
        /// </summary>
        /// <value>The command.</value>
        public ICommand Command
        {
            get
            {
                return (ICommand)GetValue(CommandProperty);
            }
            set
            {
                SetValue(CommandProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the parameter that is passed to <see cref="ICommand.Execute(object)" />.
        /// If this is not set, the parameter from the <seealso cref="Execute(object, object)" /> method will be used.
        /// This is an optional dependency property.
        /// </summary>
        /// <value>The command parameter.</value>
        public object CommandParameter
        {
            get
            {
                return GetValue(CommandParameterProperty);
            }
            set
            {
                SetValue(CommandParameterProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the converter that is run on the parameter from the <seealso cref="Execute(object, object)" /> method.
        /// This is an optional dependency property.
        /// </summary>
        /// <value>The input converter.</value>
        public IValueConverter InputConverter
        {
            get
            {
                return (IValueConverter)GetValue(InputConverterProperty);
            }
            set
            {
                SetValue(InputConverterProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the parameter that is passed to the <see cref="IValueConverter.Convert" />
        /// method of <see cref="InputConverter" />.
        /// This is an optional dependency property.
        /// </summary>
        /// <value>The input converter parameter.</value>
        public object InputConverterParameter
        {
            get
            {
                return GetValue(InputConverterParameterProperty);
            }
            set
            {
                SetValue(InputConverterParameterProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the language that is passed to the <see cref="IValueConverter.Convert" />
        /// method of <see cref="InputConverter" />.
        /// This is an optional dependency property.
        /// </summary>
        /// <value>The input converter language.</value>
        public string InputConverterLanguage
        {
            get { return (string)GetValue(InputConverterLanguageProperty); }
            set { SetValue(InputConverterLanguageProperty, value); }
        }

        /// <summary>
        /// Executes the action.
        /// </summary>
        /// <param name="sender">The <see cref="System.Object" /> that is passed to the action by the behavior. Generally this is <seealso cref="IBehavior.AssociatedObject" /> or a target object.</param>
        /// <param name="parameter">The value of this parameter is determined by the caller.</param>
        /// <returns>True if the command is successfully executed; else false.</returns>
        public object Execute(object sender, object parameter)
        {
            if (Command is null)
            {
                return false;
            }

            object resolvedParameter;

            if (ReadLocalValue(CommandParameterProperty) != DependencyProperty.UnsetValue)
            {
                resolvedParameter = CommandParameter;
            }
            else if (InputConverter != null)
            {
                resolvedParameter = InputConverter.Convert(
                    parameter,
                    typeof(object),
                    InputConverterParameter,
                    InputConverterLanguage);
            }
            else
            {
                resolvedParameter = parameter;
            }

            if (!Command.CanExecute(resolvedParameter))
            {
                return false;
            }

            Command.Execute(resolvedParameter);

            if (sender is FrameworkElement element)
            {
                FlyoutBase.ShowAttachedFlyout(element);
            }

            return true;
        }
    }
}
