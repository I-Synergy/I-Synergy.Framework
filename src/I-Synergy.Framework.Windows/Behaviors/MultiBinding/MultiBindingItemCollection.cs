using System.Collections.Specialized;
using System.Linq;
using ISynergy.Framework.Windows.Collections;
using Windows.UI.Xaml;

namespace ISynergy.Framework.Windows.Behaviors
{
    /// <summary>
    /// Represents a collection of <see cref="MultiBindingBehavior" />.
    /// </summary>
    public class MultiBindingItemCollection : DependencyObjectCollection<MultiBindingItem>
    {
        /// <summary>
        /// The updating
        /// </summary>
        private bool _updating;

        /// <summary>
        /// Gets or sets the multiple binding value.
        /// </summary>
        /// <value>The multiple binding value.</value>
        public object[] Value
        {
            get { return (object[])GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// Identifier for the <see cref="Value" /> dependency property.
        /// </summary>
        internal static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(object[]), typeof(MultiBindingItemCollection), new PropertyMetadata(null, OnValueChanged));

        /// <summary>
        /// Handles the <see cref="E:ValueChanged" /> event.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var multiBindingItemCollection = (MultiBindingItemCollection)d;

            multiBindingItemCollection.UpdateSource();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiBindingItemCollection" /> class.
        /// </summary>
        public MultiBindingItemCollection()
        {
            CollectionChanged += OnCollectionChanged;
        }

        /// <summary>
        /// Handles the <see cref="E:CollectionChanged" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (MultiBindingItem item in e.OldItems)
                {
                    item.Parent = null;
                }
            }

            if (e.NewItems != null)
            {
                foreach (MultiBindingItem item in e.NewItems)
                {
                    item.Parent = this;
                }
            }

            Update();
        }

        /// <summary>
        /// Updates this instance.
        /// </summary>
        internal void Update()
        {
            if (_updating)
            {
                return;
            }

            try
            {
                _updating = true;

                Value = this
                    .OfType<MultiBindingItem>()
                    .Select(x => x.Value)
                    .ToArray();
            }
            finally
            {
                _updating = false;
            }
        }

        /// <summary>
        /// Updates the source.
        /// </summary>
        private void UpdateSource()
        {
            if (_updating)
            {
                return;
            }

            try
            {
                _updating = true;

                for (var index = 0; index < this.Count; index++)
                {
                    if (this[index] is MultiBindingItem multiBindingItem)
                    {
                        multiBindingItem.Value = Value[index];
                    }
                }
            }
            finally
            {
                _updating = false;
            }
        }
    }
}
