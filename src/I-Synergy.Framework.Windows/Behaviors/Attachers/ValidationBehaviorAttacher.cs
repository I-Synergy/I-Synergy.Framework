using Microsoft.Xaml.Interactions.Core;
using Microsoft.Xaml.Interactivity;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace ISynergy.Behaviors
{
    public static class ValidationBehaviorAttacher
    {
        public static readonly DependencyProperty PropertyNameProperty = DependencyProperty.RegisterAttached(
        "PropertyName", typeof(string), typeof(ValidationBehaviorAttacher), new PropertyMetadata(default(string), PropertyNameChanged));

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

        public static void SetPropertyName(DependencyObject element, string value)
        {
            element.SetValue(PropertyNameProperty, value);
        }

        public static string GetPropertyName(DependencyObject element)
        {
            return element.GetValue(PropertyNameProperty).ToString();
        }
    }
}
