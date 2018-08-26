using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace ISynergy.Behaviours
{
    public class NumericTextBoxBehavior : DependencyObject, IBehavior
    {
        /// 
        /// Track the last valid text value.
        /// 
        private string _lastText;

        /// 
        /// Backing storage for the AllowDecimal property
        /// 
        public static readonly DependencyProperty AllowDecimalProperty = DependencyProperty.Register(nameof(AllowDecimal), typeof(bool), typeof(NumericTextBoxBehavior), new PropertyMetadata(false));

        /// 
        /// True to allow a decimal point.
        /// 
        public bool AllowDecimal
        {
            get
            {
                return (bool)base.GetValue(AllowDecimalProperty);
            }
            set
            {
                base.SetValue(AllowDecimalProperty, value);
            }
        }

        /// 
        /// Used to attach this behavior to an element.
        /// Must be a TextBox.
        /// 
        ///TextBox to assocate this behavior with.
        public void Attach(DependencyObject associatedObject)
        {
            TextBox tb = associatedObject as TextBox;

            if (tb == null)
            {
                throw new ArgumentException("NumericTextBoxBehavior can only be used with a TextBox.");
            }

            AssociatedObject = associatedObject;

            _lastText = tb.Text;

            tb.TextChanged += TbOnTextChanged;

            if (tb.InputScope == null)
            {
                var inputScope = new InputScope();
                inputScope.Names.Add(new InputScopeName(InputScopeNameValue.Number));
                tb.InputScope = inputScope;
            }
        }

        /// 
        /// Handles the TextChanged event on the TextBox and watches for
        /// numeric entries.
        /// 
        private void TbOnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (AssociatedObject is TextBox tb)
            {
                if (AllowDecimal)
                {
                    if (string.IsNullOrEmpty(tb.Text) || Double.TryParse(tb.Text, out double value))
                    {
                        _lastText = tb.Text;
                        return;
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(tb.Text) || long.TryParse(tb.Text, out long value))
                    {
                        _lastText = tb.Text;
                        return;
                    }
                }

                tb.Text = _lastText;
                tb.SelectionStart = tb.Text.Length;
            }
        }

        /// 
        /// Detaches the behavior from the TextBox.
        /// 
        public void Detach()
        {
            if (AssociatedObject is TextBox tb) tb.TextChanged -= this.TbOnTextChanged;
        }

        /// 
        /// The associated object (TextBox).
        /// 
        public DependencyObject AssociatedObject { get; private set; }
    }
}
