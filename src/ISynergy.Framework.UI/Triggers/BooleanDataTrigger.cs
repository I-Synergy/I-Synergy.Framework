using ISynergy.Framework.Core.Validation;

#if (WINDOWS_UWP || HAS_UNO)
using Windows.UI.Xaml;
#else
using Microsoft.UI.Xaml;
#endif

namespace ISynergy.Framework.UI.Triggers
{
    /// <summary>
    /// Class BooleanDataTrigger.
    /// Implements the <see cref="StateTriggerBase" />
    /// </summary>
    /// <seealso cref="StateTriggerBase" />
    public class BooleanDataTrigger : StateTriggerBase
    {
        #region DataValue
        /// <summary>
        /// Gets or sets a value indicating whether [data value].
        /// </summary>
        /// <value><c>true</c> if [data value]; otherwise, <c>false</c>.</value>
        public bool DataValue
        {
            get => (bool)GetValue(DataValueProperty);
            set => SetValue(DataValueProperty, value);
        }

        /// <summary>
        /// The data value property
        /// </summary>
        public static readonly DependencyProperty DataValueProperty =
            DependencyProperty.Register(nameof(DataValue), typeof(bool), typeof(BooleanDataTrigger), new PropertyMetadata(false, DataValueChanged));

        /// <summary>
        /// Datas the value changed.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void DataValueChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            TriggerStateCheck(
                target,
                (bool)e.NewValue,
                (bool)target.GetValue(TriggerValueProperty));
        }
        #endregion

        #region TriggerValue
        /// <summary>
        /// Gets or sets a value indicating whether [trigger value].
        /// </summary>
        /// <value><c>true</c> if [trigger value]; otherwise, <c>false</c>.</value>
        public bool TriggerValue
        {
            get => (bool)GetValue(TriggerValueProperty);
            set => SetValue(TriggerValueProperty, value);
        }

        /// <summary>
        /// The trigger value property
        /// </summary>
        public static readonly DependencyProperty TriggerValueProperty =
            DependencyProperty.Register(nameof(TriggerValue), typeof(bool), typeof(BooleanDataTrigger), new PropertyMetadata(false, TriggerValueChanged));

        /// <summary>
        /// Triggers the value changed.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void TriggerValueChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            TriggerStateCheck(
                target,
                (bool)target.GetValue(DataValueProperty),
                (bool)e.NewValue);
        }
        #endregion

        /// <summary>
        /// Triggers the state check.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="dataValue">if set to <c>true</c> [data value].</param>
        /// <param name="triggerValue">if set to <c>true</c> [trigger value].</param>
        private static void TriggerStateCheck(DependencyObject target, bool dataValue, bool triggerValue)
        {
            Argument.IsNotNull(nameof(target), target);

            if (!(target is BooleanDataTrigger trigger)) return;
            trigger.SetActive(dataValue == triggerValue);
        }
    }
}
