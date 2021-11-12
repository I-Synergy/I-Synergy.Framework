using ISynergy.Framework.Core.Utilities;
using ISynergy.Framework.Core.Validation;

#if WINDOWS_UWP
using Windows.UI.Xaml;
#else
using Microsoft.UI.Xaml;
#endif

namespace ISynergy.Framework.UI.Triggers
{
    /// <summary>
    /// Class DecimalDataTrigger.
    /// Implements the <see cref="StateTriggerBase" />
    /// </summary>
    /// <seealso cref="StateTriggerBase" />
    public class DecimalDataTrigger : StateTriggerBase
    {
        #region DataValue
        /// <summary>
        /// Gets or sets the data value.
        /// </summary>
        /// <value>The data value.</value>
        public decimal DataValue
        {
            get => (decimal)GetValue(DataValueProperty);
            set => SetValue(DataValueProperty, value);
        }

        /// <summary>
        /// The data value property
        /// </summary>
        public static readonly DependencyProperty DataValueProperty =
            DependencyProperty.Register(nameof(DataValue), typeof(decimal), typeof(DecimalDataTrigger), new PropertyMetadata(0m, DataValueChanged));

        /// <summary>
        /// Datas the value changed.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void DataValueChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            TriggerStateCheck(
                target,
                (string)target.GetValue(OperatorProperty),
                decimal.TryParse(e.NewValue.ToString(), out var resultA) ? resultA : 0m,
                decimal.TryParse(target.GetValue(TriggerValueProperty).ToString(), out var resultB) ? resultB : 0m);
        }
        #endregion

        #region TriggerValue
        /// <summary>
        /// Gets or sets the trigger value.
        /// </summary>
        /// <value>The trigger value.</value>
        public decimal TriggerValue
        {
            get => (decimal)GetValue(TriggerValueProperty);
            set => SetValue(TriggerValueProperty, value);
        }

        /// <summary>
        /// The trigger value property
        /// </summary>
        public static readonly DependencyProperty TriggerValueProperty =
            DependencyProperty.Register(nameof(TriggerValue), typeof(decimal), typeof(DecimalDataTrigger), new PropertyMetadata(0m, TriggerValueChanged));

        /// <summary>
        /// Triggers the value changed.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void TriggerValueChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            TriggerStateCheck(
                target,
                (string)target.GetValue(OperatorProperty),
                decimal.TryParse(target.GetValue(DataValueProperty).ToString(), out var resultA) ? resultA : 0m,
                decimal.TryParse(e.NewValue.ToString(), out var resultB) ? resultB : 0m);
        }
        #endregion

        #region Operator
        /// <summary>
        /// Gets or sets the operator.
        /// </summary>
        /// <value>The operator.</value>
        public string Operator
        {
            get => (string)GetValue(OperatorProperty);
            set => SetValue(OperatorProperty, value);
        }

        /// <summary>
        /// The operator property
        /// </summary>
        public static readonly DependencyProperty OperatorProperty =
            DependencyProperty.Register(nameof(Operator), typeof(string), typeof(DecimalDataTrigger), new PropertyMetadata("=="));
        #endregion

        /// <summary>
        /// Triggers the state check.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="operation">The operation.</param>
        /// <param name="dataValue">The data value.</param>
        /// <param name="triggerValue">The trigger value.</param>
        private static void TriggerStateCheck(DependencyObject target, string operation, decimal dataValue, decimal triggerValue)
        {
            Argument.IsNotNull(nameof(target), target);
            Argument.IsNotNullOrEmpty(nameof(operation), operation);

            if (!(target is DecimalDataTrigger trigger)) return;
            trigger.SetActive(CompareUtility.Compare(operation, dataValue, triggerValue));
        }
    }
}
