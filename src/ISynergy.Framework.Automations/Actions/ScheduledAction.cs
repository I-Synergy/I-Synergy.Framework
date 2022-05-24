using ISynergy.Framework.Automations.Actions.Base;
using System;

namespace ISynergy.Framework.Automations.Actions
{
    /// <summary>
    /// Wait action.
    /// </summary>
    public class ScheduledAction : BaseAction
    {
        /// <summary>
        /// Gets or sets the ExecutionTime property value.
        /// </summary>
        public DateTimeOffset ExecutionTime
        {
            get { return GetValue<DateTimeOffset>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Timeout property value.
        /// </summary>
        public TimeSpan Timeout
        {
            get { return GetValue<TimeSpan>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="automationId"></param>
        public ScheduledAction(Guid automationId)
            : base(automationId)
        {
            ExecutionTime = DateTimeOffset.Now;
            Timeout = TimeSpan.Zero;
        }
    }
}
