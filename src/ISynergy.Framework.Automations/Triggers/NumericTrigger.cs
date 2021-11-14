namespace ISynergy.Framework.Automations.Triggers
{
    /// <summary>
    /// Numeric trigger based on an integer.
    /// </summary>
    public class IntegerTrigger : BaseNumericTrigger<int>
    {
        /// <summary>
        /// Trigger for integer properties.
        /// </summary>
        /// <param name="automationId"></param>
        /// <param name="function"></param>
        /// <param name="below"></param>
        /// <param name="above"></param>
        /// <param name="callbackAsync"></param>
        public IntegerTrigger(
            Guid automationId,
            Func<(IObservableClass Entity, IProperty<int> Property)> function,
            int below,
            int above,
            Func<int, Task> callbackAsync)
            : base(automationId, function, below, above, callbackAsync)
        {
        }
    }

    /// <summary>
    /// Numeric trigger based on a decimal.
    /// </summary>
    public class DecimalTrigger : BaseNumericTrigger<decimal>
    {
        /// <summary>
        /// Trigger for integer properties.
        /// </summary>
        /// <param name="automationId"></param>
        /// <param name="function"></param>
        /// <param name="below"></param>
        /// <param name="above"></param>
        /// <param name="callbackAsync"></param>
        public DecimalTrigger(
            Guid automationId,
            Func<(IObservableClass Entity, IProperty<decimal> Property)> function,
            decimal below,
            decimal above,
            Func<decimal, Task> callbackAsync)
            : base(automationId, function, below, above, callbackAsync)
        {
        }
    }

    /// <summary>
    /// Numeric trigger based on a double.
    /// </summary>
    public class DoubleTrigger : BaseNumericTrigger<double>
    {
        /// <summary>
        /// Trigger for integer properties.
        /// </summary>
        /// <param name="automationId"></param>
        /// <param name="function"></param>
        /// <param name="below"></param>
        /// <param name="above"></param>
        /// <param name="callbackAsync"></param>
        public DoubleTrigger(
            Guid automationId,
            Func<(IObservableClass Entity, IProperty<double> Property)> function,
            double below,
            double above,
            Func<double, Task> callbackAsync)
            : base(automationId, function, below, above, callbackAsync)
        {
        }
    }
}