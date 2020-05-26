using ISynergy.Framework.Core.Utilities;
using ISynergy.Framework.Core.Validation;
using Windows.UI.Xaml;

namespace ISynergy.Framework.Windows.Triggers
{
    public class IntegerDataTrigger : StateTriggerBase
    {
        #region DataValue
        public int DataValue
        {
            get { return (int)GetValue(DataValueProperty); }
            set { SetValue(DataValueProperty, value); }
        }

        public static readonly DependencyProperty DataValueProperty =
            DependencyProperty.Register(nameof(DataValue), typeof(int), typeof(IntegerDataTrigger), new PropertyMetadata(0, DataValueChanged));

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
        public int TriggerValue
        {
            get { return (int)GetValue(TriggerValueProperty); }
            set { SetValue(TriggerValueProperty, value); }
        }

        public static readonly DependencyProperty TriggerValueProperty =
            DependencyProperty.Register(nameof(TriggerValue), typeof(int), typeof(IntegerDataTrigger), new PropertyMetadata(0, TriggerValueChanged));

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
        public string Operator
        {
            get { return (string)GetValue(OperatorProperty); }
            set { SetValue(OperatorProperty, value); }
        }

        public static readonly DependencyProperty OperatorProperty =
            DependencyProperty.Register(nameof(Operator), typeof(string), typeof(IntegerDataTrigger), new PropertyMetadata("=="));
        #endregion

        private static void TriggerStateCheck(DependencyObject target, string operation, int dataValue, int triggerValue)
        {
            Argument.IsNotNull(nameof(target), target);
            Argument.IsNotNullOrEmpty(nameof(operation), operation);

            if (!(target is IntegerDataTrigger trigger)) return;
            trigger.SetActive(CompareUtility.Compare(operation, dataValue, triggerValue));
        }
    }
}
