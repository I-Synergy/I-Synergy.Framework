using Windows.UI.Xaml;

namespace ISynergy.Framework.Windows.Behaviors
{
    /// <summary>
    /// A multiple binding item.
    /// </summary>
    public class MultiBindingItem : DependencyObject
    {
        /// <summary>
        /// Gets or sets the binding value.
        /// </summary>
        /// <value>The binding value.</value>
        public object Value
        {
            get { return GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// Identifier for the <see cref="Value" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(object), typeof(MultiBindingItem), new PropertyMetadata(null, OnValueChanged));

        /// <summary>
        /// Handles the <see cref="E:ValueChanged" /> event.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var multiBindingItem = (MultiBindingItem)d;

            multiBindingItem.Update();
        }

        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>The parent.</value>
        internal MultiBindingItemCollection Parent { get; set; }

        /// <summary>
        /// Updates this instance.
        /// </summary>
        private void Update()
        {
            var parent = Parent;

            if (parent != null)
            {
                parent.Update();
            }
        }
    }
}
