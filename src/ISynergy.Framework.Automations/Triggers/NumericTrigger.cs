using ISynergy.Framework.Automations.Triggers.Base;
using System;

namespace ISynergy.Framework.Automations.Triggers
{
    /// <summary>
    /// Numeric trigger based on an integer.
    /// </summary>
    public class IntegerTrigger : BaseNumericTrigger<int>
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="automationId"></param>
        /// <param name="below"></param>
        /// <param name="above"></param>
        /// <param name="for"></param>
        public IntegerTrigger(Guid automationId, int below, int above, TimeSpan @for)
            : base(automationId, below, above, @for)
        {
        }
    }

    /// <summary>
    /// Numeric trigger based on a decimal.
    /// </summary>
    public class DecimalTrigger : BaseNumericTrigger<decimal>
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="automationId"></param>
        /// <param name="below"></param>
        /// <param name="above"></param>
        /// <param name="for"></param>
        public DecimalTrigger(Guid automationId, decimal below, decimal above, TimeSpan @for)
            : base(automationId, below, above, @for)
        {
        }
    }

    /// <summary>
    /// Numeric trigger based on a double.
    /// </summary>
    public class DoubleTrigger : BaseNumericTrigger<double>
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="automationId"></param>
        /// <param name="below"></param>
        /// <param name="above"></param>
        /// <param name="for"></param>
        public DoubleTrigger(Guid automationId, double below, double above, TimeSpan @for)
            : base(automationId, below, above, @for)
        {
        }
    }
}
