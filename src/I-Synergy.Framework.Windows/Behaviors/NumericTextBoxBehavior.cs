using Microsoft.Xaml.Interactivity;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace ISynergy.Framework.Windows.Behaviors
{
    /// <summary>
    /// Class NumericTextBoxBehavior.
    /// Implements the <see cref="Windows.UI.Xaml.DependencyObject" />
    /// Implements the <see cref="IBehavior" />
    /// </summary>
    /// <seealso cref="Windows.UI.Xaml.DependencyObject" />
    /// <seealso cref="IBehavior" />
    public class NumericTextBoxBehavior : DependencyObject, IBehavior
    {
        /// <summary>
        /// The last text
        /// </summary>
        /// Track the last valid text value.
        private string _lastText;

        /// <summary>
        /// The allow decimal property
        /// </summary>
        /// Backing storage for the AllowDecimal property
        public static readonly DependencyProperty AllowDecimalProperty = DependencyProperty.Register(nameof(AllowDecimal), typeof(bool), typeof(NumericTextBoxBehavior), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets a value indicating whether [allow decimal].
        /// </summary>
        /// <value><c>true</c> if [allow decimal]; otherwise, <c>false</c>.</value>
        /// True to allow a decimal point.
        public bool AllowDecimal
        {
            get
            {
                return (bool)GetValue(AllowDecimalProperty);
            }
            set
            {
                SetValue(AllowDecimalProperty, value);
            }
        }

        /// <summary>
        /// Attaches to the specified object.
        /// </summary>
        /// <param name="associatedObject">The <see cref="T:Windows.UI.Xaml.DependencyObject" /> to which the <seealso cref="T:Microsoft.Xaml.Interactivity.IBehavior" /> will be attached.</param>
        /// <exception cref="ArgumentException">NumericTextBoxBehavior can only be used with a TextBox.</exception>
        /// Used to attach this behavior to an element.
        /// Must be a TextBox.
        /// TextBox to assocate this behavior with.
        public void Attach(DependencyObject associatedObject)
        {
            if (!(associatedObject is TextBox tb))
            {
                throw new ArgumentException("NumericTextBoxBehavior can only be used with a TextBox.");
            }

            AssociatedObject = associatedObject;

            _lastText = tb.Text;

            tb.TextChanged += TbOnTextChanged;

            if (tb.InputScope is null)
            {
                var inputScope = new InputScope();
                inputScope.Names.Add(new InputScopeName(InputScopeNameValue.Number));
                tb.InputScope = inputScope;
            }
        }

        /// <summary>
        /// Tbs the on text changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="TextChangedEventArgs"/> instance containing the event data.</param>
        /// Handles the TextChanged event on the TextBox and watches for
        /// numeric entries.
        private void TbOnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (AssociatedObject is TextBox tb)
            {
                if (AllowDecimal)
                {
                    if (string.IsNullOrEmpty(tb.Text) || Double.TryParse(tb.Text, out _))
                    {
                        _lastText = tb.Text;
                        return;
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(tb.Text) || long.TryParse(tb.Text, out _))
                    {
                        _lastText = tb.Text;
                        return;
                    }
                }

                tb.Text = _lastText;
                tb.SelectionStart = tb.Text.Length;
            }
        }

        /// <summary>
        /// Detaches this instance from its associated object.
        /// </summary>
        /// Detaches the behavior from the TextBox.
        public void Detach()
        {
            if (AssociatedObject is TextBox tb) tb.TextChanged -= TbOnTextChanged;
        }

        /// <summary>
        /// Gets the associated object.
        /// </summary>
        /// <value>The associated object.</value>
        /// The associated object (TextBox).
        public DependencyObject AssociatedObject { get; private set; }
    }
}
