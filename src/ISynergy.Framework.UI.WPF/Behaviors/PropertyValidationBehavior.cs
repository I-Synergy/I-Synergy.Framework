using System.Windows;
using System.Windows.Controls;

namespace ISynergy.Framework.UI.Behaviors
{
    /// <summary>
    /// Class ValidationBehaviorAttacher.
    /// </summary>
    public static class PropertyValidationBehavior
    {
        /// <summary>
        /// The property name property
        /// </summary>
        public static readonly DependencyProperty PropertyNameProperty = DependencyProperty.RegisterAttached(
            "PropertyName", typeof(string), typeof(PropertyValidationBehavior), new PropertyMetadata(default(string), PropertyNameChanged));

        private static ValidationBehavior _validationBehavior;

        /// <summary>
        /// Properties the name changed.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="arg">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void PropertyNameChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs arg)
        {
            var control = (Control)dependencyObject;
            _validationBehavior = new ValidationBehavior() { PropertyName = arg.NewValue.ToString() };
            control.Loaded += Control_Loaded;
            
        }

        private static void Control_Loaded(object sender, RoutedEventArgs e)
        {
            _validationBehavior.Attach(sender as DependencyObject);
        }

        /// <summary>
        /// Sets the name of the property.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="value">The value.</param>
        public static void SetPropertyName(DependencyObject element, string value) =>
            element.SetValue(PropertyNameProperty, value);

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>System.String.</returns>
        public static string GetPropertyName(DependencyObject element) =>
            element.GetValue(PropertyNameProperty).ToString();
    }
}
