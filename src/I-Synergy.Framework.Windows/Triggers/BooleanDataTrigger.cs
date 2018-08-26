using ISynergy.Common;
using Windows.UI.Xaml;

namespace ISynergy.Triggers
{
    public class BooleanDataTrigger : StateTriggerBase
    {
        #region DataValue
        public bool DataValue
        {
            get { return (bool)GetValue(DataValueProperty); }
            set { SetValue(DataValueProperty, value); }
        }

        public static readonly DependencyProperty DataValueProperty =
            DependencyProperty.Register(nameof(DataValue), typeof(bool), typeof(BooleanDataTrigger), new PropertyMetadata(false, DataValueChanged));

        private static void DataValueChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            TriggerStateCheck(
                target,
                (bool)e.NewValue,
                (bool)target.GetValue(TriggerValueProperty));
        }
        #endregion

        #region TriggerValue
        public bool TriggerValue
        {
            get { return (bool)GetValue(TriggerValueProperty); }
            set { SetValue(TriggerValueProperty, value); }
        }

        public static readonly DependencyProperty TriggerValueProperty =
            DependencyProperty.Register(nameof(TriggerValue), typeof(bool), typeof(BooleanDataTrigger), new PropertyMetadata(false, TriggerValueChanged));

        private static void TriggerValueChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            TriggerStateCheck(
                target,
                (bool)target.GetValue(DataValueProperty),
                (bool)e.NewValue);
        }
        #endregion

        private static void TriggerStateCheck(DependencyObject target, bool dataValue, bool triggerValue)
        {
            Argument.IsNotNull(nameof(target), target);

            if (!(target is BooleanDataTrigger trigger)) return;
            trigger.SetActive(dataValue == triggerValue);
        }
    }
}
