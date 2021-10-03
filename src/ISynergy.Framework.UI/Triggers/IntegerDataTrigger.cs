using ISynergy.Framework.Core.Utilities;
using ISynergy.Framework.Core.Validation;

#if (NETFX_CORE || HAS_UNO)
using Windows.UI.Xaml;
#elif (NET5_0 && WINDOWS)
using Microsoft.UI.Xaml;
#endif

namespace ISynergy.Framework.UI.Triggers
{
    /// <summary>
    /// Class IntegerDataTrigger.
    /// Implements the <see cref="StateTriggerBase" />
    /// </summary>
    /// <seealso cref="StateTriggerBase" />
    public class IntegerDataTrigger : StateTriggerBase
    {
        #region DataValue
        /// <summary>
        /// Gets or sets the data value.
        /// </summary>
        /// <value>The data value.</value>
        public int DataValue
        {
            get => (int)GetValue(DataValueProperty);
            set => SetValue(DataValueProperty, value);
        }

        /// <summary>
        /// The data value property
        /// </summary>
        public static readonly DependencyProperty DataValueProperty =
            DependencyProperty.Register(nameof(DataValue), typeof(int), typeof(IntegerDataTrigger), new PropertyMetadata(0, DataValueChanged));

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
                int.TryParse(e.NewValue.ToString(), out var resultA) ? resultA : 0,
                int.TryParse(target.GetValue(TriggerValueProperty).ToString(), out var resultB) ? resultB : 0);
        }
        #endregion

        #region TriggerValue
        /// <summary>
        /// Gets or sets the trigger value.
        /// </summary>
        /// <value>The trigger value.</value>
        public int TriggerValue
        {
            get => (int)GetValue(TriggerValueProperty);
            set => SetValue(TriggerValueProperty, value);
        }

        /// <summary>
        /// The trigger value property
        /// </summary>
        public static readonly DependencyProperty TriggerValueProperty =
            DependencyProperty.Register(nameof(TriggerValue), typeof(int), typeof(IntegerDataTrigger), new PropertyMetadata(0, TriggerValueChanged));

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
                int.TryParse(target.GetValue(DataValueProperty).ToString(), out var resultA) ? resultA : 0,
                int.TryParse(e.NewValue.ToString(), out var resultB) ? resultB : 0);
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
            DependencyProperty.Register(nameof(Operator), typeof(string), typeof(IntegerDataTrigger), new PropertyMetadata("=="));
        #endregion

        /// <summary>
        /// Triggers the state check.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="operation">The operation.</param>
        /// <param name="dataValue">The data value.</param>
        /// <param name="triggerValue">The trigger value.</param>
        private static void TriggerStateCheck(DependencyObject target, string operation, int dataValue, int triggerValue)
        {
            Argument.IsNotNull(nameof(target), target);
            Argument.IsNotNullOrEmpty(nameof(operation), operation);

            if (!(target is IntegerDataTrigger trigger)) return;
            trigger.SetActive(CompareUtility.Compare(operation, dataValue, triggerValue));
        }
    }
}
