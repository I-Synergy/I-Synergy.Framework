using ISynergy.Framework.UI.Enumerations;
using System.ComponentModel;

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

        [Browsable(false)]
        [Bindable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new string Text
        {
            get => (string)GetValue(TextProperty);
            internal set => SetValue(TextProperty, value);
        }

        protected NumericEntry()
            : base()
        {
            Keyboard = Keyboard.Numeric;
            IsPassword = false;
            IsTextPredictionEnabled = false;
            IsSpellCheckEnabled = false;
            TextChanged += NumericEntry_TextChanged;
        }

        public abstract void NumericEntry_TextChanged(object sender, TextChangedEventArgs e);
    }
}