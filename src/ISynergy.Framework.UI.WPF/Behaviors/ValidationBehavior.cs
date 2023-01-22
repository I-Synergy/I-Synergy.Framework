using ISynergy.Framework.Core.Abstractions.Base;
using Microsoft.Xaml.Behaviors;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ISynergy.Framework.UI.Behaviors
{
    /// <summary>
    /// Validation behavior.
    /// </summary>
    public class ValidationBehavior : Behavior<Control>
    {
        /// <summary>
        /// Setup when OnAttach event is raised.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.DataContextChanged += Control_DataContextChanged;

            if (AssociatedObject.DataContext != null)
            {
                Setup();
            }
        }

        /// <summary>
        /// Tear down when OnDetaching event is raised.
        /// </summary>
        protected override void OnDetaching()
        {
            TearDown();
            base.OnDetaching();
        }

        /// <summary>
        /// Controls the data context changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The instance containing the event data.</param>
        private void Control_DataContextChanged(object sender, DependencyPropertyChangedEventArgs args) => Setup();

        /// <summary>
        /// Handles the PropertyChanged event of the Property control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void Property_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ExecuteActions();
        }

        /// <summary>
        /// Setups this instance.
        /// </summary>
        private void Setup()
        {
            TearDown();
            GetProperty().PropertyChanged += Property_PropertyChanged;
            ExecuteActions();
        }

        /// <summary>
        /// Tears down.
        /// </summary>
        private void TearDown()
        {
            if (GetProperty(false) != null)
            {
                GetProperty(false).PropertyChanged -= Property_PropertyChanged;
            }

            if (AssociatedObject != null)
            {
                AssociatedObject.DataContextChanged -= Control_DataContextChanged;
            }
        }

        /// <summary>
        /// Executes the actions.
        /// </summary>
        private void ExecuteActions()
        {
            if (GetProperty().IsValid)
            {
                AssociatedObject.ClearValue(Control.BorderBrushProperty);
            }
            else
            {
                AssociatedObject.BorderBrush = new SolidColorBrush(Colors.Red);
            }
        }

        /// <summary>
        /// The property
        /// </summary>
        IProperty _property;

        /// <summary>
        /// Gets the property.
        /// </summary>
        /// <param name="throwException">if set to <c>true</c> [throw exception].</param>
        /// <returns>IProperty.</returns>
        /// <exception cref="NullReferenceException">AssociatedObject is not valid</exception>
        /// <exception cref="NullReferenceException">PropertyName is not set</exception>
        /// <exception cref="NullReferenceException">DataContext is not IModel</exception>
        /// <exception cref="KeyNotFoundException">PropertyName is not found</exception>
        IProperty GetProperty(bool throwException = true)
        {
            if (_property != null)
                return _property;
            var context = (AssociatedObject as FrameworkElement)?.DataContext;
            if (context is null)
                if (throwException) throw new NullReferenceException("DataContext is null or invalid");
                else return null;
            if (string.IsNullOrEmpty(PropertyName))
                if (throwException) throw new NullReferenceException($"PropertyName '{PropertyName}' is not set");
                else return null;
            if (!(context is IObservableClass model))
                if (throwException) throw new NullReferenceException("DataContext is not of type IObservableClass");
                else return null;
            if (!model.Properties.ContainsKey(PropertyName))
                if (throwException) throw new KeyNotFoundException($"PropertyName '{PropertyName}' is not found");
                else return null;
            try
            {
                return _property = model.Properties[PropertyName];
            }
            catch
            {
                if (throwException) throw;
                else return null;
            }
        }

        /// <summary>
        /// Gets or sets the name of the property.
        /// </summary>
        /// <value>The name of the property.</value>
        public string PropertyName
        {
            get { return (string)GetValue(PropertyNameProperty); }
            set { SetValue(PropertyNameProperty, value); }
        }

        /// <summary>
        /// The property name property
        /// </summary>
        public static readonly DependencyProperty PropertyNameProperty =
            DependencyProperty.Register(nameof(PropertyName), typeof(string),
                typeof(ValidationBehavior), new PropertyMetadata(string.Empty, PropertyNameChanged));

        /// <summary>
        /// Properties the name changed.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        /// <exception cref="InvalidOperationException">PropertyName cannot be changed once set.</exception>
        private static void PropertyNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.OldValue?.ToString() ?? string.Empty))
            {
                throw new InvalidOperationException($"PropertyName '{e.Property.ToString()}' cannot be changed once set.");
            }
        }
    }
}
