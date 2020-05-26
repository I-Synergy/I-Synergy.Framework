using ISynergy.Framework.Core.Utilities;
using ISynergy.Framework.Core.Validation;
using Windows.UI.Xaml;

namespace ISynergy.Framework.Windows.Triggers
{
    public class DecimalDataTrigger : StateTriggerBase
    {
        #region DataValue
        public decimal DataValue
        {
            get { return (decimal)GetValue(DataValueProperty); }
            set { SetValue(DataValueProperty, value); }
        }

        public static readonly DependencyProperty DataValueProperty =
            DependencyProperty.Register(nameof(DataValue), typeof(decimal), typeof(DecimalDataTrigger), new PropertyMetadata(0m, DataValueChanged));

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
        public decimal TriggerValue
        {
            get { return (decimal)GetValue(TriggerValueProperty); }
            set { SetValue(TriggerValueProperty, value); }
        }

        public static readonly DependencyProperty TriggerValueProperty =
            DependencyProperty.Register(nameof(TriggerValue), typeof(decimal), typeof(DecimalDataTrigger), new PropertyMetadata(0m, TriggerValueChanged));

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
        public string Operator
        {
            get { return (string)GetValue(OperatorProperty); }
            set { SetValue(OperatorProperty, value); }
        }

        public static readonly DependencyProperty OperatorProperty =
            DependencyProperty.Register(nameof(Operator), typeof(string), typeof(DecimalDataTrigger), new PropertyMetadata("=="));
        #endregion

        private static void TriggerStateCheck(DependencyObject target, string operation, decimal dataValue, decimal triggerValue)
        {
            Argument.IsNotNull(nameof(target), target);
            Argument.IsNotNullOrEmpty(nameof(operation), operation);

            if (!(target is DecimalDataTrigger trigger)) return;
            trigger.SetActive(CompareUtility.Compare(operation, dataValue, triggerValue));
        }
    }
}
