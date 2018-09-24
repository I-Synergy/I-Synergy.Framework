using Windows.UI.Xaml;

namespace ISynergy.Triggers
{
    public class ObjectDataTrigger : StateTriggerBase
    {
        #region DataValue
        public object DataValue
        {
            get { return (object)GetValue(DataValueProperty); }
            set { SetValue(DataValueProperty, value); }
        }

        public static readonly DependencyProperty DataValueProperty =
            DependencyProperty.Register(nameof(DataValue), typeof(object), typeof(ObjectDataTrigger), new PropertyMetadata(null, DataValueChanged));

        private static void DataValueChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            TriggerStateCheck(
                target,
                e.NewValue,
                target.GetValue(TriggerValueProperty));
        }
        #endregion

        #region TriggerValue
        public object TriggerValue
        {
            get { return (object)GetValue(TriggerValueProperty); }
            set { SetValue(TriggerValueProperty, value); }
        }

        public static readonly DependencyProperty TriggerValueProperty =
            DependencyProperty.Register(nameof(TriggerValue), typeof(object), typeof(ObjectDataTrigger), new PropertyMetadata(null, TriggerValueChanged));

        private static void TriggerValueChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            TriggerStateCheck(
                target,
                target.GetValue(DataValueProperty),
                e.NewValue);
        }
        #endregion

        private static void TriggerStateCheck(DependencyObject target, object dataValue, object triggerValue)
        {
            Argument.IsNotNull(nameof(target), target);

            if (!(target is ObjectDataTrigger trigger) || dataValue is null) return;
            trigger.SetActive(dataValue.Equals(triggerValue));
        }
    }
}
