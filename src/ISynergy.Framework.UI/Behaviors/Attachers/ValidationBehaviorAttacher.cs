using Microsoft.Xaml.Interactions.Core;
using Microsoft.Xaml.Interactivity;

#if (__UWP__ || HAS_UNO)
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
#elif (__WINUI__)
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
#endif

namespace ISynergy.Framework.UI.Behaviors
{
    /// <summary>
    /// Class ValidationBehaviorAttacher.
    /// </summary>
    public static class ValidationBehaviorAttacher
    {
        /// <summary>
        /// The property name property
        /// </summary>
        public static readonly DependencyProperty PropertyNameProperty = DependencyProperty.RegisterAttached(
        "PropertyName", typeof(string), typeof(ValidationBehaviorAttacher), new PropertyMetadata(default(string), PropertyNameChanged));

        /// <summary>
        /// Properties the name changed.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="arg">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void PropertyNameChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs arg)
        {
            var control = (Control)dependencyObject;
            var collection = new BehaviorCollection();
            var validationBehavior = new ValidationBehavior() { PropertyName = arg.NewValue.ToString() };
            var changePropertyIsValidAction = new ChangePropertyAction
            {
                PropertyName = new PropertyPath(nameof(Control.Style)),
                Value = null
            };

            var errorStyle = new Style
            {
                TargetType = typeof(Control)
            };

            errorStyle.Setters.Add(new Setter(Control.BorderBrushProperty, new SolidColorBrush(Colors.Red)));

            var changePropertyIsInvalidAction = new ChangePropertyAction
            {
                PropertyName = new PropertyPath(nameof(Control.Style)),
                Value = errorStyle
            };

            validationBehavior.WhenValidActions.Add(changePropertyIsValidAction);
            validationBehavior.WhenInvalidActions.Add(changePropertyIsInvalidAction);

            collection.Add(validationBehavior);
            control.SetValue(Interaction.BehaviorsProperty, collection);
        }

        /// <summary>
        /// Sets the name of the property.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="value">The value.</param>
        public static void SetPropertyName(DependencyObject element, string value)
        {
            element.SetValue(PropertyNameProperty, value);
        }

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>System.String.</returns>
        public static string GetPropertyName(DependencyObject element)
        {
            return element.GetValue(PropertyNameProperty).ToString();
        }
    }
}
