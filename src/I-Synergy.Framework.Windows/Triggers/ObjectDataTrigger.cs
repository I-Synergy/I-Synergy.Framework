using ISynergy.Framework.Core.Utilities;
using ISynergy.Framework.Core.Validation;
using Windows.UI.Xaml;

namespace ISynergy.Framework.Windows.Triggers
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
                (string)target.GetValue(OperatorProperty),
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
                (string)target.GetValue(OperatorProperty),
                target.GetValue(DataValueProperty),
                e.NewValue);
        }
        #endregion

        #region Operator
        public string Operator
        {
            get { return (string)GetValue(OperatorProperty); }
            set { SetValue(OperatorProperty, value); }
        }

        public static readonly DependencyProperty OperatorProperty =
            DependencyProperty.Register(nameof(Operator), typeof(string), typeof(ObjectDataTrigger), new PropertyMetadata("=="));
        #endregion

        private static void TriggerStateCheck(DependencyObject target, string operation, object dataValue, object triggerValue)
        {
            Argument.IsNotNull(nameof(target), target);
            Argument.IsNotNullOrEmpty(nameof(operation), operation);

            if (!(target is ObjectDataTrigger trigger) || dataValue is null) return;
            trigger.SetActive(CompareUtility.Compare(operation, dataValue, triggerValue));
        }
    }
}
