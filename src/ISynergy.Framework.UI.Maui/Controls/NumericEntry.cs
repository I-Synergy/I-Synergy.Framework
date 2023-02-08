using ISynergy.Framework.UI.Enumerations;

namespace ISynergy.Framework.UI.Controls
{
    public abstract class NumericEntry<T> : Entry
        where T : struct
    {
        public static readonly BindableProperty ValueProperty = BindableProperty.Create(nameof(Type), typeof(T), typeof(NumericEntry<T>), default(T), propertyChanged: ValueChanged);

        private static void ValueChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (oldValue.Equals(default(T)) && !oldValue.Equals(newValue))
                bindable.SetValue(TextProperty, newValue.ToString());
        }

        public T Value
        {
            get => (T)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        protected NumericEntry()
            : base()
        {
            Keyboard = Keyboard.Numeric;
            IsPassword = false;
            IsTextPredictionEnabled = false;
            IsSpellCheckEnabled = false;
            Text = default(T).ToString();
            TextChanged += NumericEntry_TextChanged;
        }

        public abstract void NumericEntry_TextChanged(object sender, TextChangedEventArgs e);
    }
}